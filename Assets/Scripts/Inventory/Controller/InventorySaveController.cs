using UnityEngine;
using System.IO;

public class InventorySaveController : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private HotbarController hotbarController;
    [SerializeField] private bool autoSaveOnChange = true;
    [SerializeField] private string saveFileName = "inventory.json";

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, saveFileName);

    void Start()
    {
        Load();

        if (autoSaveOnChange)
        {
            SubscribeToChanges();
        }
    }

    void OnDestroy()
    {
        if (autoSaveOnChange)
        {
            UnsubscribeFromChanges();
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Save();
        }
    }

    private void SubscribeToChanges()
    {
        if (inventoryController != null && inventoryController.Model != null)
        {
            inventoryController.Model.OnInventoryChanged += Save;
        }

        if (hotbarController != null && hotbarController.Model != null)
        {
            hotbarController.Model.OnHotbarChanged += Save;
            hotbarController.Model.OnActiveSlotChanged += OnActiveSlotChanged;
        }
    }

    private void UnsubscribeFromChanges()
    {
        if (inventoryController != null && inventoryController.Model != null)
        {
            inventoryController.Model.OnInventoryChanged -= Save;
        }

        if (hotbarController != null && hotbarController.Model != null)
        {
            hotbarController.Model.OnHotbarChanged -= Save;
            hotbarController.Model.OnActiveSlotChanged -= OnActiveSlotChanged;
        }
    }

    private void OnActiveSlotChanged(int slotIndex)
    {
        Save();
    }

    public void Save()
    {
        var saveData = CreateSaveData();
        string json = JsonUtility.ToJson(saveData, true);

        try
        {
            File.WriteAllText(SaveFilePath, json);
        }
        catch (System.Exception)
        {
        }
    }

    public void Load()
    {
        if (!File.Exists(SaveFilePath))
        {
            return;
        }

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            var saveData = JsonUtility.FromJson<InventorySaveData>(json);

            if (saveData != null)
            {
                ApplySaveData(saveData);
            }
        }
        catch (System.Exception)
        {
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(SaveFilePath))
        {
            try
            {
                File.Delete(SaveFilePath);
            }
            catch (System.Exception)
            {
            }
        }
    }

    private InventorySaveData CreateSaveData()
    {
        var saveData = new InventorySaveData();

        if (inventoryController != null && inventoryController.Model != null)
        {
            var slots = inventoryController.Model.Slots;
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                if (!slot.IsEmpty)
                {
                    saveData.inventorySlots.Add(new SlotSaveData(i, slot.item.itemId, slot.item.amount));
                }
            }
        }

        if (hotbarController != null && hotbarController.Model != null)
        {
            var slots = hotbarController.Model.Slots;
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                if (!slot.IsEmpty)
                {
                    saveData.hotbarSlots.Add(new SlotSaveData(i, slot.item.itemId, slot.item.amount));
                }
            }

            saveData.activeHotbarSlot = hotbarController.Model.ActiveSlotIndex;
        }

        return saveData;
    }

    private void ApplySaveData(InventorySaveData saveData)
    {
        if (inventoryController != null && inventoryController.Model != null)
        {
            inventoryController.Model.LoadFromSaveData(saveData.inventorySlots);
        }

        if (hotbarController != null && hotbarController.Model != null)
        {
            hotbarController.Model.LoadFromSaveData(saveData.hotbarSlots, saveData.activeHotbarSlot);
        }
    }
}
