using NeatJet.Scripts.Simulation.Creatures;
using NeatJet.Scripts.Simulation.Creatures.Genes;
using NeatJet.Scripts.Simulation.Storage;

using UnityEngine;

namespace NeatJet.Scripts.Visualization
{
    public class SimulationVisualiser : MonoBehaviour
    {
        private float Time;
        private bool Play;
        private string PlayString;
        private JetCreature[] Models;
        private SimulationRun[] Runs;
        private int NumberOfRuns;
        private float MaximumTime;

        private int PlaySpeedIndex;
        private readonly float[] PlaySpeeds = new float[] { 0.25f, 0.5f, 0.75f, 1f, 1.25f, 1.5f, 2f };

        /// <summary>
        /// Add a <see cref="SimulationVisualiser"/> to the given game object to visualise the given runs.
        /// </summary>
        /// <param name="parent">The game object to add the visualiser to.</param>
        /// <param name="runs">The simulation runs to visualise.</param>
        /// <returns>The addes simulation visualiser.</returns>
        public static SimulationVisualiser Add(GameObject parent, SimulationRun[] runs)
        {
            var visualiser = parent.AddComponent<SimulationVisualiser>();

            // prepare to visualise the given run
            visualiser.Runs = runs;
            visualiser.NumberOfRuns = runs.Length;
            visualiser.Models = new JetCreature[visualiser.NumberOfRuns];
            visualiser.MaximumTime = visualiser.Runs[0].MaximumTime;
            for (var i = 0; i < visualiser.NumberOfRuns; i++)
            {
                visualiser.Models[i] = CreateModelFromRun(runs[i], "CreatureModel" + i);
                if (visualiser.MaximumTime < runs[i].MaximumTime)
                {
                    visualiser.MaximumTime = runs[i].MaximumTime;
                }
            }

            // initialize time related variables
            visualiser.Time = 0f;
            visualiser.Play = true;
            visualiser.PlayString = "Pause";
            visualiser.PlaySpeedIndex = 3;

            return visualiser;
        }

        private static JetCreature CreateModelFromRun(SimulationRun run, string name)
        {
            var gene = new Gene(run.Radius, run.Jets, run.JetLength, run.JetRadius, run.JetArm, 0f, 0f, 0f, 0f);
            return JetCreatureBuilder.CreateCreature(gene, name, true);
        }

        // if the simulation is playing, automatically incement the time
        private void UpdateTimeWhenPlaying()
        {
            if (Play)
            {
                Time += UnityEngine.Time.deltaTime * PlaySpeeds[PlaySpeedIndex];
                // if time would go over the upper bound, set it back to the upper bound
                if (Time > MaximumTime)
                {
                    Time = MaximumTime;
                }
            }
        }

        public void Update()
        {
            for (var i = 0; i < NumberOfRuns; i++)
            {
                var InterpolatedState = Runs[i].GetStateAtExactTime(Time);
                Models[i].MatchState(InterpolatedState);
            }

            UpdateTimeWhenPlaying();
        }

        public void OnGUI()
        {
            TimeSlider();
            PlayButton();
            PlaySpeedButton();
        }

        /// <summary>
        /// UI that shows and allows the user to change the time that the visualizer visualizes.
        /// </summary>
        private void TimeSlider()
        {
            // determine where on the screen the UI should be shown
            var sliderWidth = Screen.width * 2f / 3f;
            var sliderHeight = 30f;
            var x = (Screen.width - sliderWidth) / 2f;
            var y = Screen.height - sliderHeight - 20f;
            Time = GUI.HorizontalSlider(new Rect(x, y, sliderWidth, sliderHeight), Time, 0f, MaximumTime);
        }

        /// <summary>
        /// A button that toggles whether the visualizer will automatically play.
        /// </summary>
        private void PlayButton()
        {
            // determine where on the screen the UI should be shown
            var buttonWidth = 50f;
            var buttonHeight = 30f;
            var x = (Screen.width * 1f / 6f) - buttonWidth - 10f;
            var y = Screen.height - 20f - (buttonHeight * 4f / 3f);
            //, 
            if (GUI.Button(new Rect(x, y, buttonWidth, buttonHeight), PlayString))
            {
                Play = !Play;
                if (Play)
                {
                    PlayString = "Pause";
                }
                else
                {
                    PlayString = "Play";
                }
            }
        }

        /// <summary>
        /// A button that sets the speed at which the visualiser will automatically play.
        /// </summary>
        private void PlaySpeedButton()
        {
            // determine where on the screen the UI should be shown
            var buttonWidth = 50f;
            var buttonHeight = 30f;
            var x = (Screen.width * 5f / 6f) + buttonWidth + 10f;
            var y = Screen.height - 20f - (buttonHeight * 4f / 3f);
            if (GUI.Button(new Rect(x, y, buttonWidth, buttonHeight), PlaySpeeds[PlaySpeedIndex].ToString() + "X"))
            {
                PlaySpeedIndex++;
                if (PlaySpeedIndex >= PlaySpeeds.Length)
                {
                    PlaySpeedIndex = 0;
                }
            }
        }

        public void OnDestroy()
        {
            foreach (var model in Models)
            {
                Destroy(model);
            }
        }
    }
}
