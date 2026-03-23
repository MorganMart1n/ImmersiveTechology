using UnityEngine;
using TMPro;

public sealed class SmoothPositionSwitcher : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Settings")]
    [SerializeField] private float transitionSpeed = 2.0f;
    [SerializeField] private float interactionRange = 2.0f;
    [SerializeField] private Transform playerTransform;

    [Header("Audio")]
    [SerializeField] private AudioSource doorAudioSource;

    // Rotation Settings (Your specific angles)
    private Vector3 closedEuler = new Vector3(0, 228.6026f, 0);
    private Vector3 openEuler = new Vector3(0, 68.25015f, 0);

    private bool movingToTarget = false;
    private float transitionProgress = 0f;

    void Start()
    {
        if (playerTransform == null && Camera.main != null)
            playerTransform = Camera.main.transform;

        // Force the door to the closed rotation at the start
        transform.rotation = Quaternion.Euler(closedEuler);
        UpdateUI();
    }

    void Update()
    {
        float distance = (playerTransform != null) ? Vector3.Distance(playerTransform.position, transform.position) : 999f;

        // Check for "E" and Range
        if (distance <= interactionRange && Input.GetKeyDown(KeyCode.E))
        {
            movingToTarget = !movingToTarget;

            if (doorAudioSource != null)
                doorAudioSource.Play();
        }

        HandleMovement();
        UpdateUI();
    }

    private void HandleMovement()
    {
        // Progress Logic
        if (movingToTarget)
            transitionProgress += Time.deltaTime * transitionSpeed;
        else
            transitionProgress -= Time.deltaTime * transitionSpeed;

        transitionProgress = Mathf.Clamp01(transitionProgress);

        // Calculate Rotations
        Quaternion startRot = Quaternion.Euler(closedEuler);
        Quaternion endRot = Quaternion.Euler(openEuler);

        // Apply Slerp for smooth rotation
        transform.rotation = Quaternion.Slerp(startRot, endRot, transitionProgress);
    }

    private void UpdateUI()
    {
        if (statusText != null)
        {
            string mode = movingToTarget ? "Open" : "Closed";
            statusText.text = $"<b>Door Status:</b> {mode}\n<b>Press E to Interact</b>";
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}