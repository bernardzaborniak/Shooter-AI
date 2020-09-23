using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemInteractionType itemInteractionType;

    [Tooltip("some weapons like rpg or machine gun takes longer to get out of the inventory than other weapons")]
    public float pullOutItemTime;
    public float hideItemTime;


    public int GetItemInteractionTypeID()
    {
        if (itemInteractionType == ItemInteractionType.BareHands)
        {
            return 0;
        }
        else if (itemInteractionType == ItemInteractionType.Rifle)
        {
            return 1;
        }
        else if (itemInteractionType == ItemInteractionType.Pistol)
        {
            return 2;
        }
        else if(itemInteractionType == ItemInteractionType.Grenade)
        {
            return 3;
        }

        return 0;
    }
}
