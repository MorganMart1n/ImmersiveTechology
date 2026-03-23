using UnityEngine;

public sealed class CompanionFollow : MonoBehaviour
{
    [Header("Tracking Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Offset Settings")]
    [SerializeField] private Vector3 followOffset = new Vector3(1.5f, 1.2f, 2.5f); // Relative to player's front
    [SerializeField] private Vector3 lookOffset = new Vector3(0, 1.6f, 0); // Aim at eyes/head

    [Header("Hover Animation")]
    [SerializeField] private float hoverAmplitude = 0.15f;
    [SerializeField] private float hoverFrequency = 2.0f;

    private void Update()
    {
        if (playerTransform == null) return;

        HandleMovement();
        HandleLookAtPlayer();
    }

    private void HandleMovement()
    {
        // Position the companion based on where the player is facing
        Vector3 targetPosition = playerTransform.TransformPoint(followOffset);

        // Gentle floating bob
        float hoverY = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        targetPosition.y += hoverY;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void HandleLookAtPlayer()
    {
        // Calculate the direction from the companion to the player's head area
        Vector3 targetPoint = playerTransform.position + lookOffset;
        Vector3 direction = targetPoint - transform.position;

        if (direction != Vector3.zero)
        {
            // Create a rotation that faces the player
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate to face them
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}