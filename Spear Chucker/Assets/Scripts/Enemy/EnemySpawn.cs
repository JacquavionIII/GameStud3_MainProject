using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    public string enemyTag = "Enemy";       //this is the tag that's in the object pool
    public string humanEnemyTag = "HumanEnemy"; //tag for human enemy in the object pool
    public float spawnIntervals = 5f;       //time between the enemy spawns
    public int maxEnemiesAlive = 3;            //max number of enemies in the scene at once
    public int maxHumanEnemiesAlive = 2;      //max number of human enemies in the scene at once
    public float timer;                  //timer to keep track of the spawn intervals
    private int enemiesAlive;         //current number of enemies in the scene which will get refereence from our enemy script
    private int humanEnemiesAlive;   //current number of human enemies in the scene

    public void Update()
    {
        timer -= Time.deltaTime;

        //Check if we can spawn an enemy
        if (timer <= 0f && enemiesAlive < maxEnemiesAlive)
        {
            SpawnEnemy();
            timer = spawnIntervals; //Reset the timer
        }

        if (timer <= 0f && humanEnemiesAlive < maxHumanEnemiesAlive)
        {
            SpawnHumanEnemy();
            timer = spawnIntervals; //Reset the timer
        }
    }

    public void SpawnEnemy()
    {
        GameObject enemy = ObjectPool.Instance.SpawnFromPool(enemyTag, transform.position, Quaternion.identity);
        if (enemy != null)
        {
            enemiesAlive++;
            Enemy enemyScript = enemy.GetComponent<Enemy>(); //this way we can keep track of what happens in the enemy script
            if (enemyScript != null)
            {
                enemyScript.onDeath += HandleEnemyDeath; //subscribing to an enemy death event
            }
        }
    }

    public void SpawnHumanEnemy()
    {
        GameObject humanEnemy = ObjectPool.Instance.SpawnFromPool(humanEnemyTag, transform.position, Quaternion.identity);
        if (humanEnemy != null)
        {
            humanEnemiesAlive++;
            HumanEnemy humanEnemyScript = humanEnemy.GetComponent<HumanEnemy>(); //this way we can keep track of what happens in the human enemy script
            if (humanEnemyScript != null)
            {
                humanEnemyScript.onDeath += HandleEnemyDeath; //subscribing to an enemy death event
            }
        }
    }

    private void HandleEnemyDeath()
    {
        enemiesAlive--;
        humanEnemiesAlive--;
    }
}
