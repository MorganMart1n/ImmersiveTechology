using UnityEngine;
using System.Collections.Generic;

public class CauldronMix : MonoBehaviour
{
    [Header("Ingredient Tracking")]
    [SerializeField] private int requiredIngredients = 3;
    private int ingredientsAdded = 0;
    private HashSet<int> processedIngredients = new HashSet<int>();

    [Header("Audio")]
    [SerializeField] private AudioSource completionSound;

    [Header("Ritual Object (The Spinning One)")]
    [SerializeField] private GameObject objectToSpin;
    [SerializeField] private float spinSpeed = 100f;
    [SerializeField] private float riseSpeed = 1f;
    private Vector3 targetPosition;

    [Header("Reward Object (The One to Reveal)")]
    [Tooltip("This object is hidden until the ritual is 100% complete.")]
    [SerializeField] private GameObject objectToReveal;

    [Header("Light")]
    [SerializeField] private Light ritualLight;
    [SerializeField] private float lightFadeSpeed = 2f;
    [SerializeField] private float targetLightIntensity = 1f;

    private Renderer liquidRenderer;
    private bool isRitualComplete = false;
    private bool lightActivated = false;

    void Start()
    {
        liquidRenderer = GetComponent<Renderer>();

        // 1. Initial Light Setup
        if (ritualLight != null)
        {
            ritualLight.enabled = false;
            ritualLight.intensity = 0f;
        }

        // 2. Initial Reveal Object Setup
        if (objectToReveal != null)
        {
            objectToReveal.SetActive(false);
        }

        Debug.Log($"Cauldron Ready. Audio: {(completionSound != null ? "OK" : "MISSING")}");
    }

    private void Update()
    {
        if (!isRitualComplete || objectToSpin == null) return;

        // Move the spinning ritual object upward
        objectToSpin.transform.position = Vector3.MoveTowards(
            objectToSpin.transform.position,
            targetPosition,
            riseSpeed * Time.deltaTime
        );

        // While the sound plays, spin the ritual object
        if (completionSound != null && completionSound.isPlaying)
        {
            objectToSpin.transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);

            if (!lightActivated)
            {
                ActivateLightAtStartOfSpin();
                lightActivated = true;
            }
        }

        // Smoothly fade the light in
        if (ritualLight != null && ritualLight.enabled && ritualLight.intensity < targetLightIntensity)
        {
            ritualLight.intensity = Mathf.MoveTowards(
                ritualLight.intensity,
                targetLightIntensity,
                lightFadeSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isRitualComplete) return;

        // Find the root object (in case the collider is on a child)
        GameObject rootObj = other.transform.parent != null ? other.transform.parent.gameObject : other.gameObject;
        int instanceID = rootObj.GetInstanceID();

        if (processedIngredients.Contains(instanceID)) return;

        string name = other.gameObject.name.ToLower();
        string pName = other.transform.parent != null ? other.transform.parent.name.ToLower() : "";

        // Simple color detection based on object name
        if (name.Contains("blue") || pName.Contains("blue"))
            ProcessIngredient(Color.blue, rootObj, instanceID);
        else if (name.Contains("red") || pName.Contains("red"))
            ProcessIngredient(Color.red, rootObj, instanceID);
        else if (name.Contains("green") || pName.Contains("green"))
            ProcessIngredient(Color.green, rootObj, instanceID);
    }

    void ProcessIngredient(Color col, GameObject obj, int id)
    {
        processedIngredients.Add(id);
        ingredientsAdded++;

        Debug.Log($"<color=magenta>Ingredient Added:</color> {ingredientsAdded}/{requiredIngredients}");

        ApplyPotionColor(col);

        // Destroy the ingredient after it enters the pot
        obj.SetActive(false);
        Destroy(obj, 0.1f);

        if (ingredientsAdded >= requiredIngredients)
        {
            CompleteRitual();
        }
    }

    void CompleteRitual()
    {
        isRitualComplete = true;
        Debug.Log("<color=cyan><b>The ritual is complete!</b></color>");

        if (objectToSpin != null)
        {
            targetPosition = new Vector3(
                objectToSpin.transform.position.x,
                transform.position.y + 3f, // Rise 3 units above cauldron
                objectToSpin.transform.position.z
            );
        }

        // --- THE REVEAL ---
        if (objectToReveal != null)
        {
            objectToReveal.SetActive(true);
            Debug.Log($"<color=lime>Reward Revealed: {objectToReveal.name}</color>");
        }

        if (completionSound != null)
        {
            completionSound.Play();
        }
    }

    private void ActivateLightAtStartOfSpin()
    {
        if (ritualLight != null)
        {
            ritualLight.enabled = true;
        }
    }

    void ApplyPotionColor(Color col)
    {
        if (liquidRenderer != null)
        {
            // Support for URP/HDRP (_BaseColor) and Standard Shader (_Color)
            if (liquidRenderer.material.HasProperty("_BaseColor"))
                liquidRenderer.material.SetColor("_BaseColor", col);
            else
                liquidRenderer.material.SetColor("_Color", col);
        }
    }
}