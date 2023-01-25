using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class TestVisualiser : Visualiser
public class TestVisualiser : MonoBehaviour
{
    private Creature c;
    protected GameObject Object;
    protected float time;

    // Start is called before the first frame update
    void Start() {
        this.time = 0f;
        this.c = new Creature();
        this.Object = new GameObject();
        // initialize the spheres that will be placed at each jet's start and end
        for (int i = 0; i < c.Jets; i++) {
            GameObject JetStart = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject JetEnd = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            JetStart.transform.parent = this.Object.transform;
            JetEnd.transform.parent = this.Object.transform;
            JetStart.transform.localScale = 2 * this.c.JetRadius * Vector3.one;
            JetEnd.transform.localScale = 2 * this.c.JetRadius * Vector3.one;
            JetStart.name = i.ToString() + "Start";
            JetEnd.name = i.ToString() + "End";
        }
        // TODO: replace body sphere with utah teapot. thanks i hate it.
        GameObject Body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Body.transform.parent = this.Object.transform;
        Body.transform.localPosition = Vector3.zero;
        Body.transform.localScale = 2 * this.c.Radius * Vector3.one;
        Body.name = "Body";
    }

    // Update is called once per frame
    void Update() {
        this.Object.transform.SetPositionAndRotation(this.c.Position, this.c.Rotation);
        // set the spheres at the positions of each jet's start and end
        for (int i = 0; i < this.Object.transform.childCount - 1; i++) {
            Transform Child = this.Object.transform.GetChild(i);
            int JetIndex = (int) i / 2;
            Vector3 JetPosition = i % 2 == 0 ? this.c.GetJetStart(JetIndex) : this.c.GetJetEnd(JetIndex);
            Child.localPosition = JetPosition - this.c.Position;
        }
    }

    void OnGUI() {
        // create sliders that control each jet
        for (int i = 0; i < this.c.Jets; i++) {
            for (int j = 0; j < 2; j++) {
                this.c.JetAngles[i][j + 1] = GUI.HorizontalSlider(new Rect(25, 25 * (2 * i + j), 100, 30), this.c.JetAngles[i][j + 1], -180.0f, 180.0f);
            }
        }
    }
}
