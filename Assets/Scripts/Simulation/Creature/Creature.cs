using UnityEngine;

// facade class and a factory class for CreatureInternals
public class Creature
{
    private readonly static CreatureDirector Director = new();
    private readonly ICreatureInternals Internals;
    private readonly IBrain Brain;

    public readonly CreatureType Type;

    public Creature(CreatureType type)
    {
        Type = type;

        ICreatureBuilder builder = Type switch
        {
            CreatureType.ConfigurableJointCreature => new ConfigurableJointCreatureBuilder(),
            CreatureType.ColliderCreature => new ColliderCreatureBuilder(),
            _ => new CreatureBuilder(),
        };
        Internals = Director.MakeSimpleCreature(builder);

        Brain = new StaticBrain(Internals.Jets);
    }

    //public Creature(IGene gene) {

    //}

    public Creature(SimulationRun run)
    {
        Type = CreatureType.ModelCreature;
        CreatureBuilder builder = new();
        Internals = Director.MakeCreatureModel(builder, run);
    }

    public float Radius { get => Internals.Radius; }
    public int Jets { get => Internals.Jets; }
    public float JetLength { get => Internals.JetLength; }
    public float JetRadius { get => Internals.JetRadius; }
    public Vector3 JetArm { get => Internals.JetArm; }

    public (Vector3, Quaternion) GetPositionAndRotation()
    {
        return Internals.GetPositionAndRotation();
    }

    public Vector3 GetLocalJetStart(int JetIndex)
    {
        return Internals.GetLocalJetStart(JetIndex);
    }

    public Vector3 GetLocalJetEnd(int JetIndex)
    {
        return Internals.GetLocalJetEnd(JetIndex);
    }

    public (Vector3, Vector3) GetLocalJetStartAndEnd(int JetIndex)
    {
        return Internals.GetLocalJetStartAndEnd(JetIndex);
    }

    // get the creature's intent from it's Brain and let the internals apply it
    public void Update(float time, float deltaTime)
    {
        if (Brain is not null)
        {
            (var rotationIntents, var thrustIntents) = Brain.GetIntent(time, this);
            Internals.Update(time, deltaTime, rotationIntents, thrustIntents);
        }
    }

    public void Destroy()
    {
        Internals.Destroy();
        //Brain.Destroy();
    }

    public void MatchState(State state)
    {
        Internals.MatchState(state);
    }
}
