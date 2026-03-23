using UnityEngine;

public class CubeInteractable : MonoBehaviour
{
    // This function is called when the player interacts
    public void OnInteract()
    {
        Debug.Log("Holding E on the cube: " + gameObject.name);
    }
}