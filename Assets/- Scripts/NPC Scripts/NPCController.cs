using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public float walkSpeed = .5f;
    public float runSpeed = 1.0f;
    public float followRadius = 5.0f;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private bool isFollowing;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < followRadius)
        {
            isFollowing = true;
        }
        else
        {
            isFollowing = false;
        }

        if (isFollowing && navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.speed = runSpeed;
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            navMeshAgent.speed = walkSpeed;
        }

        // Set blend tree parameters based on NPC state
        animator.SetBool("isFollowing", isFollowing);
        animator.SetFloat("speed", navMeshAgent.velocity.magnitude);

        // Additional blend tree parameters for directional walking
        float angle = CalculateAngleToPlayer();
        animator.SetFloat("angle", angle);
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
