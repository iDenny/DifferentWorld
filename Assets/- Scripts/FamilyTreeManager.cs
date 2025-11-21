using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all families in the world. Allows creating new dynasties and tracking heirs.
/// </summary>
public class FamilyTreeManager : MonoBehaviour
{
    // Each family is keyed by family name.
    public Dictionary<string, List<FamilyMember>> families = new Dictionary<string, List<FamilyMember>>();

    /// <summary>
    /// Add a new character to a family. If the family does not exist, it is created.
    /// </summary>
    public void RegisterCharacter(Character character)
    {
        if (string.IsNullOrEmpty(character.FamilyName)) return;
        if (!families.ContainsKey(character.FamilyName))
        {
            families[character.FamilyName] = new List<FamilyMember>();
        }
        var member = new FamilyMember(character);
        families[character.FamilyName].Add(member);
    }

    /// <summary>
    /// Get the current heir for a family based on simple primogeniture (oldest living child).
    /// </summary>
    public Character GetHeir(string familyName)
    {
        if (!families.ContainsKey(familyName)) return null;
        FamilyMember heir = null;
        foreach (var member in families[familyName])
        {
            if (member.Character == null) continue;
            if (heir == null || member.Character.transform.position.x < heir.Character.transform.position.x)
            {
                // For demo purposes, pick based on some arbitrary property; you should implement proper age/lineage logic.
                heir = member;
            }
        }
        return heir?.Character;
    }
}