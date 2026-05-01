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

                // Enable movement or jump here
            }
            else
            {
              
                // Apply gravity or disable movement here
            }
        }
    }
}