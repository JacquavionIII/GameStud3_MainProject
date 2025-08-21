using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    
    public Image healthBar;
    public Image[] healthPoints;
    float health, maxHealth = 100;
    private float lerpSpeed;

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        
        if (health > maxHealth) health = maxHealth;

        lerpSpeed = 3f * Time.deltaTime;

        HealthBarFiller();
        
    }

    public void HealthBarFiller()
    {

        for (int i = 0; i < healthPoints.Length; i++)
        {
            healthPoints[i].enabled = !DisplayHealthPoints(health, i);
        }
    }

    public bool DisplayHealthPoints(float health, int pointNumber)
    {
        return ((pointNumber * 10) >= health);
    }

    public void Damage(float damagePoints)
    {
        if (health > 0)
            health -= damagePoints;
    }

    public void Heal(float healPoints)
    {
        if (health < maxHealth)
            health += healPoints;
    }

    public void Death()
    {
        if (health <= 0)
        {
         SceneManager.LoadScene("Death Screen");
        }
    }
}
