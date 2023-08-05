using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement settings
    public float movementSpeed;
    public float rotationSpeed;
    public float sprintSpeedMultiplier = 2f;
    public float walkSpeedMultiplier = 0.5f;

    // Jumping settings
    public float jumpForce = 5f;
    public float gravity = 20f;
    public Transform cameraTransform;

    // Attack settings
    public GameObject weapon;

    // Private variables
    private CharacterController controller;
    private Animator animator;
    private bool isGrounded;
    private bool isJumping;
    private Vector3 verticalVelocity;

    private bool isAttacking;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        // Set the skinWidth value to a small positive number
        controller.skinWidth = 0.01f;
    }

    private void Update()
    {
        // Check if the player is on the ground
        isGrounded = controller.isGrounded;

        // Apply movement
        // Move the player
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Determine movement speed based on input
        float currentSpeedMultiplier = 1f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeedMultiplier = sprintSpeedMultiplier;
        }
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            currentSpeedMultiplier = walkSpeedMultiplier;
        }

        // Get the movement direction based on camera rotation
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();
        Vector3 movement = cameraForward * moveVertical + cameraTransform.right * moveHorizontal;

        // Move the character
        controller.SimpleMove(movement * movementSpeed * currentSpeedMultiplier);

        // Only rotate the character if it's grounded
        if (isGrounded)
        {
            if (movement != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // Update the velocity parameters in the animator
        animator.SetFloat("Velocity Z", moveVertical * currentSpeedMultiplier);
        animator.SetFloat("Velocity X", moveHorizontal * currentSpeedMultiplier);

        // Set the "isWalking" parameter in the animator based on Alt key input
        animator.SetBool("isWalking", Input.GetKey(KeyCode.LeftAlt));

        // Set the "isRunning" parameter in the animator based on running input
        animator.SetBool("isRunning", Input.GetKey(KeyCode.LeftShift));

        // Set the "isFalling" parameter in the animator based on vertical velocity
        animator.SetBool("isFalling", !isGrounded && verticalVelocity.y < -0.1f); // Adjust the threshold here

        // Set the "isJumping" parameter in the animator
        animator.SetBool("isJumping", isJumping);

        // Set the "isGround" parameter in the animator
        animator.SetBool("isGround", isGrounded);

        // Jump action
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }

        // Attack action
        if (Input.GetMouseButtonDown(0)) // Assuming left mouse button is used for attack
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }

        // Set the "isAttacking" parameter in the animator
        animator.SetBool("isAttacking", isAttacking);

        // Apply gravity
        if (!isGrounded)
        {
            verticalVelocity.y -= gravity * Time.deltaTime;
        }

        // Apply vertical velocity
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void Jump()
    {
        // Calculate jump velocity based on jump force
        float jumpVelocity = Mathf.Sqrt(2f * jumpForce * gravity);

        // Set the vertical velocity to the jump velocity
        verticalVelocity.y = jumpVelocity;
        isJumping = true; // Set isJumping to true when jumping

        // Reset isJumping to false after a short delay (adjust the delay as needed)
        Invoke("ResetIsJumping", 0.1f);
    }

    private void ResetIsJumping()
    {
        isJumping = false;
    }
}
