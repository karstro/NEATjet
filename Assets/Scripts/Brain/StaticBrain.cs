using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBrain : IBrain
{
    public (Vector3[], float[]) GetIntent(float time, Creature c) {
        Vector3[] jetDirections = new Vector3[c._Jets];
        float[] thrusts = new float[c._Jets];
        for (int JetIndex = 0; JetIndex < c._Jets; JetIndex++) {
            if (JetIndex % 2 == 0) {
                jetDirections[JetIndex] = Vector3.down;
            } else {
                jetDirections[JetIndex] = Vector3.right;
                //jetDirections[JetIndex] = Vector3.up;
                //jetDirections[JetIndex] = Vector3.down;
            }
            // jetAngles[JetIndex] = Vector3.zero;
            thrusts[JetIndex] = 0f;
        }
        return (jetDirections, thrusts);
    }
}
