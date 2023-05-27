using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureModelBuilder
{
    // build a sphere at the given position
    private static GameObject BuildSphereAt(Transform parent, Vector3 position, float diameter, string name) {
        // create a sphere
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Sphere.transform.parent = parent;
        Sphere.name = name;
        // set hte position and scale according to the given parameters
        Sphere.transform.localPosition = position;
        Sphere.transform.localScale = diameter * Vector3.one;
        return Sphere;
    }

    // build a cylinder whose ends are at the given start and end positions
    private static GameObject BuildCylinderFromTo(Transform parent, Vector3 Start, Vector3 End, float Width, string Name) {
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
        return Cylinder;
    }

    // build the model of the individual jet indicated by the jetIndex
    private static void BuildJet(CreatureModel model, int jetIndex, Vector3 jetStartPos, Vector3 jetEndPos) {
        float JetWidth = 2 * model._JetRadius;
        string JetName = "Jet" + jetIndex.ToString();
        // Build the spheres at the jet's start and end
        model._JetStarts[jetIndex] = BuildSphereAt(model.Transform, jetStartPos, JetWidth, JetName + "Start");
        model._JetEnds[jetIndex] = BuildSphereAt(model.Transform, jetEndPos, JetWidth, JetName + "End");
        // Build Cylinders from Center to jet's start, then to jet's end
        model._JetArms[jetIndex] = BuildCylinderFromTo(model.Transform, Vector3.zero, jetStartPos, JetWidth, JetName + "Arm");
        model._JetLegs[jetIndex] = BuildCylinderFromTo(model.Transform, jetStartPos, jetEndPos, JetWidth, JetName);
    }

    // build each jet
    private static void BuildJets(CreatureModel model, Vector3[] jetEnds) {
        for (int jetIndex = 0; jetIndex < model._Jets; jetIndex++) {
            Quaternion armRotation = CreatureBase.CalculateJetArmRotation(jetIndex, model._Jets);
            Vector3 jetStart = armRotation * model._JetArm;
            Vector3 jetEnd = jetEnds[jetIndex];
            BuildJet(model, jetIndex, jetStart, jetEnd);
        }
    }

    private static void InitializeArrays(CreatureModel model) {
        model._JetStarts = new GameObject[model._Jets];
        model._JetEnds = new GameObject[model._Jets];
        model._JetArms = new GameObject[model._Jets];
        model._JetLegs = new GameObject[model._Jets];
    }

    // build the central body of the creature
    private static void BuildBody(CreatureModel model) {
        // #TODO replace body sphere with utah teapot. thanks i hate it.
        // body is currently just a sphere
        float diameter = 2 * model._Radius;
        BuildSphereAt(model.Transform, Vector3.zero, diameter, "Body");
    }

    // Build the GameObjects that will be used to visualize the creature's simulation in the scene
    public static void Build(CreatureModel model, Vector3[] jetEnds) {
        BuildBody(model);
        InitializeArrays(model);
        BuildJets(model, jetEnds);
    }
}
