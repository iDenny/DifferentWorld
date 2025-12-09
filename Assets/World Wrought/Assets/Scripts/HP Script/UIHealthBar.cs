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
        if (Camera.main == null || target == null)
            return;

        // Use viewport test rather than raw angle so we accurately determine visibility
        Vector3 screenPos = Camera.main.WorldToViewportPoint(target.position + offset);
        bool inFront = screenPos.z > 0f;
        bool onScreen = screenPos.x >= 0f && screenPos.x <= 1f && screenPos.y >= 0f && screenPos.y <= 1f;

        bool visible = inFront && onScreen;

        foregroundImage.enabled = visible;
        backgroundImage.enabled = visible;

        // Position the UI element using screen space point so anchoring remains correct
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position + offset);
        transform.position = screenPoint;
    }

    public void SetHealthBarPercentage(float percentage)
    {
        float parentWidth = GetComponent<RectTransform>().rect.width;
        float width = parentWidth * percentage;
        foregroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
}
