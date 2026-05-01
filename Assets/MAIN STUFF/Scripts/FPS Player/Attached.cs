using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Attached : MonoBehaviour
{
    [SerializeField] private Transform anchorPoint;
    [SerializeField] private bool startAttached = true;

    private bool isAttached = false;
    private bool everDetached = false;

    private Rigidbody rb;
    private Collider col;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    [SerializeField] private string interactableLayerName = "Interactable";

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        col = GetComponent<Collider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();

        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    void Start()
    {
        if (startAttached)
            Attach();
    }

    void FixedUpdate()
    {
        if (isAttached && anchorPoint != null)
        {
            rb.MovePosition(anchorPoint.position);
            rb.MoveRotation(anchorPoint.rotation);

            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (isAttached)
            Detach();
    }

    public void Attach()
    {
        if (anchorPoint == null || isAttached || everDetached) return;

        isAttached = true;

        rb.isKinematic = true;
        rb.useGravity = false;

        col.isTrigger = true;
    }

    public void Detach()
    {
        if (!isAttached) return;

        isAttached = false;
        everDetached = true;

        rb.isKinematic = false;
        rb.useGravity = true;
        col.isTrigger = false;

        int newLayer = LayerMask.NameToLayer(interactableLayerName);

        if (newLayer != -1)
            gameObject.layer = newLayer;
        else
            Debug.LogWarning("Layer " + interactableLayerName + " not found in Tags & Layers!");
    }

    public bool IsAttached()
    {
        return isAttached;
    }
}