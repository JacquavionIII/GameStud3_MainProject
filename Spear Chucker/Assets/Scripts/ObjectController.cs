
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class ObjectController : MonoBehaviour
{
    public void Spawned()
    {
        StartCoroutine(Disable());
    }

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(2f);
        this.gameObect.SetActive(false);
        this.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }
}
