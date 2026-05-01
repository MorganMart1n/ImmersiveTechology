using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class teleportPlayer : MonoBehaviour
{
    [Header("References")]
    // Drag your Left and Right "Near-Far Interactor" objects here
    public XRBaseInteractor leftHandInteractor;
    public XRBaseInteractor rightHandInteractor;
    public Transform headset;
    public CharacterController body;

    [Header("Settings")]
    public float headHeightThreshold = 0.4f;
    public float forwardNudge = 0.7f;
    public float upwardNudge = 0.2f;

    void Update()
    {
        if (leftHandInteractor == null || rightHandInteractor == null) return;

        // Check if either hand is currently holding the wall
        bool isGrabbing = leftHandInteractor.hasSelection || rightHandInteractor.hasSelection;

        if (isGrabbing)
        {
            // Find the highest hand position
            float highestHandY = Mathf.Max(leftHandInteractor.transform.position.y, rightHandInteractor.transform.position.y);

            // Calculate how high the head is above that hand
            float gap = headset.position.y - highestHandY;

            // If head is high enough, trigger the mantle
            if (gap > headHeightThreshold)
            {
                ExecuteMantle();
            }
        }
    }

    void ExecuteMantle()
    {
        // Calculate landing spot based on where the player is looking
        Vector3 moveVector = (body.transform.forward * forwardNudge) + (Vector3.up * upwardNudge);

        body.enabled = false;
        body.transform.position += moveVector;
        body.enabled = true;

        Debug.Log("Mantle Successful - You are on the ledge!");
    }
}