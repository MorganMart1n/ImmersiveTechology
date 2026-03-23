using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    [Header("Cog References")]
    [SerializeField] private CogLogic cog1;
    [SerializeField] private int target1 = 5;

    [SerializeField] private CogLogic cog2;
    [SerializeField] private int target2 = 20;

    [SerializeField] private CogLogic cog3;
    [SerializeField] private int target3 = 15;

    [Header("Completion Settings")]
    [Tooltip("The object that is hidden until the puzzle is solved.")]
    [SerializeField] private GameObject objectToReveal;

    [Header("Puzzle Events")]
    public UnityEvent onSolved;
    private bool isSolved = false;

    private void Start()
    {
        // Safety check: Ensure the object is hidden at the start of the game
        if (objectToReveal != null)
        {
            objectToReveal.SetActive(false);
        }
    }

    public void CheckPuzzle()
    {
        if (isSolved) return;

        // Log overall progress
        Debug.Log($"Progress -> C1: {cog1.currentTurnCount}/{target1} | C2: {cog2.currentTurnCount}/{target2} | C3: {cog3.currentTurnCount}/{target3}");

        // Start confirmation if all match
        if (cog1.currentTurnCount == target1 &&
            cog2.currentTurnCount == target2 &&
            cog3.currentTurnCount == target3)
        {
            StopAllCoroutines();
            StartCoroutine(ConfirmSequence());
        }
    }

    private IEnumerator ConfirmSequence()
    {
        Debug.Log("<color=orange>Targets hit! Confirming in 0.5s...</color>");
        yield return new WaitForSeconds(0.5f);

        if (cog1.currentTurnCount == target1 &&
            cog2.currentTurnCount == target2 &&
            cog3.currentTurnCount == target3)
        {
            CompletePuzzle();
        }
    }

    private void CompletePuzzle()
    {
        isSolved = true;

        // Lock the cogs so they can't be moved anymore
        cog1.isLocked = cog2.isLocked = cog3.isLocked = true;

        // --- REVEAL THE OBJECT ---
        if (objectToReveal != null)
        {
            objectToReveal.SetActive(true);
            Debug.Log($"<color=lime>REVEALED:</color> {objectToReveal.name} is now visible.");
        }

        Debug.Log("<color=cyan><b>PUZZLE SOLVED!</b></color>");
        onSolved.Invoke();
    }
}