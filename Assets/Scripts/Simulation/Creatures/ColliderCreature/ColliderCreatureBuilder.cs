using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures.ColliderCreature
{
    public class ColliderCreatureBuilder : CreatureBuilder, ICreatureBuilder
    {
        private ColliderCreatureInternals ColliderCreature { get => (ColliderCreatureInternals)Creature; set => Creature = value; }

        public new void Reset()
        {
            ColliderCreature = new();
        }

        public new void InitializeSmoothDamp(float maxSpeed, float smoothTime)
        {
            ColliderCreature.MaxSpeed = maxSpeed;
            ColliderCreature.SmoothTime = smoothTime;
        }

        public new void InitializeAngularVelocities()
        {
            ColliderCreature.AngularVelocities = new Vector3[ColliderCreature.Jets];
            for (var jetIndex = 0; jetIndex < ColliderCreature.Jets; jetIndex++)
            {
                ColliderCreature.AngularVelocities[jetIndex] = Vector3.zero;
            }
        }

        public override void InitializeRigidbodies(float jetMass) { }
    }
}
