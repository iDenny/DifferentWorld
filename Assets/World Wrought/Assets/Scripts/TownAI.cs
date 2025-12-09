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

    public float ProductionInterval = 1f; // seconds
    private float productionTimer = 0f;

    void Update()
    {
        productionTimer += Time.deltaTime;
        if (productionTimer >= ProductionInterval)
        {
            productionTimer = 0f;
            foreach (Character villager in Villagers)
            {
                // Alternate between food and wood each tick
                if (Random.value > 0.5f)
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
}