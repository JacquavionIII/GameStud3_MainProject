using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public Image healthBar;
    public int currentHealth, maxHealth = 100;
    public playerHealth player;

    [Header("Color Settings")]
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public float colorChangeThreshold = 0.3f;
    private float lerpSpeed = 3f;

    [Header("References")]
    public Transform spawnPoint;


    void Start()
    {
        healthBar.type = Image.Type.Filled;
        healthBar.fillMethod = Image.FillMethod.Horizontal;
        healthBar.fillAmount = 1f;
        healthBar.color = fullHealthColor;
    }

    void Update()
    {
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        lerpSpeed = 3f * Time.deltaTime;
                      
        // Initialize health display
        currentHealth = player.GetCurrentHealth();
        maxHealth = player.GetMaxHealth();


        UpdateHealthBar();
        HealthColour();
        
    }

    public void Damage(int damagePoints)
    {
        if (player != null)
        {
            player.TakeDamage(damagePoints);
        }
        else
        {
            currentHealth -= damagePoints;
        }
    }

    public void Heal(int healPoints)
    {
        if (player != null)
        {
            player.Heal(healPoints);
        }
        else
        {
            currentHealth += healPoints;
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null && maxHealth > 0)
        {
            float targetFillAmount = (float)currentHealth / maxHealth;
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
        }
    }
    
    void HealthColour()
    {
        if (healthBar != null)
        {
            float healthPercentage = (float)currentHealth / maxHealth;
            
            if (healthPercentage <= colorChangeThreshold)
            {
                float t = healthPercentage / colorChangeThreshold;
                healthBar.color = Color.Lerp(lowHealthColor, fullHealthColor, t);
            }
            else
            {
                healthBar.color = fullHealthColor;
            }
        }
    }

    public void Death()
    {
        if (currentHealth <= 0)
        {
            //SceneManager.LoadScene("Death Screen");
            print("You should be dead here ig");
            //respawn
            Respawn();
        }
    }

    public void Respawn()
    {
        // Move player to spawn point position and rotation
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        // Reset physics if this object has a Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        currentHealth = maxHealth;

        // Update UI immediately
        UpdateHealthBar();
        HealthColour();
    }
}
