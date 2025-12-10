using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private DialogueView dialogueView;

    private DialogueModel model;
    private bool isLocked;

    void Awake()
    {
        model = new DialogueModel();
        model.OnDialogueChanged += HandleDialogueChanged;
        model.OnDialogueEnded += HandleDialogueEnded;
    }

    public bool IsDialogueActive => model.IsActive;

    public void StartDialogue(DialogueDefinition definition, string startNodeId = "start")
    {
        if (isLocked) return;
        isLocked = true;

        model.StartDialogue(definition, startNodeId);
        if (model.IsActive)
        {
            dialogueView.Show();
        }
        else
        {
            isLocked = false;
        }
    }

    void HandleDialogueChanged()
    {
        var def = model.CurrentDefinition;
        var node = model.CurrentNode;
        if (def == null || node == null) return;

        dialogueView.SetNpcInfo(def.npcName, def.npcPortrait);
        dialogueView.SetDialogueText(node.text);
        dialogueView.SetChoices(node.choices, OnChoiceSelected);
    }

    void HandleDialogueEnded()
    {
        dialogueView.Hide();
        isLocked = false;
    }

    void OnChoiceSelected(int index)
    {
        model.ChooseOption(index);
    }
}
