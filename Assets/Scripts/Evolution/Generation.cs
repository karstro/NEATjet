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

    // Start is called before the first frame update
    void Start()
    {
        this.GenerateCreatures();
        this.Sim = new Simulator(60, 15);
        this.Runs = this.Sim.SimulateCreatures(this.Creatures);
        this.BeginCreatureVisualisation(0);
    }

    void GenerateCreatures() {
        this.Creatures = new Creature[this.GenerationSize];
        for (int i = 0; i < this.GenerationSize; i++) {
            this.Creatures[i] = new Creature();
        }
    }

    void BeginCreatureVisualisation(int CreatureIndex) {
        this.Visualiser = new SimulationVisualiser(this.Runs[CreatureIndex], "Creature");
    }

    void Update() {
        if (this.Visualiser != null) {
            this.Visualiser.Update();
        }
    }

    void OnGUI() {
        if (this.Visualiser != null) {
            this.Visualiser.OnGUI();
        }
    }
}
