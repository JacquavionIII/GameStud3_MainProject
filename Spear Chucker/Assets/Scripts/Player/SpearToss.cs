
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;
using System.Diagnostics;
using Unity.Cinemachine;

public class SpearToss : MonoBehaviour
{
    public SpearHandler spearHandler;
    public string currentType;
    public Transform shootLocation;
    public float speed = 10f;
    public Transform aimSource; //this is basicall what the shoot  position will use to determine what direction it will be rotated in
    private CinemachineImpulseSource source; //calling the impulse source

    public enum ShootAxis { Forward, Right, Custom }
    public ShootAxis shootAxis = ShootAxis.Forward;
    public Vector3 customDirection = Vector3.forward; // local-space direction when using Custom

    private void Start()
    {
        source = GetComponent<CinemachineImpulseSource>();
    }
    
    private void LateUpdate()
    {
        if (shootLocation != null && aimSource != null)
        {
            // Make the shootLocation face the aim source
            shootLocation.rotation = aimSource.rotation;

            // If you want it to only rotate horizontally (ignore pitch), use:
            // Vector3 flatForward = Vector3.Scale(aimSource.forward, new Vector3(1f,0f,1f)).normalized;
            // if (flatForward.sqrMagnitude > 0.0001f) shootLocation.rotation = Quaternion.LookRotation(flatForward);
        }
    }
    
    public void Shoot()
    {
        //GameObect temp = Instantiate(prefab, shootLocation.position, Quaternion.identity);
        //temp.GetComponent<Rigidbody>().LinearVelocity = new Vector3(speed, 0, 0);
        //Destroy(temp, 2f);

        if (spearHandler.spearAmount > 0)
        {
            GameObject temp = ObjectPool.Instance.SpawnFromPool(currentType, shootLocation.position, shootLocation.rotation);
            // temp.GetComponent<Rigidbody>().linearVelocity = shootLocation.forward * speed;

            temp.transform.rotation = shootLocation.rotation;

            Rigidbody rb = temp.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir;
                if (shootAxis == ShootAxis.Forward)
                    dir = shootLocation.forward;
                else if (shootAxis == ShootAxis.Right)
                    dir = shootLocation.right; // local X axis
                else
                    dir = temp.transform.TransformDirection(customDirection.normalized);

                rb.linearVelocity = dir.normalized * speed;
            }

            temp.GetComponent<ObjectController>().Spawned();

            source.GenerateImpulse(Camera.main.transform.forward);
            spearHandler.spearChuck(1);
        }
        else
        {
            print("You are drawing blanks dude.");
        }
    }
}
