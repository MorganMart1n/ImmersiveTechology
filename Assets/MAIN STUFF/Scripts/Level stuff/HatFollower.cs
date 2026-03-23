using UnityEngine;
using System.Collections;

public class HatFlip : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Drag the Empty GameObject child of the Player here")]
    public Transform targetAnchor;

    [Header("Settings")]
    public float travelDuration = 0.6f;
    public int totalFlips = 2;
    public float pickupRange = 4.0f;
    public KeyCode pickupKey = KeyCode.E;

    private bool isFlying = false;
    private bool hasBeenPickedUp = false; // NEW
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Only allow pickup if it hasn't been picked up before
        if (!isFlying && !hasBeenPickedUp && Input.GetKeyDown(pickupKey))
        {
            if (Vector3.Distance(transform.position, targetAnchor.position) <= pickupRange)
            {
                StartCoroutine(PerformFlip());
            }
        }
    }

    IEnumerator PerformFlip()
    {
        isFlying = true;
        hasBeenPickedUp = true; // Lock it permanently

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;

        while (elapsed < travelDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / travelDuration;

            transform.position = Vector3.Lerp(startPos, targetAnchor.position, t);

            float angle = t * 360f * totalFlips;
            Quaternion flipRotation = Quaternion.AngleAxis(angle, Vector3.right);

            transform.rotation = Quaternion.Lerp(startRot, targetAnchor.rotation, t) * flipRotation;

            yield return null;
        }

        SnapToTarget();
    }

    void SnapToTarget()
    {
        transform.SetParent(targetAnchor);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}