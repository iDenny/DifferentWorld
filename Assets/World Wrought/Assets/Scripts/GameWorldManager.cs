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

    // Sample name lists and family names for randomization
    private static readonly string[] FirstNames = { "Aldric", "Borin", "Celia", "Darya", "Edwin", "Fara", "Galen", "Hilda" };
    private static readonly string[] FamilyNames = { "Stone", "Raven", "Iron", "Green", "Storm", "Black" };
    private static readonly string[] NemesisBackgroundTraits = { "Vengeful", "Cowardly", "Strategist", "Berserker" };

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
            // Assign random name and family
            character.CharacterName = FirstNames[Random.Range(0, FirstNames.Length)] + " " + Random.Range(1, 9999);
            var famName = FamilyNames[Random.Range(0, FamilyNames.Length)];
            character.FamilyName = famName;

            // Randomize combat stats so NPCs differ from prefab
            var hc = npcObj.GetComponent<HeroicCombat>();
            if (hc != null)
            {
                hc.MaxHealth = Random.Range(60, 151);
                hc.Health = hc.MaxHealth;
                hc.MeleeDamage = Random.Range(8, 36);
                hc.RangedDamage = Random.Range(6, 26);
                hc.DeathDelay = Random.Range(0.8f, 2.0f);
            }

            // Randomize movement speeds
            var npcCombat = npcObj.GetComponent<NPCCombat>();
            var agent = npcObj.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (npcCombat != null)
            {
                npcCombat.WalkSpeed = Random.Range(1.2f, 2.5f);
                npcCombat.RunSpeed = Random.Range(4f, 7f);
                npcCombat.ApproachDistance = Random.Range(1.2f, 2.5f);
            }
            if (agent != null)
            {
                agent.speed = npcCombat != null ? npcCombat.WalkSpeed : Random.Range(1.2f, 2.5f);
                agent.acceleration = Random.Range(4f, 12f);
            }

            // Seed a NemesisProfile trait for background
            var nem = character.GetComponent<NemesisSystem>();
            if (nem != null)
            {
                var trait = NemesisBackgroundTraits[Random.Range(0, NemesisBackgroundTraits.Length)];
                if (nem.Profiles != null)
                {
                    // create a profile keyed to self to hold background trait
                    nem.Profiles[character] = new NemesisProfile(character.CharacterName) { Personality = trait };
                }
            }

            citizens.Add(character);
            if (Colony != null)
            {
                Colony.AddCitizen(character);
            }

            // Optionally keep NPCs mostly stationary by disabling SimpleNPCMovement 70% of the time
            var simple = npcObj.GetComponent<SimpleNPCMovement>();
            if (simple != null)
            {
                if (Random.value > 0.7f)
                {
                    simple.enabled = false;
                }
                else
                {
                    // small wandering radius for patrols
                    simple.UseRandomWander = true;
                    simple.WanderRadius = Random.Range(1f, 6f);
                    simple.MinIdleTime = 1f;
                    simple.MaxIdleTime = 5f;
                }
            }

            // Add FriendlyNPC behaviour for a subset
            if (Random.value > 0.6f)
            {
                var friendly = npcObj.AddComponent<FriendlyNPC>();
                friendly.Setup(character);
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