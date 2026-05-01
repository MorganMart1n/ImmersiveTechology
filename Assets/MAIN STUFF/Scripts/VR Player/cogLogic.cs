using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CrankPuzzleFinal : MonoBehaviour
{
    [Header("Crank Movement Settings")]
    public GameObject targetLight;
    public float moveStep = 0.1f;
    public float minX = -10f;
    public float maxX = 10f;

    private XRSimpleInteractable interactable;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor grabbingInteractor;

    private float previousAngle;
    private float accumulatedDelta;
    private bool isGrabbed;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnGrabbed);
            interactable.selectExited.AddListener(OnReleased);
        }
    }

    private void Update()
    {
        if (isGrabbed && grabbingInteractor != null)
        {
            float currentAngle = GetHandAngle(grabbingInteractor.transform.position);
            float delta = Mathf.DeltaAngle(previousAngle, currentAngle) * 0.6f;
            previousAngle = currentAngle;
            accumulatedDelta += delta;

            if (Mathf.Abs(accumulatedDelta) >= 12f)
            {
                ProcessStep(Mathf.Sign(accumulatedDelta));
                accumulatedDelta = 0;
            }
        }
    }

    private void ProcessStep(float direction)
    {
        if (targetLight != null)
        {
            Vector3 pos = targetLight.transform.localPosition;
            pos.x = Mathf.Clamp(pos.x + direction * moveStep, minX, maxX);
            targetLight.transform.localPosition = pos;

            Debug.Log($"Crank X: {pos.x:F3}");
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        grabbingInteractor = args.interactorObject;
        previousAngle = GetHandAngle(grabbingInteractor.transform.position);
        isGrabbed = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        grabbingInteractor = null;
        isGrabbed = false;
    }

    private float GetHandAngle(Vector3 pos)
    {
        Vector3 offset = pos - transform.position;
        Vector3 localDir = transform.InverseTransformDirection(offset);
        return Mathf.Atan2(localDir.z, localDir.x) * Mathf.Rad2Deg;
    }
}