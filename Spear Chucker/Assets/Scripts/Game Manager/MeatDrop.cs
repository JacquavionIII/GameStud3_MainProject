using UnityEngine;
using UnityEngine.Events;

public class MeatDrop : MonoBehaviour, IPickUpAble
{
    public float spawnTime = 20f; //how long the meat drop will be available for before it despawns
    private float timer;
    public PlayerMovement player;
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
        print("yum yum in my tum tum");
        player.meatAmount++;
        ObjectPool.Instance.ReturnToPool(this.gameObject);
        OnPickedUp?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // I'll add healing logic later
            print("Player picked up meat");
            ObjectPool.Instance.ReturnToPool(this.gameObject);
            OnPickedUp.Invoke();
        }
    }
}
