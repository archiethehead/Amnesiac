
public interface Interactable {

    public bool IsInteractable { get; }
    public void Interact();
    public void Uninteract();
    public void ShowInteract();
    public void HideInteract();

}