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

        /// <summary>
        /// Multiplier applied to the base MoveSpeed when the run key (Left
        /// Shift) is held.  Set to 1 to disable running.
        /// </summary>
        public float RunMultiplier = 2f;

        /// <summary>
        /// Upward force applied when jumping.  Combine with Gravity to
        /// control jump height.
        /// </summary>
        public float JumpForce = 5f;

        /// <summary>
        /// Gravity applied to the player.  Increase this value to make the
        /// player fall faster.
        /// </summary>
        public float Gravity = 9.81f;

    private CharacterController controller;
    private HeroicCombat combat;
    private Animator animator;

        // Vertical velocity for jumping and gravity.
        private Vector3 velocity;

    // Animator parameter names for movement and attack.  These are
    // constants and will not appear in the Inspector, so your Player
    // Control component remains clean.  Modify these strings here if you
    // change the parameter names in your Animator controller.
    private const string HorizontalParam = "Velocity X";
    private const string VerticalParam = "Velocity Y";
    private const string AttackTriggerParam = "Attack";

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
            // Determine if running
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float speed = MoveSpeed * (isRunning ? RunMultiplier : 1f);
            // Build the movement vector including vertical velocity
            Vector3 motion = moveDir * speed;
            // Apply horizontal motion
            controller.Move(new Vector3(motion.x, 0f, motion.z) * Time.deltaTime);
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

        // Apply gravity and jumping
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // small negative value to keep grounded
        }
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            // Apply jump force (using basic kinematic equation)
            velocity.y = Mathf.Sqrt(JumpForce * 2f * Gravity);
        }
        // Apply gravity to vertical velocity
        velocity.y -= Gravity * Time.deltaTime;
        // Move vertically
        controller.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);

        // Handle attacks
        if (Input.GetMouseButtonDown(0))
        {
            // Left mouse button: melee attack forward
            var target = GetAttackTarget();
            combat.MeleeAttack(target);
            if (animator != null)
            {
                animator.SetTrigger(AttackTriggerParam);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            // Right mouse button: ranged attack forward
            var target = GetAttackTarget();
            combat.Shoot(target);
            if (animator != null)
            {
                animator.SetTrigger(AttackTriggerParam);
            }
        }
    }

    // Attack flag reset is no longer required because we use an animation
    // trigger.  The animator will automatically transition back when the
    // attack animation finishes.

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