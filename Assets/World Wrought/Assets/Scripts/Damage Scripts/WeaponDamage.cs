using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float amount = 20; // Amount of damage the weapon deals
    public LayerMask targetLayers = ~0;
    public float hitRadius = 0.5f; // used for overlap checks when called from animation event

    // Forward offset from weapon pivot to check hits (useful if pivot is at handle)
    public float forwardOffset = 0.7f;
    // Cone angle in degrees to filter hits so only those roughly in front of the weapon are applied
    [Range(0f, 180f)]
    public float forwardAngle = 120f;

    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;
        ApplyDamage(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var other = collision.collider;
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;
        ApplyDamage(other.gameObject);
    }

    // Returns number of hits applied
    public int PerformHitCheck()
    {
        int applied = 0;
        var attacker = GetComponentInParent<Character>();
        GameObject attackerRoot = attacker != null ? attacker.gameObject : transform.root.gameObject;

        Vector3 center1 = transform.position;
        Vector3 center2 = transform.position + transform.forward * forwardOffset;

        Collider[] hits1 = Physics.OverlapSphere(center1, hitRadius, targetLayers);
        Collider[] hits2 = Physics.OverlapSphere(center2, hitRadius, targetLayers);

        float cosThreshold = Mathf.Cos(forwardAngle * Mathf.Deg2Rad * 0.5f);

        System.Collections.Generic.HashSet<Collider> seen = new System.Collections.Generic.HashSet<Collider>();
        foreach (var hit in hits1)
        {
            if (seen.Contains(hit)) continue;
            seen.Add(hit);
            if (ShouldApplyToHit(hit, attackerRoot))
                applied += ApplyIfInFront(hit, center2, cosThreshold, attacker);
        }
        foreach (var hit in hits2)
        {
            if (seen.Contains(hit)) continue;
            seen.Add(hit);
            if (ShouldApplyToHit(hit, attackerRoot))
                applied += ApplyIfInFront(hit, center2, cosThreshold, attacker);
        }

        Debug.Log($"WeaponDamage: PerformHitCheck applied={applied} from {gameObject.name}");
        return applied;
    }

    private bool ShouldApplyToHit(Collider hit, GameObject attackerRoot)
    {
        if (hit == null) return false;
        if (hit.gameObject == gameObject) return false;
        if (hit.transform.root != null && hit.transform.root.gameObject == attackerRoot) return false;
        return true;
    }

    // returns 1 if applied, 0 otherwise
    private int ApplyIfInFront(Collider hit, Vector3 forwardCenter, float cosThreshold, Character attacker)
    {
        // If the hit is an NPC/character or has HeroicCombat, apply damage regardless of cone
        var charComp = hit.GetComponentInParent<Character>();
        var heroic = hit.GetComponentInParent<HeroicCombat>();
        if (charComp != null || heroic != null)
        {
            ApplyDamage(hit.gameObject);
            return 1;
        }

        Vector3 dir = (hit.transform.position - forwardCenter).normalized;
        float dot = Vector3.Dot(transform.forward, dir);
        if (dot < cosThreshold)
        {
            if (attacker != null)
            {
                var attackerForward = attacker.transform.forward;
                float dot2 = Vector3.Dot(attackerForward, dir);
                if (dot2 < cosThreshold)
                {
                    Debug.Log($"WeaponDamage: Ignored hit {hit.gameObject.name} (dot={dot:F2}, dotAttacker={dot2:F2})");
                    return 0;
                }
            }
            else
            {
                Debug.Log($"WeaponDamage: Ignored hit {hit.gameObject.name} (dot={dot:F2})");
                return 0;
            }
        }

        ApplyDamage(hit.gameObject);
        return 1;
    }

    public void ApplyDamage(GameObject target)
    {
        if (target == null) return;

        var attackerChar = GetComponentInParent<Character>();
        var targetChar = target.GetComponentInParent<Character>();
        if (attackerChar != null && targetChar != null && attackerChar == targetChar)
            return;

        Debug.Log($"WeaponDamage: ApplyDamage trying target={target.name} attacker={(attackerChar!=null?attackerChar.CharacterName:"null")} amount={amount}");

        var combat = target.GetComponentInParent<HeroicCombat>();
        if (combat != null)
        {
            if (attackerChar != null)
            {
                combat.TakeDamage(Mathf.CeilToInt(amount), attackerChar);
            }
            else
            {
                combat.TakeDamage(Mathf.CeilToInt(amount));
            }
            return;
        }

        var parents = target.GetComponentsInParent<MonoBehaviour>(true);
        foreach (var comp in parents)
        {
            if (comp is IDamageable idam)
            {
                idam.TakeDamage(Mathf.CeilToInt(amount));
                return;
            }
        }

        Debug.Log($"WeaponDamage: No damage target found on {target.name}");
    }
}
