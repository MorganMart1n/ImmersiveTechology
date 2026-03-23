using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes
using UnityEngine.UI; // Optional: for "Press E" text

public class BedInteract : MonoBehaviour
{
    public string sceneToLoad;
    public float interactionRange = 3f;
    public Transform player;        // Drag your Player object here

    [Header("UI (Optional)")]
    public GameObject interactText; // A UI Text object that says "Press E to Enter"

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactionRange)
        {
            // Show the "Press E" text if you have one
            if (interactText != null) interactText.SetActive(true);

            // Check for key press
            if (Input.GetKeyDown(KeyCode.E))
            {
                ChangeScene();
            }
        }
        else
        {
            // Hide the text when you walk away
            if (interactText != null) interactText.SetActive(false);
        }
    }

    void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Scene name is empty! Please type a scene name in the Inspector.");
        }
    }
}