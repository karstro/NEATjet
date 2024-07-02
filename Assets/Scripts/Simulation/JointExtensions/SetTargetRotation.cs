using UnityEngine;

namespace NeatJet.Scripts.Simulation.JointExtensions
{
    public class SetTargetRotation : MonoBehaviour
    {
        Quaternion StartRotation;
        Vector3 Intent = Vector3.right;
        ConfigurableJoint joint;
        // Start is called before the first frame update
        void Start()
        {
            joint = GetComponent<ConfigurableJoint>();
            StartRotation = gameObject.transform.localRotation;
        }

        // Update is called once per frame
        void Update()
        {
            var targetRotation = Quaternion.FromToRotation(Vector3.down, Intent);
            joint.SetTargetRotationLocal(targetRotation, StartRotation);
        }

        private void OnGUI()
        {
            float SliderLength = 100;
            float SliderHeight = 30;
            var x = 25 + SliderLength;
            var y = (Screen.height - SliderHeight) / 2;
            for (var i = 0; i < 3; i++)
            {
                var heightDiff = (i - 1) * 25;
                Intent[i] = GUI.HorizontalSlider(new Rect(x, y + heightDiff, SliderLength, SliderHeight), Intent[i], -1f, 1f);
            }

            Intent = Intent.normalized;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            var position = gameObject.transform.position;
            var rotation = gameObject.transform.parent.rotation;
            var worldIntent = position + (rotation * Intent);
            Gizmos.DrawLine(position, worldIntent);
        }
    }
}
