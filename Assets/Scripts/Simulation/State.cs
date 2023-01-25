using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override string ToString() {
        return this.time.ToString() + ", " + this.Position.ToString() + ", " + this.Rotation.ToString();
    }
}
