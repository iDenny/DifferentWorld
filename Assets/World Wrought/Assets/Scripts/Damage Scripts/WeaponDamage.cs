using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float amount = 20; // Amount of damage the weapon deals

    public void ApplyDamage(GameObject target)
    {
        if (target == null) return;
        // Look up HeroicCombat on the hit object or its parents so child colliders work
        var combat = target.GetComponentInParent<HeroicCombat>();
        if (combat != null)
        {
            combat.TakeDamage(Mathf.CeilToInt(amount));
        }
    }
}
