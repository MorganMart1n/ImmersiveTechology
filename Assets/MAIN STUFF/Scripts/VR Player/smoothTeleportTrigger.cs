using UnityEngine;


public class smoothTeleportTrigger : MonoBehaviour
{
    [Header("References")]
    // We use 'IXRSelectInteractor' to be compatible with Near-Far interactors
    public GameObject leftHandObject;
    public GameObject rightHandObject;
    public Transform headset;
    public CharacterController body;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor leftInteractor;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor rightInteractor;

    [Header("Settings")]
    public float headHeightThreshold = 0.4f;
    public float forwardNudge = 0.7f;
    public float upwardNudge = 0.2f;

    void Awake()
    {
        leftInteractor = leftHandObject.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor>();
        rightInteractor = rightHandObject.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor>();
    }

    void Update()
    {
        if (leftInteractor == null || rightInteractor == null) return;

        // Check if hands are holding onto a climbable
        bool isGrabbing = leftInteractor.hasSelection || rightInteractor.hasSelection;

        if (isGrabbing)
        {
            float highestHandY = Mathf.Max(leftHandObject.transform.position.y, rightHandObject.transform.position.y);
            float gap = headset.position.y - highestHandY;

            // If head clears the ledge, teleport forward
            if (gap > headHeightThreshold)
            {
                ExecuteMantle();
            }
        }
    }

    void ExecuteMantle()
    {
        Vector3 moveVector = (body.transform.forward * forwardNudge) + (Vector3.up * upwardNudge);

        body.enabled = false;
        body.transform.position += moveVector;
        body.enabled = true;

        Debug.Log("Mantle Triggered!");
    }
}