using UnityEngine;
using System;
using System.Collections.Generic;
public class InventoryModel
{
    public event Action OnInventoryChanged;

    private readonly List<InventorySlot> slots;
    public IReadOnlyList<InventorySlot> Slots => slots;

    public InventoryModel(int size)
    {
        slots = new List<InventorySlot>(size);
        for (int i = 0; i < size; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public bool AddItem(ItemInstance newItem, Func<string, ItemDefinition> itemLookup)
    {
        if (newItem == null || newItem.amount <= 0)
            return false;

        ItemDefinition def = itemLookup?.Invoke(newItem.itemId);
        if (def == null)
            return false;

        int remaining = newItem.amount;

        if (def.stackable)
        {
            for (int i = 0; i < slots.Count && remaining > 0; i++)
            {
                var slot = slots[i];
                if (!slot.IsEmpty && slot.item.itemId == newItem.itemId)
                {
                    int space = def.maxStack - slot.item.amount;
                    if (space > 0)
                    {
                        int toAdd = Math.Min(space, remaining);
                        slot.item.amount += toAdd;
                        remaining -= toAdd;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Count && remaining > 0; i++)
        {
            var slot = slots[i];
            if (slot.IsEmpty)
            {
                int amountToPut = remaining;
                if (def.stackable)
                    amountToPut = Math.Min(remaining, def.maxStack);

                slot.item = new ItemInstance(newItem.itemId, amountToPut);
                remaining -= amountToPut;
            }
        }

        if (remaining != newItem.amount)
        {
            OnInventoryChanged?.Invoke();
        }

        return remaining <= 0;
    }

    public ItemInstance RemoveAt(int index, int amount)
    {
        if (index < 0 || index >= slots.Count)
            return null;

        var slot = slots[index];
        if (slot.IsEmpty || amount <= 0)
            return null;

        int removed = Math.Min(amount, slot.item.amount);
        var result = new ItemInstance(slot.item.itemId, removed);

        slot.item.amount -= removed;
        if (slot.item.amount <= 0)
        {
            slot.item = null;
        }

        OnInventoryChanged?.Invoke();
        return result;
    }

    public void Move(int fromIndex, int toIndex, Func<string, ItemDefinition> itemLookup = null)
    {
        if (fromIndex == toIndex)
            return;

        if (fromIndex < 0 || fromIndex >= slots.Count ||
            toIndex < 0 || toIndex >= slots.Count)
            return;

        var fromItem = slots[fromIndex].item;
        var toItem = slots[toIndex].item;

        if (fromItem != null && toItem != null && fromItem.itemId == toItem.itemId && itemLookup != null)
        {
            var def = itemLookup(fromItem.itemId);
            if (def != null && def.stackable)
            {
                int space = def.maxStack - toItem.amount;
                if (space > 0)
                {
                    int toTransfer = Math.Min(space, fromItem.amount);
                    toItem.amount += toTransfer;
                    fromItem.amount -= toTransfer;

                    if (fromItem.amount <= 0)
                    {
                        slots[fromIndex].item = null;
                    }

                    OnInventoryChanged?.Invoke();
                    return;
                }
            }
        }

        slots[fromIndex].item = toItem;
        slots[toIndex].item = fromItem;

        OnInventoryChanged?.Invoke();
    }

    public ItemInstance GetItem(int index)
    {
        if (index < 0 || index >= slots.Count)
            return null;

        return slots[index].item;
    }

    public void NotifyChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    public void LoadFromSaveData(System.Collections.Generic.List<SlotSaveData> slotData)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].item = null;
        }

        if (slotData == null) return;

        foreach (var data in slotData)
        {
            if (data.slotIndex >= 0 && data.slotIndex < slots.Count && !data.IsEmpty)
            {
                slots[data.slotIndex].item = new ItemInstance(data.itemId, data.amount);
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public void Clear()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].item = null;
        }

        OnInventoryChanged?.Invoke();
    }
}
