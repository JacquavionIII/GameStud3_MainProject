
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObect prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObect>> poolDictionary;

    void Awake()
    {
        Instance = this;
    }
    
     void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log($"The tag {tag} does not exist");
            return null;
        }

        GameObject obectToSpawn = poolDictionary[tag].Dequeue();
        obectToSpawn.SetActive(true);
        obectToSpawn.transform.position = position;
        obectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(obectToSpawn);
        return obectToSpawn;
    }

    public void Shoot()
    {
        //GameObect temp = Instantiate(prefab, shootLocation.position, Quaternion.identity);
        //temp.GetComponent<Rigidbody>().LinearVelocity = new Vector3(speed, 0, 0);
        //Destroy(temp, 2f);

        GameObect temp = ObjectPool.Instance.SpawnFromPool(currentType, shootLocation.position, Quaternion.identity);
        temp.GetComponent<Rigidbody>().linearVelocity = new Vector3(speed, 0, 0);
        temp.GetComponent<ObjectController>().Spawned();
    }

    void Update()
    {

    }
}
