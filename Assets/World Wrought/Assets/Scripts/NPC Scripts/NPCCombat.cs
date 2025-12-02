using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Simple combat AI for NPCs.  When another <see cref="Character"/>
/// enters the attack range, the NPC will use its <see cref="HeroicCombat"/>
/// component to perform a melee attack.  This script also attempts to
/// stop or slow movement while engaging a target so NPCs don't wander
/// away while attacking. Smooth approach included.
/// </summary>
[RequireComponent(typeof(HeroicCombat))]
[RequireComponent(typeof(Character))]
public class NPCCombat : MonoBehaviour
{
    public float AttackRange = 2f;
    public float AttackCooldown = 1f;
    public float DisengageDistance = 6f;

    // Smooth approach settings
    [Tooltip("How close the NPC tries to get before switching to attack.")]
    public float ApproachDistance = 1.8f;
    [Tooltip("Speed multiplier used when approaching the target.")]
    public float ApproachSpeedMultiplier = 0.9f;

    private HeroicCombat combat;
    private Character character;
    private float lastAttackTime;

    private GameObject currentTarget;
    private NavMeshAgent navAgent;
    private SimpleNPCMovement simpleMovement;
    private float originalAgentSpeed = 3.5f;

    private void Awake()
    {
        combat = GetComponent<HeroicCombat>();
        character = GetComponent<Character>();
        navAgent = GetComponent<NavMeshAgent>();
        simpleMovement = GetComponent<SimpleNPCMovement>();
        if (navAgent != null) originalAgentSpeed = navAgent.speed;
    }

    private void Update()
    {
        // Clean up any dead target
        if (currentTarget != null && !IsValidTarget(currentTarget))
        {
            StopEngaging();
        }

        if (currentTarget != null)
        {
            EngageTarget();
            return;
        }

        // Look for targets within detection radius (use a larger radius than attack range)
        Collider[] hits = Physics.OverlapSphere(transform.position, Mathf.Max(AttackRange, DisengageDistance));
        foreach (var hit in hits)
        {
            var otherChar = hit.GetComponent<Character>();
            if (otherChar != null && otherChar != character)
            {
                var targetCombat = hit.GetComponent<HeroicCombat>();
                if (targetCombat == null || targetCombat.Health <= 0) continue;

                // Start engaging
                StartEngaging(hit.gameObject);
                // Try immediate attack if close
                if (Vector3.Distance(transform.position, hit.transform.position) <= AttackRange && Time.time - lastAttackTime >= AttackCooldown)
                {
                    combat.MeleeAttack(hit.gameObject);
                    lastAttackTime = Time.time;
                }
                break;
            }
        }
    }

    private void EngageTarget()
    {
        if (currentTarget == null) return;
        float dst = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (dst > DisengageDistance)
        {
            StopEngaging();
            return;
        }

        // Face the target smoothly
        Vector3 dir = (currentTarget.transform.position - transform.position); dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir.normalized), 10f * Time.deltaTime);
        }

        // If further than approach distance, move closer smoothly
        if (navAgent != null && dst > ApproachDistance)
        {
            navAgent.isStopped = false;
            navAgent.speed = originalAgentSpeed * ApproachSpeedMultiplier;
            navAgent.SetDestination(currentTarget.transform.position);
        }
        else
        {
            // Close enough to attack: stop moving and attack on cooldown
            if (navAgent != null)
            {
                navAgent.isStopped = true;
            }
            if (Time.time - lastAttackTime >= AttackCooldown)
            {
                combat.MeleeAttack(currentTarget);
                lastAttackTime = Time.time;
            }
        }
    }

    private bool IsValidTarget(GameObject t)
    {
        if (t == null) return false;
        var hc = t.GetComponent<HeroicCombat>();
        return hc != null && hc.Health > 0;
    }

    private void StartEngaging(GameObject target)
    {
        currentTarget = target;
        // slow or pause autonomous movement
        if (simpleMovement != null) simpleMovement.enabled = false;
        if (navAgent != null)
        {
            originalAgentSpeed = navAgent.speed;
            navAgent.isStopped = false;
        }
    }

    private void StopEngaging()
    {
        currentTarget = null;
        if (navAgent != null)
        {
            navAgent.isStopped = false;
            navAgent.speed = originalAgentSpeed;
        }
        if (simpleMovement != null)
        {
            simpleMovement.enabled = true;
            simpleMovement.PickNewDestination();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}