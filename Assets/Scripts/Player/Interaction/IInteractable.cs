public interface IInteractable
{
    string InteractionPrompt { get; }
    void Interact(PlayerInteractionController interactor);
    void ShowInteractionIndicator();
    void HideInteractionIndicator();
}

