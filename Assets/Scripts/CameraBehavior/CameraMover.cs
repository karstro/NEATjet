using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    private float Angle = 0f;
    private float Height = 30f;
    private float Distance = 5f;

    void OnGUI() {
        float SliderLength = 100;
        float SliderHeight = 30;
        float x = Screen.width - 25 - SliderLength;
        float y = (Screen.height - SliderHeight) / 2;
        this.Angle = GUI.HorizontalSlider(new Rect(x, y - 25, SliderLength, SliderHeight), this.Angle, -180.0f, 180.0f);
        this.Height = GUI.HorizontalSlider(new Rect(x, y, SliderLength, SliderHeight), this.Height, 10f, 89.9f);
        this.Distance = GUI.HorizontalSlider(new Rect(x, y + 25, SliderLength, SliderHeight), this.Distance, 1f, 10f);
        Vector3 FromCenter = Quaternion.Euler(-this.Height, this.Angle, 0) * Vector3.forward;
        Quaternion TowardCenter = Quaternion.LookRotation(-FromCenter);
        transform.position = FromCenter * this.Distance;
        transform.rotation = TowardCenter;
    }
}
