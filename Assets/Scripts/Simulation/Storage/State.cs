using NeatJet.Scripts.Simulation.Creatures;

using UnityEngine;

namespace NeatJet.Scripts.Simulation.Storage
{
    /// <summary>
    /// struct that contains the state of a simulation step at a specific time
    /// </summary>
    public readonly struct State
    {
        public readonly float Time;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector3[] JetEnds;

        /// <summary>
        /// Construct the state from a creature currently being simulated.
        /// </summary>
        /// <param name="time">The current time from the simulation.</param>
        /// <param name="creature">The creature whose state to record.</param>
        public State(float time, JetCreature creature)
        {
            Time = time;

            Position = creature.Position;
            Rotation = creature.Rotation;

            JetEnds = new Vector3[creature.JetNumber];
            for (var jetIndex = 0; jetIndex < creature.JetNumber; jetIndex++)
            {
                JetEnds[jetIndex] = creature.Jets[jetIndex].LocalEnd;
            }
        }

        /// <summary>
        /// Construct the state from the necessary base information.
        /// </summary>
        /// <param name="time">The state's time.</param>
        /// <param name="position">The state's position.</param>
        /// <param name="rotation">The state's rotation.</param>
        /// <param name="jetEnds">The state's jet end positions.</param>
        private State(float time, Vector3 position, Quaternion rotation, Vector3[] jetEnds)
        {
            Time = time;
            Position = position;
            Rotation = rotation;
            JetEnds = jetEnds;
        }

        /// <summary>
        /// Linear interpolation between to states by a given fraction.
        /// </summary>
        /// <param name="A">The lower state to interpolate.</param>
        /// <param name="B">The higher state to interpolate.</param>
        /// <param name="fraction">The fraction to interpolate with, lower is more A, higher is more B.</param>
        /// <returns>The interpolated state.</returns>
        public static State Lerp(State A, State B, float fraction)
        {
            var InterpolatedTime = Mathf.Lerp(A.Time, B.Time, fraction);
            var InterpolatedPosition = Vector3.Lerp(A.Position, B.Position, fraction);
            var InterpolatedRotation = Quaternion.Slerp(A.Rotation, B.Rotation, fraction);

            var jets = A.JetEnds.Length;
            var InterpolatedJetEnds = new Vector3[jets];
            for (var JetIndex = 0; JetIndex < jets; JetIndex++)
            {
                InterpolatedJetEnds[JetIndex] = Vector3.Lerp(A.JetEnds[JetIndex], B.JetEnds[JetIndex], fraction);
            }

            return new State(InterpolatedTime, InterpolatedPosition, InterpolatedRotation, InterpolatedJetEnds);
        }

        /// <summary>
        /// Linear interpolation between to states by the given time. The time should be between the states' times.
        /// </summary>
        /// <param name="Before">The earlier state to interpolate.</param>
        /// <param name="After">The later state to interpolate.</param>
        /// <param name="t">The time to interpolate to.</param>
        /// <returns>The interpolated state.</returns>
        public static State LerpByTime(State Before, State After, float t)
        {
            var fraction = Mathf.InverseLerp(Before.Time, After.Time, t);
            var InterpolatedPosition = Vector3.Lerp(Before.Position, After.Position, fraction);
            var InterpolatedRotation = Quaternion.Slerp(Before.Rotation, After.Rotation, fraction);

            var jets = Before.JetEnds.Length;
            var InterpolatedJetEnds = new Vector3[jets];
            for (var JetIndex = 0; JetIndex < jets; JetIndex++)
            {
                InterpolatedJetEnds[JetIndex] = Vector3.Lerp(Before.JetEnds[JetIndex], After.JetEnds[JetIndex], fraction);
            }

            return new State(t, InterpolatedPosition, InterpolatedRotation, InterpolatedJetEnds);
        }

        /// <returns>A string representation of the state.</returns>
        public override string ToString()
        {
            var s = "time: " + Time.ToString() + ", position:" + Position.ToString() + ", rotation:" + Rotation.ToString();
            s += "\njetEnds: ";
            foreach (var jetEnd in JetEnds)
            {
                s += jetEnd.ToString() + ", ";
            }

            return s;
        }
    }
}
