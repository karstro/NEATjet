using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures
{
    /// <summary>
    /// Contains functions to add components to GameObjects,
    /// </summary>
    public static class ComponentAdder
    {
        /// <summary>
        /// creates a SphereCollider at the center of the given GameObject
        /// </summary>
        /// <param name="parent">The object to add the collider to.</param>
        /// <param name="radius">The radius of the collider.</param>
        /// <returns>The created collider.</returns>
        public static SphereCollider AddSphereCollider(GameObject parent, float radius)
        {
            // create the collider component
            var collider = parent.AddComponent<SphereCollider>();
            // all needed colliders must be at the center of their parent object
            collider.center = Vector3.zero;
            collider.radius = radius;
            return collider;
        }

        /// <summary>
        /// Add a mesh and renderer to given component so the unity engine can render it.
        /// </summary>
        /// <param name="parent">The object to add the mesh to.</param>
        /// <param name="mesh">The mesh to add.</param>
        /// <param name="material">The material with which to render the mesh.</param>
        public static void AddMesh(GameObject parent, Mesh mesh, Material material)
        {
            // create MeshFilter and apply the mesh of a sphere
            var filter = parent.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            // create MeshRenderer and apply default material
            var renderer = parent.AddComponent<MeshRenderer>();
            renderer.material = material;
        }

        /// <summary>
        /// Add a rigidbody to the given object.
        /// </summary>
        /// <param name="parent">The object to add the rigidbody to.</param>
        /// <param name="mass">The mass of the rigidbody.</param>
        /// <returns>The added rigidbody.</returns>
        public static Rigidbody AddRigidbody(GameObject parent, float mass)
        {
            var rigidbody = parent.AddComponent<Rigidbody>();
            rigidbody.mass = mass;
            return rigidbody;
        }
    }
}