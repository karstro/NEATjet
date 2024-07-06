using NeatJet.Scripts.Simulation.Creatures.Jets;

using System;

using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures.Genes
{
    /// <summary>
    /// Contains all information necessary to create a <see cref="JetCreature"/>.
    /// </summary>
    [Serializable]
    public readonly struct Gene
    {
        /// <summary>
        /// Create a gene with all information needed to create a <see cref="JetCreature"/>.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="jets"></param>
        /// <param name="jetLength"></param>
        /// <param name="jetRadius"></param>
        /// <param name="jetArm"></param>
        /// <param name="maximumThrust"></param>
        /// <param name="maximumThrustChangePerSecond"></param>
        /// <param name="maximumRotationSpeed"></param>
        /// <param name="smoothTime"></param>
        public Gene(float radius, int jets, float jetLength, float jetRadius, Vector3 jetArm, float maximumThrust, float maximumThrustChangePerSecond, float maximumRotationSpeed, float smoothTime)
        {
            Radius = radius;
            Jets = jets;
            JetLength = jetLength;
            JetRadius = jetRadius;
            JetArm = jetArm;
            MaximumThrust = maximumThrust;
            MaximumThrustChangePerSecond = maximumThrustChangePerSecond;
            MaximumRotationSpeed = maximumRotationSpeed;
            SmoothTime = smoothTime;
        }

        /// <summary>
        /// The radius of the creature's body
        /// </summary>
        public readonly float Radius;

        /// <summary>
        /// The number of <see cref="Jet"/>s to create.
        /// </summary>
        public readonly int Jets;

        /// <summary>
        /// The length of the jets.
        /// </summary>
        public readonly float JetLength;

        /// <summary>
        /// The radius of each part of the jet.
        /// </summary>
        public readonly float JetRadius;

        /// <summary>
        /// The direction in which to create the arm of the jet (from the body to the start of the jet)
        /// </summary>
        public readonly Vector3 JetArm;

        /// <summary>
        /// The maximum thrust all of the jets can output together.
        /// </summary>
        public readonly float MaximumThrust;

        /// <summary>
        /// The maximum thrust one jet can output.
        /// </summary>
        public readonly float MaximumThrustPerJet { get => MaximumThrust / Jets; }

        /// <summary>
        /// The maximum change a jet can apply to its thrust in a second.
        /// </summary>
        public readonly float MaximumThrustChangePerSecond;

        /// <summary>
        /// The maximum speed with which each jet may rotate itself in UnityUnits/sec.
        /// </summary>
        public readonly float MaximumRotationSpeed;

        /// <summary>
        /// The amount of time in seconds that a jet should aim to spend rotating toward the target rotation. <br />
        /// The lower it is, the faster the jet will try to rotate, limited by <see cref="MaximumRotationSpeed"/>.
        /// </summary>
        public readonly float SmoothTime;
    }
}
