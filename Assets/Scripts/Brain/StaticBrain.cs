using NeatJet.Scripts.Simulation.Creatures;
using NeatJet.Scripts.Simulation.Creatures.Genes;

using UnityEngine;

namespace NeatJet.Scripts.Brain
{
    public class StaticBrain : MonoBehaviour, IBrain
    {
        private Vector3[] JetDirections { get; set; }
        private float[] Thrusts { get; set; }

        public static void Add(GameObject creatureObject, Gene gene)
        {
            var brain = creatureObject.AddComponent<StaticBrain>();
            brain.SetDirectionsFromJets(gene.Jets);
        }

        private void SetDirectionsFromJets(int jets)
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

        public (Vector3[], float[]) GetIntent(float time, JetCreature c)
        {
            return (JetDirections, Thrusts);
        }
    }
}
