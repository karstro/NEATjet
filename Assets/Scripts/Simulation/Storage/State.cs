using UnityEngine;

// struct that contains the state of a simulation step at a specific time
public readonly struct State
{
    public readonly float time;
    public readonly Vector3 Position;
    public readonly Quaternion Rotation;
    public readonly Vector3[] JetEnds;

    public State(float time, Creature creature) {
        this.time = time;

        (Vector3 position, Quaternion rotation) = creature.GetPositionAndRotation();
        Position = position;
        Rotation = rotation;

        JetEnds = new Vector3[creature.Jets];
        for (int jetIndex = 0; jetIndex < creature.Jets; jetIndex++) {
            JetEnds[jetIndex] = creature.GetLocalJetEnd(jetIndex);
        }
    }

    private State(float time, Vector3 position, Quaternion rotation, Vector3[] jetEnds)
    {
        this.time = time;
        Position = position;
        Rotation = rotation;
        JetEnds = jetEnds;
    }

    public static State Lerp(State A, State B, float fraction) {
        float InterpolatedTime = Mathf.Lerp(A.time, B.time, fraction);
        Vector3 InterpolatedPosition = Vector3.Lerp(A.Position, B.Position, fraction);
        Quaternion InterpolatedRotation = Quaternion.Slerp(A.Rotation, B.Rotation, fraction);

        int jets = A.JetEnds.Length;
        Vector3[] InterpolatedJetEnds = new Vector3[jets];
        for (int JetIndex = 0; JetIndex < jets; JetIndex++) {
            InterpolatedJetEnds[JetIndex] = Vector3.Lerp(A.JetEnds[JetIndex], B.JetEnds[JetIndex], fraction);
        }
        return new State(InterpolatedTime, InterpolatedPosition, InterpolatedRotation, InterpolatedJetEnds);
    }

    public static State LerpByTime(State Before, State After, float t) {
        float fraction = Mathf.InverseLerp(Before.time, After.time, t);
        Vector3 InterpolatedPosition = Vector3.Lerp(Before.Position, After.Position, fraction);
        Quaternion InterpolatedRotation = Quaternion.Slerp(Before.Rotation, After.Rotation, fraction);

        int jets = Before.JetEnds.Length;
        Vector3[] InterpolatedJetEnds = new Vector3[jets];
        for (int JetIndex = 0; JetIndex < jets; JetIndex++) {
            InterpolatedJetEnds[JetIndex] = Vector3.Lerp(Before.JetEnds[JetIndex], After.JetEnds[JetIndex], fraction);
        }
        return new State(t, InterpolatedPosition, InterpolatedRotation, InterpolatedJetEnds);
    }

    public override string ToString() {
        string s = "time: " + time.ToString() + ", position:" + Position.ToString() + ", rotation:" + Rotation.ToString();
        s += "\njetEnds: ";
        foreach (Vector3 jetEnd in JetEnds) {
            s += jetEnd.ToString() + ", ";
        }
        return s;
    }
}
