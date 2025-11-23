using UnityEngine;

/// <summary>
/// Simple combat AI for NPCs.  When another <see cref="Character"/>
/// enters the attack range, the NPC will use its <see cref="HeroicCombat"/>
/// component to perform a melee attack.  This script does not handle
/// movement; combine it with <see cref="SimpleNPCMovement"/> or
/// NavMeshAgent to chase targets.  You can expand this script with
/// faction, clan or relationship checks to decide whether two characters
/// should fight or cooperate.
/// </summary>
[RequireComponent(typeof(HeroicCombat))]
[RequireComponent(typeof(Character))]
public class NPCCombat : MonoBehaviour
{
    /// <summary>
    /// Maximum distance at which the NPC will attack another character.
    /// </summary>
    public float AttackRange = 2f;

    /// <summary>
    /// Time in seconds between consecutive attacks.  Prevents the NPC from
    /// attacking every frame.
    /// </summary>
    public float AttackCooldown = 1f;

    private HeroicCombat combat;
    private Character character;
    private float lastAttackTime;

    private void Awake()
    {
        combat = GetComponent<HeroicCombat>();
        character = GetComponent<Character>();
    }

    private void Update()
    {
        // Look for other characters within attack range.
        Collider[] hits = Physics.OverlapSphere(transform.position, AttackRange);
        foreach (var hit in hits)
        {
            var otherChar = hit.GetComponent<Character>();
            if (otherChar != null && otherChar != character)
            {
                // TODO: Add checks for clan, relationship or belief before attacking.
                if (Time.time - lastAttackTime >= AttackCooldown)
                {
                    combat.MeleeAttack(hit.gameObject);
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    // Visualise attack range in the editor.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}