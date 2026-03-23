using UnityEngine;

public class WheelInteraction : MonoBehaviour
{
    [SerializeField] private Transform wheel;
    [SerializeField] private float rotationSpeed = 90f;

    private bool playerInRange = false;
    private float currentRotation = 0f;

    void Update()
    {
        if (playerInRange && Input.GetKey(KeyCode.E))
        {
            currentRotation += rotationSpeed * Time.deltaTime;

            wheel.localRotation = Quaternion.Euler(0f, currentRotation, 0f);

            Debug.Log("Wheel rotation changed to: " + currentRotation);
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed inside wheel trigger");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name);

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player can now interact with the wheel");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exited by: " + other.name);

        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left wheel area");
        }
    }
}