using System.Collections;
using UnityEngine;

public class FPSGrab : MonoBehaviour
{
    public float waitOnPickup = 0.1f;
    public float breakForce = 35f;

    [HideInInspector] public bool pickedUp = false;
    [HideInInspector] public PlayerInteractions playerInteractions;

    public IEnumerator PickUp()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            // Use 'drag' for older Unity versions, 'linearDamping' for Unity 6
            rb.linearDamping = 10f;
            rb.angularDamping = 10f;
        }

        yield return new WaitForSecondsRealtime(waitOnPickup);
        pickedUp = true;
    }

    public void OnDrop()
    {
        pickedUp = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.linearDamping = 0.05f; // Default drag
            rb.angularDamping = 0.05f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (pickedUp && collision.relativeVelocity.magnitude > breakForce)
        {
            if (playerInteractions != null)
            {
                playerInteractions.BreakConnection();
            }
        }
    }
}