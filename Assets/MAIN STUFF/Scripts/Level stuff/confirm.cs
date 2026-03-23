using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ConfirmButton : MonoBehaviour
{
    [Header("The Three Cranks")]
    public CrankIt crank1;
    public CrankIt crank2;
    public CrankIt crank3;

    [Header("The Three Lights")]
    public GameObject light1;
    public GameObject light2;
    public GameObject light3;

    [Header("Merge Settings")]
    public float targetY = -9.56f;
    public float moveSpeed = 5f;
    public GameObject objectToReveal;

    [Header("Feedback")]
    [SerializeField] private AudioSource confirmSound;
    public UnityEvent onConfirmed;

    private bool isSolved = false;

    void Start()
    {
        if (objectToReveal != null) objectToReveal.SetActive(false);
    }

    public void Confirm()
    {
        if (isSolved) return;

        // Change "IsInRange()" to "IsActuallyInRange()" to match the new CrankIt script
        bool hairAligned = crank2.IsActuallyInRange();
        bool hatAligned = crank3.IsActuallyInRange();

        Debug.Log($"<color=yellow>Check:</color> Hair Aligned: {hairAligned} | Hat Aligned: {hatAligned}");

        if (hairAligned && hatAligned)
        {
            isSolved = true;
            Debug.Log("<color=lime><b>MERGING: Body, Hair, and Hat are aligned!</b></color>");

            if (confirmSound) confirmSound.Play();
            if (objectToReveal) objectToReveal.SetActive(true);

            onConfirmed.Invoke();
            StartCoroutine(MergeLightsDown());
        }
        else
        {
            // Optional: You could play a "Fail" sound here
            Debug.Log("<color=red>Parts are not aligned with the Body!</color>");
        }
    }

    IEnumerator MergeLightsDown()
    {
        Debug.Log("<color=lime>Merging Lights...</color>");
        GameObject[] lights = { light1, light2, light3 };
        bool moving = true;

        while (moving)
        {
            moving = false;
            foreach (GameObject l in lights)
            {
                if (l == null) continue;
                Vector3 p = l.transform.localPosition;
                p.y = Mathf.MoveTowards(p.y, targetY, moveSpeed * Time.deltaTime);
                l.transform.localPosition = p;

                if (Mathf.Abs(p.y - targetY) > 0.001f) moving = true;
            }
            yield return null;
        }
        Debug.Log("<b>Puzzle Complete:</b> Lights Merged.");
    }
}