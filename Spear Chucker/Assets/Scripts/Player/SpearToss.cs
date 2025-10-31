
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;
using System.Diagnostics;
using Unity.Cinemachine;

public class SpearToss : MonoBehaviour
{
    public string currentType;
    public Transform shootLocation;
    public float speed = 10f;
    private CinemachineImpulseSource source; //calling the impulse source
    
    private void Start()
    {
        source = GetComponent<CinemachineImpulseSource>();
    }
    
    public void Shoot()
    {
        //GameObect temp = Instantiate(prefab, shootLocation.position, Quaternion.identity);
        //temp.GetComponent<Rigidbody>().LinearVelocity = new Vector3(speed, 0, 0);
        //Destroy(temp, 2f);

        GameObject temp = ObjectPool.Instance.SpawnFromPool(currentType, shootLocation.position, Quaternion.identity);
        temp.GetComponent<Rigidbody>().linearVelocity = shootLocation.forward * speed;
        temp.GetComponent<ObjectController>().Spawned();

        source.GenerateImpulse(Camera.main.transform.forward);
    }
}
