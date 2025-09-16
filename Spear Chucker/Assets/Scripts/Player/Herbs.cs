using UnityEngine;

public class Herbs : MonoBehaviour, IPickUpAble
{
    public void OnPickUp()
    {
        print("This is the part where we store you");
    }
}
