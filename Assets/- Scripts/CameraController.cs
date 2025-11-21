using UnityEngine;

/// <summary>
/// A simple follow camera that orbits around the player with mouse drag. Attach this script to the camera.
/// </summary>
public class CameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 10f;
    public float rotationSpeed = 3f;
    private float currentYaw;

    void LateUpdate()
    {
        if (target == null) return;
        currentYaw += Input.GetAxis("Mouse X") * rotationSpeed;
        Quaternion rot = Quaternion.Euler(0, currentYaw, 0);
        Vector3 dir = rot * Vector3.forward;
        transform.position = target.position - dir * distance + Vector3.up * 5f;
        transform.LookAt(target.position + Vector3.up * 2f);
    }
}