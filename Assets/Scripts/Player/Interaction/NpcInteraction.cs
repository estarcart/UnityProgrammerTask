using UnityEngine;

public class NpcInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueDefinition dialogue;
    [SerializeField] private string startNodeId = "start";
    [SerializeField] private GameObject interactionIndicator;

    public string InteractionPrompt => "Talk";

    private void Start()
    {
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(false);
        }
    }

    public void Interact(PlayerInteractionController interactor)
    {
        var dialogueController = FindFirstObjectByType<DialogueController>();
        if (dialogueController != null && dialogue != null)
        {
            dialogueController.StartDialogue(dialogue, startNodeId);
        }
    }

    public void ShowInteractionIndicator()
    {
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(true);
        }
    }

    public void HideInteractionIndicator()
    {
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(false);
        }
    }
}
