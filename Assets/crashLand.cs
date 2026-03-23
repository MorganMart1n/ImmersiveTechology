using UnityEngine;

public class AlienTrigger : MonoBehaviour
{
    [SerializeField] private Animator alienAnimator;
    [SerializeField] private MonoBehaviour scriptToEnable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Play animation via trigger
            if (alienAnimator != null)
            {
                alienAnimator.SetTrigger("startAnimation");
            }

            // Enable alien behaviour
            if (scriptToEnable != null)
            {
                scriptToEnable.enabled = true;
            }

            // Disable trigger so it only happens once
            GetComponent<BoxCollider>().enabled = false;

            Debug.Log("Alien triggered!");
        }
    }
}