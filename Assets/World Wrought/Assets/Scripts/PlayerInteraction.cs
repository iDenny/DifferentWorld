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
    private PlayerInventory inventory;

    private void Awake()
    {
        mainCam = Camera.main;
        inventory = GetComponent<PlayerInventory>();
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
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        // Use center of screen ray so interactions work in third-person camera setups
        Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));

        // Prefer a short sphere cast for forgiving hits
        if (Physics.SphereCast(ray, 0.5f, out RaycastHit hit, InteractRange))
        {
            // Weapon pickup
            var pickup = hit.collider.GetComponentInParent<WeaponPickup>();
            if (pickup != null && inventory != null)
            {
                inventory.PickupWeapon(pickup.gameObject);
                Debug.Log($"Picked up weapon: {pickup.gameObject.name}");
                return;
            }

            // Character interaction
            var targetChar = hit.collider.GetComponentInParent<Character>();
            if (targetChar == null) return;

            var comp = targetChar.GetComponent<CompanionSystem>();
            if (comp != null && !comp.IsFollowing)
            {
                // Recruit: mark as following and enable a follow behaviour
                comp.IsFollowing = true;
                var follow = targetChar.GetComponent<CompanionFollow>();
                if (follow == null)
                {
                    follow = targetChar.gameObject.AddComponent<CompanionFollow>();
                }
                follow.SetLeader(gameObject);

                // Optionally set the NPC to an ally layer if you have one (configure in Inspector)
                Debug.Log($"{targetChar.CharacterName} has joined you.");
            }
            else
            {
                Debug.Log($"Interacted with {targetChar.CharacterName}");
            }
        }
        else
        {
            Debug.Log("PlayerInteraction: no hit on interact");
        }
    }
}