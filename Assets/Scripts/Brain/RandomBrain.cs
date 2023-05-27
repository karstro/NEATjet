using UnityEngine;

public class RandomBrain : IBrain {
    private readonly Vector3[] JetAngles;
    private readonly float[] Thrusts;

    public RandomBrain(int jets) {
        JetAngles = new Vector3[jets];
        Thrusts = new float[jets];
    }

    public (Vector3[], float[]) GetIntent(float time, Creature creature) {
        for (int JetIndex = 0; JetIndex < creature.Jets; JetIndex++) {
            JetAngles[JetIndex] = RandomVector();
            Thrusts[JetIndex] = Random.Range(-1f, 1f);
        }
        return (JetAngles, Thrusts);
    }

    private Vector3 RandomVector() {
        Vector3 randomVector = new(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        );
        return randomVector.normalized;
    }
}
