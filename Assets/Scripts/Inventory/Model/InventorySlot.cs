using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ItemInstance item;

    public bool IsEmpty => item == null || item.amount <= 0;
}
