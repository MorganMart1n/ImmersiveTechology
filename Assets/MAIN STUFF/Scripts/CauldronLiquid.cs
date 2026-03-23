using UnityEngine;

public class Fill : MonoBehaviour
{
    // Drag the object you want to "copy" the color from into this slot in the Inspector
    [SerializeField] private GameObject targetObject;

    private Renderer myRenderer;

    void Start()
    {
        // Cache our own renderer so we aren't calling GetComponent every frame
        myRenderer = GetComponent<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object we hit is the one we dragged into the SerializedField
        if (collision.gameObject == targetObject)
        {
            // Try to get the Renderer of the target object
            Renderer targetRenderer = targetObject.GetComponent<Renderer>();

            if (targetRenderer != null)
            {
                // Set our material color to the target's material color
                myRenderer.material.color = targetRenderer.material.color;

                Debug.Log("Color synced with " + targetObject.name);
            }
        }
    }
}