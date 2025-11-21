using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
[DisallowMultipleComponent]
public class ColonySystem : MonoBehaviour
{
    [Header("Core Data")]
    [SerializeField]
    private List<Character> citizens = new List<Character>();

    [SerializeField]
    private List<Family> families = new List<Family>();

    // Dictionaries are not serialized by Unity by default; keep internal.
    private Dictionary<string, int> resources = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

    [Tooltip("Settlement tier: 0=village, 1=town, 2=city...")]
    [SerializeField]
    private int settlementTier = 0;

    // Read-only accessors
    public IReadOnlyList<Character> Citizens => citizens.AsReadOnly();
    public IReadOnlyList<Family> Families => families.AsReadOnly();
    public IReadOnlyDictionary<string, int> Resources => new ReadOnlyDictionary<string, int>(resources);
    public int SettlementTier => settlementTier;

    // Events for other systems/UI
    public event Action<string, int> OnResourceChanged;
    public event Action<int> OnSettlementUpgraded;
    public event Action<Character> OnCitizenAdded;
    public event Action<Character> OnCitizenRemoved;

    /// <summary>
    /// Adds a character to the colony and optionally assigns them a family.
    /// </summary>
    public bool AddCitizen(Character character, Family family = null)
    {
        if (character == null)
        {
            Debug.LogWarning("Attempted to add null character to colony.");
            return false;
        }
        if (citizens.Contains(character))
        {
            Debug.LogWarning("Character already present in colony.");
            return false;
        }

        citizens.Add(character);

        if (family != null)
        {
            if (!families.Contains(family))
            {
                families.Add(family);
            }
            family.AddMember(character);
        }

        OnCitizenAdded?.Invoke(character);
        return true;
    }

    /// <summary>
    /// Removes a citizen from the colony and from any family they belong to.
    /// </summary>
    public bool RemoveCitizen(Character character)
    {
        if (character == null) return false;
        if (!citizens.Remove(character)) return false;

        foreach (var fam in families)
        {
            fam.RemoveMember(character);
        }

        OnCitizenRemoved?.Invoke(character);
        return true;
    }

    /// <summary>
    /// Adds (or removes if amount negative) a quantity of a resource.
    /// If resulting amount is <= 0 the resource key is removed.
    /// </summary>
    public void AddResource(string resource, int amount)
    {
        if (string.IsNullOrWhiteSpace(resource))
        {
            Debug.LogWarning("Invalid resource name.");
            return;
        }
        if (amount == 0) return;

        resources.TryGetValue(resource, out int current);
        int updated = current + amount;

        if (updated <= 0)
        {
            if (resources.Remove(resource))
            {
                OnResourceChanged?.Invoke(resource, 0);
            }
        }
        else
        {
            resources[resource] = updated;
            OnResourceChanged?.Invoke(resource, updated);
        }
    }

    /// <summary>
    /// Returns the amount of a resource.  Returns zero if not present.
    /// </summary>
    public int GetResourceAmount(string resource)
    {
        if (string.IsNullOrWhiteSpace(resource)) return 0;
        return resources.TryGetValue(resource, out int value) ? value : 0;
    }

    /// <summary>
    /// Atomically checks and consumes the provided cost dictionary.
    /// Returns true and consumes resources on success; returns false
    /// and does not change resources on failure.
    /// </summary>
    public bool TryConsumeResources(IDictionary<string, int> cost)
    {
        if (cost == null) return false;

        foreach (var kvp in cost)
        {
            if (string.IsNullOrWhiteSpace(kvp.Key) || kvp.Value < 0)
            {
                Debug.LogWarning("Invalid cost entry when trying to consume resources.");
                return false;
            }
            if (GetResourceAmount(kvp.Key) < kvp.Value)
            {
                return false; // not enough
            }
        }

        // All checks passed; consume
        foreach (var kvp in cost)
        {
            AddResource(kvp.Key, -kvp.Value);
        }
        return true;
    }

    /// <summary>
    /// Attempts to upgrade the settlement tier. Uses TryConsumeResources so
    /// no partial resources are spent on failure.
    /// </summary>
    public bool TryUpgradeSettlement()
    {
        int nextTier = settlementTier + 1;
        var required = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "wood", 10 * nextTier },
            { "stone", 5 * nextTier }
        };

        if (!TryConsumeResources(required))
        {
            Debug.Log("Not enough resources to upgrade settlement.");
            return false;
        }

        settlementTier = nextTier;
        Debug.Log($"Settlement upgraded to tier {settlementTier}!");
        OnSettlementUpgraded?.Invoke(settlementTier);
        return true;
    }
}