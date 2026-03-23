using UnityEngine;

[SelectionBase]
public class BouncingFrog : MonoBehaviour
{
    [Header("Area Settings")]
    [Tooltip("If empty, the frog uses its starting position as the center.")]
    public Transform centerPoint;
    public float sphereRadius = 5f;
    public Color gizmoColor = Color.cyan;

    [Header("Hop Settings")]
    public float hopForce = 5f;
    public float gravity = 9.8f;

    private float _verticalVelocity;
    private Vector3 _center;

    void Start()
    {
        _center = (centerPoint != null) ? centerPoint.position : transform.position;

        // Initial hop
        _verticalVelocity = hopForce;
    }

    void Update()
    {
        // Apply gravity
        _verticalVelocity -= gravity * Time.deltaTime;

        // Move frog vertically
        transform.position += Vector3.up * _verticalVelocity * Time.deltaTime;

        // Ground check (this is where the hop happens)
        if (transform.position.y <= _center.y)
        {
            transform.position = new Vector3(
                transform.position.x,
                _center.y,
                transform.position.z
            );

            // Hop upward again
            _verticalVelocity = hopForce;
        }

        // Keep frog inside semicircle horizontally
        Vector3 flatOffset = new Vector3(
            transform.position.x - _center.x,
            0,
            transform.position.z - _center.z
        );

        if (flatOffset.magnitude > sphereRadius)
        {
            Vector3 clamped = flatOffset.normalized * sphereRadius;

            transform.position = new Vector3(
                _center.x + clamped.x,
                transform.position.y,
                _center.z + clamped.z
            );
        }
    }

    // --- VISUALIZATION LOGIC ---

    void OnDrawGizmos()
    {
        Vector3 drawPos = (centerPoint != null)
            ? centerPoint.position
            : transform.position;

        Gizmos.color = gizmoColor;

        int segments = 64;
        float step = (Mathf.PI * 2) / segments;

        for (int i = 0; i < segments; i++)
        {
            Vector3 start = drawPos + new Vector3(
                Mathf.Cos(step * i) * sphereRadius,
                0,
                Mathf.Sin(step * i) * sphereRadius
            );

            Vector3 end = drawPos + new Vector3(
                Mathf.Cos(step * (i + 1)) * sphereRadius,
                0,
                Mathf.Sin(step * (i + 1)) * sphereRadius
            );

            Gizmos.DrawLine(start, end);
        }
    }
}