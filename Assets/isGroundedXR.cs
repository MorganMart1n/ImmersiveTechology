using UnityEngine;

public class XRGroundCheck : MonoBehaviour
{
    [SerializeField]
    private CharacterController characterController;

    void Start()
    {
        if (characterController == null)
        {
            Debug.LogError("CharacterController not assigned in the Inspector!");
        }
    }

    void Update()
    {
        if (characterController != null)
        {
            if (characterController.isGrounded)
            {
                Debug.Log("Player is grounded");
                // Enable movement or jump here
            }
            else
            {
                Debug.Log("Player is NOT grounded");
                // Apply gravity or disable movement here
            }
        }
    }
}