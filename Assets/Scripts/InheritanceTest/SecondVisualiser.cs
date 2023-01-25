using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using State = System.ValueTuple<float, Vector3, Quaternion>;
// using State = (float time, Vector3 Position, Quaternion Rotation);

public class SecondVisualiser : Visualiser
{
    // private Visualiser v;

    // Start is called before the first frame update
    void Start() {
        this.time = 0f;
        this.c = new Creature();
        // this.v = new InheritanceVisualiser();
    }

    // Update is called once per frame
    void Update() {
        this.time += Time.deltaTime;
        // Debug.Log(this.c.Position);
        // this.v.SayHi(this.c);
        // Debug.Log(this.c.Position);
        // Tuple<Vector3, Quaternion> tup = (this.c.Position, this.c.Rotation);
        // var tup = (Position: this.c.Position, Rotation: this.c.Rotation);
        // (Vector3 Position, Quaternion Rotation) tup = (this.c.Position, this.c.Rotation);
        // State tup = (this.c.Position, this.c.Rotation);
        // Debug.Log(tup.GetType());
        // Debug.Log(tup.Item1);
        // Debug.Log(tup.Position);
        // this.v.SayHi(this.c);
        // Debug.Log(tup);
    }

    public override void SayHi(Creature c) {}
}
