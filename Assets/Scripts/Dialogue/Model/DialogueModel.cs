using UnityEngine;
using System;

public class DialogueModel
{
    public event Action OnDialogueChanged;
    public event Action OnDialogueEnded;

    private DialogueDefinition currentDefinition;
    private DialogueNode currentNode;

    public DialogueDefinition CurrentDefinition => currentDefinition;
    public DialogueNode CurrentNode => currentNode;

    public bool IsActive => currentDefinition != null && currentNode != null;

    public void StartDialogue(DialogueDefinition definition, string startNodeId = "start")
    {
        currentDefinition = definition;
        currentNode = definition?.GetNode(startNodeId);

        if (currentNode == null)
        {
            EndDialogue();
            return;
        }

        OnDialogueChanged?.Invoke();
    }

    public void ChooseOption(int choiceIndex)
    {
        if (!IsActive || currentNode == null) return;
        if (choiceIndex < 0 || choiceIndex >= currentNode.choices.Count) return;

        var choice = currentNode.choices[choiceIndex];

        if (choice.closesDialogue || string.IsNullOrEmpty(choice.nextNodeId))
        {
            EndDialogue();
            return;
        }

        currentNode = currentDefinition.GetNode(choice.nextNodeId);

        if (currentNode == null)
        {
            EndDialogue();
            return;
        }

        OnDialogueChanged?.Invoke();
    }

    public void EndDialogue()
    {
        currentDefinition = null;
        currentNode = null;
        OnDialogueEnded?.Invoke();
    }
}
