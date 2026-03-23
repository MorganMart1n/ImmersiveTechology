using UnityEngine;

public class IslandHover : MonoBehaviour
{
    public float hoverHeight = 0.3f;
    public float hoverSpeed = 0.5f;
    public float tiltAmount = 1f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        float hoverOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = startPosition + new Vector3(0, hoverOffset, 0);

        float tilt = Mathf.Sin(Time.time * hoverSpeed * 0.5f) * tiltAmount;
        transform.rotation = startRotation * Quaternion.Euler(tilt, 0, -tilt);
    }
}