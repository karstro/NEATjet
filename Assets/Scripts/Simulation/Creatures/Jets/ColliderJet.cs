using NeatJet.Scripts.Simulation.Creatures.Genes;

using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures.Jets
{
    public class ColliderJet : Jet
    {
        /// <summary>
        /// The maximum speed with which the jet may rotate itself in UnityUnits/sec.
        /// </summary>
        protected float MaximumRotationSpeed { get; private set; }

        /// <summary>
        /// The amount of time in seconds that the jet should aim to spend rotating toward the target rotation. <br />
        /// The lower it is, the faster the jet will try to rotate, limited by <see cref="MaximumRotationSpeed"/>.
        /// </summary>
        protected float SmoothTime { get; private set; }

        /// <summary>
        /// The current velocity as set by SmoothDamp.
        /// </summary>
        protected Vector3 AngularVelocity;

        #region Update

        /// <inheritdoc />
        public override void UpdateByIntent(Vector3 rotationIntent, float thrustIntent, float deltaTime)
        {
            // update the joints based on intent
            UpdateJetRotation(rotationIntent, deltaTime);
            UpdateJetLimbs();
            // #TODO update thrusts based on intent
            // #TODO apply force at jets based on thrusts
        }

        /// <summary>
        /// Updates the Jet's rotation toward the target rotation.
        /// </summary>
        /// <param name="jetSpaceTargetRotation">The target rotation (direction to face) as seen from the Jet (right is outward).</param>
        /// <param name="deltaTime">The time that passed since the last update.</param>
        private void UpdateJetRotation(Vector3 jetSpaceTargetRotation, float deltaTime)
        {
            var creatureSpaceTargetRotation = JetRotation * jetSpaceTargetRotation;

            // If the Vectors are opposite, nudge the target to bias the rotation toward the jet's "forward" direction
            if (Vector3.Dot(LocalRotation, -creatureSpaceTargetRotation) > 0.95f)
            {
                creatureSpaceTargetRotation += JetRotation * Vector3.right * 0.05f;
            }

            var nextDirection = Vector3.SmoothDamp(LocalRotation, creatureSpaceTargetRotation, ref AngularVelocity, SmoothTime, MaximumRotationSpeed, deltaTime);
            LocalRotation = nextDirection;
        }

        #endregion

        #region MatchState

        /// <inheritdoc />
        public override void MatchState(Vector3 state)
        {
            JetEnd.transform.localPosition = state;
            UpdateJetLimbs();
        }

        #endregion

        #region Builder

        /// <summary>
        /// Initialize the parameters specific to the <see cref="ColliderJet"/>.
        /// </summary>
        /// <param name="gene">The <see cref="Gene"/> to initialize from.</param>
        private void InitializeColliderParameters(Gene gene)
        {
            AngularVelocity = Vector3.zero;
            MaximumRotationSpeed = gene.MaximumRotationSpeed;
            SmoothTime = gene.SmoothTime;
        }

        public static ColliderJet Add(GameObject parent, Gene gene, int jetIndex, bool makeModel)
        {
            var jet = parent.AddComponent<ColliderJet>();

            jet.InitializeParameters(gene, jetIndex);
            jet.InitializeColliderParameters(gene);

            jet.InitializeGameObjects(parent, jetIndex);

            if (makeModel)
            {
                // #TODO create a better way of replacing the meshes/materials.
                var defaultMaterial = CreatureModelProvider.DefaultMaterial;
                var sphereMesh = CreatureModelProvider.SphereMesh;
                var cylinderMesh = CreatureModelProvider.CylinderMesh;

                jet.InitializeJetModel(
                    sphereMesh, defaultMaterial,
                    sphereMesh, defaultMaterial,
                    cylinderMesh, defaultMaterial,
                    cylinderMesh, defaultMaterial);
            }

            return jet;
        }

        public static ColliderJet[] AddMultiple(GameObject parent, Gene gene, bool makeModel)
        {
            var jets = new ColliderJet[gene.Jets];

            for (var jetIndex = 0; jetIndex < jets.Length; jetIndex++)
            {
                Add(parent, gene, jetIndex, makeModel);
            }

            return jets;
        }

        #endregion
    }
}