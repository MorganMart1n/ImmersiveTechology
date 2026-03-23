using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastInteract : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GunRayCast GunRayCast;
    private LineRenderer lr;

    [Header("Grapple Settings")]
    [SerializeField] private float reelSpeed = 35f;
    [SerializeField] private float minLength = 5.0f;
    [SerializeField] private float maxReach = 9999999f;

    public bool IsGrappling { get; private set; }
    public Vector3 AnchorPoint { get; private set; }
    public float RopeLength { get; private set; }

    [Header("Grapple Control")]
    [SerializeField] private float grappleCooldown = 0.2f;
    private float lastGrappleTime = -10f;

    private FPS_Movement movement;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        movement = GetComponent<FPS_Movement>();

        if (GunRayCast == null)
            GunRayCast = GetComponentInChildren<GunRayCast>();
    }

    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
            StartGrapple();

        if (Mouse.current.rightButton.wasReleasedThisFrame)
            StopGrapple();

        if (IsGrappling && Mouse.current.rightButton.isPressed)
        {
            RopeLength = Mathf.MoveTowards(RopeLength, minLength, reelSpeed * Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        if (Time.time - lastGrappleTime < grappleCooldown) return;
        if (GunRayCast == null || !GunRayCast.isHitting) return;

        float distanceToHit = Vector3.Distance(transform.position, GunRayCast.currentHit.point);

        if (distanceToHit <= maxReach)
        {
            AnchorPoint = GunRayCast.currentHit.point;
            RopeLength = distanceToHit;
            IsGrappling = true;
            lastGrappleTime = Time.time;

            if (lr) lr.positionCount = 2;
        }
    }

    public void StopGrapple()
    {
        IsGrappling = false;

        if (movement != null)
        {
            float boostStrength = Mathf.Min(6f, movement.CurrentSpeed() * 0.15f);
            Vector3 boost = transform.forward * boostStrength;
            movement.AddVelocity(boost);
        }

        if (lr) lr.positionCount = 0;
    }

    void DrawRope()
    {
        if (!IsGrappling || lr == null) return;

        lr.SetPosition(0, GunRayCast.transform.position);
        lr.SetPosition(1, AnchorPoint);
    }
}