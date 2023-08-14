using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    UIHealthBar healthBar;
    Animator animator;

    private Collider objectCollider; // Reference to the collider

    private void Start()
    {
        healthBar = GetComponentInChildren<UIHealthBar>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Get the collider component attached to the GameObject
        objectCollider = GetComponent<Collider>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.SetHealthBarPercentage(currentHealth / maxHealth);

        if (currentHealth <= 0.0f)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle death logic here, such as playing an animation, disabling the character, etc.
        healthBar.gameObject.SetActive(false);

        // Trigger death animation
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        // Disable all scripts attached to the GameObject
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }

        // Disable the collider
        if (objectCollider != null)
        {
            objectCollider.enabled = false;
        }
    }
}
