using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculates a character's mood based on their needs and modifiers.
/// Mood ranges from 0 (extremely unhappy) to 1 (ecstatic).
/// Low mood can trigger mental breaks; high mood can give inspirations.
/// </summary>
public class MoodSystem : MonoBehaviour
{
    public List<Need> Needs = new List<Need>();
    [Range(0, 1)]
    public float Mood = 1.0f;

    // Update is called every frame to recalculate mood.
    void Update()
    {
        float total = 0f;
        foreach (Need need in Needs)
        {
            total += need.Normalized;
        }
        if (Needs.Count > 0)
        {
            // Average of all needs defines the baseline mood.
            Mood = total / Needs.Count;
        }
        else
        {
            Mood = 1f;
        }
        // Potential place to apply trait modifiers here.
    }
}