using UnityEngine;

// stores one creature's run as a sequence of states.
public class SimulationRun {
    public readonly float Radius;
    public readonly int Jets;
    public readonly Vector3 JetArm;
    public readonly float JetLength;
    public readonly float JetRadius;

    private int NumStates;
    private readonly State[] States;
    private bool Finished;

    public float MaximumTime { get; private set; }
    public readonly int MaxStates;

    // Initialize empty SimulationRun, to be filled by a simulation
    public SimulationRun(Creature creature, int maxStates) {
        Radius = creature.Radius;
        Jets = creature.Jets;
        JetArm = creature.JetArm;
        JetLength = creature.JetLength;
        JetRadius = creature.JetRadius;

        MaxStates = maxStates;
        NumStates = 0;
        States = new State[MaxStates];

        Finished = false;
    }

    // Initialize SimulationRun from an existing list of states.
    public SimulationRun(Creature creature, State[] states) {
        Radius = creature.Radius;
        Jets = creature.Jets;
        JetArm = creature.JetArm;
        JetLength = creature.JetLength;
        JetRadius = creature.JetRadius;

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
            Debug.LogError("Tried to Finish SimulationRun while not full, only " + NumStates + "/" + MaxStates + " States.");
            return;
        }
        Finished = true;
        State lastState = States[MaxStates - 1];
        MaximumTime = lastState.time;
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

    public State GetFirstState() {
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
