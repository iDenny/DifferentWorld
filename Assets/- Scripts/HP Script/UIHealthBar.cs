using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIHealthBar : MonoBehaviour
{
    public Transform target;
    public Image foregroundImage;
    public Image backgroundImage;
    public Vector3 offset;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 directionToHealthBar = (transform.position - Camera.main.transform.position).normalized;
        float angle = Vector3.Angle(Camera.main.transform.forward, directionToHealthBar);

        bool isBehind = angle > 10f; // The health bar is behind if the angle is greater than 90 degrees
        foregroundImage.enabled = isBehind;
        backgroundImage.enabled = isBehind;
        transform.position = Camera.main.WorldToScreenPoint(target.position + offset);

    }

    public void SetHealthBarPercentage(float percentage)
    {
        float parentWidth = GetComponent < RectTransform>().rect.width;
        float width = parentWidth * percentage;
        foregroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
}
