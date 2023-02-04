using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature
{
    public float Radius;
    public float Mass;
    // public Vector3 Position;
    // public Vector3 Velocity;
    // public Quaternion Rotation;
    // public Vector3 Angular;
    private GameObject Object;
    public Vector3 Position {
            set => this.Object.transform.position = value; 
            get => this.Object.transform.position; 
        }
    public Quaternion Rotation {
            set => this.Object.transform.rotation = value; 
            get => this.Object.transform.rotation; 
        }
    public Rigidbody PhysicsBody;
    private SphereCollider[] Colliders;
    public int Jets;
    private Vector3 JetArm;
    public Vector3[] JetAngles;
    private float JetLength;
    public float JetRadius;
    public float[] Thrusts;

    // #ToDO look into factory design pattern to reduce code size
    public Creature() {
        this.Object = new GameObject();
        this.Object.transform.SetPositionAndRotation(new Vector3(0f, .75f, 0f), Quaternion.identity);

        this.PhysicsBody = this.Object.AddComponent<Rigidbody>();
        this.PhysicsBody.mass = Random.Range(0.5f, 5f);
        this.PhysicsBody.velocity = new Vector3(0f, 0f, 0f);
        this.PhysicsBody.angularVelocity = 2 * Vector3.up;
        // this.Body.angularVelocity = Vector3.zero;

        // this.Radius = Random.Range(0.05f, 1f);
        this.Radius = 0.5f;

        this.Jets = 4;
        this.JetLength = 1f;
        this.JetRadius = 0.2f;
        this.JetArm = new Vector3(1f, 0.5f, 0f);

        this.Colliders = new SphereCollider[2 * this.Jets + 1];

        // initialize the collider for the body
        int lastColliderIndex = 2 * this.Jets;
        this.Colliders[lastColliderIndex] = this.Object.AddComponent<SphereCollider>();
        this.Colliders[lastColliderIndex].center = Vector3.zero;
        this.Colliders[lastColliderIndex].radius = this.Radius;
        // #TODO make this actually load
        // this.Colliders[lastColliderIndex].material = (PhysicMaterial)Resources.Load("Assets/Materials/PhysicMaterials/PhysicsTest1");

        // Set the Colliders for the Jets
        for (int ColliderIndex = 0; ColliderIndex < lastColliderIndex; ColliderIndex++) {
            this.Colliders[ColliderIndex] = this.Object.AddComponent<SphereCollider>();
            this.Colliders[ColliderIndex].radius = this.JetRadius;
            // this.Colliders[ColliderIndex].material = (PhysicMaterial)Resources.Load("Assets/Materials/PhysicMaterials/PhysicsTest1");
        }

        this.JetAngles = new Vector3[this.Jets];
        for (int JetIndex = 0; JetIndex < this.Jets; JetIndex++) {
            this.SetJetAngle(JetIndex, Vector3.zero);
        }
        // change 1 angle to test if angle changing works
        this.SetJetAngle(0, new Vector3(0, 0, 90));
    }

    // used for visualizing the results of a simulation
    // as such it does not need to have a rigidbody
    public Creature(State state, Creature c) {
        this.Object = new GameObject();
        this.Object.transform.SetPositionAndRotation(state.Position, state.Rotation);
        
        this.Radius = c.Radius;
        this.Jets = c.Jets;
        this.JetArm = c.JetArm;

        // get this from the state once the state stores this.
        this.JetAngles = new Vector3[this.Jets];
        for (int i = 0; i < this.Jets; i++) {
            this.JetAngles[i] = c.JetAngles[i];
        }
        this.JetLength = c.JetLength;
        this.JetRadius = c.JetRadius;
    }

    public GameObject GetGameObject() {
        return this.Object;
    }
    
    // returns the world coordinates of the start and end of the given Jet
    public (Vector3, Vector3) GetJetStartAndEnd(int JetIndex) {
        Quaternion ArmRotation = Quaternion.Euler(0, JetIndex * 360 / this.Jets, 0);
        Vector3 RelativePosition = this.Rotation * (ArmRotation * this.JetArm);
        Vector3 JetStart = RelativePosition + this.Position;
        Quaternion JetRotation = Quaternion.Euler(0, JetIndex * 360 / this.Jets + this.JetAngles[JetIndex].y, this.JetAngles[JetIndex].z);
        Vector3 JetEnd = JetRotation * Vector3.down * this.JetLength + JetStart;
        return (JetStart, JetEnd);
    }

    // returns the local coordinates of the start and end of the given Jet
    public (Vector3, Vector3) GetLocalJetStartAndEnd(int JetIndex) {
        Quaternion ArmRotation = Quaternion.Euler(0, JetIndex * 360 / this.Jets, 0);
        Vector3 JetStart = ArmRotation * this.JetArm;
        Quaternion JetRotation = Quaternion.Euler(0, JetIndex * 360 / this.Jets + this.JetAngles[JetIndex].y, this.JetAngles[JetIndex].z);
        Vector3 JetEnd = JetRotation * Vector3.down * this.JetLength + JetStart;
        return (JetStart, JetEnd);
    }

    private void SetJetAngle(int JetIndex, Vector3 EulerAngles) {
        this.JetAngles[JetIndex] = EulerAngles;
        (Vector3 JetStartPos, Vector3 JetEndPos) = this.GetLocalJetStartAndEnd(JetIndex);
        this.Colliders[2 * JetIndex].center = JetStartPos;
        this.Colliders[2 * JetIndex + 1].center = JetEndPos;
    }
}
