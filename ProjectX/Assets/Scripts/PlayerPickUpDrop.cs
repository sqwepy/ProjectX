using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerPickUpDrop : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickupLayerMask;
    [SerializeField] private Image chargeBarUI;
    [SerializeField] private Image chargeBarBackgroundUI;

    private ObjectGrabbable objectGrabbable;
    private bool preparingThrow = false;
    private float holdtime = 0f;



    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;  // Exit if this is not the local player
        }
        if (Input.GetKeyDown(KeyCode.E))    // Check if the player presses the 'E' key
        {
            if (objectGrabbable == null)
            {
                TryPickUp();    // Not holding an object, try to pick one up
            }
            else
            {
                preparingThrow = true;
                holdtime = 0f;
                chargeBarUI.gameObject.SetActive(true);
                chargeBarBackgroundUI.gameObject.SetActive(true);
            }
        }

        if (Input.GetKey(KeyCode.E) && preparingThrow && objectGrabbable != null)  // Check if the player is holding an object and preparing a throw
        {
            if (holdtime < 0.5f)        // Drop the object if the hold time is less than 0.5 seconds
            {
                holdtime += Time.deltaTime;
            }

            else if (holdtime >= 0.5f)  // Charge the throw if the hold time is greater than or equal to 0.5 seconds
            {
                objectGrabbable.ChargeThrow();
                float chargePercent = 4 * Mathf.Clamp01(holdtime / objectGrabbable.GetMaxChargeTime());
                chargeBarUI.fillAmount = chargePercent;
                Debug.Log(holdtime + " " + objectGrabbable.GetMaxChargeTime() + " " + chargePercent);
            }

        }

        if (Input.GetKeyUp(KeyCode.E) && objectGrabbable != null && preparingThrow)    // Check if the player releases the 'E' key while holding an object and preparing a throw
        {
            float finalThrowForce = objectGrabbable.ReleaseThrow();
            Vector3 throwDirection = playerCameraTransform.forward;
            CmdThrow(objectGrabbable.gameObject, throwDirection, finalThrowForce);
            objectGrabbable = null;
            preparingThrow = false;
            chargeBarUI.fillAmount = 0f;
            chargeBarUI.gameObject.SetActive(false);
            chargeBarBackgroundUI.gameObject.SetActive(false);
        }

    }

    private void TryPickUp()    // Method to pick up an object
    {
        float pickUpDistance = 3f;
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickupLayerMask))
        {

            if (raycastHit.transform.TryGetComponent(out ObjectGrabbable grabbable))
            {
                CmdGrab(raycastHit.transform.gameObject);
                objectGrabbable = grabbable;
            }

        }
    }

    [Command]
    private void CmdThrow(GameObject obj, Vector3 direction, float force)
    {
        if (obj.TryGetComponent(out ObjectGrabbable grabbable))
        {
            var playerCollider = GetComponent<Collider>();
            grabbable.Throw(direction, force);
        }
    }

    [Command]
    private void CmdGrab(GameObject obj)
    { 
        if (obj.TryGetComponent(out ObjectGrabbable grabbable))
        {
            var playerCollider = GetComponent<Collider>();
            grabbable.Grab(objectGrabPointTransform, playerCollider);
        }
    }
}
