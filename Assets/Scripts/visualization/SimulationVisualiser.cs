using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Assertions;

// public class SimulationVisualiser : Visualiser
public class SimulationVisualiser : MonoBehaviour
{
    
    protected GameObject Object;
    protected float time;
    private bool Play;
    private string PlayString;
    private float PlaySpeed;
    private Simulator Sim;
    private State[] States;

    // Start is called before the first frame update
    // #TODO turn this into a constructor
    void Start() {
        // initialize the simulator and get the states from a simulation
        // this is temporary, eventually it should be given states from another source based on a simulation that happened, potentially before the visualizer existed
        this.Sim = gameObject.GetComponent<Simulator>();
        // this creature shoulld be generated based on the first state of the array.
        Creature c = new Creature();
        this.Object = CreatureModelBuilder.BuildCreatureModel(c, "Creature");
        this.States = this.Sim.simulateCreature(c);

        // initialize time related variables
        this.time = 0f;
        this.Play = true;
        this.PlayString = "Pause";
        this.PlaySpeed = 1;
    }

    private int FindStateBeforeTime() {
        // binary search algorithm, since time is ordered in the states array
        int low = 0;
        int high = this.States.Length - 2;
        int result = 0;

        // if the target time is out of bounds, throw an error
        // Assert.IsFalse(this.time < this.States[low].time || this.time > this.States[high + 1].time,
        //                "Time variable is out of bounds of the States");
        
        // if the target time is out of bounds, clamp it within bounds
        this.time = Mathf.Clamp(this.time, this.States[low].time, this.States[high + 1].time);

        // keep count of iterations in case of endless looping
        int i = 0;
        int MaxSearchIterations = 100;
        // while the search hasn't converged
        while (true) {
            // check if search has gone on too long
            if (i > MaxSearchIterations) {
                Debug.Log("Could not find time " + this.time + "in States array.");
                break;
            }
            i++;
            
            // calculate the middle of the search range
            int mid = (low + high) / 2;

            // check where the wanted time is relative to the middle of the range
            if (this.time < this.States[mid].time) {
                // if this.time is below the middle range, restrict the search range to the lower half
                high = mid - 1;
            } else if (this.time > this.States[mid + 1].time) {
                // if this.time is above the middle range, restrict the search range to the upper half
                low = mid + 1;
            } else {
                // if this.time is in the middle range, the search is complete
                result = mid;
                break;
            }
        }

        return result;
    }

    // interpolate in between states from the States array based on this.time
    private State GetInterpolatedState() {
        // find where this.time is within the States array
        int BeforeStateIndex = FindStateBeforeTime();

        // linearly interpolate between the two states around the result using this.time as the fraction.
        State Before = this.States[BeforeStateIndex];
        State After = this.States[BeforeStateIndex + 1];
        return State.LerpByTime(Before, After, this.time);
    }

    // apply the given state to the GameObjects in the scene
    private void VisualiseState(State s) {
        this.Object.transform.SetPositionAndRotation(s.Position, s.Rotation);
        // #TODO when state is expanded with more information about the creature,
        // visualize that information as well.
    }

    // if the simulation is playing, automatically incement the time
    private void UpdateTimeWhenPlaying() {
        if (this.Play) {
            this.time += Time.deltaTime * this.PlaySpeed;
            // if time would go over the upper bound, set it back to the upper bound
            if (this.time > this.States[this.States.Length - 1].time) {
                this.time = this.States[this.States.Length - 1].time;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        // if the visualizer has states
        if (this.States != null && this.States.Length > 1) {
            State InterpolatedState = this.GetInterpolatedState();
            this.VisualiseState(InterpolatedState);
            this.UpdateTimeWhenPlaying();
        }
    }

    void OnGUI() {
        // determine where on the screen the UI should be shown
        float SliderWidth = Screen.width * 2 / 3;
        float SliderHeight = 30;
        float x = (Screen.width - SliderWidth) / 2;
        float y = Screen.height - SliderHeight - 20;

        // UI that shows and allows the user to change the time that the visualizer visualizes
        this.time = GUI.HorizontalSlider(new Rect(x, y, SliderWidth, SliderHeight), this.time, 0f, this.Sim.TimeOut);

        // a button that toggles whether the visualizer will automatically play.
        float ButtonWidth = 50;
        if (GUI.Button(new Rect(x - ButtonWidth - 10, y - SliderHeight / 3, ButtonWidth, SliderHeight), PlayString)) {
            this.Play = !this.Play;
            if (this.Play) {
                this.PlayString = "Pause";
            } else {
                this.PlayString = "Play";
            }
        }
    }
}
