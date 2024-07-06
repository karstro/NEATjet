using NeatJet.Scripts.Simulation.Creatures;

using UnityEngine;

namespace NeatJet.Scripts.Simulation.Storage
{
    // stores one creature's run as a sequence of states.
    public class SimulationRun
    {
        public readonly float Radius;
        public readonly int Jets;
        public readonly Vector3 JetArm;
        public readonly float JetLength;
        public readonly float JetRadius;

        private int NumStates;
        private readonly State[] States;

        public float MaximumTime { get => States[^1].Time; }
        public readonly int MaxStates;

        // Initialize empty SimulationRun, to be filled by a simulation
        public SimulationRun(JetCreature creature, int maxStates)
        {
            Radius = creature.Radius;
            Jets = creature.JetNumber;

            var firstJet = creature.Jets[0];
            JetArm = firstJet.Arm;
            JetLength = firstJet.Length;
            JetRadius = firstJet.Radius;

            MaxStates = maxStates;
            NumStates = 0;
            States = new State[MaxStates];
        }

        // Initialize SimulationRun from an existing list of states.
        public SimulationRun(JetCreature creature, State[] states)
        {
            Radius = creature.Radius;
            Jets = creature.JetNumber;

            var firstJet = creature.Jets[0];
            JetArm = firstJet.Arm;
            JetLength = firstJet.Length;
            JetRadius = firstJet.Radius;

            MaxStates = states.Length;
            NumStates = states.Length;
            States = states;
        }

        public void AddState(State state)
        {
            States[NumStates] = state;
            NumStates++;
        }

        public State GetStateAtExactTime(float time)
        {
            // find where time is within the States array
            var beforeStateIndex = FindStateBeforeTime(time);

            // linearly interpolate between the two states around the result using time as the fraction.
            var before = States[beforeStateIndex];
            var after = States[beforeStateIndex + 1];
            return State.LerpByTime(before, after, time);
        }

        // Find the index of the state before the current time
        private int FindStateBeforeTime(float time)
        {
            // binary search algorithm, since time is ordered in the states array
            var low = 0;
            var high = States.Length - 2;
            var result = 0;

            // if the target time is out of bounds, clamp it within bounds
            time = Mathf.Clamp(time, States[low].Time, States[high + 1].Time);

            // keep count of iterations in case of endless looping
            var i = 0;
            var maxSearchIterations = 100;
            // while the search hasn't converged
            while (true)
            {
                // check if search has gone on too long
                if (i > maxSearchIterations)
                {
                    Debug.Log("Could not find time " + time + " in States array.");
                    break;
                }

                i++;

                // calculate the middle of the search range
                var mid = (low + high) / 2;

                // check where the wanted time is relative to the middle of the range
                if (time < States[mid].Time)
                {
                    // if time is below the middle range, restrict the search range to the lower half
                    high = mid - 1;
                }
                else if (time > States[mid + 1].Time)
                {
                    // if time is above the middle range, restrict the search range to the upper half
                    low = mid + 1;
                }
                else
                {
                    // if time is in the middle range, the search is complete
                    result = mid;
                    break;
                }
            }

            return result;
        }

        public State GetFirstState()
        {
            return States[0];
        }

        //public override string ToString() {
        //    string s = NumStates + " states\n";
        //    for (int i = 0; i < NumStates; i++) {
        //        s += "state " + i + ": " + States[i] + "\n";
        //    }
        //    return s;
        //}
    }
}
