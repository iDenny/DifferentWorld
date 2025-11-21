using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all living beings in the world. Contains needs, traits, mood and family references.
/// </summary>
[RequireComponent(typeof(MoodSystem))]
public class Character : MonoBehaviour
{
    public string CharacterName;
    public string FamilyName;

    // List of needs this character has.
    public List<Need> Needs = new List<Need>();
    // Traits attached to this character.
    public List<Trait> Traits = new List<Trait>();

    private MoodSystem moodSystem;

    void Awake()
    {
        moodSystem = GetComponent<MoodSystem>();
        moodSystem.Needs = Needs;

        // Apply trait effects at creation.
        foreach (Trait trait in Traits)
        {
            trait.Apply(this);
        }
    }

    void Update()
    {
        // Update each need; pass in deltaTime
        float dt = Time.deltaTime;
        foreach (Need need in Needs)
        {
            need.Update(dt);
        }
    }

    /// <summary>
    /// Fulfill a specific need by name.
    /// </summary>
    public void FulfillNeed(NeedType type, float amount)
    {
        foreach (Need need in Needs)
        {
            if (need.Type == type)
            {
                need.Fulfill(amount);
                break;
            }
        }
    }

    /// <summary>
    /// Get the current mood (normalized 0–1) from the mood system.
    /// </summary>
    public float GetMood()
    {
        return moodSystem.Mood;
    }
}