public interface IPlayerInteractable
{

    string InteractionName { get; }

    void Interact(Player player);
    
}