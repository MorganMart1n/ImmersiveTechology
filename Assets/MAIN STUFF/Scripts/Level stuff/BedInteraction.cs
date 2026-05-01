using UnityEngine;
using UnityEngine.SceneManagement;

public class BedInteract : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad = "Space";

    private void OnTriggerEnter(Collider other)
    {
        // Log every touch to help you debug
        Debug.Log("Bed Trigger touched by: " + other.name + " with Tag: " + other.tag);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected! Attempting to load scene: " + sceneToLoad);
            ChangeScene();
        }
        else
        {
            Debug.LogWarning("Object touched the bed, but it wasn't tagged 'Player'.");
        }
    }

    void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            // Check if the scene actually exists in the Build Settings before loading
            if (Application.CanStreamedLevelBeLoaded(sceneToLoad))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                Debug.LogError("CANNOT LOAD SCENE: '" + sceneToLoad + "' is not in your Build Settings!");
            }
        }
        else
        {
            Debug.LogError("Scene name is empty in the Inspector!");
        }
    }
}