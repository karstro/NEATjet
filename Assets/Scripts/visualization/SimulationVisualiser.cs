using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// public class SimulationVisualiser : Visualiser
public class SimulationVisualiser : MonoBehaviour
{
    
    protected GameObject Object;
    protected float time;
    private bool Play;
    private float PlaySpeed;
    private Simulator Sim;
    private State[] States;

    private void InitializeSphereAt(Vector3 Pos, float Diameter, string Name) {
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Sphere.transform.parent = this.Object.transform;
        Sphere.transform.localPosition = Pos;
        Sphere.transform.localScale = Diameter * Vector3.one;
        Sphere.name = Name;
    }

    private void InitializeCylinderFromTo(Vector3 Start, Vector3 End, float Width, string Name) {
        GameObject Cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Cylinder.transform.parent = this.Object.transform;
        Cylinder.transform.localPosition = (Start + End) / 2;
        Vector3 StartToEnd = End - Start;
        Cylinder.transform.localRotation = Quaternion.FromToRotation(Vector3.up, StartToEnd);
        Cylinder.transform.localScale = new Vector3(Width, StartToEnd.magnitude / 2, Width);
        Cylinder.name = Name;
    }

    private void InitializeJet(Creature c, int JetIndex) {
        // Get the relative positions of the jet's start and end
        (Vector3 JetStartPos, Vector3 JetEndPos) = c.GetJetStartAndEnd(JetIndex);
        JetStartPos -= c.Position;
        JetEndPos -= c.Position;
        float JetWidth = 2 * c.JetRadius;
        // Initialize the spheres at the jet's start and end
        this.InitializeSphereAt(JetStartPos, JetWidth, "Jet" + JetIndex.ToString() + "Start");
        this.InitializeSphereAt(JetEndPos, JetWidth, "Jet" + JetIndex.ToString() + "End");
        // Initialize Cylinders from Center to jet's start to jet's end
        this.InitializeCylinderFromTo(Vector3.zero, JetStartPos, JetWidth, "Jet" + JetIndex.ToString() + "Arm");
        this.InitializeCylinderFromTo(JetStartPos, JetEndPos, JetWidth, "Jet" + JetIndex.ToString());
    }

    private void InitializeCreatureModel(Creature c) {
        // initialize the GameObjects that will be used to visualize the creature's simulation in the scene
        this.Object = new GameObject();
        this.Object.transform.SetPositionAndRotation(c.Position, c.Rotation);
        this.Object.name = "Creature";
        for (int i = 0; i < c.Jets; i++) {
            this.InitializeJet(c, i);
        }
        // replace body sphere with utah teapot. thanks i hate it.
        this.InitializeSphereAt(Vector3.zero, 2 * c.Radius, "Body");
    }

    // Start is called before the first frame update
    void Start() {
        // initialize the simulator and get the states from a simulation
        // this is temporary, eventually it should be given states from another source based on a simulation that happened, potentially before the visualizer existed
        this.Sim = gameObject.GetComponent<Simulator>();
        Creature c = new Creature();
        this.InitializeCreatureModel(c);
        this.States = this.Sim.simulateCreature(c);

        // initialize time related variables
        this.time = 0f;
        this.Play = true;
        this.PlaySpeed = 1;
    }

    // interpolate in between states from the States array based on this.time
    private State GetInterpolatedState() {
        // binary search algorithm, since time is ordered in the states array
        int low = 0;
        int high = this.States.Length - 2;
        int result;

        // if the target time is out of bounds, throw an error
        Assert.IsFalse(this.time < this.States[low].time || this.time > this.States[high + 1].time,
                       "Time variable is out of bounds of the States");

        // keep count of iterations in case of endless looping
        int i = 0;
        // while the search hasn't converged
        while (true) {
            // calculate the middle of the search range
            int mid = (low + high) / 2;
            if (i > 30) {
                result = 0;
                Debug.Log("more than 20 iterations in search");
                break;
            }
            i++;

            if (this.time < this.States[mid].time) {
                // if this.time is below the middle range, restrict the search range to the lower half
                high = mid - 1;
            } else if (this.time > this.States[mid + 1].time) {
                // if this.time is above the middle range, restrict the search range to the upper half
                low = mid + 1;
            } else {
                // if this.time is in the middle range, the search is complete
                result = mid;
                break;
            }
        }

        // linearly interpolate between the two states around the result using this.time as the fraction.
        State Before = this.States[result];
        State After = this.States[result + 1];
        float frac = Mathf.InverseLerp(Before.time, After.time, this.time);
        Vector3 InterpolatedPosition = Vector3.Lerp(Before.Position, After.Position, frac);
        Quaternion InterpolatedRotation = Quaternion.Slerp(Before.Rotation, After.Rotation, frac);
        return new State(this.time, InterpolatedPosition, InterpolatedRotation);
    }

    // apply the given state to the GameObjects in the scene
    private void VisualiseState(State s) {
        this.Object.transform.SetPositionAndRotation(s.Position, s.Rotation);
    }

    // Update is called once per frame
    void Update() {
        if (this.States != null && this.States.Length > 1) {
            State InterpolatedState = this.GetInterpolatedState();
            this.VisualiseState(InterpolatedState);
            if (this.Play) {
                this.time += Time.deltaTime * this.PlaySpeed;
                if (this.time > this.States[this.States.Length - 1].time) {
                    this.time = this.States[this.States.Length - 1].time;
                }
            }
        }
    }

    void OnGUI() {
        float SliderWidth = Screen.width * 2 / 3;
        float SliderHeight = 30;
        float x = (Screen.width - SliderWidth) / 2;
        float y = Screen.height - SliderHeight - 20;
        this.time = GUI.HorizontalSlider(new Rect(x, y, SliderWidth, SliderHeight), this.time, 0f, this.Sim.TimeOut);
        // this.time = GUI.HorizontalSlider(new Rect(x, y, SliderWidth, SliderHeight), this.time, 0f, 15f);
        // add play/pause button
        float ButtonWidth = 40;
        if (GUI.Button(new Rect(x - ButtonWidth - 10, y - SliderHeight / 3, ButtonWidth, SliderHeight), this.Play.ToString())) {
            this.Play = !this.Play;
        }
    }
}
