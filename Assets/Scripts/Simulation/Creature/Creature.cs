using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Creature : CreatureBase
{
    private Rigidbody PhysicsBody;
    private float MaxThrustChangePerSecond;
    private float MaxThrust;
    private float[] ThrustFractions;
    private IBrain Brain;
    private ConfigurableJoint[] JetJoints;
    private Quaternion[] JetRotationsAtStart;
    private bool RecordedRotations = false;

    public Rigidbody _PhysicsBody { get => PhysicsBody; set => PhysicsBody = value; }
    public float _MaxThrustChangePerSecond { get => MaxThrustChangePerSecond; set => MaxThrustChangePerSecond = value; }
    public float _MaxThrust { get => MaxThrust; set => MaxThrust = value; }
    public float[] _ThrustFractions { get => ThrustFractions; set => ThrustFractions = value; }
    public IBrain _Brain { get => Brain; set => Brain = value; }
    public ConfigurableJoint[] _JetJoints { get => JetJoints; set => JetJoints = value; }

    // Creature is constructed by CreatureBuilder
    public Creature() { }

    public (Vector3, Quaternion) GetPositionAndRotation() {
        return (Object.transform.position, Object.transform.rotation);
    }

    private Vector3[] actualIntents = new Vector3[4];

    private Quaternion GetJetSpaceTargetRotation(Vector3 creatureSpacetargetRotation, int jetIndex) {
        // make the targetRotation point in the same direction as the jetArm
        //creatureSpacetargetRotation = CalculateJetArmRotation(jetIndex, Jets) * creatureSpacetargetRotation;
        // translate targetRotation from Creature's space to the jetStart's localSpace
        //Matrix4x4 creatureSpaceToJetSpace = JetStarts[jetIndex].transform.worldToLocalMatrix * Object.transform.localToWorldMatrix;
        //Vector3 jetSpaceTargetRotation = Quaternion.Inverse(GetCurrentJetRotation(jetIndex)) * creatureSpacetargetRotation;
        //Vector3 jetSpaceTargetRotation = GetCurrentJetRotation(jetIndex) * creatureSpacetargetRotation;
        // temporary, for visualizations
        //actualIntents[jetIndex] = jetSpaceTargetRotation;
        // turn the result into a quaternion that looks toward jetSpaceTargetRotation
        //Quaternion targetRotation = Quaternion.LookRotation(jetSpaceTargetRotation, Vector3.up);
        // turn the result into a quaternion that rotates from the starting position toward the target rotation
        //Quaternion targetRotation = Quaternion.FromToRotation(Vector3.down, jetSpaceTargetRotation);

        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.down, creatureSpacetargetRotation);
        return targetRotation;
    }

    private void UpdateJoints(Vector3[] targetRotations) {
        for (int jetIndex = 0; jetIndex < Jets; jetIndex++) {
            //Quaternion startRotation = JetRotationsAtStart[jetIndex];
            Quaternion startRotation = Quaternion.identity;
            Quaternion targetRotation = GetJetSpaceTargetRotation(targetRotations[jetIndex], jetIndex);
            Debug.Log("Leg " + jetIndex + ", start = " + startRotation + ", current = " + JetStarts[jetIndex].transform.localRotation + ", target = " + targetRotation);
            JetJoints[jetIndex].SetTargetRotationLocal(targetRotation, startRotation);
        }
    }

    private void UpdateJetLimb(GameObject limb, Vector3 start, Vector3 end) {
        // the cylinder's center is the position between the start and end positions
        limb.transform.localPosition = (start + end) / 2;
        // change the rotation of the cylinder such that it points from the start toward end
        Vector3 StartToEnd = end - start;
        limb.transform.localRotation = Quaternion.FromToRotation(Vector3.up, StartToEnd);
    }

    private void UpdateJetLimbs() {
        for (int jetIndex = 0; jetIndex < Jets; jetIndex++) {
            (Vector3 start, Vector3 end) = GetLocalJetStartAndEnd(jetIndex);
            UpdateJetLimb(JetArms[jetIndex], Vector3.zero, start);
            UpdateJetLimb(JetLegs[jetIndex], start, end);
        }
    }

    private void UpdatePhysicsComponents(float time) {
        // get the creature's intent from it's Brain
        (Vector3[] rotationIntents, float[] thrustIntents) = Brain.GetIntent(time, this);
        // save intents for visualization purposes (temporary)
        Intents = rotationIntents;
        // update the joints based on intent if they exist
        if (JetJoints != null) {
            UpdateJoints(rotationIntents);
        }
        // #TODO update thrusts based on intent
        // #TODO apply force at jets based on thrusts
    }

    private Vector3[] Intents;

    // update what the creature is doing based on its brain's intent
    public void Update(float time, float deltaTime/*, Senses senses*/) {
        if (!RecordedRotations) {
            RecordedRotations = true;
            JetRotationsAtStart = new Quaternion[Jets];
            for (int jetIndex = 0; jetIndex < Jets; jetIndex++) {
                JetRotationsAtStart[jetIndex] = JetStarts[jetIndex].transform.localRotation;
            }
        }
        // update all physics based components
        if (Brain != null) {
            UpdatePhysicsComponents(time);
        }

        // update the position of the jet's limbs if they exist
        if (JetArms != null) {
            UpdateJetLimbs();
        }
    }
    public void OnDrawGizmos() {
        Gizmos.color = Color.white;
        if (Intents != null) {
            for (int jetIndex = 0; jetIndex < Jets; jetIndex++) {
                Vector3 start = JetStarts[jetIndex].transform.position;
                Quaternion objectRotation = Object.transform.rotation;
                Vector3 localIntent = CalculateJetArmRotation(jetIndex, Jets) * Intents[jetIndex].normalized;
                Vector3 worldIntent = objectRotation * localIntent;
                Vector3 end = start + worldIntent;
                Gizmos.DrawLine(start, end);
            }
        }
        Gizmos.color = Color.green;
        if (actualIntents != null) {
            for (int jetIndex = 0; jetIndex < Jets; jetIndex++) {
                Vector3 start = JetStarts[jetIndex].transform.position;
                //Quaternion objectRotation = Quaternion.Inverse(JetStarts[jetIndex].transform.rotation);
                Quaternion targetRotation = GetJetSpaceTargetRotation(Intents[jetIndex], jetIndex);
                Quaternion objectRotation = Object.transform.rotation;
                Vector3 localIntent = CalculateJetArmRotation(jetIndex, Jets) * targetRotation * Vector3.down;
                Vector3 worldIntent = objectRotation * localIntent;
                Vector3 end = start + worldIntent;
                Gizmos.DrawLine(start, end);
            }
        }
        //Gizmos.color = Color.white;
        //if (Intents != null) {
        //    for (int jetIndex = 0; jetIndex < Jets; jetIndex++) {
        //        Vector3 start = JetStarts[jetIndex].transform.position;
        //        Quaternion objectRotation = Object.transform.rotation;
        //        Vector3 localIntent = CalculateJetArmRotation(jetIndex, Jets) * Intents[jetIndex].normalized;
        //        Vector3 worldIntent = objectRotation * localIntent;
        //        Vector3 end = start + worldIntent;
        //        Gizmos.DrawLine(start, end);
        //    }
        //}
        //Gizmos.color = Color.green;
        //if (actualIntents != null) {
        //    for (int jetIndex = 0; jetIndex < Jets; jetIndex++) {
        //        Vector3 start = JetStarts[jetIndex].transform.position;
        //        Quaternion objectRotation = JetStarts[jetIndex].transform.rotation;
        //        Vector3 localIntent = intendedRotations[jetIndex] * Vector3.down;
        //        Vector3 worldIntent = objectRotation * localIntent;
        //        //Vector3 worldIntent = intendedRotations[jetIndex] * Vector3.down;
        //        Vector3 end = start + worldIntent;
        //        Gizmos.DrawLine(start, end);
        //    }
        //}
    }
}
