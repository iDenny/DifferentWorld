using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents an individual in the world.  Characters track basic data
/// such as a name, family affiliation, mood and age.  Additional
/// systems like <see cref="BeliefSystem"/> and <see cref="NemesisSystem"/>
/// attach to the same GameObject to extend behaviour.  Character also
/// exposes methods to modify mood, interact with other characters and
/// respond to in‑game events.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(BeliefSystem))]
[RequireComponent(typeof(NemesisSystem))]
public class Character : MonoBehaviour
{
    /// <summary>
    /// Name displayed for this character.  Useful for UI or debugging.
    /// </summary>
    [Tooltip("Human readable name of the character.")]
    public string CharacterName = "Unnamed";

    /// <summary>
    /// Age in years.  You can change this at runtime to age characters
    /// dynamically or use it to gate certain behaviours.
    /// </summary>
    [Tooltip("Age of the character in years.")]
    public int Age = 18;

    /// <summary>
    /// Current mood level between 0 (depressed) and 1 (ecstatic).  Mood
    /// influences AI decisions and reactions.  This value is typically
    /// modified through <see cref="ModifyMood"/> rather than direct edits.
    /// </summary>
    [Range(0f, 1f)]
    [Tooltip("Current mood level between 0 (depressed) and 1 (ecstatic).")]
    public float Mood = 0.5f;

    /// <summary>
    /// Name of the family this character belongs to.  Set automatically
    /// when added to a <see cref="Family"/> via <see cref="Family.AddMember"/>
    /// but can be overridden manually for special cases.
    /// </summary>
    [Tooltip("Family name of the character.  Updated when the character is added to a Family.")]
    public string FamilyName = string.Empty;

    /// <summary>
    /// Needs list for this character (hunger, rest, etc.).  Systems can
    /// modify or read these values.  This is a simple in-object need
    /// list; you can move it to a dedicated NeedSystem later.
    /// </summary>
    public List<Need> Needs = new List<Need>();

    // Cached references to other systems on the same GameObject.
    private BeliefSystem beliefSystem;
    private NemesisSystem nemesisSystem;

    private void Awake()
    {
        // Cache component references.  These are required by the attributes at
        // the top of the class, so they will always be present.
        beliefSystem = GetComponent<BeliefSystem>();
        nemesisSystem = GetComponent<NemesisSystem>();

        // Ensure basic needs exist so other systems can call FulfillNeed.
        EnsureNeedExists(NeedType.Hunger, 1f, 0.01f);
        EnsureNeedExists(NeedType.Rest, 1f, 0.01f);
        EnsureNeedExists(NeedType.Social, 1f, 0.005f);
        EnsureNeedExists(NeedType.Recreation, 1f, 0.002f);
    }

    /// <summary>
    /// Adjusts the character's mood by a delta, taking their primary
    /// belief into account.  Belief modifiers can either boost or
    /// dampen mood changes.  Resulting mood is clamped between 0 and 1.
    /// </summary>
    /// <param name="amount">The raw change to apply to the mood.</param>
    public void ModifyMood(float amount)
    {
        float modifier = beliefSystem != null ? beliefSystem.GetBeliefModifier() : 0f;
        Mood = Mathf.Clamp01(Mood + amount + modifier);
    }

    /// <summary>
    /// Interacts with another character.  This simple example adjusts
    /// mood based on whether the other character is a nemesis and
    /// creates or reduces hostility accordingly.  Real games can
    /// expand this to include dialogues, trades, combat or other
    /// mechanics.
    /// </summary>
    /// <param name="other">The character to interact with.</param>
    public void Interact(Character other)
    {
        if (other == null || other == this) return;
        // If the other character is already a nemesis, decrease mood and
        // increase hostility.
        if (nemesisSystem != null && nemesisSystem.IsNemesis(other))
        {
            // Interaction with a nemesis tends to be unpleasant
            ModifyMood(-0.1f);
            nemesisSystem.AddNemesis(other, 0.1f);
        }
        else
        {
            // Positive interactions improve mood and may reduce hostility
            ModifyMood(0.1f);
            // Gradually forgive the other character if they were once a
            // nemesis.  This call has no effect if other isn't a nemesis.
            nemesisSystem?.ReduceHostility(other, 0.05f);
        }
    }

    /// <summary>
    /// Public helper to add a nemesis directly.  Use this when an
    /// event causes the character to harbour a grudge against someone.
    /// </summary>
    public void AddNemesis(Character other, float hostility = 0.5f)
    {
        nemesisSystem?.AddNemesis(other, hostility);
    }

    /// <summary>
    /// Returns a collection of this character's nemeses and their
    /// hostility values.  Provides a read‑only view to external callers.
    /// </summary>
    public IReadOnlyDictionary<Character, float> GetNemeses()
    {
        return nemesisSystem != null ? nemesisSystem.Nemeses : new Dictionary<Character, float>();
    }

    /// <summary>
    /// Fulfill a need by type.  If the need does not exist it will be created
    /// with sensible defaults and then fulfilled.
    /// </summary>
    public void FulfillNeed(NeedType type, float amount)
    {
        var need = Needs.Find(n => n.Type == type);
        if (need == null)
        {
            need = EnsureNeedExists(type, 1f, 0.01f);
        }
        need.Fulfill(amount);
    }

    private Need EnsureNeedExists(NeedType type, float maxValue, float decayRate)
    {
        var need = Needs.Find(n => n.Type == type);
        if (need == null)
        {
            need = new Need(type, maxValue, decayRate);
            Needs.Add(need);
        }
        return need;
    }
}