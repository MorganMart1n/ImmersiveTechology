using UnityEngine;

public class FrogEater : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private string smallFrogTag = "SmallFrog";
    [SerializeField] private int goalCount = 10;

    [Header("Audio Clips")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip eatSound;
    [SerializeField] private AudioClip winSound;

    private int frogsEaten = 0;
    public bool HasWon = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is tagged as a small frog
        if (other.CompareTag(smallFrogTag))
        {
            EatFrog(other.gameObject);
        }
    }

    private void EatFrog(GameObject smallFrog)
    {
        frogsEaten++;

        // Play the eating sound
        if (audioSource != null && eatSound != null)
        {
            audioSource.PlayOneShot(eatSound);
        }

        // Check if we hit the goal
        if (frogsEaten >= goalCount && !HasWon)
        {
            HasWon = true;

            if (audioSource != null && winSound != null)
            {
                audioSource.PlayOneShot(winSound);
            }
        }

        // Destroy the frog object
        Destroy(smallFrog);
    }
}