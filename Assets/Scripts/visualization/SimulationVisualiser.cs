using UnityEngine;

public class SimulationVisualiser {
    private float time;
    private bool Play;
    private string PlayString;
    private float PlaySpeed;
    private readonly Creature[] Models;
    private readonly SimulationRun[] Runs;
    private readonly int NumRuns;
    private readonly float MaximumTime;

    public SimulationVisualiser(SimulationRun[] runs) {
        // prepare to visualise the given run
        Runs = runs;
        NumRuns = runs.Length;
        Models = new Creature[NumRuns];
        MaximumTime = Runs[0].MaximumTime;
        for (int i = 0; i < NumRuns; i++) {
            Models[i] = new(Runs[i]);
            if (Runs[i].MaximumTime < MaximumTime) { 
                MaximumTime = Runs[i].MaximumTime;
            }
        }

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
            if (time > MaximumTime) {
                time = MaximumTime;
            }
        }
    }

    public void Update() {
        for (int i = 0; i < NumRuns; i++) {
            State InterpolatedState = Runs[i].GetStateAtExactTime(time);
            Models[i].MatchState(InterpolatedState);
        }
        UpdateTimeWhenPlaying();
    }

    public void OnGUI() {
        // determine where on the screen the UI should be shown
        float SliderWidth = Screen.width * 2 / 3;
        float SliderHeight = 30;
        float x = (Screen.width - SliderWidth) / 2;
        float y = Screen.height - SliderHeight - 20;

        // UI that shows and allows the user to change the time that the visualizer visualizes
        time = GUI.HorizontalSlider(new Rect(x, y, SliderWidth, SliderHeight), time, 0f, MaximumTime);

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
