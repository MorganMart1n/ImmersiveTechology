using UnityEngine;

public class DebugTeleportTrigger : MonoBehaviour
{
    public Renderer boxRenderer;
    public Color normalColor = Color.white;
    public Color touchedColor = Color.red;

    private void Start()
    {
        if (boxRenderer == null)
            boxRenderer = GetComponent<Renderer>();

        boxRenderer.material.color = normalColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.name);

        if (other.CompareTag("Player") || other.CompareTag("MainCamera") || other.gameObject.name.Contains("Controller"))
        {
            boxRenderer.material.color = touchedColor;
            Debug.Log("HAND ENTERED TRIGGER");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        boxRenderer.material.color = normalColor;
        Debug.Log("Exited trigger");
    }
}