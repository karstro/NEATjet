using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBrain : IBrain
{
    public (Vector3[], float[]) GetIntent(float time, Creature c) {
        Vector3[] jetAngles = new Vector3[c.Jets];
        float[] thrusts = new float[c.Jets];
        for (int JetIndex = 0; JetIndex < c.Jets; JetIndex++) {
            jetAngles[JetIndex] = this.RandomJetAngle();
            thrusts[JetIndex] = Random.Range(-1f, 1f);
        }
        return (jetAngles, thrusts);
    }

    private Vector3 RandomJetAngle() {
        return new Vector3(
                Random.Range(-180f, 180f),
                0f,
                Random.Range(-180f, 180f)
            );
    }
}
