using UnityEngine;

public class AlienSwapper : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject interactionCanvas;

    [Header("Alien References")]
    [Tooltip("The specific Alien object currently in the scene that you want to delete.")]
    public GameObject alienToDelete;

    [Tooltip("The New Alien Prefab that will spawn on the player.")]
    public GameObject alienToSpawn;

    private bool isPlayerNearby = false;
    private Transform playerTransform;

    void Start()
    {
        if (interactionCanvas != null)
            interactionCanvas.SetActive(false);
    }

    void Update()
    {
        // When E is pressed and player is in range
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ExecuteSwap();
        }
    }

    void ExecuteSwap()
    {
        // 1. Spawn the NEW alien prefab at the player's position
        if (alienToSpawn != null && playerTransform != null)
        {
            Instantiate(alienToSpawn, playerTransform.position, playerTransform.rotation);
            Debug.Log("Spawned new alien at player position.");
        }

        // 2. Destroy the OLD alien object referenced in the field
        if (alienToDelete != null)
        {
            Destroy(alienToDelete);
            Debug.Log("Deleted the target alien.");
        }

        // 3. Clean up the UI
        if (interactionCanvas != null)
        {
            Destroy(interactionCanvas);
        }

        // 4. Finally, destroy this script's host if it's separate
        // (Optional: remove if this script is on a permanent 'Game Manager' object)
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerTransform = other.transform; // Grab the player's location

            if (interactionCanvas != null)
                interactionCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactionCanvas != null)
                interactionCanvas.SetActive(false);
        }
    }
}