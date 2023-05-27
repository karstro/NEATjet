using UnityEngine;

public class Generation : MonoBehaviour
{
    public int GenerationSize = 1;
    private Creature[] Creatures;
    private Simulator Sim;
    private SimulationRun[] Runs;
    private SimulationVisualiser Visualiser;

    // Start is called before the first frame update
    void Start()
    {
        Sim = new Simulator(60, 15);

        GenerateCreatures();
        Runs = Sim.SimulateCreatures(Creatures);
        DestroyCreatures();

        BeginCreatureVisualisation(0);
    }

    void GenerateCreatures() {
        Creatures = new Creature[GenerationSize];
        for (int i = 0; i < GenerationSize; i++) {
            Creatures[i] = new(CreatureType.ConfigurableJointCreature);
        }
    }

    void DestroyCreatures() {
        for (int i = 0; i < GenerationSize; i++) {
            Creatures[i].Destroy();
        }
    }

    void BeginCreatureVisualisation(int CreatureIndex) {
        Visualiser = new SimulationVisualiser(Runs[CreatureIndex]);
    }

    void Update() {
        Visualiser?.Update();
    }

    void OnGUI() {
        Visualiser?.OnGUI();
    }
}
