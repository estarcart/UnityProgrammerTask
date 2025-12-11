using System;
using System.Collections.Generic;

public class HotbarModel
{
    public event Action OnHotbarChanged;
    public event Action<int> OnActiveSlotChanged;

    private readonly List<InventorySlot> slots;
    private int activeSlotIndex;

    public IReadOnlyList<InventorySlot> Slots => slots;
    public int SlotCount => slots.Count;
    public int ActiveSlotIndex => activeSlotIndex;

    public HotbarModel(int size)
    {
        slots = new List<InventorySlot>(size);
        for (int i = 0; i < size; i++)
        {
            slots.Add(new InventorySlot());
        }
        activeSlotIndex = 0;
    }

    public void SetActiveSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
            return;

        if (activeSlotIndex != index)
        {
            activeSlotIndex = index;
            OnActiveSlotChanged?.Invoke(activeSlotIndex);
        }
    }

    public ItemInstance GetActiveItem()
    {
        return GetItem(activeSlotIndex);
    }

    public ItemInstance GetItem(int index)
    {
        if (index < 0 || index >= slots.Count)
            return null;

        return slots[index].item;
    }

    public bool SetItem(int index, ItemInstance item)
    {
        if (index < 0 || index >= slots.Count)
            return false;

        slots[index].item = item;
        OnHotbarChanged?.Invoke();
        return true;
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
            OnHotbarChanged?.Invoke();
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

        OnHotbarChanged?.Invoke();
        return result;
    }

    public ItemInstance ConsumeActiveItem(int amount = 1)
    {
        return RemoveAt(activeSlotIndex, amount);
    }

    public void SwapSlots(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex)
            return;

        if (fromIndex < 0 || fromIndex >= slots.Count ||
            toIndex < 0 || toIndex >= slots.Count)
            return;

        var temp = slots[fromIndex].item;
        slots[fromIndex].item = slots[toIndex].item;
        slots[toIndex].item = temp;

        OnHotbarChanged?.Invoke();
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
            return;

        slots[index].item = null;
        OnHotbarChanged?.Invoke();
    }

    public void Clear()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].item = null;
        }
        OnHotbarChanged?.Invoke();
    }

    public void NotifyChanged()
    {
        OnHotbarChanged?.Invoke();
    }
}
