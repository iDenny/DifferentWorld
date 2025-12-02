using UnityEngine;
using System.Collections;

/// <summary>
/// Provides basic combat functionality without referencing any existing
/// franchises.  Characters with this component can perform heavy melee
/// attacks with swords, hammers or other close‑range weapons and fire
/// simple ranged attacks.  This is a lightweight combat model intended
/// as a starting point for your own unique game mechanics.
/// </summary>
[RequireComponent(typeof(Character))]
public class HeroicCombat : MonoBehaviour
{
    /// <summary>
    /// Maximum hit points for the character.
    /// </summary>
    public int MaxHealth = 100;

    /// <summary>
    /// Current hit points for the character.
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

    // Optional UI and animator references
    private UIHealthBar uiHealthBar;
    private Animator animator;
    private Collider objectCollider;

    private void Awake()
    {
        Health = Mathf.Clamp(Health, 0, MaxHealth);
        // Try to find a UI health bar in children
        uiHealthBar = GetComponentInChildren<UIHealthBar>();
        animator = GetComponent<Animator>();
        objectCollider = GetComponent<Collider>();
        UpdateHealthUI();
    }

    /// <summary>
    /// Melee attack: accept any collider or child object and resolve the
    /// HeroicCombat component on the target or its parents.
    /// </summary>
    public void MeleeAttack(GameObject target)
    {
        if (target == null) return;
        var combat = target.GetComponentInParent<HeroicCombat>();
        if (combat != null && combat != this)
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
        var combat = target.GetComponentInParent<HeroicCombat>();
        if (combat != null && combat != this)
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
        if (amount <= 0) return;
        Health -= amount;
        UpdateHealthUI();
        if (Health <= 0)
        {
            Health = 0;
            Die(); // immediate death (no delay)
        }
    }

    private void UpdateHealthUI()
    {
        if (uiHealthBar != null && MaxHealth > 0)
        {
            float pct = Mathf.Clamp01((float)Health / (float)MaxHealth);
            uiHealthBar.SetHealthBarPercentage(pct);
        }
    }

    protected virtual void Die()
    {
        // Trigger death animation if available
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        if (uiHealthBar != null)
        {
            uiHealthBar.gameObject.SetActive(false);
        }

        var scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script == this) continue;
            script.enabled = false;
        }

        if (objectCollider != null)
        {
            objectCollider.enabled = false;
        }

        // Deactivate immediately so the dead object no longer participates
        gameObject.SetActive(false);
    }
}