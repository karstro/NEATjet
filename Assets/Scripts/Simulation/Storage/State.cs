using NeatJet.Scripts.Simulation.Creatures;

using UnityEngine;

namespace NeatJet.Scripts.Simulation.Storage
{
    /// <summary>
    /// struct that contains the state of a simulation step at a specific time
    /// </summary>
    public readonly struct State
    {
        public readonly float time;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector3[] JetEnds;

        public State(float time, Creature creature)
        {
            this.time = time;

            (var position, var rotation) = creature.GetPositionAndRotation();
            Position = position;
            Rotation = rotation;

            JetEnds = new Vector3[creature.Jets];
            for (var jetIndex = 0; jetIndex < creature.Jets; jetIndex++)
            {
                JetEnds[jetIndex] = creature.GetLocalJetEnd(jetIndex);
            }
        }

        private State(float time, Vector3 position, Quaternion rotation, Vector3[] jetEnds)
        {
            this.time = time;
            Position = position;
            Rotation = rotation;
            JetEnds = jetEnds;
        }

        public static State Lerp(State A, State B, float fraction)
        {
            var InterpolatedTime = Mathf.Lerp(A.time, B.time, fraction);
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

        public static State LerpByTime(State Before, State After, float t)
        {
            var fraction = Mathf.InverseLerp(Before.time, After.time, t);
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

        public override string ToString()
        {
            var s = "time: " + time.ToString() + ", position:" + Position.ToString() + ", rotation:" + Rotation.ToString();
            s += "\njetEnds: ";
            foreach (var jetEnd in JetEnds)
            {
                s += jetEnd.ToString() + ", ";
            }

            return s;
        }
    }
}
