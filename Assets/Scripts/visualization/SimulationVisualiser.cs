using UnityEngine;

public class SimulationVisualiser
{
    private CreatureModel Model;
    private float time;
    private bool Play;
    private string PlayString;
    private float PlaySpeed;
    private readonly SimulationRun Run;

    public SimulationVisualiser(SimulationRun run) {
        // prepare to visualise the given run
        Run = run;
        Model = Run.GetCreatureModel();

        // initialize time related variables
        time = 0f;
        Play = true;
        PlayString = "Pause";
        PlaySpeed = 1f;
    }

    // if the simulation is playing, automatically incement the time
    private void UpdateTimeWhenPlaying() {
        if (Play) {
            time += Time.deltaTime * PlaySpeed;
            // if time would go over the upper bound, set it back to the upper bound
            if (time > Run.MaximumTime) {
                time = Run.MaximumTime;
            }
        }
    }

    public void Update() {
        State InterpolatedState = Run.GetStateAtExactTime(time);
        Model.VisualiseState(InterpolatedState);
        UpdateTimeWhenPlaying();
    }

    public void OnGUI() {
        // determine where on the screen the UI should be shown
        float SliderWidth = Screen.width * 2 / 3;
        float SliderHeight = 30;
        float x = (Screen.width - SliderWidth) / 2;
        float y = Screen.height - SliderHeight - 20;

        // UI that shows and allows the user to change the time that the visualizer visualizes
        time = GUI.HorizontalSlider(new Rect(x, y, SliderWidth, SliderHeight), time, 0f, Run.MaximumTime);

        // a button that toggles whether the visualizer will automatically play.
        float ButtonWidth = 50;
        if (GUI.Button(new Rect(x - ButtonWidth - 10, y - SliderHeight / 3, ButtonWidth, SliderHeight), PlayString)) {
            Play = !Play;
            if (Play) {
                PlayString = "Pause";
            } else {
                PlayString = "Play";
            }
        }
    }
}
