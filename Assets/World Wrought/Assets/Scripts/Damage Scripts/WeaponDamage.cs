using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float amount = 20; // Amount of damage the weapon deals

    public void ApplyDamage(GameObject target)
    {
        Health health = target.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(amount);
        }
    }
}
