using UnityEngine;

/// <summary>
/// Handles player interactions with NPCs.  When the player presses the
/// interact key (E by default) and looks at a non‑hostile character,
/// this script will either initiate dialogue or recruit the NPC as a
/// companion if they have a <see cref="CompanionSystem"/>.  Attach this
/// to your player object alongside <see cref="PlayerControl"/>.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    /// <summary>
    /// Maximum distance at which the player can interact with an NPC.
    /// </summary>
    public float InteractRange = 3f;

    /// <summary>
    /// Key used for interactions.
    /// </summary>
    public KeyCode InteractKey = KeyCode.E;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(InteractKey))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (mainCam == null) return;
        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, InteractRange))
        {
            // Only interact with objects that have a Character component
            Character targetChar = hit.collider.GetComponent<Character>();
            if (targetChar == null) return;

            // Check if the target has a CompanionSystem; if so, recruit
            CompanionSystem comp = targetChar.GetComponent<CompanionSystem>();
            if (comp != null && !comp.IsFollowing)
            {
                comp.IsFollowing = true;
                // Optionally initialise loyalty or title here
                Debug.Log($"{targetChar.CharacterName} has joined as a companion.");
            }
            else
            {
                // TODO: Implement dialogue or other interaction for non‑companions
                Debug.Log($"Interacted with {targetChar.CharacterName}");
            }
        }
    }
}