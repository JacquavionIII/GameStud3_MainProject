using UnityEngine;

public class SpearHandler : MonoBehaviour
{
    public int woodAmount = 0;
    public int stoneAmount = 0;
    public bool canCraftSpear = false;
    public int spearAmount = 0;

    public void Update()
    {
        canCraftSpear = (woodAmount > 0 && stoneAmount > 0);
        if (woodAmount > 0 && stoneAmount > 0)
        {
            canCraftSpear = true;
            CraftSpear();
        }
        else
        {
            canCraftSpear = false;
        }
    }

    // Call this once when you want to craft (e.g. button press or input event)
    public void CraftSpear()
    {
        if (woodAmount > 0 && stoneAmount > 0)
        {
            woodAmount -= 1;
            stoneAmount -= 1;
            spearAmount += 1;
            canCraftSpear = (woodAmount > 0 && stoneAmount > 0);
            Debug.Log("Crafted 1 spear. Total spears: " + spearAmount);
        }
    }
    
    public void spearChuck(int spear)
    {
        spearAmount -= spear;
    }

    public void woodIn(int wood)
    {
        woodAmount += wood;
    }

    public void stoneIN(int stone)
    {
        stoneAmount += stone;
    }
}
