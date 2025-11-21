using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a simple colony or kingdom.  This system tracks citizens,
/// families, resources and upgrades.  Over time you can grow from a
/// village into a city by adding new characters, constructing buildings
/// and unlocking technologies.  Diplomacy with other factions can be
/// handled via <see cref="DiplomacyManager"/> while social AI is driven
/// by <see cref="NPCController"/>.  This script provides a foundation for
/// the “Reincarnated as a Slime” kingdom‑building loop described in the
/// game concept.
/// </summary>
public class ColonySystem : MonoBehaviour
{
    /// <summary>
    /// List of characters living in the colony.  Citizens can be added
    /// manually or through recruitment events.  Attach <see cref="NPCController"/>
    /// and other systems to each citizen for full behaviour.
    /// </summary>
    public List<Character> Citizens = new();

    /// <summary>
    /// Families present in the colony.  Families group characters and
    /// maintain lineage.  Use <see cref="Family.AddMember"/> to assign
    /// characters to families.
    /// </summary>
    public List<Family> Families = new();

    /// <summary>
    /// Simple resource ledger.  Keys are resource names (e.g. wood, stone,
    /// food), values are amounts.  Expand this into a proper economy as
    /// needed.
    /// </summary>
    public Dictionary<string, int> Resources = new();

    /// <summary>
    /// Current settlement tier: 0=village, 1=town, 2=city, 3=nation, 4=federation.
    /// Upgrading your tier can unlock new buildings and mechanics.
    /// </summary>
    public int SettlementTier = 0;

    /// <summary>
    /// Adds a character to the colony and optionally assigns them a family.
    /// </summary>
    public void AddCitizen(Character character, Family family = null)
    {
        if (character == null || Citizens.Contains(character)) return;
        Citizens.Add(character);
        if (family != null)
        {
            if (!Families.Contains(family))
            {
                Families.Add(family);
            }
            family.AddMember(character);
        }
    }

    /// <summary>
    /// Adds a quantity of a resource to the colony.  If the resource
    /// doesn’t exist yet, it is created.  Negative values remove
    /// resources.
    /// </summary>
    public void AddResource(string resource, int amount)
    {
        if (string.IsNullOrEmpty(resource)) return;
        if (!Resources.ContainsKey(resource))
        {
            Resources[resource] = 0;
        }
        Resources[resource] += amount;
    }

    /// <summary>
    /// Attempts to upgrade the settlement tier.  Upgrades consume
    /// resources; this method checks requirements and applies the change.
    /// </summary>
    public bool TryUpgradeSettlement()
    {
        // Example requirements: each tier requires double the previous amount of wood and stone.
        int requiredWood = 10 * (SettlementTier + 1);
        int requiredStone = 5 * (SettlementTier + 1);
        if (GetResourceAmount("wood") >= requiredWood && GetResourceAmount("stone") >= requiredStone)
        {
            AddResource("wood", -requiredWood);
            AddResource("stone", -requiredStone);
            SettlementTier++;
            Debug.Log($"Settlement upgraded to tier {SettlementTier}!");
            return true;
        }
        Debug.Log("Not enough resources to upgrade settlement.");
        return false;
    }

    /// <summary>
    /// Returns the amount of a resource.  Returns zero if not present.
    /// </summary>
    public int GetResourceAmount(string resource)
    {
        return Resources.TryGetValue(resource, out int value) ? value : 0;
    }
}