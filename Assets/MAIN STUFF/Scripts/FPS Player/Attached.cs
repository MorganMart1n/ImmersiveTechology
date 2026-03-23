using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attached : MonoBehaviour
{
    [SerializeField] private Transform anchorPoint;
    [SerializeField] private bool startAttached = true;

    private bool isAttached = false;
    private bool everDetached = false;
    private Rigidbody rb;
    private Collider col;
    [SerializeField] private string interactableLayerName = "Interactable"; // Layer to switch to when detached

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>(); // Add Rigidbody if missing
        }

        col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>(); // Add a default collider if missing
        }
    }

    void Start()
    {
        if (startAttached)
        {
            Attach();
        }
    }

    void FixedUpdate()
    {
        if (isAttached && anchorPoint != null)
        {
            // MovePosition is the correct way to move a kinematic RB
            rb.MovePosition(anchorPoint.position);
            rb.MoveRotation(anchorPoint.rotation);

            // Only reset velocity if the RB is NOT kinematic to avoid warnings
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    public void Attach()
    {
        if (anchorPoint == null || isAttached || everDetached) return;

        isAttached = true;

        rb.isKinematic = true;
        rb.useGravity = false;

        col.isTrigger = true; // Prevent collisions while attached
    }

    public void Detach()
    {
        if (!isAttached) return;

        isAttached = false;
        everDetached = true;

        rb.isKinematic = false;
        rb.useGravity = true;
        col.isTrigger = false;

        // Ensure this layer matches what your Player script is looking for
        int newLayer = LayerMask.NameToLayer(interactableLayerName);
        if (newLayer != -1)
        {
            gameObject.layer = newLayer;
        }
        else
        {
            Debug.LogWarning("Layer " + interactableLayerName + " not found in Tags & Layers!");
        }
    }

    public bool IsAttached()
    {
        return isAttached;
    }
}