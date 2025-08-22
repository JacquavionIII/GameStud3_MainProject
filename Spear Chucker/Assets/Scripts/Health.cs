using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private Material hpMaterial; //Reference for the Hpbar material.
    float health, maxHealth = 100;
    private float lerpSpeed;

    private static readonly int FlowProperty = Shader.PropertyToID("_Flow"); //We're calling the shader property we want to change (the underscore is cause unity scripts list the shader thing like this).

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (health > maxHealth) health = maxHealth;

        lerpSpeed = 3f * Time.deltaTime;

        UpdateShaderBar();    
    }

    private void UpdateShaderBar()
    {
        float targetValue = health / maxHealth;

        float currentValue = hpMaterial.GetFloat(FlowProperty);
        float newValue = Mathf.Lerp(currentValue, targetValue, lerpSpeed);//makes the smooth transition between values for the hp bar

        hpMaterial.SetFloat(FlowProperty, newValue); //changes the value of the material so makes it go up or down
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
            //SceneManager.LoadScene("Death Screen");
            print("You should be dead here ig");
        }
    }
}
