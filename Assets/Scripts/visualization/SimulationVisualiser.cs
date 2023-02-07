using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationVisualiser
{
    private CreatureModel Model;
    private float time;
    private bool Play;
    private string PlayString;
    private float PlaySpeed;
    private SimulationRun Run;

    public SimulationVisualiser(SimulationRun run, string name) {
        // prepare to visualise the given run
        this.Run = run;
        Creature c = this.Run.GetCreatureAtStartOfRun();
        this.Model = CreatureModelBuilder.Build(c, name);
        // this.Object = CreatureModelBuilder.BuildCreatureModel(c, name);

        // initialize time related variables
        this.time = 0f;
        this.Play = true;
        this.PlayString = "Pause";
        this.PlaySpeed = 1f;
    }

    // if the simulation is playing, automatically incement the time
    private void UpdateTimeWhenPlaying() {
        if (this.Play) {
            this.time += Time.deltaTime * this.PlaySpeed;
            // if time would go over the upper bound, set it back to the upper bound
            if (this.time > this.Run.ReadMaximumTime) {
                this.time = this.Run.ReadMaximumTime;
            }
        }
    }

    // Update is called once per frame
    public void Update() {
        State InterpolatedState = this.Run.GetStateAtExactTime(this.time);
        this.Model.VisualiseState(InterpolatedState);
        this.UpdateTimeWhenPlaying();
    }

    public void OnGUI() {
        // determine where on the screen the UI should be shown
        float SliderWidth = Screen.width * 2 / 3;
        float SliderHeight = 30;
        float x = (Screen.width - SliderWidth) / 2;
        float y = Screen.height - SliderHeight - 20;

        // UI that shows and allows the user to change the time that the visualizer visualizes
        this.time = GUI.HorizontalSlider(new Rect(x, y, SliderWidth, SliderHeight), this.time, 0f, this.Run.ReadMaximumTime);

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
