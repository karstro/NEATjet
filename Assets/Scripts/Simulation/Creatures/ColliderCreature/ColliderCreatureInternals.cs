using NeatJet.Scripts.Simulation.Storage;

using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures.ColliderCreature
{
    public class ColliderCreatureInternals : CreatureInternals, ICreatureInternals
    {
        public Rigidbody PhysicsBody { get; set; }
        public float MaxThrustChangePerSecond { get; set; }
        public float MaxThrust { get; set; }
        public float[] ThrustFractions { get; set; }
        public Vector3[] AngularVelocities { get; set; }
        public float MaxSpeed { get; set; }
        public float SmoothTime { get; set; }

        // CreatureInternals is constructed by ICreatureBuilder
        public ColliderCreatureInternals() { }

        private Vector3 GetRotation(int jetIndex)
        {
            var (start, end) = GetLocalJetStartAndEnd(jetIndex);
            return end - start;
        }

        private void SetRotation(int jetIndex, Vector3 direction)
        {
            JetEnds[jetIndex].transform.localPosition = GetLocalJetStart(jetIndex) + direction.normalized; // Shouldn't normalized direction be multiplied by jetlength?
        }

        private void UpdateJoints(Vector3[] targetRotations, float deltaTime)
        {
            for (var jetIndex = 0; jetIndex < Jets; jetIndex++)
            {
                var jetRotation = CalculateJetArmRotation(jetIndex, Jets);

                var currentRotation = GetRotation(jetIndex);
                var targetRotation = jetRotation * targetRotations[jetIndex];

                if (Vector3.Dot(currentRotation, -targetRotation) > 0.95f)
                {
                    // if the Vectors are opposite, nudge the target to bias the rotation toward the jet's "forward" direction
                    targetRotation += jetRotation * Vector3.right * 0.05f;
                }

                var nextDirection = Vector3.SmoothDamp(currentRotation, targetRotation, ref AngularVelocities[jetIndex], SmoothTime, MaxSpeed, deltaTime);
                SetRotation(jetIndex, nextDirection);
            }
        }

        private void UpdatePhysicsComponents(float deltaTime, Vector3[] rotationIntents, float[] thrustIntents)
        {
            // update the joints based on intent
            UpdateJoints(rotationIntents, deltaTime);
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
            if (JetArms != null)
            {
                UpdateJetLimbs();
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
