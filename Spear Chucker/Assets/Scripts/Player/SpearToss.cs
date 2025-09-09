
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class SpearToss : MonoBehaviour
{
    public string currentType;
    public Transform shootLocation;
    public float speed = 10f;
    
    public void Shoot()
    {
        //GameObect temp = Instantiate(prefab, shootLocation.position, Quaternion.identity);
        //temp.GetComponent<Rigidbody>().LinearVelocity = new Vector3(speed, 0, 0);
        //Destroy(temp, 2f);

        GameObject temp = ObjectPool.Instance.SpawnFromPool(currentType, shootLocation.position, Quaternion.identity);
        temp.GetComponent<Rigidbody>().linearVelocity = new Vector3(speed, 0, 0);
        temp.GetComponent<ObjectController>().Spawned();
    }
}
