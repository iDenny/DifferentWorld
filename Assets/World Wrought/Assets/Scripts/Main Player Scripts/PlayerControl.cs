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
    private Animator animator;

    // Animator parameter names for movement and attack.  These default
    // values correspond to your existing Animator.  You can change them in
    // the Inspector if your parameter names differ.
    [Header("Animator Parameter Names")]
    public string HorizontalParam = "Velocity X";
    public string VerticalParam = "Velocity Y";
    public string AttackBoolParam = "isAttacking";
    /// <summary>
    /// Duration in seconds to keep the attack bool set to true after an
    /// attack input.  The Animator will transition back once this
    /// duration expires.  Adjust this to match the length of your attack
    /// animation.
    /// </summary>
    public float AttackAnimDuration = 0.5f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        combat = GetComponent<HeroicCombat>();
        // Attempt to fetch an Animator to drive movement and attack animations.
        animator = GetComponent<Animator>();
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
            // Use Move instead of SimpleMove to control movement explicitly and avoid slide lag
            controller.Move(moveDir * MoveSpeed * Time.deltaTime);
            // Smoothly rotate towards movement direction
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, TurnSpeed * Time.deltaTime);
            // Update animator with movement speed
            if (animator != null)
            {
                // Pass raw input values directly into the blend tree
                animator.SetFloat(HorizontalParam, horiz);
                animator.SetFloat(VerticalParam, vert);
            }
        }
        else
        {
            // When not moving, ensure animator speed parameter is zero
            if (animator != null)
            {
                animator.SetFloat(HorizontalParam, 0f);
                animator.SetFloat(VerticalParam, 0f);
            }
        }

        // Handle attacks
        if (Input.GetMouseButtonDown(0))
        {
            // Left mouse button: melee attack forward
            var target = GetAttackTarget();
            combat.MeleeAttack(target);
            if (animator != null)
            {
                // Trigger attack animation by setting a bool; reset after a delay
                animator.SetBool(AttackBoolParam, true);
                CancelInvoke(nameof(ResetAttackFlag));
                Invoke(nameof(ResetAttackFlag), AttackAnimDuration);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            // Right mouse button: ranged attack forward
            var target = GetAttackTarget();
            combat.Shoot(target);
            if (animator != null)
            {
                animator.SetBool(AttackBoolParam, true);
                CancelInvoke(nameof(ResetAttackFlag));
                Invoke(nameof(ResetAttackFlag), AttackAnimDuration);
            }
        }
    }

    /// <summary>
    /// Resets the attack flag used to trigger attack animations.  Called
    /// automatically via Invoke after AttackAnimDuration.
    /// </summary>
    private void ResetAttackFlag()
    {
        if (animator != null)
        {
            animator.SetBool(AttackBoolParam, false);
        }
    }

    /// <summary>
    /// Casts a ray forward to find a target with a HeroicCombat
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