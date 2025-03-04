using UnityEngine;

public class InteractionListener : MonoBehaviour
{
    private InteractionCommand _interactionCommand;
    private GameObject _currentPlayer;
    private Transform _endPos;
    
    
    void Start()
    {
        _currentPlayer = GameObject.FindGameObjectWithTag("Player");
        _interactionCommand = _currentPlayer.GetComponent<InteractionCommand>();

        _endPos = GetComponentInChildren<Transform>();
        
        _interactionCommand.InteractionStart += TeleportPlayer;
        _interactionCommand.InteractionEnd += StopInteractionTest;
    }

    void Update()
    {
        
    }

    private void TeleportPlayer(GameObject player, Transform target)
    {
        _currentPlayer = player; //the player who invoked the interaction successfully
        Debug.Log($"Player: {_currentPlayer.name}");
        _currentPlayer.transform.position = _endPos.position;
        _currentPlayer.transform.rotation = _endPos.rotation;

        _currentPlayer = null;
    }

    private void StopInteractionTest(Transform target)
    {
        Debug.Log("Interaction Stop Test");
    }

    private void OnDestroy()
    {
        _interactionCommand.InteractionStart -= TeleportPlayer;
        _interactionCommand.InteractionEnd -= StopInteractionTest;
    }
}
