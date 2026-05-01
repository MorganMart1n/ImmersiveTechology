using UnityEngine;
using System.Collections;

public class PlaySoundOnTrigger : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource secondAudioSource;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 8.0f;
    [SerializeField] private LayerMask playerLayer;

    [Header("References")]
    [SerializeField] private CauldronMix cauldron;
    [SerializeField] private GameObject player;

    private bool hasPlayed = false;
    private bool secondPlayed = false;

    private void Update()
    {
        bool playerInRange = Physics.CheckSphere(transform.position, detectionRadius, playerLayer);

        // First sound
        if (!hasPlayed && playerInRange)
        {
            audioSource.Play();
            hasPlayed = true;
        }

        // Second sound + start teleport sequence
        if (!secondPlayed &&
            cauldron != null &&
            cauldron.IsRitualComplete &&
            playerInRange)
        {
            secondPlayed = true;
            secondAudioSource.Play();

            StartCoroutine(TeleportAfterSound());
        }
    }

    private IEnumerator TeleportAfterSound()
    {
        if (secondAudioSource.clip != null)
        {
            yield return new WaitForSeconds(secondAudioSource.clip.length);
        }

        TeleportPlayer();
    }

    private void TeleportPlayer()
    {
        if (player == null) return;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Your exact world transform values
        player.transform.position = new Vector3(-163.5f, 62.75f, 118.7f);
        player.transform.rotation = Quaternion.Euler(0f, -86.204f, 0f);

        if (cc != null) cc.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}