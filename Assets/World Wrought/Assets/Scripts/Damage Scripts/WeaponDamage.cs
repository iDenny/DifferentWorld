using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float amount = 20; // Amount of damage the weapon deals

    public void ApplyDamage(GameObject target)
    {
        if (target == null) return;
        // Prefer IDamageable so any weapon implementation can be supported
        var damageable = target.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(Mathf.CeilToInt(amount));
            return;
        }
        // Backwards compatibility: try HeroicCombat specifically
        var combat = target.GetComponentInParent<HeroicCombat>();
        if (combat != null)
        {
            combat.TakeDamage(Mathf.CeilToInt(amount));
        }
    }
}
