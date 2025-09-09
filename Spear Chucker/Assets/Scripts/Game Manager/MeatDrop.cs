using UnityEngine;

public class MeatDrop : MonoBehaviour
{
    public float spawnTime = 20f; //how long the meat drop will be available for before it despawns
    private float timer;

    private void OnEnable()
    {
        timer = spawnTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ObjectPool.Instance.ReturnToPool(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // I'll add healing logic later
            print("Player picked up meat");
            ObjectPool.Instance.ReturnToPool(this.gameObject);
        }
    }
}
