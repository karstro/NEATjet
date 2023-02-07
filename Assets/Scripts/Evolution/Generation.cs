using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    public int GenerationSize = 1;
    private Creature[] Creatures;
    private Simulator Sim;
    private SimulationRun[] Runs;
    private SimulationVisualiser Visualiser;
    private CreatureBuilder CBuilder;
    private CreatureDirector CDirector;

    // Start is called before the first frame update
    void Start()
    {
        CBuilder = new CreatureBuilder();
        CDirector = new CreatureDirector();
        GenerateCreatures();
        Sim = new Simulator(60, 15);
        Runs = Sim.SimulateCreatures(Creatures);
        BeginCreatureVisualisation(0);
    }

    void GenerateCreatures() {
        Creatures = new Creature[GenerationSize];
        for (int i = 0; i < GenerationSize; i++) {
            CDirector.MakeSimpleCreature(CBuilder);
            Creatures[i] = CBuilder.GetResult();
        }
    }

    void BeginCreatureVisualisation(int CreatureIndex) {
        Visualiser = new SimulationVisualiser(Runs[CreatureIndex], "Creature");
    }

    void Update() {
        if (Visualiser != null) {
            Visualiser.Update();
        }
    }

    void OnGUI() {
        if (Visualiser != null) {
            Visualiser.OnGUI();
        }
    }
}
