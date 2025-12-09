using System.Collections.Generic;
using UnityEngine;

public class MoodSystem : MonoBehaviour
{
    private Character character;
    [Range(0, 1)]
    public float Mood = 1.0f;

    private void Awake()
    {
        character = GetComponent<Character>();
        // Remove internal Needs in favor of Character's Needs
    }

    void Update()
    {
        if (character == null)
        {
            return;
        }

        var needs = character.Needs;
        if (needs == null || needs.Count == 0)
        {
            Mood = 1f;
            return;
        }

        float total = 0f;
        foreach (var need in needs)
        {
            total += need.Normalized;
        }
        Mood = total / needs.Count;
    }
}