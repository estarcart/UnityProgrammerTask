using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private Image npcPortraitImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private Button choiceButtonPrefab;

    private List<Button> currentButtons = new();

    public void Show()
    {
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    public void SetNpcInfo(string npcName, Sprite portrait)
    {
        npcNameText.text = npcName;
        npcPortraitImage.sprite = portrait;
        npcPortraitImage.enabled = portrait != null;
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    public void SetChoices(List<DialogueChoice> choices, System.Action<int> onChoiceSelected)
    {
        foreach (var b in currentButtons)
        {
            Destroy(b.gameObject);
        }
        currentButtons.Clear();

        if (choices == null || choices.Count == 0)
        {
            var btn = Instantiate(choiceButtonPrefab, choicesContainer);
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = "Continue";
            btn.onClick.AddListener(() => onChoiceSelected(0));
            currentButtons.Add(btn);
            return;
        }

        for (int i = 0; i < choices.Count; i++)
        {
            int index = i;
            var btn = Instantiate(choiceButtonPrefab, choicesContainer);
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = choices[i].text;
            btn.onClick.AddListener(() => onChoiceSelected(index));
            currentButtons.Add(btn);
        }
    }
}
