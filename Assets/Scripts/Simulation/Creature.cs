using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature
{
    public float Radius;
    public float Mass;
    public Vector3 Position;
    public Vector3 Velocity;
    public Quaternion Rotation;
    public Vector3 Angular;
    public int Jets;
    private Vector3 JetPosition;
    public Vector3[] JetAngles;
    private float JetLength;
    public float JetRadius;
    public float[] Thrusts;

    public Creature() {
        this.Radius = .5f;
        this.Mass = 1f;
        this.Position = new Vector3(0f, .75f, 0f);
        this.Velocity = new Vector3(0.5f, 0f, 0f);
        this.Rotation = Quaternion.identity;
        this.Angular = 2 * Vector3.up;
        this.Jets = 4;
        this.JetPosition = new Vector3(1f, 0.5f, 0f);
        this.JetAngles = new Vector3[this.Jets];
        for (int i = 0; i < this.Jets; i++) {
            this.JetAngles[i] = Vector3.zero;
        }
        this.JetLength = 1f;
        this.JetRadius = 0.2f;

        this.JetAngles[0] = new Vector3(0, 0, 90);
    }

    // returns the world coordinates of the start of the given Jet
    public Vector3 GetJetStart(int JetIndex) {
        Quaternion JetRotation = Quaternion.Euler(0, JetIndex * 360 / this.Jets, 0);
        Vector3 RelativePosition = this.Rotation * (JetRotation * this.JetPosition);
        return RelativePosition + this.Position;
    }

    // returns the world coordinates of the end of the given Jet
    public Vector3 GetJetEnd(int JetIndex) {
        Vector3 Start = this.GetJetStart(JetIndex);
        Quaternion JetRotation = Quaternion.Euler(0, JetIndex * 360 / this.Jets + this.JetAngles[JetIndex].y, this.JetAngles[JetIndex].z);
        return JetRotation * Vector3.down * this.JetLength + Start;
    }
    
    // returns both jetstart and end
    public (Vector3, Vector3) GetJetStartAndEnd(int JetIndex) {
        Quaternion JetRotation = Quaternion.Euler(0, JetIndex * 360 / this.Jets, 0);
        Vector3 RelativePosition = this.Rotation * (JetRotation * this.JetPosition);
        Vector3 JetStart = RelativePosition + this.Position;
        JetRotation = Quaternion.Euler(0, JetIndex * 360 / this.Jets + this.JetAngles[JetIndex].y, this.JetAngles[JetIndex].z);
        Vector3 JetEnd = JetRotation * Vector3.down * this.JetLength + JetStart;
        return (JetStart, JetEnd);
    }
    // maybe add versions that gives it in relative space, i.e. without this.Position?
}
