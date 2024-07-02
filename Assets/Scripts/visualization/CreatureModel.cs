using NeatJet.Scripts.Simulation.Creatures;
using NeatJet.Scripts.Simulation.Storage;

using UnityEngine;

namespace NeatJet.Scripts.Visualization
{
    public class CreatureModel : CreatureInternals
    {
        public Transform Transform { get => Object.transform; }
        //public string Name { get => Object.name; set => Object.name = value; }

        public CreatureModel(State state, Creature c)
        {
            // create the GameObject that will be the parent of all the
            Object = new GameObject();
            SetPositionAndRotation(state.Position, state.Rotation);

            // set the static values
            Radius = c.Radius;
            Jets = c.Jets;
            JetArm = c.JetArm;
            JetLength = c.JetLength;
            JetRadius = c.JetRadius;

            // build the GameObjects that show the CreatureModel
            CreatureModelBuilder.Build(this, state.JetEnds);
        }

        // update the objects that represent the jet of the given index
        private void UpdateJetCylinder(int jetIndex, Vector3 jetEnd)
        {
            // find the transforms of the objects that represent the jet
            var jetLeg = JetLegs[jetIndex].transform;
            var jetStart = GetLocalJetStart(jetIndex);
            // update the JetCylinder's position so that it is between start and end
            jetLeg.localPosition = (jetStart + jetEnd) / 2;
            // create a rotation such that the cylinder's start and end line up with the jet
            var startToEnd = jetEnd - jetStart;
            jetLeg.localRotation = Quaternion.FromToRotation(Vector3.up, startToEnd);
        }

        // update the objects that represent the jet of the given index
        private void UpdateJetEndSphere(int jetIndex, Vector3 jetEnd)
        {
            // find the transforms of the objects that represent the jet
            var jetEndSphere = JetEnds[jetIndex].transform;
            jetEndSphere.localPosition = jetEnd;
        }

        // update the jetangles and the objects that represent the jets
        private void UpdateJets(Vector3[] jetEnds)
        {
            for (var jetIndex = 0; jetIndex < Jets; jetIndex++)
            {
                var jetEnd = jetEnds[jetIndex];
                UpdateJetCylinder(jetIndex, jetEnd);
                UpdateJetEndSphere(jetIndex, jetEnd);
            }
        }

        // apply the given state to the GameObjects in the scene
        public void VisualiseState(State s)
        {
            SetPositionAndRotation(s.Position, s.Rotation);
            UpdateJets(s.JetEnds);
            // #TODO when state is expanded with more information about the creature,
            // visualize that information as well.
        }
    }
}
