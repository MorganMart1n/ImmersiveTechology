using UnityEngine;

public sealed class CompanionFollow : MonoBehaviour
{
    [Header("Tracking Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Offset Settings")]
    [SerializeField] private Vector3 followOffset = new Vector3(1.5f, 1.2f, 2.5f);
    [SerializeField] private Vector3 lookOffset = new Vector3(0, 1.6f, 0);

    [Header("Hover Animation")]
    [SerializeField] private float hoverAmplitude = 0.15f;
    [SerializeField] private float hoverFrequency = 2.0f;

    // The "Switch" to start following
    private bool isTriggered = false;

    private void Update()
    {
        // Only run the movement logic if the player has touched the trigger
        if (!isTriggered || playerTransform == null) return;

        HandleMovement();
        HandleLookAtPlayer();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("I was touched by: " + other.gameObject.name); // Add this!

            isTriggered = true;
    
    }

    private void HandleMovement()
    {
        Vector3 targetPosition = playerTransform.TransformPoint(followOffset);
        float hoverY = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        targetPosition.y += hoverY;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void HandleLookAtPlayer()
    {
        Vector3 targetPoint = playerTransform.position + lookOffset;
        Vector3 direction = targetPoint - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}