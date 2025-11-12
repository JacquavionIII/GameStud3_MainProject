using UnityEngine;

public class SpearInHand : MonoBehaviour
{
    public SpearHandler spearHandler;
    public Transform spear;
    public bool areThereSpears = false;

    void Update()
    {
        if (spearHandler.spearAmount > 0) //if the spear amount is more than 0, then the spear will appear in the player's hand
        {
            spear.gameObject.SetActive(true);
        }
        else
        {
            spear.gameObject.SetActive(false);
        }
        
    }
}
