using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class that contains the state of a simulation step at a certain time
public readonly struct State
{
    public readonly float time;
    public readonly Vector3 Position;
    public readonly Quaternion Rotation;
    public readonly Vector3[] JetAngles;

    public State(float time, Vector3 Position, Quaternion Rotation, Vector3[] jetAngles) {
        this.time = time;
        this.Position = Position;
        this.Rotation = Rotation;
        this.JetAngles = jetAngles;
    }

    public State(float time, Creature c) {
        this.time = time;
        this.Position = c.Position;
        this.Rotation = c.Rotation;
        // this.JetAngles = c.JetAngles;
        this.JetAngles = new Vector3[c.Jets];
        c.JetAngles.CopyTo(this.JetAngles, 0);
    }

    public static State Lerp(State A, State B, float fraction) {
        float InterpolatedTime = Mathf.Lerp(A.time, B.time, fraction);
        Vector3 InterpolatedPosition = Vector3.Lerp(A.Position, B.Position, fraction);
        Quaternion InterpolatedRotation = Quaternion.Slerp(A.Rotation, B.Rotation, fraction);
        int Jets = A.JetAngles.Length;
        Vector3[] InterpolatedJetAngles = new Vector3[Jets];
        for (int JetIndex = 0; JetIndex < Jets; JetIndex++) {
            InterpolatedJetAngles[JetIndex] = Vector3.Lerp(A.JetAngles[JetIndex], B.JetAngles[JetIndex], fraction);
        }
        return new State(InterpolatedTime, InterpolatedPosition, InterpolatedRotation, InterpolatedJetAngles);
    }

    public static State LerpByTime(State Before, State After, float t) {
        float fraction = Mathf.InverseLerp(Before.time, After.time, t);
        Vector3 InterpolatedPosition = Vector3.Lerp(Before.Position, After.Position, fraction);
        Quaternion InterpolatedRotation = Quaternion.Slerp(Before.Rotation, After.Rotation, fraction);
        int Jets = Before.JetAngles.Length;
        Vector3[] InterpolatedJetAngles = new Vector3[Jets];
        for (int JetIndex = 0; JetIndex < Jets; JetIndex++) {
            InterpolatedJetAngles[JetIndex] = Vector3.Lerp(Before.JetAngles[JetIndex], After.JetAngles[JetIndex], fraction);
        }
        return new State(t, InterpolatedPosition, InterpolatedRotation, InterpolatedJetAngles);
    }

    public override string ToString() {
        string s = this.time.ToString() + ", " + this.Position.ToString() + ", " + this.Rotation.ToString();
        foreach(Vector3 JetAngle in this.JetAngles) {
            s += ", " + JetAngle.ToString();
        }
        return s;
    }
}
