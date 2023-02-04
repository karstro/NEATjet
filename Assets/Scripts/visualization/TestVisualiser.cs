using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a class used to visualize how the jets move in response to changes in their jetAngles
public class TestVisualiser : MonoBehaviour
{
    private Creature c;
    private GameObject Object;

    // Start is called before the first frame update
    void Start() {
        this.c = new Creature();
        this.Object = CreatureModelBuilder.BuildCreatureModel(this.c, "TestCreature");
    }

    // Update is called once per frame
    void Update() {
        this.Object.transform.SetPositionAndRotation(this.c.Position, this.c.Rotation);
        // set the spheres at the positions of each jet's start and end
        for (int i = 0; i < this.Object.transform.childCount - 1; i++) {
            Transform Child = this.Object.transform.GetChild(i);
            int JetIndex = (int) i / 2;
            (Vector3 JetStartPos, Vector3 JetEndPos) = c.GetJetStartAndEnd(JetIndex);
            Vector3 JetPosition = i % 2 == 0 ? JetStartPos : JetEndPos;
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
