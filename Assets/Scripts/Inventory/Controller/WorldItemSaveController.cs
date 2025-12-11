using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class WorldItemSaveController : MonoBehaviour
{
    public static WorldItemSaveController Instance { get; private set; }

    [SerializeField] private string saveFileName = "worlditems.json";
    [SerializeField] private ItemDatabase itemDatabase;

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, saveFileName);
    private HashSet<string> collectedItemIds = new();
    private List<DroppedItemSaveData> droppedItems = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Load();
    }

    void Start()
    {
        RestoreDroppedItems();
    }

    void OnApplicationQuit()
    {
        CaptureDroppedItems();
        Save();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            CaptureDroppedItems();
            Save();
        }
    }

    public void RegisterCollectedItem(string uniqueId)
    {
        if (string.IsNullOrEmpty(uniqueId)) return;

        if (collectedItemIds.Add(uniqueId))
        {
            Save();
        }
    }

    public bool IsItemCollected(string uniqueId)
    {
        if (string.IsNullOrEmpty(uniqueId)) return false;
        return collectedItemIds.Contains(uniqueId);
    }

    public void Save()
    {
        var saveData = new WorldItemsSaveData();
        saveData.collectedItemIds = new List<string>(collectedItemIds);
        saveData.droppedItems = new List<DroppedItemSaveData>(droppedItems);

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
        collectedItemIds.Clear();
        droppedItems.Clear();

        if (!File.Exists(SaveFilePath))
        {
            return;
        }

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            var saveData = JsonUtility.FromJson<WorldItemsSaveData>(json);

            if (saveData?.collectedItemIds != null)
            {
                foreach (var id in saveData.collectedItemIds)
                {
                    collectedItemIds.Add(id);
                }
            }

            if (saveData?.droppedItems != null)
            {
                droppedItems = new List<DroppedItemSaveData>(saveData.droppedItems);
            }
        }
        catch (System.Exception)
        {
        }
    }

    public void DeleteSave()
    {
        collectedItemIds.Clear();
        droppedItems.Clear();

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

    public void ResetCollectedItems()
    {
        collectedItemIds.Clear();
        droppedItems.Clear();
        DeleteSave();
    }

    private void CaptureDroppedItems()
    {
        droppedItems.Clear();

        var allWorldItems = FindObjectsByType<WorldItemView>(FindObjectsSortMode.None);
        foreach (var item in allWorldItems)
        {
            if (!item.IsScenePlaced && item.gameObject.activeSelf && !string.IsNullOrEmpty(item.ItemId))
            {
                droppedItems.Add(new DroppedItemSaveData(item.ItemId, item.Amount, item.transform.position));
            }
        }
    }

    private void RestoreDroppedItems()
    {
        if (droppedItems == null || droppedItems.Count == 0) return;
        if (WorldItemPool.Instance == null) return;

        foreach (var data in droppedItems)
        {
            Sprite icon = null;
            if (itemDatabase != null)
            {
                var def = itemDatabase.GetItem(data.itemId);
                if (def != null)
                {
                    icon = def.icon;
                }
            }

            WorldItemPool.Instance.Get(data.Position, data.itemId, data.amount, icon);
        }

        droppedItems.Clear();
    }
}
