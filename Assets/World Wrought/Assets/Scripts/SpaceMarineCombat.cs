using UnityEngine;

/// <summary>
/// Provides basic combat functionality inspired by Warhammer 40K's
/// Space Marines.  Characters with this component can perform heavy
/// melee attacks with chainswords or hammers and fire ranged weapons like
/// bolters or plasma rifles.  This is a highly simplified combat model
/// intended as a starting point for more elaborate systems.
/// </summary>
[RequireComponent(typeof(Character))]
public class SpaceMarineCombat : MonoBehaviour
{
    /// <summary>
    /// Hit points for the character.  When this reaches zero the
    /// character is considered dead.
    /// </summary>
    public int Health = 100;

    /// <summary>
    /// Base damage for melee attacks.  This can be modified by weapon
    /// types or traits.
    /// </summary>
    public int MeleeDamage = 25;

    /// <summary>
    /// Base damage for ranged attacks.  This can be modified by weapon
    /// upgrades or ammunition types.
    /// </summary>
    public int RangedDamage = 15;

    /// <summary>
    /// Triggers a melee attack against a target.  In a real game this
    /// would involve animations, hit detection and feedback.  Here we
    /// simply subtract health from the target if they have a combat
    /// component.
    /// </summary>
    public void MeleeAttack(GameObject target)
    {
        if (target == null) return;
        var combat = target.GetComponent<SpaceMarineCombat>();
        if (combat != null)
        {
            combat.TakeDamage(MeleeDamage);
        }
    }

    /// <summary>
    /// Triggers a ranged attack against a target.  This stub functions
    /// similarly to melee; in a full implementation you would spawn
    /// projectiles and handle ballistics.
    /// </summary>
    public void Shoot(GameObject target)
    {
        if (target == null) return;
        var combat = target.GetComponent<SpaceMarineCombat>();
        if (combat != null)
        {
            combat.TakeDamage(RangedDamage);
        }
    }

    /// <summary>
    /// Applies damage to this character.  If health falls to zero or
    /// below, the character dies.
    /// </summary>
    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Handles the character's death.  Override this to add death
    /// animations or respawn logic.
    /// </summary>
    protected virtual void Die()
    {
        Debug.Log($"{GetComponent<Character>().CharacterName} has died.");
        // Disable character components or trigger a death event here.
        gameObject.SetActive(false);
    }
}