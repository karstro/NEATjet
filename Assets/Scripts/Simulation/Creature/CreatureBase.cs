using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureBase
{
    protected float Radius;
    protected int Jets;
    protected float JetLength;
    protected float JetRadius;
    protected Vector3 JetArm;
    protected GameObject Object;
    protected GameObject BodyObject;
    protected GameObject[] JetStarts;
    protected GameObject[] JetEnds;
    protected GameObject[] JetArms;
    protected GameObject[] JetLegs;

    public float _Radius { get => Radius; set => Radius = value; }
    public int _Jets { get => Jets; set => Jets = value; }
    public float _JetLength { get => JetLength; set => JetLength = value; }
    public float _JetRadius { get => JetRadius; set => JetRadius = value; }
    public Vector3 _JetArm { get => JetArm; set => JetArm = value; }
    public GameObject _Object { get => Object; set => Object = value; }
    public GameObject _BodyObject { get => BodyObject; set => BodyObject = value; }
    public GameObject[] _JetStarts { get => JetStarts; set => JetStarts = value; }
    public GameObject[] _JetEnds { get => JetEnds; set => JetEnds = value; }
    public GameObject[] _JetArms { get => JetArms; set => JetArms = value; }
    public GameObject[] _JetLegs { get => JetLegs; set => JetLegs = value; }

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
    public (Vector3, Vector3) GetLocalJetStartAndEnd(int JetIndex)
    {
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
