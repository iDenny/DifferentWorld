using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks interpersonal relationships for a character.  Each relationship
/// has a score between -1 (hatred) and 1 (friendship).  This simple
/// manager allows NPCs to form friendships, rivalries and alliances over
/// time.  Use this alongside <see cref="DiplomacyManager"/> for faction
/// relations and <see cref="NemesisSystem"/> for personal grudges.
/// </summary>
[RequireComponent(typeof(Character))]
public class RelationshipManager : MonoBehaviour
{
    private Character self;
    /// <summary>
    /// Relationships with other characters.  The float value ranges from
    /// -1 (enemies) to 1 (allies).  Neutral relationships default to 0.
    /// </summary>
    private readonly Dictionary<Character, float> relationships = new Dictionary<Character, float>();

    private void Awake()
    {
        self = GetComponent<Character>();
    }

    /// <summary>
    /// Adjusts the relationship score with another character by the given
    /// delta.  Scores are clamped between -1 and 1.  A positive delta
    /// improves the relationship, negative harms it.
    /// </summary>
    public void ModifyRelationship(Character other, float delta)
    {
        if (other == null || other == self) return;
        if (!relationships.ContainsKey(other))
        {
            relationships[other] = 0f;
        }
        relationships[other] = Mathf.Clamp(relationships[other] + delta, -1f, 1f);
    }

    /// <summary>
    /// Returns the current relationship score with another character.  If
    /// no relationship is recorded, returns 0 (neutral).
    /// </summary>
    public float GetRelationship(Character other)
    {
        return other != null && relationships.TryGetValue(other, out float value) ? value : 0f;
    }
}