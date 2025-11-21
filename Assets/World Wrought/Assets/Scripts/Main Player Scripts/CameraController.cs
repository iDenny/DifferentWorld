using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public Transform cameraTarget;

    private void Update()
    {
        // Get the mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotate the camera target based on mouse movement
        cameraTarget.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime, Space.World);
        cameraTarget.Rotate(Vector3.right, -mouseY * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
