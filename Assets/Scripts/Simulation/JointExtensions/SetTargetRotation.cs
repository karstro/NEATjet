using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTargetRotation : MonoBehaviour
{
    Quaternion StartRotation;
    Vector3 Intent = Vector3.right;
    ConfigurableJoint joint;
    // Start is called before the first frame update
    void Start() {
        joint = GetComponent<ConfigurableJoint>();
        StartRotation = gameObject.transform.localRotation;
    }

    // Update is called once per frame
    void Update() {
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.down, Intent);
        joint.SetTargetRotationLocal(targetRotation, StartRotation);
    }

    private void OnGUI() {
        float SliderLength = 100;
        float SliderHeight = 30;
        float x = 25 + SliderLength;
        float y = (Screen.height - SliderHeight) / 2;
        for (int i = 0; i < 3; i++) {
            int heightDiff = (i - 1) * 25;
            Intent[i] = GUI.HorizontalSlider(new Rect(x, y + heightDiff, SliderLength, SliderHeight), Intent[i], -1f, 1f);
        }
        Intent = Intent.normalized;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Vector3 position = gameObject.transform.position;
        Quaternion rotation = gameObject.transform.parent.rotation;
        Vector3 worldIntent = position + rotation * Intent;
        Gizmos.DrawLine(position, worldIntent);
    }
}
