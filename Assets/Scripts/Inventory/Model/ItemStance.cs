using UnityEngine;
using System;

[Serializable]
public class ItemInstance
{
    public string itemId;
    public int amount;

    public ItemInstance(string itemId, int amount)
    {
        this.itemId = itemId;
        this.amount = amount;
    }
}
