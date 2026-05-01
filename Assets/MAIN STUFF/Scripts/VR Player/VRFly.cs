using UnityEngine;
using UnityEngine.InputSystem;

public class VRFly : MonoBehaviour
{
    [Header("Flight Settings")]
    public float flightSpeed = 15.0f;
    public InputActionProperty flyButton;
    public Transform headTransform;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    public float gravityRampUpTime = 1.5f;

    private CharacterController controller;
    private MonoBehaviour bodyTransformer;
    private MonoBehaviour continuousMoveProvider;
    private MonoBehaviour gravityProvider;

    private float verticalVelocity = 0f;
    private float gravityScale = 0f;
    private bool isFlying = false;

    private MonoBehaviour FindComponentInChildren(string typeName)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>(true))
        {
            var comp = t.GetComponent(typeName) as MonoBehaviour;
            if (comp != null) return comp;
        }
        return null;
    }

    void Start()
    {
        flyButton.action.Enable(); // ← must be here

        controller = GetComponent<CharacterController>();
        bodyTransformer = GetComponent("XRBodyTransformer") as MonoBehaviour;
        continuousMoveProvider = FindComponentInChildren("ContinuousMoveProvider");
        gravityProvider = FindComponentInChildren("GravityProvider");

        if (gravityProvider != null) gravityProvider.enabled = false;
        if (headTransform == null) headTransform = Camera.main.transform;
    }

    void Update()
    {
        if (flyButton.action.IsPressed())
        {
            Debug.Log("Fly pressed: " + flyButton.action.IsPressed());
            isFlying = true;

            if (bodyTransformer != null) bodyTransformer.enabled = false;
            if (continuousMoveProvider != null) continuousMoveProvider.enabled = false;

            // Reset so gravity always ramps from zero on release
            gravityScale = 0f;
            verticalVelocity = 0f;

            controller.Move(headTransform.forward * flightSpeed * Time.deltaTime);
        }
        else
        {
            isFlying = false;

            if (bodyTransformer != null) bodyTransformer.enabled = true;
            if (continuousMoveProvider != null) continuousMoveProvider.enabled = true;

            if (controller.isGrounded)
            {
                verticalVelocity = -2f;
                gravityScale = 0f; // reset for next flight
            }
            else
            {
                // Gradually ramp up gravity after releasing fly button
                gravityScale = Mathf.MoveTowards(gravityScale, 1f, Time.deltaTime / gravityRampUpTime);
                verticalVelocity += gravity * gravityScale * Time.deltaTime;
            }

            controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
        }
    }
}