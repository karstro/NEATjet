using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator
{
    public int StepsPerSecond = 60;
    public int TimeOut = 15;
    private float dt;
    private int Steps;

    public Simulator(int StepsPerSecond, int TimeOut) {
        // Calculate how big each timestep should be
        this.dt = 1 / (float) this.StepsPerSecond;

        // Calculate how many steps will result from a simulation run
        this.Steps = this.TimeOut * this.StepsPerSecond + 1;

        // Disable automatic physics simulation so this class can manually control it
        if (Physics.autoSimulation) {
            Physics.autoSimulation = false;
        }
    }

    public State[,] SimulateCreatures(Creature[] Creatures) {
        int NumCreatures = Creatures.Length;
        State[,] States = new State[NumCreatures, Steps];

        // Store each creature's state before the first timestep
        for (int CreatureIndex = 0; CreatureIndex < NumCreatures; CreatureIndex++) {
            States[CreatureIndex, 0] = new State(0, Creatures[CreatureIndex]);
        }

        // run each step sequentially and save the state it ends up in
        for (int TimeStep = 1; TimeStep < Steps; TimeStep++) {
            float time = TimeStep * this.dt;
            // #TODO:
            // Creature[Index].UpdateIntent(time, other senses)
            // Creature[Index].ApplyIntentForces()

            // Unity simulates a physics step that is this.dt seconds long
            Physics.Simulate(this.dt);
            
            // Store each creature's state after the timestep
            for (int CreatureIndex = 0; CreatureIndex < NumCreatures; CreatureIndex++) {
                States[CreatureIndex, TimeStep] = new State(time, Creatures[CreatureIndex]);
            }
        }
        return States;
    }
}
