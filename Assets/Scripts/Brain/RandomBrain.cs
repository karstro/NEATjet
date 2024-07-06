using NeatJet.Scripts.Simulation.Creatures;
using NeatJet.Scripts.Simulation.Creatures.Genes;

using UnityEngine;

namespace NeatJet.Scripts.Brain
{
    public class RandomBrain : MonoBehaviour, IBrain
    {
        private Vector3[] JetAngles;
        private float[] Thrusts;

        public static RandomBrain Add(GameObject gameObject, Gene gene)
        {
            var brain = gameObject.AddComponent<RandomBrain>();
            brain.JetAngles = new Vector3[gene.Jets];
            brain.Thrusts = new float[gene.Jets];
            return brain;
        }

        public (Vector3[], float[]) GetIntent(float time, JetCreature creature)
        {
            for (var JetIndex = 0; JetIndex < creature.JetNumber; JetIndex++)
            {
                JetAngles[JetIndex] = RandomVector();
                Thrusts[JetIndex] = Random.Range(-1f, 1f);
            }

            return (JetAngles, Thrusts);
        }

        private Vector3 RandomVector()
        {
            Vector3 randomVector = new(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            );
            return randomVector.normalized;
        }
    }
}
