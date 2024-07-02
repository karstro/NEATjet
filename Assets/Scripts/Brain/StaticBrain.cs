using UnityEngine;

public class StaticBrain : IBrain
{
    private readonly Vector3[] JetDirections;
    private readonly float[] Thrusts;

    public StaticBrain(Vector3[] jetDirections, float[] thrusts)
    {
        JetDirections = jetDirections;
        Thrusts = thrusts;
    }

    public StaticBrain(int jets)
    {
        JetDirections = new Vector3[jets];
        Thrusts = new float[jets];
        for (var JetIndex = 0; JetIndex < jets; JetIndex++)
        {
            if (JetIndex % 2 == 0)
            {
                JetDirections[JetIndex] = Vector3.down;
            }
            else
            {
                //JetDirections[JetIndex] = Vector3.right;
                JetDirections[JetIndex] = Vector3.up;
                //JetDirections[JetIndex] = Vector3.down;
            }

            //JetDirections[JetIndex] = (Vector3.right + Vector3.down) * 0.5f;

            Thrusts[JetIndex] = 0f;
        }
    }

    public (Vector3[], float[]) GetIntent(float time, Creature c)
    {
        return (JetDirections, Thrusts);
    }
}
