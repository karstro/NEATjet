using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class that contains the state of a simulation step at a certain time
public readonly struct State
{
    public readonly float time;
    public readonly Vector3 Position;
    public readonly Quaternion Rotation;

    public State(float time, Vector3 Position, Quaternion Rotation) {
        this.time = time;
        this.Position = Position;
        this.Rotation = Rotation;
    }

    public State(float time, Creature c) {
        this.time = time;
        this.Position = c.Position;
        this.Rotation = c.Rotation;
    }

    public static State Lerp(State A, State B, float fraction) {
        float InterpolatedTime = Mathf.Lerp(A.time, B.time, fraction);
        Vector3 InterpolatedPosition = Vector3.Lerp(A.Position, B.Position, fraction);
        Quaternion InterpolatedRotation = Quaternion.Slerp(A.Rotation, B.Rotation, fraction);
        return new State(InterpolatedTime, InterpolatedPosition, InterpolatedRotation);
    }

    public static State LerpByTime(State Before, State After, float t) {
        float fraction = Mathf.InverseLerp(Before.time, After.time, t);
        Vector3 InterpolatedPosition = Vector3.Lerp(Before.Position, After.Position, fraction);
        Quaternion InterpolatedRotation = Quaternion.Slerp(Before.Rotation, After.Rotation, fraction);
        return new State(t, InterpolatedPosition, InterpolatedRotation);
    }

    public override string ToString() {
        return this.time.ToString() + ", " + this.Position.ToString() + ", " + this.Rotation.ToString();
    }
}
