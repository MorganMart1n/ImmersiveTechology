using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class checkingFrogDistance : MonoBehaviour
{
    [SerializeField] private AudioSource frogSound;
    [SerializeField] private string targetTag = "Player";

    void Start()
    {
        if (frogSound == null) frogSound = GetComponent<AudioSource>();

        // Ensure the AudioSource is set up for 2D for testing
        frogSound.playOnAwake = false;
        frogSound.spatialBlend = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        // This will tell us exactly what is touching the box in the Console
        Debug.Log("Trigger hit by: " + other.gameObject.name);

        if (other.CompareTag(targetTag))
        {
            if (frogSound != null)
            {
                frogSound.Play();
                Debug.Log("<color=cyan>Frog Sound Played!</color>");
            }
        }
    }

    // This draws the box in the SCENE view so you can find it
    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.color = new Color(0, 1, 1, 0.3f); // Transparent Cyan
            Gizmos.DrawCube(transform.position + box.center, box.size);
        }
    }
}