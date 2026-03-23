using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPS_Movement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    private RaycastInteract grapple;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 16f;
    [SerializeField] private float airControl = 10f;
    [SerializeField] private float groundFriction = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Gravity")]
    [SerializeField] private float gravity = -45f;

    [Header("Speed Limits")]
    [SerializeField] private float maxSpeed = 50f;

    [Header("Grapple Physics")]
    [SerializeField] private float grappleAcceleration = 2f;

    private Vector3 velocity;
    private Vector2 moveInput;
    public bool grounded;

    private float previousDistanceToAnchor;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        grapple = GetComponent<RaycastInteract>();
    }

    void Update()
    {
        GroundCheck();
        ApplyMovementInput();
        ApplyGravity();
        ApplyGrapplePull();

        HardClampVelocity();

        controller.Move(velocity * Time.deltaTime);

        if (grapple != null && grapple.IsGrappling)
        {
            ApplyRopeConstraint();
        }
    }

    void HardClampVelocity()
    {
        Vector3 horizontalVel = new Vector3(velocity.x, 0f, velocity.z);
        float horizontalSpeed = horizontalVel.magnitude;

        if (horizontalSpeed > maxSpeed)
        {
            horizontalVel = horizontalVel.normalized * maxSpeed;
            velocity.x = horizontalVel.x;
            velocity.z = horizontalVel.z;
        }

        velocity.y = Mathf.Clamp(velocity.y, -Mathf.Abs(gravity), maxSpeed);
    }

    void GroundCheck()
    {
        grounded = controller.isGrounded;

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpsRemaining = maxJumps;
        }
    }

    void ApplyMovementInput()
    {
        Vector3 wishDir = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;

        if (grounded)
        {
            Vector3 target = wishDir * moveSpeed;

            velocity.x = Mathf.Lerp(velocity.x, target.x, groundFriction * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, target.z, groundFriction * Time.deltaTime);
        }
        else
        {
            velocity += wishDir * airControl * Time.deltaTime;
        }
    }

    void ApplyGrapplePull()
    {
        if (grapple == null || !grapple.IsGrappling)
            return;

        Vector3 dirToAnchor = grapple.AnchorPoint - transform.position;
        float distance = dirToAnchor.magnitude;
        dirToAnchor.Normalize();

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            previousDistanceToAnchor = distance;
        }

        float grappleTargetSpeed = Mathf.Min(maxSpeed * 0.75f, distance * 2f);

        Vector3 targetVelocity = dirToAnchor * grappleTargetSpeed;

        velocity = Vector3.Lerp(velocity, targetVelocity, Time.deltaTime * grappleAcceleration);

        float newDistance = Vector3.Distance(transform.position, grapple.AnchorPoint);

        if (newDistance > previousDistanceToAnchor)
        {
            grapple.StopGrapple();
        }

        previousDistanceToAnchor = newDistance;
    }

    void ApplyRopeConstraint()
    {
        Vector3 toPlayer = transform.position - grapple.AnchorPoint;
        float distance = toPlayer.magnitude;

        if (distance > grapple.RopeLength)
        {
            Vector3 dirToPlayer = toPlayer.normalized;

            controller.enabled = false;
            transform.position = grapple.AnchorPoint + (dirToPlayer * grapple.RopeLength);
            controller.enabled = true;

            float outwardVel = Vector3.Dot(velocity, dirToPlayer);

            if (outwardVel > 0)
            {
                velocity -= dirToPlayer * outwardVel;

                Vector3 tangentDir = Vector3.ProjectOnPlane(velocity, dirToPlayer).normalized;
                velocity += tangentDir * (outwardVel * 0.15f);
            }
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    public void ReceiveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void AddVelocity(Vector3 v)
    {
        velocity += v;
    }

    public float CurrentSpeed()
    {
        return new Vector3(velocity.x, 0f, velocity.z).magnitude;
    }

    public void OnJumpPressed()
    {
        if (grapple != null && grapple.IsGrappling) return;

        if (grounded || jumpsRemaining > 0)
        {
            velocity.y = jumpForce;
            jumpsRemaining--;
        }
    }
}