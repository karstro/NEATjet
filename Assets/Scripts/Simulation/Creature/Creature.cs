using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : CreatureBase
{
    private Rigidbody PhysicsBody;
    private SphereCollider[] JetEndColliders;
    private float MaxJetAngleChangePerSecond;
    private float[] Thrusts;
    private float ThrustToWeight;
    private IBrain Brain;

    public Rigidbody _PhysicsBody { get => PhysicsBody; set => PhysicsBody = value; }
    public SphereCollider[] _JetEndColliders { get => JetEndColliders; set => JetEndColliders = value; }
    public float _MaxJetAngleChangePerSecond { get => MaxJetAngleChangePerSecond; set => MaxJetAngleChangePerSecond = value; }
    public float _ThrustToWeight { get => ThrustToWeight; set => ThrustToWeight = value; }
    public float[] _Thrusts { get => Thrusts; set => Thrusts = value; }
    public IBrain _Brain { get => Brain; set => Brain = value; }

    // Creature is constructed by CreatureBuilder
    public Creature() { }

    public override void SetPositionAndRotation(Vector3 position, Quaternion rotation) {
        Object.transform.SetPositionAndRotation(position, rotation);
    }

    public void SetJetAngle(int jetIndex, Vector3 jetAngle) {
        // make sure JetAngles stay between -180 and 180
        for (int dim = 0; dim < 3; dim++) {
            jetAngle[dim] = Mathf.Clamp(jetAngle[dim], -180, 180);
        }
        // update the JetAngle
        JetAngles[jetIndex] = jetAngle;
        // update the Collider of the jet's end. the jet's start collider is stationary and does not need to be updated
        (_, Vector3 jetEndPos) = GetLocalJetStartAndEnd(jetIndex);
        JetEndColliders[jetIndex].center = jetEndPos;
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
        float MaxJetAngleChange = deltaTime * MaxJetAngleChangePerSecond;
        for (int JetIndex = 0; JetIndex < Jets; JetIndex++) {
            Vector3 nextJetAngle = Vector3.MoveTowards(JetAngles[JetIndex], jetAngleIntents[JetIndex], MaxJetAngleChange);
            SetJetAngle(JetIndex, nextJetAngle);
        }
    }
}
