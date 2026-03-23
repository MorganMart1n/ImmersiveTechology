using UnityEngine;

public class CrankIt : MonoBehaviour
{
    [Header("Dependencies")]
    public FrogEater frogEater;
    public GameObject targetLight;

    [Header("Alignment Reference")]
    [Tooltip("For the Body, leave this empty. For Hair/Hat, drag the Body's Light here.")]
    public Transform masterTarget;

    [Header("Movement Constraints")]
    public float moveStep = 0.3f;
    public float minX = -10f;
    public float maxX = 10f;

    [Header("Grace Period (Tolerance)")]
    [Tooltip("How many units of 'Error' are allowed?")]
    public float tolerance = 0.6f;

    [Header("Materials")]
    [SerializeField] private Material inRangeMaterial;
    [SerializeField] private Material outOfRangeMaterial;

    private Renderer cubeRenderer;

    private void Awake()
    {
        cubeRenderer = GetComponent<Renderer>();
        // Initialize material on start
        UpdateMaterial();
    }

    public void UpdateLightPosition(float scrollDelta)
    {
        // Basic safety checks
        if (frogEater == null || !frogEater.HasWon || targetLight == null) return;

        if (Mathf.Abs(scrollDelta) > 0.001f)
        {
            float direction = Mathf.Sign(scrollDelta);
            Vector3 pos = targetLight.transform.localPosition;

            // Move the light and rotate the crank handle
            pos.x = Mathf.Clamp(pos.x + (direction * moveStep), minX, maxX);
            targetLight.transform.localPosition = pos;
            transform.Rotate(Vector3.up, direction * 20f);

            // Logic for the Hair/Hat alignment feedback
            if (masterTarget != null)
            {
                float dist = targetLight.transform.localPosition.x - masterTarget.localPosition.x;
                string dir = dist > 0 ? "LEFT" : "RIGHT";

                if (IsActuallyInRange())
                    Debug.Log($"<color=green><b>[{gameObject.name}]</b> ALIGNED</color>");
                else
                    Debug.Log($"<b>[{gameObject.name}]</b> Turn {dir} ({Mathf.Abs(dist):F2} units away)");
            }

            // Always update the visual state after moving
            UpdateMaterial();
        }
    }

    public void UpdateMaterial()
    {
        if (cubeRenderer == null) return;

        // Apply material based on range check
        cubeRenderer.material = IsActuallyInRange() ? inRangeMaterial : outOfRangeMaterial;
    }

    public bool IsActuallyInRange()
    {
        // RULE: The Body (Master) is always "In Range" because it's the leader.
        if (masterTarget == null)
        {
            return true;
        }

        // RULE: Hair/Hat check distance against the Body's light position.
        float myX = targetLight.transform.localPosition.x;
        float masterX = masterTarget.localPosition.x;

        return Mathf.Abs(myX - masterX) <= tolerance;
    }
}