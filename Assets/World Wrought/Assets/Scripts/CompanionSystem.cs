using UnityEngine;

/// <summary>
/// Manages a companion character that follows and supports the player.  A
/// companion has loyalty and personal stats which can change through
/// actions and dialogue.  Companions may leave, betray or defend the
/// player based on their loyalty and history.  This system is intended to
/// be combined with <see cref="NPCController"/> for schedules and
/// <see cref="Character"/> for basic identity.
/// </summary>
[RequireComponent(typeof(Character))]
public class CompanionSystem : MonoBehaviour
{
    /// <summary>
    /// Current loyalty level between 0 (disloyal) and 1 (devoted).  Loyalty
    /// influences whether a companion follows orders, defends the player or
    /// considers betraying them.  Modify loyalty through dialogue or
    /// events.
    /// </summary>
    [Range(0f, 1f)]
    public float Loyalty = 0.5f;

    /// <summary>
    /// Experience level of the companion.  Increases through combat or
    /// training.  Higher levels unlock new abilities or titles.
    /// </summary>
    public int Level = 1;

    /// <summary>
    /// Title reflecting the companion's standing (e.g. Squire, Knight).
    /// Updated as they level up or complete quests.
    /// </summary>
    public string Title = "Follower";

    /// <summary>
    /// Determines whether the companion is currently following the player.
    /// Toggle this to allow the companion to take on independent jobs.
    /// </summary>
    public bool IsFollowing = true;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    /// <summary>
    /// Adjusts loyalty by the specified amount.  Loyalty is clamped
    /// between 0 and 1.  Negative values represent loyalty loss while
    /// positive values represent gains.
    /// </summary>
    public void ModifyLoyalty(float amount)
    {
        Loyalty = Mathf.Clamp01(Loyalty + amount);
    }

    /// <summary>
    /// Call this to handle a betrayal check.  If loyalty drops below a
    /// threshold, the companion may decide to betray or defect.  This
    /// example simply logs the decision; in a full implementation you
    /// would switch allegiances or trigger an event.
    /// </summary>
    public void EvaluateLoyalty()
    {
        if (Loyalty <= 0.1f)
        {
            Debug.Log($"{character.CharacterName} is considering betrayal due to low loyalty!");
            // Additional logic: change factions, join enemy, etc.
        }
    }

    /// <summary>
    /// Levels up the companion, increasing their level and optionally
    /// granting a new title.  This can be triggered after battles or
    /// completing quests.
    /// </summary>
    public void LevelUp(string newTitle = null)
    {
        Level++;
        if (!string.IsNullOrEmpty(newTitle))
        {
            Title = newTitle;
        }
    }
}