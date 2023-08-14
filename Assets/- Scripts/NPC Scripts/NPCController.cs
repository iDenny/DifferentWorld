using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public float walkSpeed = 1.0f;
    public float followRadius = 5.0f;
    public float stopDistance = 1.5f; // Distance to stop near the player

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private static readonly int IsFollowing = Animator.StringToHash("isFollowing");
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int Angle = Animator.StringToHash("angle");

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isFollowing = distanceToPlayer < followRadius;

        if (isFollowing && navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.speed = walkSpeed;
            navMeshAgent.SetDestination(player.position);

            // Check if the NPC is within the stop distance
            if (distanceToPlayer < stopDistance)
            {
                navMeshAgent.velocity = Vector3.zero; // Stop the NPC's movement
            }
        }
        else
        {
            navMeshAgent.speed = 0f; // Stop the NPC when not following
        }

        // Set blend tree parameters based on NPC state
        animator.SetBool(IsFollowing, isFollowing);

        // Set speed parameter based on navMeshAgent's velocity
        animator.SetFloat(Speed, navMeshAgent.velocity.magnitude);

        // Additional blend tree parameters for directional walking
        float angle = CalculateAngleToPlayer();
        animator.SetFloat(Angle, angle);
    }

    float CalculateAngleToPlayer()
    {
        Vector3 playerDirection = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, playerDirection);
        float crossProduct = Vector3.Cross(transform.forward, playerDirection).y;

        if (crossProduct < 0)
        {
            angle = -angle;
        }

        return angle;
    }
}
