using UnityEngine;

public class CreatureDirector {
    public CreatureDirector() { }

    // make a simple creature with arbitrary default parameters
    public ICreatureInternals MakeSimpleCreature(ICreatureBuilder builder)
    {
        // parameters of the creature
        float radius = 0.4f;
        float maxThrustChangePerSecond = 1f;
        float thrustToWeight = 2f;

        // parameters of the jets
        int jets = 4;
        float jetLength = 1f;
        float jetRadius = 0.2f;
        Vector3 jetArm = new(1f, 0.5f, 0f);
        float jetMass = 0.1f;

        // strength of the forces of the joints
        float spring = 500;
        float damper = 400;

        // should a visible model be created?
        bool makeModel = true;

        // calculate where the creature starts
        float creatureHeight = jetLength - jetArm.y + jetRadius + 0.5f;
        // should this include offset in position?
        Vector3 position = Vector3.up * creatureHeight;
        Quaternion rotation = Quaternion.identity;

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
        builder.InitializeJoints(spring, damper);

        // create the model components if needed
        if (makeModel) {
            builder.InitializeCreatureModel();
        }

        // return the created creature
        return builder.GetResult();
    }

    // public void MakeComplexCreature(ICreatureBuilder creatureBuilder, IBrainBuilder brainBuilder) {
    // }

    // MakeCreatureModel?
}
