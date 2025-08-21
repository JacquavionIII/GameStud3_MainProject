
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class SpearToss : MonoBehaviour
{
    public void Shoot()
    {
        //GameObect temp = Instantiate(prefab, shootLocation.position, Quaternion.identity);
        //temp.GetComponent<Rigidbody>().LinearVelocity = new Vector3(speed, 0, 0);
        //Destroy(temp, 2f);

        GameObect temp = ObjectPool.Instance.SpawnFromPool(currentType, shootLocation.position, Quaternion.identity);
        temp.GetComponent<Rigidbody>().linearVelocity = new Vector3(speed, 0, 0);
        temp.GetComponent<ObjectController>().Spawned();
    }
}
