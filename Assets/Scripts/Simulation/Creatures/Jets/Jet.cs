using NeatJet.Scripts.Simulation.Creatures.Genes;

using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures.Jets
{
    /// <summary>
    /// The base class of the Jets.
    /// </summary>
    public abstract class Jet : MonoBehaviour
    {
        /// <summary>
        /// The length of the jet.
        /// </summary>
        public float Length { get; private set; }

        /// <summary>
        /// The radius of each component of the jet.
        /// </summary>
        public float Radius { get; private set; }

        /// <summary>
        /// The rotation of the jet compared to the parent.
        /// </summary>
        protected Quaternion JetRotation { get; private set; }

        /// <summary>
        /// The jet's arm, from the body to the start.
        /// </summary>
        public Vector3 Arm { get; private set; }

        /// <summary>
        /// The maximum thrust the jet can output.
        /// </summary>
        protected float MaximumThrust { get; private set; }

        /// <summary>
        /// The maximum change 
        /// </summary>
        protected float MaximumThrustChangePerSecond { get; private set; }

        /// <summary>
        /// The current thrust, between -1f and 1f. Represents the fraction of the MaximumThrust
        /// </summary>
        protected float Thrust { get; set; }

        /// <summary>
        /// The local position of the Jet's start.
        /// </summary>
        public Vector3 LocalStart { get => JetStart.transform.localPosition; }

        /// <summary>
        /// The local position of the Jet's end.
        /// </summary>
        public Vector3 LocalEnd { get => JetEnd.transform.localPosition; }

        /// <summary>
        /// A vector representing the current rotation of the jet (in jet space)
        /// </summary>
        public Vector3 LocalRotation
        {
            get => LocalEnd - LocalStart;
            protected set => JetEnd.transform.localPosition = LocalStart + (value.normalized * Length);
        }

        /// <summary>
        /// The <see cref="GameObject"/> whose transform is the parent of the Jet objects.
        /// </summary>
        protected GameObject Parent { get; private set; }

        /// <summary>
        /// The <see cref="GameObject"/> that contains the start of the Jet.
        /// </summary>
        protected GameObject JetStart { get; private set; }

        /// <summary>
        /// The <see cref="GameObject"/> that contains the end of the Jet.
        /// </summary>
        protected GameObject JetEnd { get; private set; }

        /// <summary>
        /// The <see cref="GameObject"/> that contains the model of the Jet's arm. <br />
        /// Can be null if there is no model.
        /// </summary>
        protected GameObject ArmModel { get; private set; }

        /// <summary>
        /// The <see cref="GameObject"/> that contains the model of the Jet's leg. <br />
        /// Can be null if there is no model.
        /// </summary>
        protected GameObject LegModel { get; private set; }

        /// <summary>
        /// Make the Jet match the given state.
        /// </summary>
        /// <param name="state">The state to match.</param>
        public abstract void MatchState(Vector3 state);

        /// <summary>
        /// Updates the Jet by the given intent, then apply thrust.
        /// </summary>
        /// <param name="rotationIntent">Where the jet should intend to rotate toward.</param>
        /// <param name="thrustIntent">How much thrust the jet should intend to output.</param>
        /// <param name="deltaTime">The time that has passed since the last update took place.</param>
        public abstract void UpdateByIntent(Vector3 rotationIntent, float thrustIntent, float deltaTime);

        /// <summary>
        /// Updates the jet's limbs to match the current position of the jet objects.
        /// </summary>
        public void UpdateJetLimbs()
        {
            if (LegModel == null)
            {
                return;
            }

            // the leg's center is the position between the start and end positions
            LegModel.transform.localPosition = (LocalStart + LocalEnd) / 2;
            // change the rotation of the leg such that it points from the start toward end
            var StartToEnd = LocalEnd - LocalStart;
            LegModel.transform.localRotation = Quaternion.FromToRotation(Vector3.up, StartToEnd);
        }

        #region Builder

        /// <summary>
        /// Initialize all of the jet's parameters
        /// </summary>
        /// <param name="gene"></param>
        /// <param name="jetIndex"></param>
        protected void InitializeParameters(Gene gene, int jetIndex)
        {
            Thrust = 0f;
            MaximumThrust = gene.MaximumThrustPerJet;
            MaximumThrustChangePerSecond = gene.MaximumThrustChangePerSecond;
            Length = gene.JetLength;
            Radius = gene.JetRadius;
            Arm = gene.JetArm;
            JetRotation = CalculateRotation(jetIndex, gene.Jets);
        }

        /// <summary>
        /// Creates each of the necessary <see cref="GameObject"/>s needed for the jet on the <paramref name="parent"/> object.
        /// </summary>
        /// <param name="parent"></param>
        protected void InitializeGameObjects(GameObject parent, int jetIndex)
        {
            Parent = parent;
            var (jetStart, jetEnd) = CalculateJetStartAndEnd();
            JetStart = CreateJetObject(jetStart, "JetStart" + jetIndex);
            JetEnd = CreateJetObject(jetEnd, "JetEnd" + jetIndex);
        }

        private GameObject CreateJetObject(Vector3 localPosition, string name)
        {
            var jetObject = new GameObject(name);

            SetJetObjectTransform(jetObject, localPosition);
            InitializeJetSphereCollider(jetObject);

            return jetObject;
        }

        /// <summary>
        /// Sets necessary variables on the jetObject's transform.
        /// </summary>
        /// <param name="jetObject"></param>
        /// <param name="localPosition"></param>
        private void SetJetObjectTransform(GameObject jetObject, Vector3 localPosition)
        {
            var transform = jetObject.transform;
            transform.parent = Parent.transform;
            transform.SetLocalPositionAndRotation(localPosition, JetRotation);

            var diameter = Radius * 2;
            transform.localScale = diameter * Vector3.one;
        }

        /// <summary>
        /// creates a SphereCollider at the center of the given GameObject
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private SphereCollider InitializeJetSphereCollider(GameObject parent)
        {
            // create the collider component
            var collider = parent.AddComponent<SphereCollider>();
            // all needed colliders must be at the center of their parent object
            collider.center = Vector3.zero;
            // localScale is the diameter
            collider.radius = 0.5f;
            return collider;
        }

        /// <summary>
        /// calculates the locations of the jet's start and end in the creature's local space
        /// </summary>
        /// <returns></returns>
        private (Vector3, Vector3) CalculateJetStartAndEnd()
        {
            // the start of a jet is at the end of the jetArm
            var jetStart = JetRotation * Arm;
            // when created, a creature's jets always point down
            var jetVector = Length * Vector3.down;
            // the end of the jet is at the end of the jetVector, starting from the jetStart
            var jetEnd = jetStart + jetVector;
            return (jetStart, jetEnd);
        }

        #region ModelBuilder

        protected void InitializeJetModel(
            Mesh jetStartMesh, Material jetStartMaterial,
            Mesh jetEndMesh, Material jetEndMaterial,
            Mesh jetArmMesh, Material jetArmMaterial,
            Mesh jetLegMesh, Material jetLegMaterial)
        {
            ComponentAdder.AddMesh(JetStart, jetStartMesh, jetStartMaterial);
            ComponentAdder.AddMesh(JetEnd, jetEndMesh, jetEndMaterial);

            // The jetIndex is postfixed onto the name, can be reused for the name of the limbs.
            var index = JetStart.name[^1];
            ArmModel = CreateLimbModel(Vector3.zero, LocalStart, jetArmMesh, jetArmMaterial, "Arm" + index);
            LegModel = CreateLimbModel(LocalStart, LocalEnd, jetLegMesh, jetLegMaterial, "Leg" + index);
        }

        /// <summary>
        /// Create a limb model between the given local positions using the given mesh.
        /// </summary>
        /// <param name="localFrom"></param>
        /// <param name="localTo"></param>
        /// <param name="limbMesh"></param>
        /// <param name="limbMaterial"></param>
        /// <returns></returns>
        private GameObject CreateLimbModel(Vector3 localFrom, Vector3 localTo, Mesh limbMesh, Material limbMaterial, string name)
        {
            var limb = new GameObject(name);
            limb.transform.parent = Parent.transform;

            // the cylinder's center is the position between the start and end positions
            limb.transform.localPosition = (localFrom + localTo) / 2;

            // change the rotation of the cylinder such that it points from the start toward end
            var startToEnd = localTo - localFrom;
            limb.transform.localRotation = Quaternion.FromToRotation(Vector3.up, startToEnd);

            // shape the cylinder so that it is exacly long enough to touch start and end
            var length = startToEnd.magnitude / 2;
            var width = Radius * 2;
            limb.transform.localScale = new Vector3(width, length, width);

            // Add the mesh
            ComponentAdder.AddMesh(limb, limbMesh, limbMaterial);

            return limb;
        }

        #endregion

        #endregion

        /// <summary>
        /// returns a Quaternion describing the rotation of the given jet's arm relative to the creature's forward direction
        /// </summary>
        /// <param name="jetIndex"></param>
        /// <param name="jets"></param>
        /// <returns></returns>
        public static Quaternion CalculateRotation(int jetIndex, int jets)
        {
            // the angle in degrees
            var armFacingAngle = jetIndex * 360f / jets;
            // a rotation of that many degrees around the Y axis
            return Quaternion.Euler(0, armFacingAngle, 0);
        }
    }
}