using UnityEngine;

// creates one real-time creature for observation and debugging
public class TestGen : MonoBehaviour {
    private Creature creature;

    public void Start() {
        creature = new(CreatureType.ConfigurableJointCreature);
    }

    // Update is called once per frame
    public void Update() {
        creature.Update(Time.time, Time.deltaTime);
    }
}
