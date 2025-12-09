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
public class HeroicCombat : MonoBehaviour, IDamageable
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

    // Optional: time to wait for death animation before deactivation
    public float DeathDelay = 1.0f;

    // Track the last attacker so we can record promotions or history on death
    private Character lastAttacker;

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
            var attackerChar = GetComponent<Character>();
            combat.TakeDamage(MeleeDamage, attackerChar);
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
            var attackerChar = GetComponent<Character>();
            combat.TakeDamage(RangedDamage, attackerChar);
        }
    }

    /// <summary>
    /// Applies damage to this character.  If health falls to zero or
    /// below, the character dies.
    /// </summary>
    public void TakeDamage(int amount)
    {
        TakeDamage(amount, null);
    }

    /// <summary>
    /// New overload that accepts the source of the damage so we can record nemesis interactions
    /// </summary>
    public void TakeDamage(int amount, Character source)
    {
        if (amount <= 0) return;

        // Record last attacker for death processing
        if (source != null)
            lastAttacker = source;

        Debug.Log($"HeroicCombat: {gameObject.name} taking {amount} damage from {(source!=null?source.CharacterName:"unknown")}");

        Health -= amount;
        UpdateHealthUI();

        // If we have a NemesisSystem on this character, record the interaction
        var myChar = GetComponent<Character>();
        var nem = myChar != null ? myChar.GetComponent<NemesisSystem>() : null;
        if (nem != null && source != null)
        {
            float hostilityDelta = Mathf.Clamp01((float)amount / Mathf.Max(1, MaxHealth));
            nem.RecordInteraction(source, $"Hit by {source.CharacterName} for {amount} HP", hostilityDelta);
        }

        if (Health <= 0)
        {
            Health = 0;
            // Play death animation then deactivate after DeathDelay
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        if (uiHealthBar != null)
        {
            uiHealthBar.gameObject.SetActive(false);
        }

        // Disable other scripts to stop behaviour while death animation plays
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

        // If someone killed this character, promote them in their NemesisSystem
        if (lastAttacker != null)
        {
            var attackerNem = lastAttacker.GetComponent<NemesisSystem>();
            if (attackerNem != null)
            {
                attackerNem.PromoteNemesis(GetComponent<Character>(), 1);
            }
        }

        // Wait for death animation or fallback delay
        float wait = DeathDelay;
        if (animator != null)
        {
            // If there's a death animation clip length available we could query it, but keep simple
            // Animator may be in a controller with transitions; use DeathDelay as a reasonable default
        }

        yield return new WaitForSeconds(wait);

        // Deactivate immediately so the dead object no longer participates
        gameObject.SetActive(false);
    }

    private void UpdateHealthUI()
    {
        if (uiHealthBar != null && MaxHealth > 0)
        {
            float pct = Mathf.Clamp01((float)Health / (float)MaxHealth);
            uiHealthBar.SetHealthBarPercentage(pct);
        }
    }
}