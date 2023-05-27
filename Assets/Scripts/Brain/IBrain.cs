using UnityEngine;

public interface IBrain {
    public (Vector3[], float[]) GetIntent(float time, Creature creature);
}
