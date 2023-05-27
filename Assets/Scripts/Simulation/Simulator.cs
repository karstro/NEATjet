using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator
{
    private readonly int StepsPerSecond;
    private readonly int TimeOut;
    private readonly float dt;
    private readonly int Steps;

    public Simulator(int stepsPerSecond, int timeOut) {
        StepsPerSecond = stepsPerSecond;
        TimeOut = timeOut;

        // Calculate how big each timestep should be
        dt = 1f / StepsPerSecond;

        // Calculate how many steps will result from a simulation run
        Steps = TimeOut * StepsPerSecond + 1;

        // Disable automatic physics simulation so this class can manually control it
        if (Physics.autoSimulation) {
            Physics.autoSimulation = false;
        }
    }

    public SimulationRun[] SimulateCreatures(Creature[] Creatures) {
        int NumCreatures = Creatures.Length;
        SimulationRun[] Runs = new SimulationRun[NumCreatures];

        // Store each creature's state before the first timestep
        for (int CreatureIndex = 0; CreatureIndex < NumCreatures; CreatureIndex++) {
            Creature creature = Creatures[CreatureIndex];
            Runs[CreatureIndex] = new SimulationRun(creature, Steps);
            Runs[CreatureIndex].AddState(new State(0, creature));
        }

        // run each step sequentially and save the state it ends up in
        for (int TimeStep = 1; TimeStep < Steps; TimeStep++) {
            float time = TimeStep * dt;
            foreach(Creature c in Creatures) {
                c.Update(time, dt);
            }

            // Unity simulates a physics step that is dt seconds long
            Physics.Simulate(dt);
            
            // Store each creature's state after the timestep
            for (int CreatureIndex = 0; CreatureIndex < NumCreatures; CreatureIndex++) {
                //Debug.Log(new State(time, Creatures[CreatureIndex]));
                Runs[CreatureIndex].AddState(new State(time, Creatures[CreatureIndex]));
            }
        }
            
        // Finish each SimulationRun
        for (int CreatureIndex = 0; CreatureIndex < NumCreatures; CreatureIndex++) {
            Runs[CreatureIndex].Finish();
        }

        return Runs;
    }
}
