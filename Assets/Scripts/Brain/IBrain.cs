using NeatJet.Scripts.Simulation.Creatures;

using UnityEngine;

namespace NeatJet.Scripts.Brain
{
    public interface IBrain
    {
        public (Vector3[], float[]) GetIntent(float time, JetCreature creature);
    }
}
