using UnityEngine;
using UnityEngine.Events;

public class WoodDrop : MonoBehaviour, IPickUpAble
{
public float spawnTime = 30f; //how long the wood drop will be available for before it despawns
    private float timer;
    public UnityEvent OnPickedUp;

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

    public void OnPickUp()
    {
        print("I ain't a lumberjack but shawty let me give you this wood");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // I'll add the inventory and crafting later
            print("Player picked up wood");
            ObjectPool.Instance.ReturnToPool(this.gameObject);
            OnPickedUp.Invoke();
        }
    }
}
