using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks personal enemies for a character.  Each nemesis is associated
/// with a hostility value between 0 and 1.  Hostility can increase due to
/// negative interactions and decrease over time or through positive actions.
/// When hostility reaches zero, the nemesis is removed from the list.
/// </summary>
[RequireComponent(typeof(Character))]
public class NemesisSystem : MonoBehaviour
{
    /// <summary>
    /// Dictionary mapping nemesis characters to a hostility value.  Higher
    /// values indicate deeper grudges.  Hostility influences whether the
    /// nemesis will attack on sight or harbour resentment.  See also
    /// <see cref="Profiles"/> for extended information.
    /// </summary>
    public Dictionary<Character, float> Nemeses { get; private set; } = new Dictionary<Character, float>();

    /// <summary>
    /// Detailed profiles for each nemesis.  Profiles store rank, traits,
    /// strengths, weaknesses, personality and a history of interactions.
    /// These allow for a more robust nemesis system similar to
    /// Middle‑earth: Shadow of War.  Profiles are created when a
    /// nemesis is first added.
    /// </summary>
    public Dictionary<Character, NemesisProfile> Profiles { get; private set; } = new Dictionary<Character, NemesisProfile>();

    /// <summary>
    /// Adds a character as a nemesis or increases hostility if already a
    /// nemesis.  Hostility is clamped between 0 and 1.
    /// </summary>
    public void AddNemesis(Character character, float hostility = 0.5f)
    {
        if (character == null || character == GetComponent<Character>()) return;
        if (Nemeses.ContainsKey(character))
        {
            Nemeses[character] = Mathf.Clamp01(Nemeses[character] + hostility);
        }
        else
        {
            Nemeses[character] = Mathf.Clamp01(hostility);
            // When first adding a nemesis, create a profile with base info.
            if (!Profiles.ContainsKey(character))
            {
                var profile = new NemesisProfile(character.CharacterName)
                {
                    Rank = 0,
                    Personality = "Vengeful"
                };
                Profiles[character] = profile;
            }
        }
    }

    /// <summary>
    /// Reduces hostility towards a nemesis.  If hostility drops to zero,
    /// the nemesis is forgiven and removed from the list.
    /// </summary>
    public void ReduceHostility(Character character, float amount)
    {
        if (character == null) return;
        if (Nemeses.ContainsKey(character))
        {
            Nemeses[character] = Mathf.Max(0f, Nemeses[character] - amount);
            if (Nemeses[character] <= 0f)
            {
                Nemeses.Remove(character);
            }
        }
    }

    /// <summary>
    /// Checks whether the specified character is a nemesis.
    /// </summary>
    public bool IsNemesis(Character character) => character != null && Nemeses.ContainsKey(character);

    /// <summary>
    /// Gets the hostility level towards a nemesis.  Returns 0 if the
    /// character is not a nemesis.
    /// </summary>
    public float GetHostility(Character character)
    {
        return character != null && Nemeses.TryGetValue(character, out var value) ? value : 0f;
    }
}