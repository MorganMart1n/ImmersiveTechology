using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PushButtonLogger : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnPressed);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnPressed);
    }

    public void OnPressed(SelectEnterEventArgs args)
    {
        Debug.Log("Pressed!");
    }
}