using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures
{
    public static class CreatureModelProvider
    {
        public static Material DefaultMaterial { get; } = GetDefaultMaterial();
        public static Mesh SphereMesh { get; } = GetSphereMesh();
        public static Mesh CylinderMesh { get; } = GetCylinderMesh();

        /// <summary>
        /// Retrieve a copy of the default material.
        /// </summary>
        /// <returns></returns>
        private static Material GetDefaultMaterial()
        {
            // create a primitive sphere
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // copy the necessary component
            var material = sphere.GetComponent<MeshRenderer>().material;
            var materialCopy = Object.Instantiate(material);
            // destroy the sphere, including its original components
            Object.Destroy(sphere);
            // return the copy
            return materialCopy;
        }

        /// <summary>
        /// Retrieve a copy of the primitive sphere mesh.
        /// </summary>
        /// <returns></returns>
        private static Mesh GetSphereMesh()
        {
            // create a primitive sphere
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // copy the necessary component
            var sphereMesh = sphere.GetComponent<MeshFilter>().mesh;
            var sphereMeshCopy = Object.Instantiate(sphereMesh);
            // destroy the sphere, including its original components
            Object.Destroy(sphere);
            // return the copy
            return sphereMeshCopy;
        }

        /// <summary>
        /// Retrieve a copy of the mesh and material of the primitive cylinder.
        /// </summary>
        /// <returns></returns>
        private static Mesh GetCylinderMesh()
        {
            // create a primitive cylinder
            var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            // copy the necessary component
            var cylinderMesh = cylinder.GetComponent<MeshFilter>().mesh;
            var cylinderMeshCopy = Object.Instantiate(cylinderMesh);
            // destroy the cylinder, including its original components
            Object.Destroy(cylinder);
            // return the copy
            return cylinderMeshCopy;
        }
    }
}