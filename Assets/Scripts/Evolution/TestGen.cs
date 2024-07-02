using NeatJet.Scripts.Simulation.Creatures;

using UnityEngine;

namespace NeatJet.Scripts.Evolution
{
    // creates one real-time creature for observation and debugging
    public class TestGen : MonoBehaviour
    {
        public CreatureType CreatureType;
        private Creature Creature;

        public void Start()
        {
            Creature = new(CreatureType);
        }

        // Update is called once per frame
        public void Update()
        {
            Creature.Update(Time.time, Time.deltaTime);
        }
    }
}
