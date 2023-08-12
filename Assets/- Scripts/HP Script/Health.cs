using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    UIHealthBar healthBar;

    private void Start()
    {
        healthBar = GetComponentInChildren<UIHealthBar>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        healthBar.SetHealthBarPercentage(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle death logic here, such as playing an animation, disabling the character, etc.
        Debug.Log(gameObject.name + " has died.");
        healthBar.gameObject.SetActive(false);
    }
}
