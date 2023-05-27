using UnityEngine;

public abstract class CreatureInternals
{
    public float Radius { get; set; }
    public int Jets { get; set; }
    public float JetLength { get; set; }
    public float JetRadius { get; set; }
    public Vector3 JetArm { get; set; }
    public GameObject Object { get; set; }
    public GameObject BodyObject { get; set; }
    public GameObject[] JetStarts { get; set; }
    public GameObject[] JetEnds { get; set; }
    public GameObject[] JetArms { get; set; }
    public GameObject[] JetLegs { get; set; }

    // gets the position and rotation of the creature's GameObject
    public (Vector3, Quaternion) GetPositionAndRotation() {
        return (Object.transform.position, Object.transform.rotation);
    }

    // set the position and rotation of the creature's GameObject
    public void SetPositionAndRotation(Vector3 position, Quaternion rotation) {
        Object.transform.SetPositionAndRotation(position, rotation);
    }

    // returns the local coordinates of the start of the given Jet
    public Vector3 GetLocalJetStart(int JetIndex) {
        return JetStarts[JetIndex].transform.localPosition;
    }

    // returns the local coordinates of the end of the given Jet
    public Vector3 GetLocalJetEnd(int JetIndex) {
        return JetEnds[JetIndex].transform.localPosition;
    }

    // returns the local coordinates of the start and end of the given Jet
    public (Vector3, Vector3) GetLocalJetStartAndEnd(int JetIndex) {
        Vector3 jetStart = JetStarts[JetIndex].transform.localPosition;
        Vector3 jetEnd = JetEnds[JetIndex].transform.localPosition;
        return (jetStart, jetEnd);
    }

    // returns a Quaternion describing the rotation of the given jet's arm relative to the creature's forward direction
    public static Quaternion CalculateJetArmRotation(int jetIndex, int jets) {
        // the angle in degrees
        float armFacingAngle = jetIndex * 360 / jets;
        // a rotation of that many degrees around the Y axis
        return Quaternion.Euler(0, armFacingAngle, 0); ;
    }
}
