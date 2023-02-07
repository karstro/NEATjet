using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature
{
    private float Radius;
    private GameObject Object;
    private Rigidbody PhysicsBody;
    private SphereCollider[] JetEndColliders;
    private int Jets;
    private Vector3 JetArm;
    private Vector3[] JetAngles;
    private float JetLength;
    private float JetRadius;
    private float MaxJetAngleChangePerSecond;
    private float[] Thrusts;
    private float ThrustToWeight;
    private IBrain Brain;


    public float _Radius { get => Radius; set => Radius = value; }
    public GameObject _Object { get => Object; set => Object = value; }
    public Rigidbody _PhysicsBody { get => PhysicsBody; set => PhysicsBody = value; }
    public SphereCollider[] _JetEndColliders { get => JetEndColliders; set => JetEndColliders = value; }
    public int _Jets { get => Jets; set => Jets = value; }
    public float _JetLength { get => JetLength; set => JetLength = value; }
    public float _JetRadius { get => JetRadius; set => JetRadius = value; }
    public Vector3 _JetArm { get => JetArm; set => JetArm = value; }
    public Vector3[] _JetAngles { get => JetAngles; set => JetAngles = value; }
    public float _MaxJetAngleChangePerSecond { get => MaxJetAngleChangePerSecond; set => MaxJetAngleChangePerSecond = value; }
    public float _ThrustToWeight { get => ThrustToWeight; set => ThrustToWeight = value; }
    public float[] _Thrusts { get => Thrusts; set => Thrusts = value; }
    public IBrain _Brain { get => Brain; set => Brain = value; }
    
    public Vector3 Position
    {
        set => Object.transform.position = value;
        get => Object.transform.position;
    }
    public Quaternion Rotation
    {
        set => Object.transform.rotation = value;
        get => Object.transform.rotation;
    }

    // Creature is constructed by CreatureBuilder
    public Creature() { }

    // used for visualizing the results of a simulation
    // as such it does not need to have a rigidbody
    public Creature(State state, Creature c) {
        Object = new GameObject();
        SetPositionAndRotation(state.Position, state.Rotation);
        
        Radius = c.Radius;
        Jets = c.Jets;
        JetArm = c.JetArm;
        JetLength = c.JetLength;
        JetRadius = c.JetRadius;

        JetAngles = new Vector3[Jets];
        for (int JetIndex = 0; JetIndex < Jets; JetIndex++) {
            JetAngles[JetIndex] = state.JetAngles[JetIndex];
        }
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation) {
        Object.transform.SetPositionAndRotation(position, rotation);
    }
    
    // returns the world coordinates of the start and end of the given Jet
    public (Vector3, Vector3) GetJetStartAndEnd(int JetIndex) {
        Quaternion ArmRotation = Quaternion.Euler(0, JetIndex * 360 / Jets, 0);
        Vector3 RelativePosition = Rotation * (ArmRotation * JetArm);
        Vector3 JetStart = RelativePosition + Position;
        Quaternion JetRotation = Quaternion.Euler(0, JetIndex * 360 / Jets + JetAngles[JetIndex].y, JetAngles[JetIndex].z);
        Vector3 JetEnd = JetRotation * Vector3.down * JetLength + JetStart;
        return (JetStart, JetEnd);
    }

    // returns the local coordinates of the start and end of the given Jet
    public (Vector3, Vector3) GetLocalJetStartAndEnd(int JetIndex) {
        Quaternion ArmRotation = Quaternion.Euler(0, JetIndex * 360 / Jets, 0);
        Vector3 JetStart = ArmRotation * JetArm;
        Quaternion JetRotation = Quaternion.Euler(0, JetIndex * 360 / Jets + JetAngles[JetIndex].y, JetAngles[JetIndex].z);
        Vector3 JetEnd = JetRotation * Vector3.down * JetLength + JetStart;
        return (JetStart, JetEnd);
    }

    public void SetJetAngle(int JetIndex, Vector3 JetAngle) {
        // #TODO This should make sure JetAngles stay between -180 and 180
        JetAngles[JetIndex] = JetAngle;
        (Vector3 JetStartPos, Vector3 JetEndPos) = GetLocalJetStartAndEnd(JetIndex);
        if (JetEndColliders != null) {
            JetEndColliders[JetIndex].center = JetEndPos;
        }
    }

    public void Update(float time, float deltaTime/*, Senses senses*/) {
        // get the creature's intent from it's Brain
        (Vector3[] jetAngleIntents, float[] thrustIntents) = Brain.GetIntent(time, this);
        // update jetangles and the jets' colliders based on intent
        UpdateJetAnglesByIntent(jetAngleIntents, deltaTime);
        // #TODO update thrusts based on intent
        // #TODO apply force based on thrusts
    }

    private void UpdateJetAnglesByIntent(Vector3[] jetAngleIntents, float deltaTime) {
        // consider changing the maximum change to account for the total angle change instead of each angle change individually
        float MaxJetAngleChange = deltaTime * MaxJetAngleChangePerSecond;
        for (int JetIndex = 0; JetIndex < Jets; JetIndex++) {
            Vector3 nextJetAngle = Vector3.MoveTowards(JetAngles[JetIndex], jetAngleIntents[JetIndex], MaxJetAngleChange);
            SetJetAngle(JetIndex, nextJetAngle);
        }
    }
}
