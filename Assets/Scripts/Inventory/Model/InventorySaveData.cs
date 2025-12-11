using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySaveData
{
    public List<SlotSaveData> inventorySlots = new();
    public List<SlotSaveData> hotbarSlots = new();
    public int activeHotbarSlot;
}

[Serializable]
public class WorldItemsSaveData
{
    public List<string> collectedItemIds = new();
    public List<DroppedItemSaveData> droppedItems = new();
}

[Serializable]
public class DroppedItemSaveData
{
    public string itemId;
    public int amount;
    public float posX;
    public float posY;
    public float posZ;

    public DroppedItemSaveData() { }

    public DroppedItemSaveData(string itemId, int amount, Vector3 position)
    {
        this.itemId = itemId;
        this.amount = amount;
        this.posX = position.x;
        this.posY = position.y;
        this.posZ = position.z;
    }

    public Vector3 Position => new Vector3(posX, posY, posZ);
}

[Serializable]
public class SlotSaveData
{
    public int slotIndex;
    public string itemId;
    public int amount;

    public SlotSaveData() { }

    public SlotSaveData(int slotIndex, string itemId, int amount)
    {
        this.slotIndex = slotIndex;
        this.itemId = itemId;
        this.amount = amount;
    }

    public bool IsEmpty => string.IsNullOrEmpty(itemId) || amount <= 0;
}
