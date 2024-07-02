using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures.ConfigurableJointCreature
{
    public class ConfigurableJointCreatureBuilder : CreatureBuilder, ICreatureBuilder
    {
        private ConfigurableJointCreatureInternals JointCreature { get => (ConfigurableJointCreatureInternals)Creature; set => Creature = value; }

        public new void Reset()
        {
            JointCreature = new();
        }

        // initialize the creature's Rigidbodies, but distribute the weight differently
        //public new void InitializeRigidbodies(float jetMass) {
        //    // create a Rigidbody for each jet's start and end
        //    for (int jetIndex = 0; jetIndex < Creature.Jets; jetIndex++) {
        //        //float fraction = .95f;
        //        //CreateRigidbody(creature.JetStarts[jetIndex], jetMass * fraction);
        //        //CreateRigidbody(creature.JetEnds[jetIndex], jetMass * (1 - fraction));
        //    }
        //}

        // create a ConfigurableJoint on the given GameObject
        private ConfigurableJoint CreateJoint(GameObject parent, Rigidbody connectedObject, float spring, float damper)
        {
            // create the ConfigurableJoint
            var joint = parent.AddComponent<ConfigurableJoint>();
            joint.connectedBody = connectedObject;

            // lock unwanted motion
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            // move toward target by spherical interpolation
            joint.rotationDriveMode = RotationDriveMode.Slerp;
            var drive = joint.slerpDrive;
            drive.positionSpring = spring;
            drive.positionDamper = damper;
            joint.slerpDrive = drive;

            joint.projectionMode = JointProjectionMode.PositionAndRotation;
            joint.massScale = 2f;
            joint.connectedMassScale = 0.5f;

            return joint;
        }

        // create the creature's Joints and initialize their settings
        public new void InitializeJoints(float spring, float damper)
        {
            JointCreature.JetJoints = new ConfigurableJoint[JointCreature.Jets];
            for (var jetIndex = 0; jetIndex < JointCreature.Jets; jetIndex++)
            {
                // gather the objects to be connected by the joint
                var jetStart = JointCreature.JetStarts[jetIndex];
                var jetEndBody = JointCreature.JetEnds[jetIndex].GetComponent<Rigidbody>();
                var creatureBody = JointCreature.PhysicsBody;

                // attach the jetStart and jetEnd with a FixedJoint
                var fixedJoint = jetStart.AddComponent<FixedJoint>();
                fixedJoint.connectedBody = jetEndBody;

                // attach the JetStart GameObject to the creature's body with a configurable joint
                JointCreature.JetJoints[jetIndex] = CreateJoint(jetStart, creatureBody, spring, damper);
            }
        }
    }
}
