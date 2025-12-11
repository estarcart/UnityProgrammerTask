using UnityEngine;
using System.Collections.Generic;

public class WorldItemPool : MonoBehaviour
{
    public static WorldItemPool Instance { get; private set; }

    [SerializeField] private GameObject worldItemPrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private Transform poolContainer;

    private Queue<WorldItemView> availableItems = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializePool();
    }

    private void InitializePool()
    {
        if (poolContainer == null)
        {
            var containerGo = new GameObject("PooledWorldItems");
            containerGo.transform.SetParent(transform);
            poolContainer = containerGo.transform;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewPooledItem();
        }
    }

    private WorldItemView CreateNewPooledItem()
    {
        var go = Instantiate(worldItemPrefab, poolContainer);
        go.SetActive(false);
        var worldItem = go.GetComponent<WorldItemView>();
        availableItems.Enqueue(worldItem);
        return worldItem;
    }

    public WorldItemView Get(Vector3 position, string itemId, int amount, Sprite icon)
    {
        WorldItemView worldItem;

        if (availableItems.Count > 0)
        {
            worldItem = availableItems.Dequeue();
        }
        else
        {
            worldItem = CreateNewPooledItem();
            availableItems.Dequeue();
        }

        worldItem.transform.SetParent(null);
        worldItem.transform.position = position;
        worldItem.transform.rotation = Quaternion.identity;
        worldItem.SetItem(itemId, amount, icon);
        worldItem.gameObject.SetActive(true);

        return worldItem;
    }

    public void Return(WorldItemView worldItem)
    {
        if (worldItem == null) return;

        worldItem.gameObject.SetActive(false);
        worldItem.transform.SetParent(poolContainer);
        worldItem.Reset();
        availableItems.Enqueue(worldItem);
    }
}
