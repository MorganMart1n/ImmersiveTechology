using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [Header("Interactable Info")]
    public float sphereCastRadius = 0.5f;
    public int interactableLayerIndex;
    private Vector3 raycastPos;
    public GameObject lookObject;
    private FPSGrab physicsObject;
    [SerializeField] Camera mainCamera;

    [Header("Pickup")]
    [SerializeField] private Transform pickupParent;
    public GameObject currentlyPickedUpObject;
    private Rigidbody pickupRB;

    [Header("Object Follow")]
    [SerializeField] private float holdDistance = 2.5f;
    [SerializeField] private float followSpeed = 20f;
    [SerializeField] private float maxDistance = 10f;

    [Header("Rotation")]
    public float rotationSpeed = 100f;

    [Header("Destruction & UI")]
    [SerializeField] private GameObject interactionCanvas;

    void Update()
    {
        if (mainCamera == null) return;

        HandleDetection();

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            HandleInteractionInput();
        }

        HandleScrollInteractions();
    }

    private void HandleDetection()
    {
        raycastPos = mainCamera.transform.position;
        RaycastHit hit;

        // Perform the spherecast
        if (Physics.SphereCast(raycastPos, sphereCastRadius, mainCamera.transform.forward, out hit, maxDistance, 1 << interactableLayerIndex))
        {
            lookObject = hit.collider.gameObject;

            if (interactionCanvas != null && currentlyPickedUpObject == null)
                interactionCanvas.SetActive(true);
        }
        else
        {
            lookObject = null;
            if (interactionCanvas != null)
                interactionCanvas.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (mainCamera == null) return;
   
        Gizmos.color = Color.magenta;

        float debugDistance = 1.5f;
        Vector3 debugSpherePos = mainCamera.transform.position + (mainCamera.transform.forward * debugDistance);

        Gizmos.DrawLine(mainCamera.transform.position, debugSpherePos);

        Gizmos.DrawWireSphere(debugSpherePos, sphereCastRadius);

        Gizmos.color = new Color(1f, 0f, 1f, 0.2f);
        Vector3 maxReachPos = mainCamera.transform.position + (mainCamera.transform.forward * maxDistance);
        Gizmos.DrawWireSphere(maxReachPos, sphereCastRadius);
    }

    private void HandleInteractionInput()
    {
        if (lookObject == null && currentlyPickedUpObject == null) return;

        if (currentlyPickedUpObject != null)
        {
            BreakConnection();
            return;
        }

        if (lookObject.GetComponentInParent<FPSGrab>() != null)
        {
            OnGrabPressed();
            return;
        }

        ConfirmButton confirm = lookObject.GetComponentInParent<ConfirmButton>();
        Attached attached = lookObject.GetComponentInParent<Attached>();

        if (confirm != null || (attached != null && attached.IsAttached()))
        {
            OnGrabPressed();
            return;
        }

        if (lookObject.GetComponentInParent<CogLogic>() != null || lookObject.GetComponentInParent<CrankIt>() != null)
        {
            return;
        }

        DestroyLookObject();
    }

    private void HandleScrollInteractions()
    {
        if (lookObject == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0) return;

        CogLogic cog = lookObject.GetComponentInParent<CogLogic>();
        if (cog != null)
        {
            cog.UpdateCogRotation(scroll);
            return;
        }

        CrankIt crank = lookObject.GetComponentInParent<CrankIt>();
        if (crank != null)
        {
            crank.UpdateLightPosition(scroll);
        }
    }

    public void OnGrabPressed()
    {
        if (lookObject == null) return;

        Attached attachedObject = lookObject.GetComponentInParent<Attached>();
        if (attachedObject != null && attachedObject.IsAttached())
        {
            attachedObject.Detach();
            return;
        }

        ConfirmButton confirm = lookObject.GetComponentInParent<ConfirmButton>();
        if (confirm != null)
        {
            confirm.Confirm();
            return;
        }

        if (currentlyPickedUpObject == null)
        {
            PickUpObject();
        }
    }

    private void DestroyLookObject()
    {
        if (interactionCanvas != null) interactionCanvas.SetActive(false);
        Destroy(lookObject);
        lookObject = null;
    }

    public void PickUpObject()
    {
        physicsObject = lookObject.GetComponentInParent<FPSGrab>();
        if (physicsObject == null) return;

        currentlyPickedUpObject = lookObject;
        pickupRB = currentlyPickedUpObject.GetComponent<Rigidbody>();

        if (pickupRB == null) return;

        // 1. Kill momentum and gravity
        pickupRB.linearVelocity = Vector3.zero;
        pickupRB.angularVelocity = Vector3.zero;
        pickupRB.useGravity = false;
        
        Vector3 liftOffset = (mainCamera.transform.position - currentlyPickedUpObject.transform.position).normalized * 0.2f;
        currentlyPickedUpObject.transform.position += liftOffset + (Vector3.up * 0.1f);

        pickupRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        pickupRB.constraints = RigidbodyConstraints.FreezeRotation;
        physicsObject.playerInteractions = this;
        StartCoroutine(physicsObject.PickUp());
    }

    public void BreakConnection()
    {
        if (pickupRB != null)
        {
            pickupRB.constraints = RigidbodyConstraints.None;
            pickupRB.useGravity = true;

            pickupRB.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }

        if (physicsObject != null) physicsObject.pickedUp = false;

        currentlyPickedUpObject = null;
        pickupRB = null;
        physicsObject = null;
    }

    void FixedUpdate()
    {
        if (currentlyPickedUpObject != null && pickupRB != null)
        {
            Vector3 targetPos = mainCamera.transform.position + (mainCamera.transform.forward * holdDistance);
            Vector3 moveDirection = targetPos - pickupRB.position;
            pickupRB.linearVelocity = moveDirection * followSpeed;

            Quaternion targetRotation = mainCamera.transform.rotation;
            pickupRB.MoveRotation(Quaternion.Slerp(pickupRB.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

            if (Vector3.Distance(targetPos, pickupRB.position) > maxDistance) BreakConnection();
        }
    }
}