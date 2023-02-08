using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureBase
{
    protected GameObject Object;
    protected float Radius;
    protected int Jets;
    protected float JetLength;
    protected float JetRadius;
    protected Vector3 JetArm;
    protected Vector3[] JetAngles;


    public GameObject _Object { get => Object; set => Object = value; }
    public float _Radius { get => Radius; set => Radius = value; }
    public int _Jets { get => Jets; set => Jets = value; }
    public float _JetLength { get => JetLength; set => JetLength = value; }
    public float _JetRadius { get => JetRadius; set => JetRadius = value; }
    public Vector3 _JetArm { get => JetArm; set => JetArm = value; }
    public Vector3[] _JetAngles { get => JetAngles; set => JetAngles = value; }
    
    public Vector3 Position { get => Object.transform.position; }
    public Quaternion Rotation { get => Object.transform.rotation; }

    // set the position and rotation of the creature's GameObject
    public abstract void SetPositionAndRotation(Vector3 position, Quaternion rotation);

    // returns the local coordinates of the start and end of the given Jet
    public (Vector3, Vector3) GetLocalJetStartAndEnd(int JetIndex) {
        // calculate which direction the arm faces relative to the creature's facing
        float armFacingAngle = JetIndex * 360 / Jets;
        Quaternion armRotation = Quaternion.Euler(0, armFacingAngle, 0);
        // the start of the jet is at the end of the JetArm
        Vector3 jetStart = armRotation * JetArm;
        // the rotation of the jet relative to the arm
        Quaternion jetRotation = Quaternion.Euler(0, JetAngles[JetIndex].y, JetAngles[JetIndex].z);
        // the vector that travels from the jet's start to the jet's end;
        Vector3 jetVector = armRotation * jetRotation * Vector3.down * JetLength;
        Vector3 jetEnd = jetVector + jetStart;
        return (jetStart, jetEnd);
    }
}
