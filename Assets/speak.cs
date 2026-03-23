using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float detectionRadius = 8.0f;
    [SerializeField] private LayerMask playerLayer;

    private bool hasPlayed = false;

    private void Update()
    {
        // If we've already played, we're done (unless you want it to loop/reset)
        if (hasPlayed) return;

        // Check if any collider on the Player layer is within our radius
        if (Physics.CheckSphere(transform.position, detectionRadius, playerLayer))
        {
            Debug.Log("Player detected within range of Kacper's child!");
            audioSource.Play();
            hasPlayed = true;
        }
    }

    // This lets you see the actual range in the Scene view so you can tune it
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}