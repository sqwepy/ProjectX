using TMPro;
using UnityEngine;
using Mirror;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkIdentity))]
public class PlayerMovement : Mirror.NetworkBehaviour
{
    [Header("References")]
    public Transform playerCam;
    public Transform orientation;

    private Rigidbody rb;

    [Header("Mouse Look")]
    private float xRotation;
    public float sensitivity = 50f;
    public float sensMultiplier = 1f;

    [Header("Movement")]
    public float moveSpeed = 4500;
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
        // Nur die eigene Kamera aktivieren

        playerCam.gameObject.SetActive(true);

        // Setze Kamera-Fokus, z. B. das MoveCamera-Script an der Kamera selbst
        playerCam.GetComponent<MoveCamera>()?.SetTarget(transform);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        if (!isLocalPlayer && playerCam != null)
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
        if (!isLocalPlayer) return;

        HandleInput();
        Look();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

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
        if (!isLocalPlayer || !grounded || !readyToJump) return;
    
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
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        Vector3 rot = playerCam.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCam.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

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
        float lookAngle = orientation.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.linearVelocity.x, rb.linearVelocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float mag = rb.linearVelocity.magnitude;
        float yMag = mag * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = mag * Mathf.Cos(v * Mathf.Deg2Rad);

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
