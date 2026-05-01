using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRGrab : MonoBehaviour
{
    public float followSpeed = 30f;
    private Transform activeHand;
    private Rigidbody rb;
    private bool isPickedUp = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // This is the specific method the XR Interactor will call
    public void StartGrab(SelectEnterEventArgs args)
    {
        // args.interactorObject is the Hand that grabbed the object
        activeHand = args.interactorObject.transform;
        isPickedUp = true;

        rb.useGravity = false;
        rb.linearDamping = 10f;
        rb.angularDamping = 10f;
    }

    public void EndGrab(SelectExitEventArgs args)
    {
        isPickedUp = false;
        activeHand = null;

        rb.useGravity = true;
        rb.linearDamping = 0.05f;
        rb.angularDamping = 0.05f;
    }

    void FixedUpdate()
    {
        if (isPickedUp && activeHand != null)
        {
            // Move the object to the hand position
            rb.linearVelocity = (activeHand.position - transform.position) * followSpeed;

            // Match the hand rotation
            rb.MoveRotation(Quaternion.Lerp(transform.rotation, activeHand.rotation, Time.fixedDeltaTime * followSpeed));
        }
    }
}