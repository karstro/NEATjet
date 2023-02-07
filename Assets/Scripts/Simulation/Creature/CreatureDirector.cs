using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureDirector
{
    public CreatureDirector() { }
    public void MakeSimpleCreature(ICreatureBuilder builder)
    {
        builder.Reset();
        builder.InitializeGameObject();
        builder.InitializePhysicsBody();
        builder.InitializeCreatureLimits(0.4f, 90, 2);
        builder.SetPositionAndRotation(new Vector3(0, 0.75f, 0), Quaternion.identity);

        Vector3 jetArm = new Vector3(1f, 0.5f, 0f);
        builder.InitializeJets(4, 1f, 0.2f, jetArm);
        builder.SetJetAngles();
        builder.SetThrusts();
        builder.InitializeColliders();

        IBrain brain = new StaticBrain();
        builder.SetBrain(brain);
    }

    // public void MakeComplexCreature(ICreatureBuilder creatureBuilder, IBrainBuilder brainBuilder) {
    // }

    // public void MakeCreatureModel(ICreatureBuilder builder){
    // 
    // }
}
