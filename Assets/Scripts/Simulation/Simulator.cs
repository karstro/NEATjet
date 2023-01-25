using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    public float g = 0.981f;
    private float dt;
    public int StepsPerSecond = 60;
    public int TimeOut = 15;
    public float DragCoeff;
    public float FrictionCoeff;
    public float Elasticity = 0.5f;

    // uses awake to make sure it triggers before it can be used by other components
    void Awake() {
        this.dt = 1 / (float) this.StepsPerSecond;
    }

    // public Simulator(float g, float dt, float Elasticity) {
    //     this.g = g;
    //     this.dt = dt;
    //     this.TimeOut = 1;
    //     this.DragCoeff = 0;
    //     this.FrictionCoeff = 0;
    //     this.Elasticity = Elasticity;
    // }

    // returns point of contact
    private Vector3 CollideSphere(Vector3 Position, float Radius) {
        if (Position.y < Radius) {
            Position.y = 0;
            return Position;
        }
        // no contact
        return Vector3.down;
    }

    private Vector3 GetCollisionResponse(Creature c, Vector3 PointOfContact) {
        // simplified response magnitude
        float jr = -(1 + this.Elasticity) * c.Velocity.y;
        // response vector
        return jr * Vector3.up;
    }

    // check each sphere for a collision
    private Vector3[] GetCollisions(Creature c) {
        Vector3[] Collisions = new Vector3[2 * c.Jets + 1];
        Collisions[0] = CollideSphere(c.Position, c.Radius);
        for (int i = 0; i < c.Jets; i++) {
            Vector3 JetStart = c.GetJetStart(i);
            Vector3 JetEnd = c.GetJetEnd(i);
            Collisions[2 * i + 1] = CollideSphere(JetStart, c.JetRadius);
            Collisions[2 * i + 2] = CollideSphere(JetEnd, c.JetRadius);
        }
        return Collisions;
    }

    // get the change in Angular momentum to creature c if Force is applied at PointOfContact
    private Vector3 GetAngularImpulseAt(Creature c, Vector3 Force, Vector3 PointOfContact) {
        Vector3 ContactArm = PointOfContact - c.Position;
        Vector3 AngledComponent = Force - Vector3.Project(Force, ContactArm);
        Vector3 AngularImpulse = Vector3.Cross(ContactArm.normalized, AngledComponent);
        return AngularImpulse;
    }

    private void updateCreature(Creature c) {
        // Vector3 Fg = new Vector3(0f, -c.Mass * this.g, 0f);
        Vector3 Fg = c.Mass * this.g * Vector3.down;
        // update jet rotations based on intent
        // insert thrust + angular thrust
        // insert drag + angular drag
        Vector3 Fr = Fg;
        Vector3 a = Fr / c.Mass;
        
        // Vector3 OldVel = c.Velocity;
        c.Velocity += this.dt * a;

        // calculate where the creature would go if there were no collisions (or friction)
        Vector3 OldPos = c.Position;
        c.Position += this.dt * c.Velocity;
        // calculate angular movement
        Quaternion OldRotation = c.Rotation;
        float RotationDeg = c.Angular.magnitude * this.dt * Mathf.Rad2Deg;
        Quaternion RotationChange = Quaternion.AngleAxis(RotationDeg, c.Angular);
        c.Rotation *= RotationChange;

        // initialize totals for collision results
        Vector3 ResponseImpulseTotal = Vector3.zero;
        Vector3 ResponseAngularImpulseTotal = Vector3.zero;
        int NumCollisions = 0;
        // for each collision the creature has with the ground
        foreach (Vector3 CollisionPoint in GetCollisions(c)) {
            if (CollisionPoint.y > -1) {
                NumCollisions++;
                // calculate the response to the collision
                Vector3 ResponseImpulse = GetCollisionResponse(c, CollisionPoint);
                Vector3 ResponseAngularImpulse = GetAngularImpulseAt(c, ResponseImpulse, CollisionPoint);
                // collect the responses to the totals
                ResponseImpulseTotal += ResponseImpulse;
                ResponseAngularImpulseTotal += ResponseAngularImpulse;
            }
        }
        // apply the response to the collision(s)
        if (NumCollisions != 0) {
            // calculate how collisions changed the velocity and angular momentum.
            c.Velocity += ResponseImpulseTotal / (NumCollisions * c.Mass);
            // c.Velocity += ResponseImpulseTotal / c.Mass;
            // should this be divided by NumCollisions?
            c.Angular += ResponseAngularImpulseTotal / NumCollisions;
            // also insert friction + angular friction based on NumCollisions

            // retroactively apply the changes in velocity and angular momentum
            c.Position = OldPos + this.dt * c.Velocity;
            RotationDeg = c.Angular.magnitude * this.dt * Mathf.Rad2Deg;
            RotationChange = Quaternion.AngleAxis(RotationDeg, c.Angular);
            c.Rotation = RotationChange * OldRotation;
        }
        // Debug.Log(NumCollisions);

        // // old collision code
        // if (c.Position.y < c.Radius) {
        //     c.Velocity.y *= -this.Elasticity;
        //     c.Position = OldPos + this.dt * c.Velocity;
        // }
    }

    // run a creature's full simulation and save the states so the visualiser can show them later
    public State[] simulateCreature(Creature c) {
        // number of simulation steps
        int Steps = this.TimeOut * this.StepsPerSecond;
        State[] States = new State[Steps];
        // run each step sequentially and save the state it ends up in
        for (int i = 0; i < Steps; i++) {
            // should I save the first state?
            // c.UpdateIntent(State?)
            this.updateCreature(c);
            States[i] = new State(i * this.dt, c);
        }
        return States;
    }
}
