using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures
{
    public interface ICreatureBuilder
    {
        public void Reset();
        public void InitializeParentObject();
        public void InitializePhysicsBody();
        public void InitializeCreatureLimits(
            float radius,
            float maxThrustChangePerSecond,
            float thrustToWeight
        );
        public void InitializeJetParameters(
            int jets,
            float jetLength,
            float jetRadius,
            Vector3 jetArm
        );
        public void InitializeThrusts();
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation);
        public void InitializeCreatureGameObjects();
        public void InitializeColliders();
        public void InitializeRigidbodies(float jetMass);
        public void InitializeCreatureModel();
        public ICreatureInternals GetResult();

        // settings unique to ConfigurableJointCreatureInternals
        public void InitializeJoints(float spring, float damper);

        // settings unique to ColliderCreatureInternals
        public void InitializeSmoothDamp(float maxSpeed, float smoothTime);
        public void InitializeAngularVelocities();
    }
}
