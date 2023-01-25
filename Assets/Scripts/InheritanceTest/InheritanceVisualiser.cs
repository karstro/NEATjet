using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InheritanceVisualiser : Visualiser
{
    // Start is called before the first frame update
    void Start() {
        this.time = 0f;
        this.c = new Creature();
    }

    // Update is called once per frame
    void Update() {
        this.time += Time.deltaTime;
    }

    public override void SayHi(Creature c) {
        Debug.Log("Hi");
        c.Position = new Vector3(2,2,2);
        c.Rotation = new Quaternion(0, 1, 2, 3);
    }
}
