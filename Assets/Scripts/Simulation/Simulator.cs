using NeatJet.Scripts.Simulation.Creatures;
using NeatJet.Scripts.Simulation.Storage;

using UnityEngine;

namespace NeatJet.Scripts.Simulation
{
    /// <summary>
    /// Simulates a set of <see cref="JetCreature"/>s and stores what happens in <see cref="SimulationRun"/>s
    /// </summary>
    public class Simulator : MonoBehaviour
    {
        private int StepsPerSecond { get; set; }
        private int TimeOut { get; set; }
        private float DeltaTime { get; set; }
        private int Steps { get; set; }

        /// <summary>
        /// Add a <see cref="Simulator"/> component to the given GameObject.
        /// </summary>
        /// <param name="parent">The object to add the component to.</param>
        /// <param name="stepsPerSecond"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static Simulator Add(GameObject parent, int stepsPerSecond, int timeOut)
        {
            var simulator = parent.AddComponent<Simulator>();

            simulator.StepsPerSecond = stepsPerSecond;
            simulator.TimeOut = timeOut;

            // Calculate how big each timestep should be
            simulator.DeltaTime = 1f / simulator.StepsPerSecond;

            // Calculate how many steps will result from a simulation run
            simulator.Steps = (simulator.TimeOut * simulator.StepsPerSecond) + 1;

            // Disable automatic physics simulation so this class can manually control it
            if (Physics.autoSimulation)
            {
                Physics.autoSimulation = false;
            }

            return simulator;
        }

        /// <summary>
        /// Simulates the given creatures and returns the results in the same order the creatures were given.
        /// </summary>
        /// <param name="creatures">The creatures to simulate.</param>
        /// <returns>The SimulationRuns describing what happened to each creature.</returns>
        public SimulationRun[] SimulateCreatures(JetCreature[] creatures)
        {
            var creatureAmount = creatures.Length;
            var runs = new SimulationRun[creatureAmount];

            // Store each creature's state before the first timestep
            for (var creatureIndex = 0; creatureIndex < creatureAmount; creatureIndex++)
            {
                var creature = creatures[creatureIndex];
                runs[creatureIndex] = new SimulationRun(creature, Steps);
                runs[creatureIndex].AddState(new State(0, creature));
            }

            // run each step sequentially and save the state it ends up in
            for (var TimeStep = 1; TimeStep < Steps; TimeStep++)
            {
                var time = TimeStep * DeltaTime;
                foreach (var c in creatures)
                {
                    c.UpdateByIntent(time, DeltaTime);
                }

                // Unity simulates a physics step that is DeltaTime seconds long
                Physics.Simulate(DeltaTime);

                // Store each creature's state after the timestep
                for (var creatureIndex = 0; creatureIndex < creatureAmount; creatureIndex++)
                {
                    //Debug.Log(new State(time, Creatures[CreatureIndex]));
                    runs[creatureIndex].AddState(new State(time, creatures[creatureIndex]));
                }
            }

            return runs;
        }
    }
}