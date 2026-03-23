using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform followTarget;
    [SerializeField] private string playerLayerName = "Player";

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;

        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {

            audioSource.Play();
            hasPlayed = true;
        }
    }

 
 
}