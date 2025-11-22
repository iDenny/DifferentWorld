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
    /// Distance within which the agent is considered to have reached its
    /// destination.  When the agent is closer than this threshold, a new
    /// destination will be selected.
    /// </summary>
    public float ArrivalThreshold = 1f;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // Prevent the agent from automatically updating rotation; we'll allow
        // the model to control its own orientation if needed.
        agent.updateRotation = true;
        PickNewDestination();
    }

    private void Update()
    {
        // If we don't have any destinations configured, do nothing.
        if (Destinations == null || Destinations.Count == 0) return;
        // Check if we've reached the current destination; if so, pick another.
        if (!agent.pathPending && agent.remainingDistance <= ArrivalThreshold)
        {
            PickNewDestination();
        }
    }

    /// <summary>
    /// Selects a random destination from the list and instructs the NavMesh
    /// agent to move there.  This can be called manually to override the
    /// default wandering behaviour.
    /// </summary>
    public void PickNewDestination()
    {
        if (Destinations == null || Destinations.Count == 0) return;
        Transform target = Destinations[Random.Range(0, Destinations.Count)];
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }
}