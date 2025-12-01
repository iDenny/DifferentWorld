using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Provides very simple AI movement for NPCs using Unity's NavMeshAgent.  This
/// script will continually pick random destinations from a set of target
/// positions and send the NPC there.  In a real game, targets would be
/// workstations, patrol waypoints or other contextual points.  Attach this
/// component alongside <see cref="NPCController"/> and ensure a NavMesh has
/// been baked.  NPCs will then wander around instead of hovering in place.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPCController))]
public class SimpleNPCMovement : MonoBehaviour
{
    /// <summary>
    /// List of world positions the NPC can choose from as destinations.  You
    /// can populate this list in the Inspector with empty GameObjects placed
    /// around your scene (e.g. houses, farms, patrol points).  The NPC will
    /// walk to a random point and then choose another when it arrives.
    /// </summary>
    public List<Transform> Destinations = new List<Transform>();

    /// <summary>
    /// If true, the NPC will ignore the Destinations list and instead
    /// wander by picking random points on the navmesh within a certain
    /// radius.  This allows NPCs to move autonomously without needing
    /// predefined waypoints.  Set this to false if you prefer to use
    /// specific waypoints.
    /// </summary>
    public bool UseRandomWander = false;

    /// <summary>
    /// The maximum distance from the NPC's current position when choosing a
    /// new random destination.  Larger values result in broader wandering.
    /// </summary>
    public float WanderRadius = 10f;

    /// <summary>
    /// Distance within which the agent is considered to have reached its
    /// destination.  When the agent is closer than this threshold, a new
    /// destination will be selected.
    /// </summary>
    public float ArrivalThreshold = 1f;

    /// <summary>
    /// Minimum time in seconds to idle after reaching a destination.  NPCs
    /// will wait a random amount between MinIdleTime and MaxIdleTime
    /// before moving again.  Set both values to zero for no idle delay.
    /// </summary>
    public float MinIdleTime = 0f;

    /// <summary>
    /// Maximum time in seconds to idle after reaching a destination.
    /// </summary>
    public float MaxIdleTime = 0f;

    private float idleTimer;
    private bool waiting;

    private NavMeshAgent agent;
    private Animator animator;

    // Animator hashes
    private static readonly int HorizontalParam = Animator.StringToHash("Velocity X");
    private static readonly int VerticalParam = Animator.StringToHash("Velocity Y");
    private static readonly int WalkParam = Animator.StringToHash("isWalking");
    private static readonly int RunParam = Animator.StringToHash("isRunning");
    private static readonly int SprintParam = Animator.StringToHash("isSprinting");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        // Let the script handle rotation so we can match animator-driven turns
        agent.updateRotation = false;
        PickNewDestination();
    }

    private void Update()
    {
        if (!UseRandomWander && (Destinations == null || Destinations.Count == 0))
        {
            UpdateAnimatorFromVelocity();
            return;
        }
        if (waiting)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                waiting = false;
                PickNewDestination();
            }
            UpdateAnimatorFromVelocity();
            return;
        }
        if (!agent.pathPending && agent.remainingDistance <= ArrivalThreshold)
        {
            if (MaxIdleTime > 0f)
            {
                waiting = true;
                idleTimer = Random.Range(MinIdleTime, MaxIdleTime);
            }
            else
            {
                PickNewDestination();
            }
        }

        UpdateAnimatorFromVelocity();
    }

    /// <summary>
    /// Selects a random destination from the list and instructs the NavMesh
    /// agent to move there.  This can be called manually to override the
    /// default wandering behaviour.
    /// </summary>
    public void PickNewDestination()
    {
        Vector3 newPos;
        if (UseRandomWander || Destinations == null || Destinations.Count == 0)
        {
            // Choose a random point on the navmesh within the wander radius.
            newPos = RandomNavmeshLocation(WanderRadius);
        }
        else
        {
            Transform target = Destinations[Random.Range(0, Destinations.Count)];
            newPos = target != null ? target.position : transform.position;
        }
        agent.SetDestination(newPos);
    }

    /// <summary>
    /// Samples a random position on the NavMesh within a specified radius.
    /// This helper method uses NavMesh.SamplePosition to ensure the
    /// resulting point is valid for the agent to travel to.
    /// </summary>
    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        // If sampling fails, fall back to current position.
        return transform.position;
    }

    private void UpdateAnimatorFromVelocity()
    {
        if (animator == null || agent == null) return;

        Vector3 worldVel = agent.velocity;
        Vector3 localVel = transform.InverseTransformDirection(worldVel);

        float vx = localVel.x;
        float vy = localVel.z;

        animator.SetFloat(HorizontalParam, vx, 0.1f, Time.deltaTime);
        animator.SetFloat(VerticalParam, vy, 0.1f, Time.deltaTime);

        float speed = worldVel.magnitude;
        bool isWalking = speed > 0.1f && speed <= agent.speed + 0.1f;
        bool isRunning = speed > agent.speed + 0.1f;

        animator.SetBool(WalkParam, isWalking);
        animator.SetBool(RunParam, isRunning);
        animator.SetBool(SprintParam, isRunning && speed >= agent.speed - 0.1f);

        // Smoothly rotate towards movement direction for visuals
        if (speed > 0.1f)
        {
            Vector3 lookDir = worldVel.normalized;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion target = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, 10f * Time.deltaTime);
            }
        }
    }
}