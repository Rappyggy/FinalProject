using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    // Assignables
    public Transform playerCam;
    public Transform orientation;

    // Other
    private Rigidbody rb;

    // Rotation and look
    private float xRotation;
    private float yRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    // Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    // Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    // Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;
    public float wallJumpForce = 550f;

    // Input
    float x, y;
    bool jumping, sprinting, crouching;

    // Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    // Wall Running
    public LayerMask wallRunLayer;
    public float wallRunForce = 20f;
    public float wallRunDuration = 1.5f;
    public float wallCheckDistance = 0.6f;
    private float wallRunTimer = 0f;
    private bool isWallRunning = false;
    private bool wallLeft, wallRight;

    // Camera tilt
    public float camTilt = 15f;
    private float currentTilt = 0f;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate() {
        Movement();
    }

    private void Update() {
        MyInput();
        Look();
        CheckWall();
        HandleWallRun();
        HandleFootsteps();
    }

    private void MyInput() {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.C);

        if (Input.GetKeyDown(KeyCode.C)) StartCrouch();
        if (Input.GetKeyUp(KeyCode.C)) StopCrouch();
    }

    private void StartCrouch() {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.linearVelocity.magnitude > 0.5f && grounded) {
            rb.AddForce(orientation.transform.forward * slideForce);
        }
    }

    private void StopCrouch() {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement() {
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        CounterMovement(x, y, mag);

        if (readyToJump && jumping && grounded) Jump();
        else if (readyToJump && jumping && isWallRunning) JumpFromWall();

        float maxSpeed = this.maxSpeed;

        if (crouching && grounded && readyToJump) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
        }

        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        float multiplier = 1f, multiplierV = 1f;

        if (!grounded) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        if (grounded && crouching) multiplierV = 1f;

        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }

    private void Jump() {
        if (readyToJump) {
            readyToJump = false;

            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            Vector3 vel = rb.linearVelocity;
            if (rb.linearVelocity.y < 0.5f)
                rb.linearVelocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.linearVelocity.y > 0)
                rb.linearVelocity = new Vector3(vel.x, vel.y / 2, vel.z);

            GameAudioManager.Instance?.PlayJump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void JumpFromWall() {
        readyToJump = false;

        Vector3 jumpDir = Vector3.up * 1.5f + wallNormalVector * 0.5f;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(jumpDir.normalized * wallJumpForce, ForceMode.Impulse);

        GameAudioManager.Instance?.PlayJump();

        StopWallRun();
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump() {
        readyToJump = true;
    }

    private float desiredX;
    private void Look() {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        desiredX += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        float targetTilt = isWallRunning ? (wallLeft ? -camTilt : camTilt) : 0f;
        currentTilt = Mathf.LerpAngle(currentTilt, targetTilt, Time.deltaTime * 10f);

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, currentTilt);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void HandleFootsteps() {
        if (grounded && rb.linearVelocity.magnitude > 0.1f) {
            GameAudioManager.Instance?.PlayFootsteps();
        } else {
            GameAudioManager.Instance?.StopFootsteps();
        }
    }

    private void CounterMovement(float x, float y, Vector2 mag) {
        if (!grounded || jumping) return;

        if (crouching) {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.linearVelocity.normalized * slideCounterMovement);
            return;
        }

        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        if (Mathf.Sqrt((Mathf.Pow(rb.linearVelocity.x, 2) + Mathf.Pow(rb.linearVelocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.linearVelocity.y;
            Vector3 n = rb.linearVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.linearVelocity.x, rb.linearVelocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.linearVelocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;
    private void OnCollisionStay(Collision other) {
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
    }

    private void CheckWall() {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, wallCheckDistance, wallRunLayer);
        wallRight = Physics.Raycast(transform.position, orientation.right, wallCheckDistance, wallRunLayer);
    }

    private void HandleWallRun() {
        if (grounded || rb.linearVelocity.y > 0) {
            StopWallRun();
            return;
        }

        if ((wallLeft || wallRight) && Input.GetKey(KeyCode.W)) {
            if (!isWallRunning) StartWallRun();

            wallNormalVector = wallLeft ? -orientation.right : orientation.right;
            Vector3 wallForward = Vector3.Cross(wallNormalVector, Vector3.up);

            if (Vector3.Dot(wallForward, orientation.forward) < 0)
                wallForward = -wallForward;

            rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
            rb.useGravity = false;

            wallRunTimer += Time.deltaTime;
            if (wallRunTimer > wallRunDuration) StopWallRun();
        } else {
            StopWallRun();
        }
    }

    private void StartWallRun() {
        isWallRunning = true;
        wallRunTimer = 0f;
        rb.useGravity = false;
    }

    private void StopWallRun() {
        if (!isWallRunning) return;
        isWallRunning = false;
        rb.useGravity = true;
    }
}
