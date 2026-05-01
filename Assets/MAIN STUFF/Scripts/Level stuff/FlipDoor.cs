using UnityEngine;
using TMPro;

public sealed class SmoothPositionSwitcher : MonoBehaviour
{
    [Header("Companion Spawning")]
    [SerializeField] private GameObject companionPrefab;
    [SerializeField] private Transform spawnLocation;
    private bool hasSpawned = false;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Settings")]
    [SerializeField] private float transitionSpeed = 2.0f;
    [SerializeField] private float interactionRange = 2.0f;
    [SerializeField] private Transform playerTransform;

    [Header("Audio")]
    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private AudioSource spawnAudioSource; // NEW: The sound the Alien/Sphere makes

    private Vector3 closedEuler = new Vector3(0, 228.6026f, 0);
    private Vector3 openEuler = new Vector3(0, 68.25015f, 0);
    private bool movingToTarget = false;
    private float transitionProgress = 0f;

    void Start()
    {
        if (playerTransform == null && Camera.main != null)
            playerTransform = Camera.main.transform;

        transform.rotation = Quaternion.Euler(closedEuler);
        UpdateUI();
    }

    public void ToggleDoor()
    {
        movingToTarget = !movingToTarget;

        // Play the door opening sound immediately
        if (doorAudioSource != null) doorAudioSource.Play();

        // If we are opening the door and haven't spawned the companion yet
        if (movingToTarget && !hasSpawned)
        {
            // Delay the spawning (and the sound) by 1 second
            Invoke("SpawnCompanion", 1.0f);
            hasSpawned = true;
        }
    }

    private void SpawnCompanion()
    {
        if (companionPrefab != null && spawnLocation != null)
        {
            Instantiate(companionPrefab, spawnLocation.position, spawnLocation.rotation);

            // Play the "Alien" sound right when it appears
            if (spawnAudioSource != null)
            {
                spawnAudioSource.Play();
            }
        }
    }

    void Update()
    {
        HandleMovement();
        UpdateUI();

        if (Vector3.Distance(playerTransform.position, transform.position) <= interactionRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }
    }

    private void HandleMovement()
    {
        if (movingToTarget) transitionProgress += Time.deltaTime * transitionSpeed;
        else transitionProgress -= Time.deltaTime * transitionSpeed;

        transitionProgress = Mathf.Clamp01(transitionProgress);
        transform.rotation = Quaternion.Slerp(Quaternion.Euler(closedEuler), Quaternion.Euler(openEuler), transitionProgress);
    }

    private void UpdateUI()
    {
        if (statusText != null)
        {
            string mode = movingToTarget ? "Open" : "Closed";
            statusText.text = $"<b>Door Status:</b> {mode}\n<b>Use Controller to Interact</b>";
        }
    }
}