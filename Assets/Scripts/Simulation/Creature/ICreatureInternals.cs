using UnityEngine;

public interface ICreatureInternals {
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

    public Rigidbody PhysicsBody { get; set; }
    public float MaxThrustChangePerSecond { get; set; }
    public float MaxThrust { get; set; }
    public float[] ThrustFractions { get; set; }

    public (Vector3, Quaternion) GetPositionAndRotation();
    public void SetPositionAndRotation(Vector3 position, Quaternion rotation);
    public Vector3 GetLocalJetStart(int JetIndex);
    public Vector3 GetLocalJetEnd(int JetIndex);
    public (Vector3, Vector3) GetLocalJetStartAndEnd(int JetIndex);

    public void Update(float time, float deltaTime, Vector3[] rotationIntents, float[] thrustIntents);
    public void Destroy();
    public void MatchState(State state);
}
