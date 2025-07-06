using TMPro;
using UnityEngine;
using Mirror;
using System;
using Steamworks;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(NetworkIdentity))]
public class PlayerMovement : Mirror.NetworkBehaviour
{
    [Header("References")]
    public Transform playerCam;
    public Transform orientation;
    private MoveCamera camScript;
    public Transform head;
    public static bool inputBlocked = false;

    private Rigidbody rb;
    private NetworkIdentity rootIdentity;

    [Header("Mouse Look")]
    public float sens = 10000;
    private float xRotation;
    private float yaw; // Add this at the top of your script (just like xRotation)

    [Header("Movement")]
    public float moveSpeed = 80;
    public float maxSpeed = 20;
    public float counterMovement = 0.175f;
    public LayerMask whatIsGround;
    private float threshold = 0.01f;
    private Vector3 normalVector = Vector3.up;
    public float maxSlopeAngle = 35f;
    public bool grounded;

    [Header("Crouch & Slide")]
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;

    [Header("Jumping")]
    public float jumpForce = 0.01f;
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;

    // Inputs
    private float x, y;
    private bool jumping, crouching;

    private bool cancellingGrounded;
    private float desiredX;


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        rootIdentity = GetComponentInParent<NetworkIdentity>();
        Debug.Log($"OnStartLocalPlayer called on {gameObject.name}");
        Debug.Log($"playerCam assigned? {(playerCam != null)}");

        if (playerCam == null)
            playerCam = Camera.main.transform;

        if (rootIdentity == null)
            rootIdentity = GetComponentInParent<NetworkIdentity>();

        if (rootIdentity == null || !rootIdentity.isLocalPlayer)
        {
            var tag = GetComponentInChildren<NameTag>();
            if (SteamManager.Initialized)
                tag.SetName(SteamFriends.GetPersonaName());
            else
                tag.SetName("Player");

            tag.orientation = orientation;
        }
        else
        {
            GetComponentInChildren<Canvas>()?.gameObject.SetActive(false);
        }

        playerCam.gameObject.SetActive(true);

        // Cache the MoveCamera component in playerCam or its children
        camScript = playerCam.GetComponentInChildren<MoveCamera>();

        if (camScript != null)
        {
            camScript.SetTarget(head);
            Debug.LogWarning("MoveCamera component found and assigned");
        }
        else
        {
            Debug.LogWarning("MoveCamera component missing on playerCam or its children");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        FindObjectOfType<PauseMenuHUD>()?.SetLocalPlayer(this);
    }



    void Start()
    {
        if ((rootIdentity == null || !rootIdentity.isLocalPlayer) && playerCam != null)
        {
            playerCam.gameObject.SetActive(false);
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerScale = transform.localScale;

        rb.isKinematic = false; // Enable physics
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (rootIdentity == null || !rootIdentity.isLocalPlayer || inputBlocked) return;

        HandleInput();
        Look();
    }

    private void FixedUpdate()
    {
        if (rootIdentity == null || !rootIdentity.isLocalPlayer || inputBlocked) return;
        Move();
    }

    private void HandleInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);

        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    private void StartCrouch()
    {
        transform.localScale = crouchScale;
        transform.position -= new Vector3(0, 0.5f, 0);

        if (rb.linearVelocity.magnitude > 0.5f && grounded)
        {
            rb.AddForce(orientation.forward * slideForce);
        }
    }

    private void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position += new Vector3(0, 0.5f, 0);
    }

    private void Move()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        CounterMovement(x, y, mag);

        if (readyToJump && jumping) Jump();

        if (crouching && grounded && readyToJump)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        float multiplier = grounded ? 1f : 0.5f;
        float multiplierV = (grounded && !crouching) ? 1f : 0.5f;

        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        rb.AddForce(orientation.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.right * x * moveSpeed * Time.deltaTime * multiplier);
    }

    private void Jump()
    {
        if (rootIdentity == null || !rootIdentity.isLocalPlayer || !grounded || !readyToJump || inputBlocked) return;
    
        readyToJump = false;
    
        // Use ForceMode.Impulse to simulate a jump
        rb.AddForce(Vector3.up * jumpForce * 1.5f, ForceMode.Impulse);
        rb.AddForce(normalVector * jumpForce * 0.5f, ForceMode.Impulse);
    
        // Optional: stabilize vertical velocity
        Vector3 vel = rb.linearVelocity;
        if (vel.y < 0.5f)
            rb.linearVelocity = new Vector3(vel.x, 0, vel.z);
        else if (vel.y > 0)
            rb.linearVelocity = new Vector3(vel.x, vel.y / 2, vel.z);
    
        Invoke(nameof(ResetJump), jumpCooldown);
    }


    private void ResetJump() => readyToJump = true;

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        //Debug.Log(sens);
        // Pass mouse input to camera script
        camScript?.RotateCamera(mouseX, mouseY, sens);

        // Rotate player body to match camera yaw (horizontal rotation only)
        if (camScript != null)
        {
            // Align player rotation's Y to camera's yaw
            Vector3 camEuler = camScript.transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, camEuler.y, 0f);

            // Keep orientation in sync with player rotation
            orientation.rotation = transform.rotation;
        }
}




    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping || inputBlocked) return;

        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.linearVelocity.normalized * slideCounterMovement);
            return;
        }

        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f ||
            (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.right * Time.deltaTime * -mag.x * counterMovement);
        }

        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f ||
            (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        Vector2 flatVel = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            float yVel = rb.linearVelocity.y;
            Vector3 limitedVel = rb.linearVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, yVel, limitedVel.z);
        }
    }

    private Vector2 FindVelRelativeToLook()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 forward = orientation.forward;
        Vector3 right = orientation.right;

        float xMag = Vector3.Dot(flatVel, right);
        float yMag = Vector3.Dot(flatVel, forward);

        return new Vector2(xMag, yMag);
    }


    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private void OnCollisionStay(Collision other)
    {
        if (!isLocalPlayer) return;
        if ((whatIsGround.value & (1 << other.gameObject.layer)) == 0) return;

        foreach (ContactPoint contact in other.contacts)
        {
            if (IsFloor(contact.normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = contact.normal;
                CancelInvoke(nameof(StopGrounded));
                break;
            }
        }

        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * 3f);
        }
    }

    private void StopGrounded() => grounded = false;
}
