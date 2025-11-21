using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a town's resource production, construction and villager assignments.
/// In this simplified example, the town stores resources and assigns villagers to basic tasks.
/// </summary>
public class TownAI : MonoBehaviour
{
    public string TownName;
    public List<Character> Villagers = new List<Character>();

    public int FoodStock = 0;
    public int WoodStock = 0;
    public int StoneStock = 0;

    void Update()
    {
        // Basic resource production: each villager gathers a resource per second
        foreach (Character villager in Villagers)
        {
            // Naive example: alternate between food and wood
            if (Time.frameCount % 2 == 0)
            {
                FoodStock++;
                villager.FulfillNeed(NeedType.Hunger, 0.1f);
            }
            else
            {
                WoodStock++;
                villager.FulfillNeed(NeedType.Rest, 0.05f);
            }
        }
    }
}