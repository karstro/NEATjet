using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGen : MonoBehaviour
{
    Creature c;
    // Start is called before the first frame update
    public void Start() {
        CreatureBuilder builder = new CreatureBuilder();
        CreatureDirector director = new CreatureDirector();
        director.MakeSimpleCreature(builder);
        c = builder.GetResult();
    }

    // Update is called once per frame
    public void Update() {
        c.Update(Time.time, Time.deltaTime);
    }

    public void OnDrawGizmos() {
        if (c != null) {
            c.OnDrawGizmos();
        }
    }
}
