using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;
    public float moveSpeed = 3f;
    public float stopDistance = 2.0f; // Increased slightly for better personal space
    public float rotationSpeed = 5f; // How fast he turns to face you

    [Header("Detection")]
    public float viewDistance = 15f;
    public float fieldOfView = 110f;
    public LayerMask obstacleMask;

    private Animator anim;
    private bool canSeePlayer = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        // Ensure the animator doesn't fight our script's movement
        if (anim) anim.applyRootMotion = false;
    }

    void Update()
    {
        if (player == null) return;

        CheckLineOfSight();

        if (canSeePlayer)
        {
            // Lock target position to the ground level of the NPC
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            float distance = Vector3.Distance(transform.position, targetPosition);

            // ALWAYS rotate to face the player if they are within view distance
            FacePlayer(targetPosition);

            if (distance > stopDistance)
            {
                // MOVE toward player
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                if (anim) anim.SetBool("isWalking", true);
            }
            else
            {
                // STOPPED but still idling/rotating
                if (anim) anim.SetBool("isWalking", false);
            }
        }
        else
        {
            // IDLE if player is hidden or out of range
            if (anim) anim.SetBool("isWalking", false);
        }
    }

    void FacePlayer(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            // Smoothly rotate toward the player
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void CheckLineOfSight()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= viewDistance)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer < fieldOfView / 2f)
            {
                // Raycast starts a bit higher (chest height) to avoid hitting the floor
                if (!Physics.Raycast(transform.position + Vector3.up * 1f, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    canSeePlayer = true;
                    return;
                }
            }
        }
        canSeePlayer = false;
    }
}