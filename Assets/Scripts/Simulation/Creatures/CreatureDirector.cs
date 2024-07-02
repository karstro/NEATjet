using NeatJet.Scripts.Simulation.Storage;

using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures
{
    public class CreatureDirector
    {
        public CreatureDirector() { }

        // make a simple creature with arbitrary default parameters
        public ICreatureInternals MakeSimpleCreature(ICreatureBuilder builder)
        {
            // parameters of the creature
            var radius = 0.4f;
            var maxThrustChangePerSecond = 1f;
            var thrustToWeight = 2f;

            // parameters of the jets
            var jets = 4;
            var jetLength = 1f;
            var jetRadius = 0.2f;
            Vector3 jetArm = new(1f, 0.5f, 0f);
            var jetMass = 0.1f;

            // strength of the forces of the ConfigurableJointCreature's joints
            var spring = 500f;
            var damper = 400f;

            // parameters of the ColliderCreature's smoothDamp
            var maxSpeed = 20f;
            var smoothTime = 0.5f;

            // should a visible model be created?
            var makeModel = true;

            // calculate where the creature starts
            var creatureHeight = jetLength - jetArm.y + jetRadius + 0.5f;
            // should this include offset in position?
            var position = Vector3.up * creatureHeight;
            var rotation = Quaternion.identity;

            // begin building creature
            builder.Reset();

            builder.InitializeParentObject();
            builder.InitializePhysicsBody();

            // initialize creature's parameters
            builder.InitializeCreatureLimits(
                radius,
                maxThrustChangePerSecond,
                thrustToWeight
            );
            builder.InitializeJetParameters(
                jets,
                jetLength,
                jetRadius,
                jetArm
            );
            builder.InitializeThrusts();
            builder.SetPositionAndRotation(position, rotation);

            // initialize creature's objects
            builder.InitializeCreatureGameObjects();

            // initialize the physics components on those objects
            builder.InitializeColliders();
            builder.InitializeRigidbodies(jetMass);
            // ConfigurableJoints only
            builder.InitializeJoints(spring, damper);
            // Collider only
            builder.InitializeAngularVelocities();
            builder.InitializeSmoothDamp(maxSpeed, smoothTime);

            // create the model components if needed
            if (makeModel)
            {
                builder.InitializeCreatureModel();
            }

            // return the created creature
            return builder.GetResult();
        }

        // public void MakeComplexCreature(ICreatureBuilder builder, IBrainBuilder brainBuilder) {
        // }

        public ICreatureInternals MakeCreatureModel(ICreatureBuilder builder, SimulationRun run)
        {
            var startingState = run.GetFirstState();

            // begin building creature
            builder.Reset();

            builder.InitializeParentObject();
            builder.InitializePhysicsBody();

            // initialize creature's parameters
            builder.InitializeCreatureLimits(
                run.Radius,
                0,
                0
            );
            builder.InitializeJetParameters(
                run.Jets,
                run.JetLength,
                run.JetRadius,
                run.JetArm
            );
            builder.SetPositionAndRotation(startingState.Position, startingState.Rotation);

            // initialize creature's objects
            builder.InitializeCreatureGameObjects();
            builder.InitializeCreatureModel();

            return builder.GetResult();
        }
    }
}
