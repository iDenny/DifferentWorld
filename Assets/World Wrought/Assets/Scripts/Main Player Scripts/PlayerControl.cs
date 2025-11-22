using UnityEngine;

/// <summary>
/// Basic player controller that moves using Unity's CharacterController and
/// triggers melee or ranged attacks through <see cref="HeroicCombat"/>.
/// Attach this script to your player object along with a
/// CharacterController and a HeroicCombat component.  Left‑click to
/// perform a melee attack, right‑click to shoot.  Movement uses the
/// horizontal and vertical input axes (WASD by default).
/// </summary>
[RequireComponent(typeof(CharacterController))]
// Require the custom combat system instead of SpaceMarineCombat to avoid
// referring to external franchises.
[RequireComponent(typeof(HeroicCombat))]
public class PlayerControl : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float TurnSpeed = 720f;

    private CharacterController controller;
    private HeroicCombat combat;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        combat = GetComponent<HeroicCombat>();
        // Lock cursor for a first‑person or third‑person view.  Remove this
        // line if you don't need mouse look.
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Handle movement based on input axes.
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(horiz, 0f, vert);
        if (inputDir.sqrMagnitude > 0.01f)
        {
            // Convert input direction to world space relative to the camera
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0f;
            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0f;
            Vector3 moveDir = (camForward.normalized * vert + camRight.normalized * horiz).normalized;
            controller.SimpleMove(moveDir * MoveSpeed);
            // Smoothly rotate towards movement direction
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, TurnSpeed * Time.deltaTime);
        }

        // Handle attacks
        if (Input.GetMouseButtonDown(0))
        {
            // Left mouse button: melee attack forward
            var target = GetAttackTarget();
            combat.MeleeAttack(target);
        }
        if (Input.GetMouseButtonDown(1))
        {
            // Right mouse button: ranged attack forward
            var target = GetAttackTarget();
            combat.Shoot(target);
        }
    }

    /// <summary>
    /// Casts a ray forward to find a target with a SpaceMarineCombat
    /// component.  Returns the GameObject if found, otherwise null.
    /// </summary>
    private GameObject GetAttackTarget()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            return hit.collider.gameObject;
        }
        return null;
    }
}