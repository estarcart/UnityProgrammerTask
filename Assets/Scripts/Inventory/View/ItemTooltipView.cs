using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipView : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;

    public void Show(ItemDefinition def, Vector3 screenPosition)
    {
        if (def == null) return;
        root.SetActive(true);
        titleText.text = def.displayName;
        descriptionText.text = def.description;
        transform.position = screenPosition;
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
