using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// to avoid using multidimensional arrays of states for multiple simulation runs, store one creature's run in one object.
public class SimulationRun
{
    private Creature C;
    public readonly int MaxStates;
    private int NumStates;
    public int ReadNumStates { get => NumStates; }
    private State[] States;
    public State[] ReadState { get => States; }
    private bool Finished;
    private float MaximumTime;
    public float ReadMaximumTime {get => MaximumTime; }

    // Initialize empty SimulationRun, to be filled by a simulation
    public SimulationRun(Creature c, int maxStates) {
        this.C = c;

        this.MaxStates = maxStates;
        this.NumStates = 0;
        this.States = new State[this.MaxStates];

        this.Finished = false;
    }

    // Initialize SimulationRun from an existing list of states.
    public SimulationRun(Creature c, State[] states) {
        this.C = c;

        this.MaxStates = states.Length;
        this.NumStates = states.Length;
        this.States = states;

        this.Finished = true;
    }

    public void AddState(State state) {
        if (this.Finished || this.NumStates >= this.MaxStates) {
            Debug.Log("Tried to add state when SimulationRun is already Finished.");
            return;
        }
        this.States[this.NumStates] = state;
        this.NumStates++;
    }

    public void Finish() {
        if (this.Finished) {
            Debug.Log("Tried to Finish an already Finished SimulationRun.");
        }
        if (this.NumStates < this.MaxStates) {
            Debug.Log("Tried to Finish SimulationRun while not full, only " + this.NumStates + "/" + this.MaxStates + " States.");
            return;
        }
        this.Finished = true;
        State lastState = this.States[this.MaxStates - 1];
        this.MaximumTime = lastState.time;
    }

    public State GetStateBeforeTime(float time) {
        // find where this.time is within the States array
        int BeforeStateIndex = FindStateBeforeTime(time);
        return this.States[BeforeStateIndex];
    }

    public State GetStateAtExactTime(float time) {
        // find where this.time is within the States array
        int BeforeStateIndex = FindStateBeforeTime(time);

        // linearly interpolate between the two states around the result using this.time as the fraction.
        State Before = this.States[BeforeStateIndex];
        State After = this.States[BeforeStateIndex + 1];
        return State.LerpByTime(Before, After, time);
    }
    
    // Find the index of the state before the current time
    private int FindStateBeforeTime(float time) {
        // binary search algorithm, since time is ordered in the states array
        int low = 0;
        int high = this.States.Length - 2;
        int result = 0;
        
        // if the target time is out of bounds, clamp it within bounds
        time = Mathf.Clamp(time, this.States[low].time, this.States[high + 1].time);

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
            if (time < this.States[mid].time) {
                // if time is below the middle range, restrict the search range to the lower half
                high = mid - 1;
            } else if (time > this.States[mid + 1].time) {
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
    public Creature GetCreatureAtStartOfRun() {
        return new Creature(this.States[0], this.C);
    }

    public override string ToString() {
        string s = this.C.ToString() + "\n";
        foreach (State state in this.States) {
            s += state.ToString() + "\n";
        }
        return s;
    }
}
