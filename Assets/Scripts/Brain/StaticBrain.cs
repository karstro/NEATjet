using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBrain : IBrain
{
    public (Vector3[], float[]) GetIntent(float time, Creature c) {
        Vector3[] jetAngles = new Vector3[c.Jets];
        float[] thrusts = new float[c.Jets];
        for (int JetIndex = 0; JetIndex < c.Jets; JetIndex++) {
            if (JetIndex % 2 == 0) {
                jetAngles[JetIndex] = Vector3.zero;
            } else {
                jetAngles[JetIndex] = new Vector3(0, 0, 90);
            }
            // jetAngles[JetIndex] = Vector3.zero;
            thrusts[JetIndex] = 0f;
        }
        return (jetAngles, thrusts);
    }
}
