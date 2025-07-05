using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;
    private float throwForceCharge;
    private float maxThrowForce = 1000f;
    private float chargeRate = 100f;
    private bool isBeingGrabbed = false;
    private Collider playerCollider;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        objectRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        objectRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }
    public void Grab(Transform objectGrabPointTransform, Collider playerCollider)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        this.playerCollider = playerCollider;

        objectRigidbody.useGravity = false;
        objectRigidbody.isKinematic = false;
        objectRigidbody.linearVelocity = Vector3.zero;
        objectRigidbody.angularVelocity = Vector3.zero;

        if (TryGetComponent<Collider>(out var objectCollider))
        {
            Physics.IgnoreCollision(objectCollider, playerCollider, true);
        }

        isBeingGrabbed = true;
        throwForceCharge = 0f;
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
        objectRigidbody.isKinematic = false;
        isBeingGrabbed = false;

        if (TryGetComponent<Collider>(out var objectCollider) && playerCollider != null)
        {
            Physics.IgnoreCollision(objectCollider, playerCollider, false);
        }

        playerCollider = null;
    }

    public void Throw(Vector3 throwDirection, float throwForce)
    {
        Drop();
        objectRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
    }


    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float followSpeed = 1200f;
            Vector3 toTarget = objectGrabPointTransform.position - transform.position;
            objectRigidbody.linearVelocity = toTarget * followSpeed * Time.fixedDeltaTime;
            Quaternion targetRotation = objectGrabPointTransform.rotation;
            objectRigidbody.MoveRotation(targetRotation);
        }
    }

    public void ChargeThrow()
    {
        if (isBeingGrabbed)
        {
            throwForceCharge += chargeRate * Time.deltaTime;
            throwForceCharge = Mathf.Min(throwForceCharge, maxThrowForce);
        }
    }

    public float ReleaseThrow()
    {
        float finalForce = throwForceCharge;
        throwForceCharge = 0f;
        return finalForce;
    }

    public float GetMaxChargeTime()
    {
        return maxThrowForce / chargeRate;
    }
}
