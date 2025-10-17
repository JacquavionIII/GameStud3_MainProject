using UnityEngine;
using UnityEngine.Events;

public class StoneDrop : MonoBehaviour, IPickUpAble
{
    public float spawnTime = 30f; //how long the stone drop will be available for before it despawns
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
        print("You know they used to stone people to death, right?"); //you come up with brainrot jokes for rocks and ill let you complain
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // I'll add the inventory and crafting later
            print("Player picked up stones");
            ObjectPool.Instance.ReturnToPool(this.gameObject);
            OnPickedUp.Invoke();
        }
    }
}
