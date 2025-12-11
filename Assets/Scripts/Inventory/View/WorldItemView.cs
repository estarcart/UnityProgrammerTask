using UnityEngine;

public class WorldItemView : MonoBehaviour
{
    [SerializeField] private string uniqueId;
    [SerializeField] private string itemId;
    [SerializeField] private int amount = 1;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isScenePlaced = true;

    public string UniqueId => uniqueId;
    public string ItemId => itemId;
    public int Amount => amount;
    public bool IsScenePlaced => isScenePlaced;

    public void SetItem(string itemId, int amount, Sprite icon = null)
    {
        this.itemId = itemId;
        this.amount = amount;
        this.isScenePlaced = false;
        
        if (icon != null && spriteRenderer != null)
            spriteRenderer.sprite = icon;
    }

    public ItemInstance ToItemInstance()
    {
        return new ItemInstance(itemId, amount);
    }

    public void Reset()
    {
        itemId = string.Empty;
        amount = 0;
        uniqueId = string.Empty;
        isScenePlaced = false;
        if (spriteRenderer != null)
            spriteRenderer.sprite = null;
    }

    public void MarkAsCollected()
    {
        if (isScenePlaced && !string.IsNullOrEmpty(uniqueId))
        {
            WorldItemSaveController.Instance?.RegisterCollectedItem(uniqueId);
        }
    }

    public bool WasCollected()
    {
        if (!isScenePlaced || string.IsNullOrEmpty(uniqueId))
            return false;

        return WorldItemSaveController.Instance?.IsItemCollected(uniqueId) ?? false;
    }

    void Start()
    {
        if (isScenePlaced && WasCollected())
        {
            gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!isScenePlaced) return;

        bool isSceneInstance = !UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && 
                               gameObject.scene.IsValid();

        if (isSceneInstance && string.IsNullOrEmpty(uniqueId))
        {
            uniqueId = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

    [UnityEditor.MenuItem("CONTEXT/WorldItemView/Generate New Unique ID")]
    private static void GenerateNewUniqueId(UnityEditor.MenuCommand command)
    {
        var worldItem = (WorldItemView)command.context;
        worldItem.uniqueId = System.Guid.NewGuid().ToString();
        UnityEditor.EditorUtility.SetDirty(worldItem);
    }

    [UnityEditor.MenuItem("CONTEXT/WorldItemView/Regenerate All Scene IDs")]
    private static void RegenerateAllSceneIds(UnityEditor.MenuCommand command)
    {
        var allItems = FindObjectsByType<WorldItemView>(FindObjectsSortMode.None);
        foreach (var item in allItems)
        {
            if (item.isScenePlaced && !UnityEditor.PrefabUtility.IsPartOfPrefabAsset(item))
            {
                item.uniqueId = System.Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(item);
            }
        }
    }
#endif
}
