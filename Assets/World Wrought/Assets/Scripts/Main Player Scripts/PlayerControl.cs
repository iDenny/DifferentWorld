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

    // Ground checking
    [Tooltip("Layer mask used to detect ground for reliable jumping.")]
    public LayerMask GroundLayer = ~0;
    [Tooltip("Radius used for the ground check sphere at the player's feet.")]
    public float GroundCheckRadius = 0.15f;
    [Tooltip("Vertical offset from the transform position where the ground check sphere is cast.")]
    public Vector3 GroundCheckOffset = new Vector3(0f, -0.9f, 0f);

    private CharacterController controller;
    private HeroicCombat combat;
    private Animator animator;

        // Vertical velocity for jumping and gravity.
        private Vector3 velocity;

    // Animator parameter names for movement and attack.  These are
    // constants and will not appear in the Inspector, so your Player
    // Control component remains clean.  Modify these strings here if you
    // change the parameter names in your Animator controller.
    private static readonly int HorizontalParam = Animator.StringToHash("Velocity X");
    private static readonly int VerticalParam = Animator.StringToHash("Velocity Y");
    private static readonly int WalkParam = Animator.StringToHash("isWalking");
    private static readonly int RunParam = Animator.StringToHash("isRunning");
    private static readonly int SprintParam = Animator.StringToHash("isSprinting");
    private static readonly int JumpParam = Animator.StringToHash("isJumping");
    private static readonly int FallParam = Animator.StringToHash("isFalling");
    private static readonly int AttackParam = Animator.StringToHash("isAttacking");
    private static readonly int GroundParam = Animator.StringToHash("isGround");

    private Coroutine attackResetRoutine;
    private const float AttackResetTime = 0.2f;

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
        bool hasInput = inputDir.sqrMagnitude > 0.01f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        if (hasInput)
        {
            // Convert input direction to world space relative to the camera
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0f;
            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0f;
            Vector3 moveDir = (camForward.normalized * vert + camRight.normalized * horiz).normalized;
            // Determine if running
            float speed = MoveSpeed * (isRunning ? RunMultiplier : 1f);
            // Build the movement vector including vertical velocity
            Vector3 motion = moveDir * speed;
            // Apply horizontal motion
            controller.Move(new Vector3(motion.x, 0f, motion.z) * Time.deltaTime);
            // Smoothly rotate towards movement direction
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, TurnSpeed * Time.deltaTime);
        }

        UpdateAnimatorMovement(horiz, vert, hasInput, isRunning);

        // Apply gravity and jumping
        bool grounded = IsGrounded();
        if (grounded && velocity.y < 0f)
        {
            // small negative value to keep controller grounded reliably
            velocity.y = -2f;
        }
        if (Input.GetButtonDown("Jump") && grounded)
        {
            // Apply jump force (using basic kinematic equation)
            velocity.y = Mathf.Sqrt(JumpForce * 2f * Gravity);
            UpdateJumpState(true);
            // move immediately so character leaves ground this frame
            controller.Move(new Vector3(0f, velocity.y * Time.deltaTime, 0f));
        }
        // Apply gravity to vertical velocity
        velocity.y -= Gravity * Time.deltaTime;
        // Move vertically
        controller.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);

        UpdateGroundedAndFalling();

        // Handle attacks
        if (Input.GetMouseButtonDown(0))
        {
            // Left mouse button: melee attack forward
            var target = GetAttackTarget();
            combat.MeleeAttack(target);
            TriggerAttackAnimation();
        }
        if (Input.GetMouseButtonDown(1))
        {
            // Right mouse button: ranged attack forward
            var target = GetAttackTarget();
            combat.Shoot(target);
            TriggerAttackAnimation();
        }
    }

    // Animator helpers keep movement, jumping, falling and attacking
    // parameters synchronized with the controller.

    private void UpdateAnimatorMovement(float horiz, float vert, bool hasInput, bool isRunning)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat(HorizontalParam, hasInput ? horiz : 0f);
        animator.SetFloat(VerticalParam, hasInput ? vert : 0f);
        animator.SetBool(WalkParam, hasInput && !isRunning);
        animator.SetBool(RunParam, hasInput && isRunning);
        animator.SetBool(SprintParam, hasInput && isRunning);
    }

    private void UpdateJumpState(bool isJumping)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool(JumpParam, isJumping);
        animator.SetBool(FallParam, false);
    }

    private void UpdateGroundedAndFalling()
    {
        if (animator == null)
        {
            return;
        }

        // Use our reliable ground check rather than relying solely on CharacterController.isGrounded
        bool grounded = IsGrounded();
        bool ascending = velocity.y > 0.1f;
        animator.SetBool(GroundParam, grounded && !ascending);

        if (grounded && velocity.y <= 0.1f)
        {
            animator.SetBool(JumpParam, false);
            animator.SetBool(FallParam, false);
        }
        else
        {
            bool falling = velocity.y <= -0.5f;
            animator.SetBool(FallParam, falling);
            animator.SetBool(JumpParam, !falling && velocity.y > 0f);
        }
    }

    private void TriggerAttackAnimation()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool(AttackParam, true);
        if (attackResetRoutine != null)
        {
            StopCoroutine(attackResetRoutine);
        }
        attackResetRoutine = StartCoroutine(ResetAttackFlag());
    }

    private System.Collections.IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(AttackResetTime);
        if (animator != null)
        {
            animator.SetBool(AttackParam, false);
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

    private bool IsGrounded()
    {
        // First ask CharacterController - it's cheap and usually accurate
        if (controller.isGrounded)
            return true;

        // Fallback: sphere check at feet to handle small gaps due to skin width or moving platforms
        Vector3 checkPos = transform.position + GroundCheckOffset;
        return Physics.CheckSphere(checkPos, GroundCheckRadius, GroundLayer, QueryTriggerInteraction.Ignore);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + GroundCheckOffset, GroundCheckRadius);
    }
#endif
}