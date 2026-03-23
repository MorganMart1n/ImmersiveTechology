using UnityEngine;
using System.Collections;

public class CogLogic : MonoBehaviour
{
    [Header("Status")]
    public int currentTurnCount = 0;
    public bool isLocked = false;

    [Header("Visual Settings")]
    [SerializeField] private float degreesPerTurn = 30f; // Adjust this to match your art

    [SerializeField] private PuzzleManager manager;

    // This is called by your PlayerInteractions script
    public void UpdateCogRotation(float scrollAmount)
    {
        if (isLocked || manager == null) return;

        // 1. Logic: Update the count
        int direction = (int)Mathf.Sign(scrollAmount);
        currentTurnCount += direction;

        // 2. Visuals: Physically rotate the object on the Y axis
        // We multiply degreesPerTurn by direction so it spins the right way
        transform.Rotate(0, direction * degreesPerTurn, 0);

        // 3. Log: Show exactly what happened in the console
        Debug.Log($"<color=white>{gameObject.name} rotated.</color> New Count: <b>{currentTurnCount}</b>");

        // 4. Trigger the Manager to check if we hit the goal
        manager.CheckPuzzle();
    }
}