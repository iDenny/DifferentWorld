using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float amount = 20; // Amount of damage the weapon deals
    public LayerMask targetLayers = ~0;
    public float hitRadius = 0.5f; // used for overlap checks when called from animation event

    private void OnTriggerEnter(Collider other)
    {
        // Only apply damage to layers the weapon should hit
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;
        Debug.Log($"WeaponDamage: OnTriggerEnter hit {other.gameObject.name}");
        ApplyDamage(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Support non-trigger colliders (player may have non-trigger weapon collider)
        var other = collision.collider;
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;
        Debug.Log($"WeaponDamage: OnCollisionEnter hit {other.gameObject.name}");
        ApplyDamage(other.gameObject);
    }

    // Public method intended to be called from an animation event on the weapon/attacker.
    // This performs an overlap sphere check around the weapon and applies damage to any valid targets.
    public void PerformHitCheck()
    {
        // Determine attacker root so we can ignore self-hits
        var attacker = GetComponentInParent<Character>();
        GameObject attackerRoot = attacker != null ? attacker.gameObject : transform.root.gameObject;

        Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius, targetLayers);
        Debug.Log($"WeaponDamage: PerformHitCheck found {hits.Length} hits around {gameObject.name}");
        foreach (var hit in hits)
        {
            // Skip this weapon's own collider
            if (hit.gameObject == gameObject) continue;
            // Skip any collider that belongs to the attacker's root (avoids hitting self)
            if (hit.transform.root != null && hit.transform.root.gameObject == attackerRoot) continue;
            Debug.Log($"WeaponDamage: PerformHitCheck applying to {hit.gameObject.name}");
            ApplyDamage(hit.gameObject);
        }
    }

    public void ApplyDamage(GameObject target)
    {
        if (target == null) return;

        // Skip applying damage to our own root
        var attackerChar = GetComponentInParent<Character>();
        var targetChar = target.GetComponentInParent<Character>();
        if (attackerChar != null && targetChar != null && attackerChar == targetChar)
            return;

        // Prefer HeroicCombat
        var combat = target.GetComponentInParent<HeroicCombat>();
        if (combat != null)
        {
            if (attackerChar != null)
            {
                Debug.Log($"WeaponDamage: Applying {amount} from {attackerChar.CharacterName} to {target.name} via HeroicCombat");
                combat.TakeDamage(Mathf.CeilToInt(amount), attackerChar);
            }
            else
            {
                Debug.Log($"WeaponDamage: Applying {amount} to {target.name} via HeroicCombat (no attacker info)");
                combat.TakeDamage(Mathf.CeilToInt(amount));
            }
            return;
        }

        // Try IDamageable by scanning parent components (GetComponentInParent<T>() does not reliably work for interfaces)
        var parents = target.GetComponentsInParent<MonoBehaviour>(true);
        foreach (var comp in parents)
        {
            if (comp is IDamageable idam)
            {
                Debug.Log($"WeaponDamage: Applying {amount} to {target.name} via IDamageable on {comp.GetType().Name}");
                idam.TakeDamage(Mathf.CeilToInt(amount));
                return;
            }
        }

        Debug.Log($"WeaponDamage: No damageable found on {target.name}");
    }
}
