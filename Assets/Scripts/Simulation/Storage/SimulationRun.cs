using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// to avoid using multidimensional arrays of states for multiple simulation runs, store one creature's run in one object.
public class SimulationRun
{
    private Creature C;
    private int NumStates;
    private State[] States;
    private bool Finished;
    private float MaximumTime;

    public readonly int MaxStates;
    // public int _NumStates { get => NumStates; }
    // public State[] _States { get => States; }
    public float _MaximumTime {get => MaximumTime; }

    // Initialize empty SimulationRun, to be filled by a simulation
    public SimulationRun(Creature c, int maxStates) {
        C = c;

        MaxStates = maxStates;
        NumStates = 0;
        States = new State[MaxStates];

        Finished = false;
    }

    // Initialize SimulationRun from an existing list of states.
    public SimulationRun(Creature c, State[] states) {
        C = c;

        MaxStates = states.Length;
        NumStates = states.Length;
        States = states;

        Finished = true;
    }

    public void AddState(State state) {
        if (Finished || NumStates >= MaxStates) {
            Debug.Log("Tried to add state when SimulationRun is already Finished.");
            return;
        }
        States[NumStates] = state;
        NumStates++;
    }

    public void Finish() {
        if (Finished) {
            Debug.Log("Tried to Finish an already Finished SimulationRun.");
        }
        if (NumStates < MaxStates) {
            Debug.Log("Tried to Finish SimulationRun while not full, only " + NumStates + "/" + MaxStates + " States.");
            return;
        }
        Finished = true;
        State lastState = States[MaxStates - 1];
        MaximumTime = lastState.time;
    }

    public State GetStateBeforeTime(float time) {
        // find where time is within the States array
        int BeforeStateIndex = FindStateBeforeTime(time);
        return States[BeforeStateIndex];
    }

    public State GetStateAtExactTime(float time) {
        // find where time is within the States array
        int BeforeStateIndex = FindStateBeforeTime(time);

        // linearly interpolate between the two states around the result using time as the fraction.
        State Before = States[BeforeStateIndex];
        State After = States[BeforeStateIndex + 1];
        return State.LerpByTime(Before, After, time);
    }
    
    // Find the index of the state before the current time
    private int FindStateBeforeTime(float time) {
        // binary search algorithm, since time is ordered in the states array
        int low = 0;
        int high = States.Length - 2;
        int result = 0;
        
        // if the target time is out of bounds, clamp it within bounds
        time = Mathf.Clamp(time, States[low].time, States[high + 1].time);

        // keep count of iterations in case of endless looping
        int i = 0;
        int MaxSearchIterations = 100;
        // while the search hasn't converged
        while (true) {
            // check if search has gone on too long
            if (i > MaxSearchIterations) {
                Debug.Log("Could not find time " + time + " in States array.");
                break;
            }
            i++;
            
            // calculate the middle of the search range
            int mid = (low + high) / 2;

            // check where the wanted time is relative to the middle of the range
            if (time < States[mid].time) {
                // if time is below the middle range, restrict the search range to the lower half
                high = mid - 1;
            } else if (time > States[mid + 1].time) {
                // if time is above the middle range, restrict the search range to the upper half
                low = mid + 1;
            } else {
                // if time is in the middle range, the search is complete
                result = mid;
                break;
            }
        }

        return result;
    }

    // returns a new creature like the simulated creature at the start of the run
    public CreatureModel GetCreatureModel() {
        return new CreatureModel(States[0], C, "RunModel");
    }

    public override string ToString() {
        string s = C.ToString() + "\n";
        foreach (State state in States) {
            s += state.ToString() + "\n";
        }
        return s;
    }
}
