using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureModelBuilder
{
    // build a sphere at the given position
    private static void BuildSphereAt(Transform parent, Vector3 Pos, float Diameter, string Name) {
        // create a sphere
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Sphere.transform.parent = parent;
        Sphere.name = Name;
        // set hte position and scale according to the given parameters
        Sphere.transform.localPosition = Pos;
        Sphere.transform.localScale = Diameter * Vector3.one;
    }

    // build a cylinder whose ends are at the given start and end positions
    private static void BuildCylinderFromTo(Transform parent, Vector3 Start, Vector3 End, float Width, string Name) {
        // create a cylinder
        GameObject Cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Cylinder.transform.parent = parent;
        Cylinder.name = Name;
        // the cylinder's center is the position between the start and end positions
        Cylinder.transform.localPosition = (Start + End) / 2;
        // change the rotation of the cylinder such that it points from the start toward end
        Vector3 StartToEnd = End - Start;
        Cylinder.transform.localRotation = Quaternion.FromToRotation(Vector3.up, StartToEnd);
        // shape the cylinder so that it is exacly long enough to touch start and end
        Cylinder.transform.localScale = new Vector3(Width, StartToEnd.magnitude / 2, Width);
    }

    // build the model of the individual jet indicated by the jetIndex
    private static void BuildJet(CreatureModel model, int JetIndex) {
        // Get the positions of the jet's start and end
        (Vector3 JetStartPos, Vector3 JetEndPos) = model.GetLocalJetStartAndEnd(JetIndex);
        float JetWidth = 2 * model._JetRadius;
        string JetName = "Jet" + JetIndex.ToString();
        // Build the spheres at the jet's start and end
        BuildSphereAt(model.Transform, JetStartPos, JetWidth, JetName + "Start");
        BuildSphereAt(model.Transform, JetEndPos, JetWidth, JetName + "End");
        // Build Cylinders from Center to jet's start, then to jet's end
        BuildCylinderFromTo(model.Transform, Vector3.zero, JetStartPos, JetWidth, JetName + "Arm");
        BuildCylinderFromTo(model.Transform, JetStartPos, JetEndPos, JetWidth, JetName);
    }

    // build each jet
    private static void BuildJets(CreatureModel model) {
        for (int i = 0; i < model._Jets; i++) {
            BuildJet(model, i);
        }
    }

    // build the central body of the creature
    private static void BuildBody(CreatureModel model) {
        // #TODO replace body sphere with utah teapot. thanks i hate it.
        // body is currently just a sphere
        float diameter = 2 * model._Radius;
        BuildSphereAt(model.Transform, Vector3.zero, diameter, "Body");
    }

    // Build the GameObjects that will be used to visualize the creature's simulation in the scene
    public static void Build(CreatureModel model) {
        BuildBody(model);
        BuildJets(model);
    }
}
