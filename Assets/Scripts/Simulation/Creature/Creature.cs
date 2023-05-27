using UnityEngine;

public enum CreatureType {
    ConfigurableJointCreature
}

// facade class and a factory class for CreatureInternals
public class Creature {
    private readonly static CreatureDirector Director = new();
    private readonly ICreatureInternals Internals;
    private readonly IBrain Brain;

    public readonly CreatureType Type;

    public Creature(CreatureType type/*, IGene? gene = null*/) {
        Type = type;
        if (type == CreatureType.ConfigurableJointCreature) {
            // #TODO consider dependency injection for the builder
            ConfigurableJointCreatureBuilder Builder = new();
            Internals = Director.MakeSimpleCreature(Builder);
        }

        Brain = new StaticBrain(Internals.Jets);
    }

    public float Radius { get => Internals.Radius; }
    public int Jets { get => Internals.Jets; }
    public float JetLength { get => Internals.JetLength; }
    public float JetRadius { get => Internals.JetRadius; }
    public Vector3 JetArm { get => Internals.JetArm; }

    public (Vector3, Quaternion) GetPositionAndRotation() {
        return Internals.GetPositionAndRotation();
    }
    
    public Vector3 GetLocalJetStart(int JetIndex) {
        return Internals.GetLocalJetStart(JetIndex);
    }
    
    public Vector3 GetLocalJetEnd(int JetIndex) {
        return Internals.GetLocalJetEnd(JetIndex);
    }

    public (Vector3, Vector3) GetLocalJetStartAndEnd(int JetIndex) {
        return Internals.GetLocalJetStartAndEnd(JetIndex);
    }

    public void Update(float time, float deltaTime) {
        // get the creature's intent from it's Brain
        (Vector3[] rotationIntents, float[] thrustIntents) = Brain.GetIntent(time, this);
        Internals.Update(time, deltaTime, rotationIntents, thrustIntents);
    }

    public void Destroy() {
        Internals.Destroy();
        //Brain.Destroy();
    }
}
