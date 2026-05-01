using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class CrankPuzzleFinal : MonoBehaviour
{
    public enum CrankPart { Hair, Hat }

    [Header("Puzzle Assignment")]
    public CrankPart partType;
    public GameObject targetLight;

    [Header("Specific Lock Points (Horizontal)")]
    public float targetX;
    public float tolerance = 0.05f;

    [Header("Lock Padding")]
    public float lockPadding = 0.15f;

    [Header("Movement")]
    public float moveStep = 0.1f;
    public float minX = -10f;
    public float maxX = 10f;

    [Header("Crank Rotation")]
    public Transform crankVisual;
    public float rotationSpeed = 1.5f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("Global Merge")]
    public static int AlignedCount = 0;
    public GameObject bodyLight;
    public GameObject hairLight;
    public GameObject hatLight;

    [Header("Final Positions")]
    public Vector3 hairFinalPos;
    public Vector3 hatFinalPos;
    public float bodyTargetY = -9.56f;
    public float mergeSpeed = 5f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip mergeSound;

    [Header("Player Teleport")]
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 teleportPosition = new Vector3(9.77999973f, 133.070007f, 205.360001f);

    private XRSimpleInteractable interactable;
    private XRGrabInteractable grabInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor grabbingInteractor;

    private float previousAngle;
    private float accumulatedDelta;

    private bool isGrabbed;
    private bool isLocked;
    private static bool isMerging;

    private Rigidbody rb;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnGrabbed);
            interactable.selectExited.AddListener(OnReleased);
        }

        AlignedCount = 0;
        isMerging = false;
    }

    private void Update()
    {
        if (isLocked || isMerging) return;

        if (isGrabbed && grabbingInteractor != null)
        {
            float currentAngle = GetHandAngle(grabbingInteractor.transform.position);
            float delta = Mathf.DeltaAngle(previousAngle, currentAngle);
            previousAngle = currentAngle;

            if (crankVisual != null)
            {
                crankVisual.Rotate(rotationAxis, -delta * rotationSpeed, Space.Self);
            }

            accumulatedDelta += delta * 0.6f;

            if (Mathf.Abs(accumulatedDelta) >= 12f)
            {
                ProcessStep(Mathf.Sign(accumulatedDelta));
                accumulatedDelta = 0;
            }
        }

        CheckAlignment();
    }

    private void ProcessStep(float direction)
    {
        if (targetLight == null) return;

        Vector3 pos = targetLight.transform.localPosition;
        float nextX = pos.x + direction * moveStep;

        bool crossingTarget =
            (pos.x < targetX && nextX > targetX) ||
            (pos.x > targetX && nextX < targetX);

        if (crossingTarget)
        {
            nextX = targetX;
        }

        pos.x = Mathf.Clamp(nextX, minX, maxX);
        targetLight.transform.localPosition = pos;
    }

    private void CheckAlignment()
    {
        if (targetLight == null || isLocked) return;

        float currentX = targetLight.transform.localPosition.x;
        float distance = Mathf.Abs(currentX - targetX);

        if (distance <= lockPadding)
        {
            Vector3 pos = targetLight.transform.localPosition;
            pos.x = targetX;
            targetLight.transform.localPosition = pos;

            LockThisCrank();
        }
    }

    private void LockThisCrank()
    {
        isLocked = true;
        isGrabbed = false;

        if (grabInteractable != null)
        {
            if (grabInteractable.isSelected && grabbingInteractor != null)
            {
                grabInteractable.interactionManager.SelectExit(grabbingInteractor, grabInteractable);
            }

            grabInteractable.enabled = false;
        }

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (interactable != null)
            interactable.enabled = false;

        AlignedCount++;

        Debug.Log($"{partType} LOCKED at {targetX}. Count: {AlignedCount}/2");

        if (AlignedCount >= 2)
        {
            StartCoroutine(MergeToExactPositions());
        }
    }

    private IEnumerator MergeToExactPositions()
    {
        isMerging = true;

        while (true)
        {
            bool done = true;

            Vector3 bp = bodyLight.transform.localPosition;
            bp.y = Mathf.MoveTowards(bp.y, bodyTargetY, mergeSpeed * Time.deltaTime);
            bodyLight.transform.localPosition = bp;

            hairLight.transform.localPosition =
                Vector3.MoveTowards(hairLight.transform.localPosition, hairFinalPos, mergeSpeed * Time.deltaTime);

            hatLight.transform.localPosition =
                Vector3.MoveTowards(hatLight.transform.localPosition, hatFinalPos, mergeSpeed * Time.deltaTime);

            if (Mathf.Abs(bodyLight.transform.localPosition.y - bodyTargetY) > 0.001f) done = false;
            if (Vector3.Distance(hairLight.transform.localPosition, hairFinalPos) > 0.001f) done = false;
            if (Vector3.Distance(hatLight.transform.localPosition, hatFinalPos) > 0.001f) done = false;

            if (done) break;

            yield return null;
        }

        Debug.Log("MERGE COMPLETE");

        if (audioSource != null && mergeSound != null)
        {
            audioSource.PlayOneShot(mergeSound);
        }

        StartCoroutine(TeleportPlayerAfterAudio());
    }

    private IEnumerator TeleportPlayerAfterAudio()
    {
        // Wait a frame to let the AudioSource start playing if it just triggered
        yield return null;

        if (audioSource != null)
        {
            // Keep checking every frame until the audio finishes
            while (audioSource.isPlaying)
            {
                yield return null;
            }
        }

        // Optional small delay after the sound for a smoother transition
        yield return new WaitForSeconds(0.5f);

        if (player != null)
        {
            player.position = teleportPosition;
            Debug.Log("Audio finished. Player teleported.");
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (isLocked) return;

        grabbingInteractor = args.interactorObject;
        previousAngle = GetHandAngle(grabbingInteractor.transform.position);
        isGrabbed = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        grabbingInteractor = null;
        isGrabbed = false;
        accumulatedDelta = 0;
    }

    private float GetHandAngle(Vector3 pos)
    {
        Vector3 offset = pos - transform.position;
        Vector3 localDir = transform.InverseTransformDirection(offset);
        return Mathf.Atan2(localDir.z, localDir.x) * Mathf.Rad2Deg;
    }
}