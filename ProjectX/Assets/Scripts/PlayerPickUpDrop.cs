using UnityEngine;
using UnityEngine.UI;

public class PlayerPickUpDrop : MonoBehaviour
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
            objectGrabbable.Throw(throwDirection, finalThrowForce);
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

            if (raycastHit.transform.TryGetComponent(out objectGrabbable))
            {
                objectGrabbable.Grab(objectGrabPointTransform);
            }

        }
    }
}
