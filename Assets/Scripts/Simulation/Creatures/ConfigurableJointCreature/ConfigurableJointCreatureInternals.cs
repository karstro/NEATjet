using NeatJet.Scripts.Simulation.JointExtensions;
using NeatJet.Scripts.Simulation.Storage;

using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures.ConfigurableJointCreature
{
    public class ConfigurableJointCreatureInternals : CreatureInternals, ICreatureInternals
    {
        public Rigidbody PhysicsBody { get; set; }
        public float MaxThrustChangePerSecond { get; set; }
        public float MaxThrust { get; set; }
        public float[] ThrustFractions { get; set; }
        public ConfigurableJoint[] JetJoints { get; set; }

        // CreatureInternals is constructed by ICreatureBuilder
        public ConfigurableJointCreatureInternals() { }

        private Quaternion GetTargetRotation(Vector3 creatureSpacetargetRotation)
        {
            var targetRotation = Quaternion.FromToRotation(Vector3.down, creatureSpacetargetRotation);
            return targetRotation;
        }

        private void UpdateJoints(Vector3[] targetRotations)
        {
            for (var jetIndex = 0; jetIndex < Jets; jetIndex++)
            {
                var startRotation = Quaternion.identity;
                var targetRotation = GetTargetRotation(targetRotations[jetIndex]);
                JetJoints[jetIndex].SetTargetRotationLocal(targetRotation, startRotation);
            }
        }

        private void UpdateJetLimb(GameObject limb, Vector3 start, Vector3 end)
        {
            // the cylinder's center is the position between the start and end positions
            limb.transform.localPosition = (start + end) / 2;
            // change the rotation of the cylinder such that it points from the start toward end
            var StartToEnd = end - start;
            limb.transform.localRotation = Quaternion.FromToRotation(Vector3.up, StartToEnd);
        }

        private void UpdateJetLimbs()
        {
            for (var jetIndex = 0; jetIndex < Jets; jetIndex++)
            {
                (var start, var end) = GetLocalJetStartAndEnd(jetIndex);
                UpdateJetLimb(JetArms[jetIndex], Vector3.zero, start);
                UpdateJetLimb(JetLegs[jetIndex], start, end);
            }
        }

        private void UpdatePhysicsComponents(float deltaTime, Vector3[] rotationIntents, float[] thrustIntents)
        {
            // update the joints based on intent if they exist
            if (JetJoints is not null)
            {
                UpdateJoints(rotationIntents);
            }
            // #TODO update thrusts based on intent
            // #TODO apply force at jets based on thrusts
        }

        // update what the creature is doing based on its brain's intent
        public void Update(float time, float deltaTime, Vector3[] rotationIntents, float[] thrustIntents)
        {
            // update all physics based components
            if (rotationIntents is not null && thrustIntents is not null)
            {
                UpdatePhysicsComponents(deltaTime, rotationIntents, thrustIntents);
            }

            // update the position of the jet's limbs if they exist
            if (JetArms is not null)
            {
                UpdateJetLimbs();
            }
        }

        public void Destroy()
        {
            foreach (Transform child in Object.transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }

            UnityEngine.Object.Destroy(Object);
        }

        public void MatchState(State state)
        {
            SetPositionAndRotation(state.Position, state.Rotation);
            for (var jetIndex = 0; jetIndex < state.JetEnds.Length; jetIndex++)
            {
                JetEnds[jetIndex].transform.position = state.JetEnds[jetIndex];
            }

            UpdateJetLimbs();
        }
    }
}
