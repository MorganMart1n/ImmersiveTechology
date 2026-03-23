using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GunRayCast : MonoBehaviour
{
    [Header("References")]
    public Transform gunTip;
    public LayerMask layerMask;
    [SerializeField] private RaycastInteract grappleLogic;

    [Header("Settings")]
    public float range = 9999999f; // UPDATED: Increased to 180 to ensure 160m reach always works
    public float laserWidth = 0.05f; // Made thinner for a more realistic cable look

    [Header("Visuals")]
    [SerializeField] private Color aimingColor = new Color(1, 1, 0, 0.2f); // Faint yellow for aiming
    [SerializeField] private Color grapplingColor = Color.white; // White/Steel for the actual cable
    [SerializeField] private bool useEmission = true;

    private LineRenderer line;

    [HideInInspector] public RaycastHit currentHit;
    [HideInInspector] public bool isHitting;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.positionCount = 2;
        line.startWidth = laserWidth;
        line.endWidth = laserWidth;

        if (grappleLogic == null)
            grappleLogic = GetComponentInParent<RaycastInteract>();
    }

    void LateUpdate()
    {

        if (gunTip == null) return;

        Vector3 origin = gunTip.position;
        Debug.DrawRay(origin, gunTip.forward * range, Color.red);
        // If we are currently grappling, the line connects Gun to Anchor
        if (grappleLogic != null && grappleLogic.IsGrappling)
        {
            UpdateLine(origin, grappleLogic.AnchorPoint, grapplingColor);
            isHitting = true; // Stay true so we don't drop the grapple
        }
        else
        {
            // If NOT grappling, we cast a ray forward to see if we CAN hit something
            Vector3 direction = gunTip.forward;
            isHitting = Physics.Raycast(origin, direction, out currentHit, range, layerMask);


            if (isHitting)
            {
                Debug.Log("Hit: " + currentHit.collider.name + " Distance: " + currentHit.distance);
                // Show a faint "aiming" line so the player knows they can hook here
                UpdateLine(origin, currentHit.point, aimingColor);
            }
            else
            {
                // Hide the line if we aren't aiming at anything or grappling
                line.enabled = false;
            }
        }
    }

    private void UpdateLine(Vector3 start, Vector3 end, Color targetColor)
    {
        line.enabled = true;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        line.startColor = targetColor;
        line.endColor = targetColor;

        if (line.material != null)
        {
            // Standard URP/HDRP property is usually _BaseColor or _Color
            if (line.material.HasProperty("_BaseColor"))
                line.material.SetColor("_BaseColor", targetColor);
            else if (line.material.HasProperty("_Color"))
                line.material.SetColor("_Color", targetColor);

            if (useEmission)
            {
                line.material.EnableKeyword("_EMISSION");
                line.material.SetColor("_EmissionColor", targetColor * 2f);
            }
        }
    }
}