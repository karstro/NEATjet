using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBuilder : ICreatureBuilder
{
    private Creature creature;
    public void Reset(){
        creature = new Creature();
    }

    public void InitializeGameObject() {
        creature._Object = new GameObject();
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation) {
        creature.SetPositionAndRotation(position, rotation);
    }

    public void InitializePhysicsBody() {
        creature._PhysicsBody = creature._Object.AddComponent<Rigidbody>();
        creature._PhysicsBody.mass = 1;
    }

    public void InitializeCreatureLimits(float radius, float maxJetAngleChangePerSecond, float thrustToWeight) {
        creature._Radius = radius;
        creature._MaxJetAngleChangePerSecond = maxJetAngleChangePerSecond;
        creature._ThrustToWeight = thrustToWeight;
    }

    public void InitializeJets(int jets, float jetLength, float jetRadius, Vector3 jetArm) {
        creature._Jets = jets;
        creature._JetLength = jetLength;
        creature._JetRadius = jetRadius;
        creature._JetArm = jetArm;
    }

    public void SetJetAngles(Vector3[] jetAngles) {
        creature._JetAngles = jetAngles;
    }

    public void SetJetAngles() {
        creature._JetAngles = new Vector3[creature._Jets];
        for (int JetIndex = 0; JetIndex < creature._Jets; JetIndex++) {
            creature._JetAngles[JetIndex] = Vector3.zero;
        }
    }

    public void SetThrusts(float[] thrusts) {
        creature._Thrusts = thrusts;
    }

    public void SetThrusts() {
        creature._Thrusts = new float[creature._Jets];
        for (int JetIndex = 0; JetIndex < creature._Jets; JetIndex++) {
            creature._Thrusts[JetIndex] = 0;
        }
    }

    public void InitializeColliders() {
        // initialize the body's collider
        SphereCollider body = creature._Object.AddComponent<SphereCollider>();
        body.center = Vector3.zero;
        body.radius = creature._Radius;
        
        // initialize the Jets' colliders
        creature._JetEndColliders = new SphereCollider[creature._Jets];
        for (int JetIndex = 0; JetIndex < creature._Jets; JetIndex++) {
            // calculate the collider's locations
            (Vector3 jetStart, Vector3 jetEnd) = creature.GetLocalJetStartAndEnd(JetIndex);
            // initialize the jetStart's collider
            SphereCollider jetStartCollider = creature._Object.AddComponent<SphereCollider>();
            jetStartCollider.center = jetStart;
            jetStartCollider.radius = creature._JetRadius;
            // store the jetEnd in the creature as they need to change over a simulation.
            creature._JetEndColliders[JetIndex] = creature._Object.AddComponent<SphereCollider>();
            creature._JetEndColliders[JetIndex].center = jetEnd;
            creature._JetEndColliders[JetIndex].radius = creature._JetRadius;
        }
    }

    public void SetBrain(IBrain brain) {
        creature._Brain = brain;
    }

    public Creature GetResult() {
        return creature;
    }
}
