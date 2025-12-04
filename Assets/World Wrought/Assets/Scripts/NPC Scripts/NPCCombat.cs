using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HeroicCombat))]
[RequireComponent(typeof(Character))]
public class NPCCombat : MonoBehaviour
{
    public float AttackRange = 2f;
    public float AttackCooldown = 1f;
    public float DisengageDistance = 6f;

    [Tooltip("Layer mask of valid targets this NPC will consider (e.g. Target or Player layers).")]
    public LayerMask TargetMask = ~0;

    [Tooltip("How close the NPC tries to get before switching to attack.")]
    public float ApproachDistance = 1.8f;

    [Tooltip("Walking speed used during wandering and precise approach.")]
    public float WalkSpeed = 2f;
    [Tooltip("Running speed used when chasing detected targets.")]
    public float RunSpeed = 6f;

    [Tooltip("How long attack animation lasts (used to reset isAttacking flag).")]
    public float AttackAnimDuration = 0.6f;

    private HeroicCombat combat;
    private Character character;
    private float lastAttackTime;

    private GameObject currentTarget;
    private NavMeshAgent navAgent;
    private SimpleNPCMovement simpleMovement;
    private float originalAgentSpeed = 3.5f;

    private Animator animator;
    private WeaponDamage[] weapons;

    private static readonly int AttackParam = Animator.StringToHash("isAttacking");

    private Coroutine attackResetRoutine;

    private void Awake()
    {
        combat = GetComponent<HeroicCombat>();
        character = GetComponent<Character>();
        navAgent = GetComponent<NavMeshAgent>();
        simpleMovement = GetComponent<SimpleNPCMovement>();
        animator = GetComponent<Animator>();
        weapons = GetComponentsInChildren<WeaponDamage>(true);
        if (navAgent != null)
        {
            originalAgentSpeed = navAgent.speed;
            navAgent.speed = WalkSpeed;
            navAgent.acceleration = 8f;
            navAgent.angularSpeed = 120f;
        }
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
        Collider[] hits = Physics.OverlapSphere(transform.position, Mathf.Max(AttackRange, DisengageDistance), TargetMask);
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
                    // If this NPC has weapon colliders, let them apply damage; otherwise use combat.MeleeAttack
                    if (weapons != null && weapons.Length > 0)
                    {
                        foreach (var w in weapons) w.PerformHitCheck();
                    }
                    else
                    {
                        combat.MeleeAttack(hit.gameObject);
                    }
                    TriggerAttackAnimation();
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

        if (navAgent != null)
        {
            float slowStart = ApproachDistance * 1.5f;
            if (dst > slowStart)
            {
                navAgent.isStopped = false;
                navAgent.speed = RunSpeed;
                navAgent.SetDestination(currentTarget.transform.position);
            }
            else if (dst > ApproachDistance)
            {
                navAgent.isStopped = false;
                float t = Mathf.Clamp01((dst - ApproachDistance) / (slowStart - ApproachDistance));
                navAgent.speed = Mathf.Lerp(WalkSpeed, RunSpeed, t);
                navAgent.SetDestination(currentTarget.transform.position);
            }
            else
            {
                navAgent.isStopped = true;
                if (Time.time - lastAttackTime >= AttackCooldown)
                {
                    if (weapons != null && weapons.Length > 0)
                    {
                        foreach (var w in weapons) w.PerformHitCheck();
                    }
                    else
                    {
                        combat.MeleeAttack(currentTarget);
                    }
                    TriggerAttackAnimation();
                    lastAttackTime = Time.time;
                }
            }

            // Update animator speed parameter if available
            if (animator != null)
            {
                float speed = navAgent.velocity.magnitude;
                if (AnimatorHasParameter("Speed"))
                {
                    animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
                }
            }
        }
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

    private bool IsValidTarget(GameObject t)
    {
        if (t == null) return false;
        var hc = t.GetComponent<HeroicCombat>();
        return hc != null && hc.Health > 0;
    }

    private void StartEngaging(GameObject target)
    {
        currentTarget = target;
        if (simpleMovement != null) simpleMovement.enabled = false;
        if (navAgent != null)
        {
            originalAgentSpeed = navAgent.speed;
            navAgent.isStopped = false;
            navAgent.speed = RunSpeed;
        }
    }

    private void StopEngaging()
    {
        currentTarget = null;
        if (navAgent != null)
        {
            navAgent.isStopped = false;
            navAgent.speed = WalkSpeed;
        }
        if (simpleMovement != null)
        {
            simpleMovement.enabled = true;
            simpleMovement.PickNewDestination();
        }
    }

    private void TriggerAttackAnimation()
    {
        if (animator == null) return;
        animator.SetBool(AttackParam, true);
        if (attackResetRoutine != null) StopCoroutine(attackResetRoutine);
        attackResetRoutine = StartCoroutine(ResetAttackFlag());
    }

    private System.Collections.IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(AttackAnimDuration);
        if (animator != null) animator.SetBool(AttackParam, false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DisengageDistance);
    }
}