using UnityEngine;

public class InteractionTeleport : MonoBehaviour
{
    private InteractionCommand _interactionCommand;
    private GameObject _currentPlayer;
    private Transform _endPos;
    
    void Start()
    {
        _currentPlayer = GameObject.FindGameObjectWithTag("Player");
        _interactionCommand = _currentPlayer.GetComponent<InteractionCommand>();

        _endPos = transform.GetChild(0); //get the first child's transform
        
        _interactionCommand.InteractionTeleport += TeleportPlayer;
    }

    private void TeleportPlayer(GameObject player, Transform target)
    {
        _currentPlayer = player; 
        Debug.Log($"Player: {_currentPlayer.name}");
        _currentPlayer.transform.position = _endPos.position;
        _currentPlayer.transform.rotation = _endPos.rotation;

        _currentPlayer = null;
    }
    private void OnDestroy()
    {
        _interactionCommand.InteractionTeleport -= TeleportPlayer;
    }
}
