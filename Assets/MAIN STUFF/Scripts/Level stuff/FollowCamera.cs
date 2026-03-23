using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform targetCamera;
    private Vector3 savedOffset;
    private Quaternion savedRotationOffset; // Store the initial rotation difference

    void Start()
    {
        if (targetCamera != null)
        {
            // Calculate the position relative to the camera
            savedOffset = targetCamera.InverseTransformPoint(transform.position);

            // Calculate the rotation relative to the camera
            // We multiply the inverse of the camera's rotation by the gun's rotation
            savedRotationOffset = Quaternion.Inverse(targetCamera.rotation) * transform.rotation;
        }
    }

    void LateUpdate()
    {
        if (targetCamera != null)
        {
            // 1. Apply the saved relative rotation
            // This adds the camera's current rotation to our original offset
            transform.rotation = targetCamera.rotation * savedRotationOffset;

            // 2. Apply the saved relative position
            transform.position = targetCamera.TransformPoint(savedOffset);
        }
    }
}