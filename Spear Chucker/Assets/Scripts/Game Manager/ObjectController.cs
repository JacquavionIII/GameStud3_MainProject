
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

public class ObjectController : MonoBehaviour
{
    public void Spawned()
    {
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(2f);
        this.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        this.gameObject.SetActive(false);
    }
}
