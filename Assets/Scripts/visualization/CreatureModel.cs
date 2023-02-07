using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureModel
{
    // #NOTE should CreatureModel inherit from Creature (or a new parent class) instead of containing one?
    private Creature C;
    private Transform ModelParent;

    public CreatureModel(Creature c) {
        this.C = c;
        this.ModelParent = C._Object.transform;
    }

    private void UpdateJet(int JetIndex) {
        // find the transforms of the objects that represent the jet
        Transform JetCylinder = this.ModelParent.Find("Jet" + JetIndex);
        Transform JetEndSphere = this.ModelParent.Find("Jet" + JetIndex + "End");
        // get the locations of the jet's start and end in relative space
        (Vector3 Start, Vector3 End) = this.C.GetLocalJetStartAndEnd(JetIndex);
        JetEndSphere.localPosition = End;
        // update the JetCylinder's position
        JetCylinder.localPosition = (Start + End) / 2;
        // create a rotation such that the cylinder's start and end line up with the jet
        Vector3 StartToEnd = End - Start;
        JetCylinder.localRotation = Quaternion.FromToRotation(Vector3.up, StartToEnd);
    }

    private void UpdateJets(Vector3[] JetAngles) {
        for(int JetIndex = 0; JetIndex < this.C._Jets; JetIndex++) {
            this.C.SetJetAngle(JetIndex, JetAngles[JetIndex]);
            this.UpdateJet(JetIndex);
        }
    }
    
    // apply the given state to the GameObjects in the scene
    public void VisualiseState(State s) {
        this.ModelParent.SetPositionAndRotation(s.Position, s.Rotation);
        this.UpdateJets(s.JetAngles);
        // #TODO when state is expanded with more information about the creature,
        // visualize that information as well.
    }
}
