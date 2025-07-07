using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Transform target;

    [Range(0f, 1f)]
    public float rotationSmoothFactor = 1f;

    private float pitch = 0f;
    private float yaw = 0f;
    private Quaternion currentRotation;


    public void SetTarget(Transform t)
    {
        target = t;
        currentRotation = transform.rotation;
    }

    public void RotateCamera(float mouseX, float mouseY, float sens)
    {
        yaw += mouseX * sens * Time.deltaTime;    // NO Time.deltaTime here
        pitch -= mouseY * sens * Time.deltaTime;  // NO Time.deltaTime here
        pitch = Mathf.Clamp(pitch, -89f, 89f);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
        currentRotation = Quaternion.Lerp(currentRotation, targetRotation, 1 - Mathf.Pow(1 - rotationSmoothFactor, Time.deltaTime * 60));

        transform.position = target.position;
        transform.rotation = currentRotation;
    }
}
