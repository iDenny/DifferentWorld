using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Central controller for setting up and managing the living world.  This
/// manager is responsible for spawning NPCs at the start of the game,
/// registering them with the colony system, and maintaining lists of
/// citizens, companions and enemies.  Over time it can also trigger
/// settlement upgrades, evaluate companion loyalty and react to events.
/// Attach this to an empty GameObject called GameWorldManager in your
/// scene.  Assign the NPC prefab, spawn points and colony system in the
/// Inspector before pressing play.
/// </summary>
public class GameWorldManager : MonoBehaviour
{
    /// <summary>
    /// Prefab of the NPC to spawn.  This should already have Character,
    /// NPCController, SimpleNPCMovement, RelationshipManager, NemesisSystem
    /// and other required components attached.  Assign this in the Inspector.
    /// </summary>
    public GameObject NpcPrefab;

    /// <summary>
    /// Number of NPCs to spawn at the start of the game.
    /// </summary>
    public int InitialCitizenCount = 5;

    /// <summary>
    /// List of spawn positions for NPCs.  Populate this with empty
    /// GameObjects placed around your map.  NPCs will be randomly assigned
    /// to these positions on creation.
    /// </summary>
    public List<Transform> NpcSpawnPoints = new List<Transform>();

    /// <summary>
    /// Reference to the colony system.  Assign your ColonyManager object
    /// here or find it at runtime.
    /// </summary>
    public ColonySystem Colony;

    private List<Character> citizens = new List<Character>();

    private void Start()
    {
        // Find the colony system if not assigned in the Inspector.
        if (Colony == null)
        {
            Colony = FindObjectOfType<ColonySystem>();
        }
        // Spawn initial citizens.
        for (int i = 0; i < InitialCitizenCount; i++)
        {
            SpawnCitizen();
        }
    }

    /// <summary>
    /// Spawns a single citizen NPC at a random spawn point and registers
    /// them with the colony system.  The new NPC's Character component is
    /// returned for further configuration if needed.
    /// </summary>
    public Character SpawnCitizen()
    {
        if (NpcPrefab == null)
        {
            Debug.LogError("GameWorldManager: No NPC prefab assigned.");
            return null;
        }
        // Determine spawn location.
        Vector3 spawnPos = transform.position;
        if (NpcSpawnPoints != null && NpcSpawnPoints.Count > 0)
        {
            int idx = Random.Range(0, NpcSpawnPoints.Count);
            spawnPos = NpcSpawnPoints[idx].position;
        }
        GameObject npcObj = Instantiate(NpcPrefab, spawnPos, Quaternion.identity);
        Character character = npcObj.GetComponent<Character>();
        if (character != null)
        {
            citizens.Add(character);
            if (Colony != null)
            {
                Colony.AddCitizen(character);
            }
        }
        return character;
    }

    private void Update()
    {
        // As a basic example, periodically try to upgrade the settlement
        // whenever resources are sufficient.  In a full implementation you
        // might check conditions less often or tie this to UI events.
        if (Colony != null)
        {
            Colony.TryUpgradeSettlement();
        }
    }
}