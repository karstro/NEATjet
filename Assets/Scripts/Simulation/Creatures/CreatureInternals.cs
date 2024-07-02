using UnityEngine;

namespace NeatJet.Scripts.Simulation.Creatures
{
    public abstract class CreatureInternals
    {
        public float Radius { get; set; }
        public int Jets { get; set; }
        public float JetLength { get; set; }
        public float JetRadius { get; set; }
        public Vector3 JetArm { get; set; }
        public GameObject Object { get; set; }
        public GameObject BodyObject { get; set; }
        public GameObject[] JetStarts { get; set; }
        public GameObject[] JetEnds { get; set; }
        public GameObject[] JetArms { get; set; }
        public GameObject[] JetLegs { get; set; }

        /// <summary>
        /// gets the position and rotation of the creature's GameObject
        /// </summary>
        /// <returns></returns>
        public (Vector3, Quaternion) GetPositionAndRotation()
        {
            return (Object.transform.position, Object.transform.rotation);
        }

        /// <summary>
        /// set the position and rotation of the creature's GameObject
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            Object.transform.SetPositionAndRotation(position, rotation);
        }

        /// <summary>
        /// returns the local coordinates of the start of the given Jet
        /// </summary>
        /// <param name="JetIndex"></param>
        /// <returns></returns>
        public Vector3 GetLocalJetStart(int JetIndex)
        {
            return JetStarts[JetIndex].transform.localPosition;
        }

        /// <summary>
        /// returns the local coordinates of the end of the given Jet
        /// </summary>
        /// <param name="JetIndex"></param>
        /// <returns></returns>
        public Vector3 GetLocalJetEnd(int JetIndex)
        {
            return JetEnds[JetIndex].transform.localPosition;
        }

        /// <summary>
        /// returns the local coordinates of the start and end of the given Jet
        /// </summary>
        /// <param name="JetIndex"></param>
        /// <returns></returns>
        public (Vector3, Vector3) GetLocalJetStartAndEnd(int JetIndex)
        {
            var jetStart = JetStarts[JetIndex].transform.localPosition;
            var jetEnd = JetEnds[JetIndex].transform.localPosition;
            return (jetStart, jetEnd);
        }

        /// <summary>
        /// returns a Quaternion describing the rotation of the given jet's arm relative to the creature's forward direction
        /// </summary>
        /// <param name="jetIndex"></param>
        /// <param name="jets"></param>
        /// <returns></returns>
        public static Quaternion CalculateJetArmRotation(int jetIndex, int jets)
        {
            // the angle in degrees
            var armFacingAngle = jetIndex * 360f / jets;
            // a rotation of that many degrees around the Y axis
            return Quaternion.Euler(0, armFacingAngle, 0);
        }
    }
}
