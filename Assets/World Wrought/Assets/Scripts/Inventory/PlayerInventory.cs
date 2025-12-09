using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerControl))]
public class PlayerInventory : MonoBehaviour
{
    public Transform WeaponHand; // parent for equipped weapon

    private GameObject equippedWeapon;
    private List<GameObject> carriedWeapons = new List<GameObject>();

    public void PickupWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null) return;
        // If pickup is a world instance, remove its pickup component and attach to hand
        var pickup = weaponPrefab.GetComponent<WeaponPickup>();
        GameObject instance = weaponPrefab;
        if (pickup != null)
        {
            // If the weapon is a pickup placed in world, use that object
            instance = weaponPrefab;
            Destroy(pickup);
        }

        // Parent under hand and reset transform
        if (WeaponHand != null)
        {
            instance.transform.SetParent(WeaponHand, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
        }

        var wd = instance.GetComponentInChildren<WeaponDamage>(true);
        if (wd != null)
        {
            carriedWeapons.Add(instance);
            EquipWeapon(instance);
        }
    }

    public void EquipWeapon(GameObject weapon)
    {
        if (weapon == null) return;
        if (equippedWeapon != null)
        {
            UnequipWeapon();
        }
        equippedWeapon = weapon;
        // enable weapon colliders
        var colliders = equippedWeapon.GetComponentsInChildren<Collider>(true);
        foreach (var c in colliders) c.enabled = true;
    }

    public void UnequipWeapon()
    {
        if (equippedWeapon == null) return;
        var colliders = equippedWeapon.GetComponentsInChildren<Collider>(true);
        foreach (var c in colliders) c.enabled = false;
        equippedWeapon = null;
    }

    public void DropEquipped()
    {
        if (equippedWeapon == null) return;
        equippedWeapon.transform.SetParent(null);
        var rb = equippedWeapon.GetComponent<Rigidbody>();
        if (rb == null) rb = equippedWeapon.AddComponent<Rigidbody>();
        rb.AddForce(transform.forward * 4f + Vector3.up * 1f, ForceMode.VelocityChange);
        equippedWeapon = null;
    }
}
