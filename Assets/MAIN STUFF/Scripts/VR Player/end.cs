using UnityEngine;

public class EndGameOnTouch : MonoBehaviour
{
    public GameObject playerRig;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EndGame();
        }
    }

    void EndGame()
    {
        playerRig.SetActive(false);
    }
}