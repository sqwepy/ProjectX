using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Transform target;

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0, 0, 0);
    public float followSpeed = 10f;
    public bool smoothFollow = true;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        if (smoothFollow)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, followSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = target.position + offset;
        }
    }
}
