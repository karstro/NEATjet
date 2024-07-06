using NeatJet.Scripts.Simulation;
using NeatJet.Scripts.Simulation.Creatures;
using NeatJet.Scripts.Simulation.Creatures.Genes;
using NeatJet.Scripts.Simulation.Storage;
using NeatJet.Scripts.Visualization;

using UnityEngine;

namespace Assets.Scripts.Evolution
{
    public class Generation : MonoBehaviour
    {
        public int GenerationSize = 1;
        public int StepsPerSecond = 60;
        public int TimeOut = 15;

        private JetCreature[] Creatures;
        private Simulator Sim;
        private SimulationRun[] Runs;
        private SimulationVisualiser Visualiser;

        void Start()
        {
            Sim = Simulator.Add(gameObject, StepsPerSecond, TimeOut);

            var genes = GenerateGenes();
            GenerateCreatures(genes);
        }

        private void StartSimulation()
        {
            Runs = Sim.SimulateCreatures(Creatures);
            DestroyCreatures();

            BeginCreatureVisualisation();
        }

        private Gene[] GenerateGenes()
        {
            var genes = new Gene[GenerationSize];
            for (var i = 0; i < GenerationSize; i++)
            {
                genes[i] = JetCreatureBuilder.DefaultCreatureGene;
            }

            return genes;
        }

        void GenerateCreatures(Gene[] genes)
        {
            Creatures = new JetCreature[GenerationSize];
            for (var i = 0; i < GenerationSize; i++)
            {
                Creatures[i] = JetCreatureBuilder.CreateCreature(genes[i], "SimulatedCreature" + i, false);
            }
        }

        void DestroyCreatures()
        {
            for (var i = 0; i < GenerationSize; i++)
            {
                Creatures[i].Destroy();
            }
        }

        void BeginCreatureVisualisation()
        {
            Visualiser = SimulationVisualiser.Add(gameObject, Runs);
        }

        void Update()
        {
            if (Visualiser == null)
            {
                StartSimulation();
            }
        }

        void OnGUI()
        {
            if (Visualiser != null)
            {
                Visualiser.OnGUI();
            }
        }
    }
}