using UnityEngine;

public class NpcInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueDefinition dialogue;
    [SerializeField] private string startNodeId = "start";

    public string InteractionPrompt => "Talk";

    public void Interact(PlayerInteractionController interactor)
    {
        var dialogueController = FindFirstObjectByType<DialogueController>();
        if (dialogueController != null && dialogue != null)
        {
            dialogueController.StartDialogue(dialogue, startNodeId);
        }
    }
}
