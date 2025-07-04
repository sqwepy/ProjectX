using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;
    private float throwForceCharge;
    private float maxThrowForce = 1000f;
    private float chargeRate = 100f;
    private bool isBeingGrabbed = false;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }
    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
        objectRigidbody.isKinematic = true;
        objectRigidbody.linearVelocity = Vector3.zero;
        objectRigidbody.angularVelocity = Vector3.zero;
        isBeingGrabbed = true;
        throwForceCharge = 0f;
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
        objectRigidbody.isKinematic = false;
        isBeingGrabbed = false;
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
            float lerpSpeed = 100f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(newPosition);
            objectRigidbody.MoveRotation(objectGrabPointTransform.rotation);
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
