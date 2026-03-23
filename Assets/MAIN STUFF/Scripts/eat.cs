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

    // This MUST be public and capitalized 'HasWon'
    public bool HasWon = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(smallFrogTag))
        {
            FPSGrab grabScript = other.GetComponentInChildren<FPSGrab>();
            if (grabScript != null && grabScript.playerInteractions != null)
            {
                grabScript.playerInteractions.BreakConnection();
            }

            EatFrog(other.gameObject);
        }
    }

    private void EatFrog(GameObject smallFrog)
    {
        frogsEaten++;

        if (audioSource != null && eatSound != null)
        {
            audioSource.PlayOneShot(eatSound);
        }

        if (frogsEaten >= goalCount && !HasWon)
        {
            HasWon = true;
            if (audioSource != null && winSound != null)
            {
                audioSource.PlayOneShot(winSound);
            }
            Debug.Log("10 Frogs Eaten! Crank is now functional.");
        }

        Destroy(smallFrog);
    }
}