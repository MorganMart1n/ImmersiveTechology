using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TerminalTeleport : MonoBehaviour
{
    [Header("Socket References")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket1;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket2;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket3;

    [Header("Teleport Settings")]
    public Transform xrOrigin;

    // Updated coordinates from image_76dfbd.png
    private Vector3 targetPosition = new Vector3(9.4f, 241.19f, 247.57f);
    private Vector3 targetRotation = new Vector3(0f, 175.477f, 0f);

    private bool hasTeleported = false;

    void OnEnable()
    {
        // Listen for when things are snapped into sockets
        socket1.selectEntered.AddListener(OnObjectAttached);
        socket2.selectEntered.AddListener(OnObjectAttached);
        socket3.selectEntered.AddListener(OnObjectAttached);
    }

    void OnDisable()
    {
        socket1.selectEntered.RemoveListener(OnObjectAttached);
        socket2.selectEntered.RemoveListener(OnObjectAttached);
        socket3.selectEntered.RemoveListener(OnObjectAttached);
    }

    private void OnObjectAttached(SelectEnterEventArgs args)
    {
        Debug.Log($"<color=cyan>Object '{args.interactableObject.transform.name}' attached to {args.interactorObject.transform.name}</color>");
        CheckAllSockets();
    }

    void CheckAllSockets()
    {
        if (hasTeleported) return;

        bool s1 = socket1.hasSelection;
        bool s2 = socket2.hasSelection;
        bool s3 = socket3.hasSelection;

        Debug.Log($"Socket Status -> 1: {s1} | 2: {s2} | 3: {s3}");

        if (s1 && s2 && s3)
        {
            TeleportPlayer();
        }
        else
        {
            Debug.Log("<color=orange>Still waiting for more objects...</color>");
        }
    }

    void TeleportPlayer()
    {
        if (xrOrigin != null)
        {
            Debug.Log("<color=green>!!! ALL OBJECTS DETECTED. TELEPORTING NOW !!!</color>");

            // Disable CharacterController to prevent it from fighting the teleport
            var characterController = xrOrigin.GetComponent<CharacterController>();
            if (characterController != null) characterController.enabled = false;

            xrOrigin.position = targetPosition;
            xrOrigin.eulerAngles = targetRotation;

            if (characterController != null) characterController.enabled = true;

            hasTeleported = true;
        }
        else
        {
            Debug.LogError("<color=red>TELEPORT FAILED: You forgot to drag the XR Origin into the script slot!</color>");
        }
    }
}