using UnityEngine;

public class CameraMover : MonoBehaviour
{
    private float Angle = 0f;
    private float Height = 30f;
    private float Distance = 5f;

    void OnGUI()
    {
        float sliderLength = 100;
        float sliderHeight = 30;
        var x = Screen.width - 25 - sliderLength;
        var y = (Screen.height - sliderHeight) / 2;

        Angle = GUI.HorizontalSlider(new Rect(x, y - 25, sliderLength, sliderHeight), Angle, -180.0f, 180.0f);
        Height = GUI.HorizontalSlider(new Rect(x, y, sliderLength, sliderHeight), Height, 10f, 89.9f);
        Distance = GUI.HorizontalSlider(new Rect(x, y + 25, sliderLength, sliderHeight), Distance, 1f, 10f);

        var fromCenter = Quaternion.Euler(-Height, Angle, 0) * Vector3.forward;
        var towardCenter = Quaternion.LookRotation(-fromCenter);
        transform.SetPositionAndRotation(fromCenter * Distance, towardCenter);
    }
}
