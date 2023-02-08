using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureModel : CreatureBase
{
    public Transform Transform { get => Object.transform; }
    public string Name { set => Object.name = value; get => Object.name; }

    public CreatureModel(State state, Creature c, string name) {
        // create the GameObject that will be the parent of all the
        Object = new GameObject();
        SetPositionAndRotation(state.Position, state.Rotation);
        
        // set the static values
        Radius = c._Radius;
        Jets = c._Jets;
        JetArm = c._JetArm;
        JetLength = c._JetLength;
        JetRadius = c._JetRadius;
        Name = name;

        JetAngles = new Vector3[Jets];
        for (int JetIndex = 0; JetIndex < Jets; JetIndex++) {
            JetAngles[JetIndex] = state.JetAngles[JetIndex];
        }

        // build the GameObjects that show the CreatureModel
        CreatureModelBuilder.Build(this);
    }

    // set the position and rotation of the creature's GameObject
    public override void SetPositionAndRotation(Vector3 position, Quaternion rotation) {
        Transform.SetPositionAndRotation(position, rotation);
    }

    // update the objects that represent the jet of the given index
    private void UpdateJet(int JetIndex) {
        // find the transforms of the objects that represent the jet
        Transform JetCylinder = Transform.Find("Jet" + JetIndex);
        Transform JetEndSphere = Transform.Find("Jet" + JetIndex + "End");
        // get the locations of the jet's start and end in relative space
        (Vector3 Start, Vector3 End) = GetLocalJetStartAndEnd(JetIndex);
        JetEndSphere.localPosition = End;
        // update the JetCylinder's position
        JetCylinder.localPosition = (Start + End) / 2;
        // create a rotation such that the cylinder's start and end line up with the jet
        Vector3 StartToEnd = End - Start;
        JetCylinder.localRotation = Quaternion.FromToRotation(Vector3.up, StartToEnd);
    }

    // update the jetangles and the objects that represent the jets
    private void UpdateJets(Vector3[] jetAngles) {
        for(int JetIndex = 0; JetIndex < Jets; JetIndex++) {
            JetAngles[JetIndex] = jetAngles[JetIndex];
            UpdateJet(JetIndex);
        }
    }
    
    // apply the given state to the GameObjects in the scene
    public void VisualiseState(State s) {
        SetPositionAndRotation(s.Position, s.Rotation);
        UpdateJets(s.JetAngles);
        // #TODO when state is expanded with more information about the creature,
        // visualize that information as well.
    }
}
