using NeatJet.Scripts.Simulation.Creatures;

using UnityEngine;

namespace NeatJet.Scripts.Evolution
{
    public class RealtimeGeneration : MonoBehaviour
    {
        [Range(1, 100)]
        public int Amount;

        private JetCreature[] Creatures { get; set; }

        // Use this for initialization
        void Start()
        {
            Creatures = new JetCreature[Amount];

            for (var i = 0; i < Amount; i++)
            {
                Creatures[i] = JetCreatureBuilder.CreateCreature(JetCreatureBuilder.DefaultCreatureGene, "TestCreature" + i);
            }
        }

        // Update is called once per frame
        void Update()
        {
            var time = Time.time;
            var deltaTime = Time.deltaTime;

            foreach (var creature in Creatures)
            {
                creature.UpdateByIntent(time, deltaTime);
            }
        }
    }
}