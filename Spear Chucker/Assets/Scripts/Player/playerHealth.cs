using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]

public class playerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public void Start()
    {
        currentHealth = maxHealth;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("HumanEnemy"))
        {
            TakeDamage(20);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    // Public method to get max health
    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
