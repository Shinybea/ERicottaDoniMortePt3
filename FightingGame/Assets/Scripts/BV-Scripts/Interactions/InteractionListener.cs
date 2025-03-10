using UnityEngine;

public class InteractionListener : MonoBehaviour
{
    private InteractionCommand _interactionCommand;
    private GameObject _currentPlayer;
    
    void Start()
    {
        _currentPlayer = null;
        _interactionCommand = null;
    }

    private void Interact(GameObject player, Transform target)
    {
        _currentPlayer = player; 
        Debug.Log($"Player: {_currentPlayer.name} is interacting with {target.name} = {this.gameObject.name}");
        
    }

    private void StopInteraction(GameObject player, Transform target)
    {
        Debug.Log($"Player: {_currentPlayer.name} has stopped interacting with {target.name} = {this.gameObject.name}");
        DeregisterInteraction();
        _currentPlayer = null;
    }

   
    public void RegisterInteraction(InteractionCommand interactionCommand)
    {
        if (_interactionCommand != null)
        {
            _interactionCommand.Interaction1 -= Interact;
            _interactionCommand.EndInteraction -= StopInteraction;
        }
        _interactionCommand = interactionCommand;
        
        if (_interactionCommand != null)
        {
            _interactionCommand.Interaction1 += Interact;
            _interactionCommand.EndInteraction += StopInteraction;
        }
    }

    public void DeregisterInteraction()
    {
        if (_interactionCommand != null)
        {
            _interactionCommand.Interaction1 -= Interact;
            _interactionCommand.EndInteraction -= StopInteraction;
            _interactionCommand = null;
        }
    }
}
