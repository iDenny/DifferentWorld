using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPCController))]
public class SimpleNPCMovement : MonoBehaviour
{
    public List<Transform> Destinations = new List<Transform>();
    public bool UseRandomWander = false;
    public float WanderRadius = 10f;
    public float ArrivalThreshold = 1f;
    public float MinIdleTime = 0f;
    public float MaxIdleTime = 0f;

    private float idleTimer;
    private bool waiting;

    private NavMeshAgent agent;
    private Animator animator;

    private static readonly string HorizontalName = "Velocity X";
    private static readonly string VerticalName = "Velocity Y";
    private static readonly string WalkName = "isWalking";
    private static readonly string RunName = "isRunning";
    private static readonly string SprintName = "isSprinting";

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
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

    public void PickNewDestination()
    {
        Vector3 newPos;
        if (UseRandomWander || Destinations == null || Destinations.Count == 0)
        {
            newPos = RandomNavmeshLocation(WanderRadius);
        }
        else
        {
            Transform target = Destinations[Random.Range(0, Destinations.Count)];
            newPos = target != null ? target.position : transform.position;
        }
        agent.SetDestination(newPos);
    }

    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    private bool AnimatorHasParameter(string paramName)
    {
        if (animator == null) return false;
        foreach (var p in animator.parameters)
        {
            if (p.name == paramName) return true;
        }
        return false;
    }

    private void UpdateAnimatorFromVelocity()
    {
        if (animator == null || agent == null) return;

        Vector3 worldVel = agent.velocity;
        Vector3 localVel = transform.InverseTransformDirection(worldVel);

        float vx = localVel.x;
        float vy = localVel.z;

        if (AnimatorHasParameter(HorizontalName))
            animator.SetFloat(HorizontalName, vx, 0.1f, Time.deltaTime);
        if (AnimatorHasParameter(VerticalName))
            animator.SetFloat(VerticalName, vy, 0.1f, Time.deltaTime);

        float speed = worldVel.magnitude;
        bool isWalking = speed > 0.1f && speed <= agent.speed + 0.1f;
        bool isRunning = speed > agent.speed + 0.1f;

        if (AnimatorHasParameter(WalkName))
            animator.SetBool(WalkName, isWalking);
        if (AnimatorHasParameter(RunName))
            animator.SetBool(RunName, isRunning);
        if (AnimatorHasParameter(SprintName))
            animator.SetBool(SprintName, isRunning && speed >= agent.speed - 0.1f);

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