using System.Collections.Generic;
using UnityEngine;

public class VRClimbable : MonoBehaviour
{
    private CharacterController playerController;
    private Transform playerRig;

    private Transform climbingHand;
    private Vector3 lastHandPosition;

    private bool isClimbing;

    [Header("Optional")]
    public bool debug = false;

    private void Start()
    {
        playerController = FindFirstObjectByType<CharacterController>();
        playerRig = playerController.transform;
    }

    public void StartClimb(Transform hand)
    {
        climbingHand = hand;
        lastHandPosition = hand.position;
        isClimbing = true;
    }

    public void StopClimb()
    {
        isClimbing = false;
        climbingHand = null;
    }

    private void Update()
    {
        if (!isClimbing || climbingHand == null) return;

        Vector3 handDelta = climbingHand.position - lastHandPosition;

        // Move player opposite to hand movement
        playerController.Move(-handDelta);

        lastHandPosition = climbingHand.position;

        if (debug)
            Debug.DrawLine(climbingHand.position, climbingHand.position + handDelta, Color.green);
    }
}