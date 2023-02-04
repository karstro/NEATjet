using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureModelBuilder
{
    // build a sphere at the given position
    private static void BuildSphereAt(Transform parent, Vector3 Pos, float Diameter, string Name) {
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Sphere.transform.parent = parent;
        Sphere.transform.localPosition = Pos;
        Sphere.transform.localScale = Diameter * Vector3.one;
        Sphere.name = Name;
    }

    // build a cylinder whose ends are at the given start and end positions
    private static void BuildCylinderFromTo(Transform parent, Vector3 Start, Vector3 End, float Width, string Name) {
        GameObject Cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Cylinder.transform.parent = parent;
        Cylinder.transform.localPosition = (Start + End) / 2;
        Vector3 StartToEnd = End - Start;
        Cylinder.transform.localRotation = Quaternion.FromToRotation(Vector3.up, StartToEnd);
        Cylinder.transform.localScale = new Vector3(Width, StartToEnd.magnitude / 2, Width);
        Cylinder.name = Name;
    }

    // build the model of the individual jet indicated by the jetIndex
    private static void BuildJet(Transform parent, Creature c, int JetIndex) {
        // Get the positions of the jet's start and end
        (Vector3 JetStartPos, Vector3 JetEndPos) = c.GetJetStartAndEnd(JetIndex);
        // turn them into the positions relative to the creature's origin
        JetStartPos -= c.Position;
        JetEndPos -= c.Position;
        float JetWidth = 2 * c.JetRadius;
        string JetName = "Jet" + JetIndex.ToString();
        // Build the spheres at the jet's start and end
        CreatureModelBuilder.BuildSphereAt(parent, JetStartPos, JetWidth, JetName + "Start");
        CreatureModelBuilder.BuildSphereAt(parent, JetEndPos, JetWidth, JetName + "End");
        // Build Cylinders from Center to jet's start, then to jet's end
        CreatureModelBuilder.BuildCylinderFromTo(parent, Vector3.zero, JetStartPos, JetWidth, JetName + "Arm");
        CreatureModelBuilder.BuildCylinderFromTo(parent, JetStartPos, JetEndPos, JetWidth, JetName);
    }

    // build the central body of the creature
    private static void BuildBody(Transform parent, Creature c) {
        // #TODO replace body sphere with utah teapot. thanks i hate it.
        // body is currently just a sphere
        CreatureModelBuilder.BuildSphereAt(parent, Vector3.zero, 2 * c.Radius, "Body");
    }

    // Build the GameObjects that will be used to visualize the creature's simulation in the scene
    public static GameObject BuildCreatureModel(Creature c, string name) {
        // create the parent object
        // GameObject Object = new GameObject();
        GameObject Object = c.GetGameObject();

        // set the parent object's properties
        // Object.transform.SetPositionAndRotation(c.Position, c.Rotation);
        Object.name = name;

        // build each jet and the body
        for (int i = 0; i < c.Jets; i++) {
            CreatureModelBuilder.BuildJet(Object.transform, c, i);
        }
        CreatureModelBuilder.BuildBody(Object.transform, c);

        // Return the parent object that now contains all the elements of the creature model
        return Object;
    }
}
