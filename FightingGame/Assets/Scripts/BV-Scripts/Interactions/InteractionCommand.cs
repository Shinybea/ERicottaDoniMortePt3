using System;
using UnityEngine;

public class InteractionCommand : MonoBehaviour
{
    public event Action<GameObject, Transform> InteractionTeleport;
    
    public event Action<GameObject, Transform> Interaction1;
    public event Action<GameObject, Transform> Interaction2;
    public event Action<GameObject, Transform> EndInteraction;



    [SerializeField] private float interactionDistance;
    [SerializeField] private float interactionRadius;

    private GameObject _player;
    private Transform _currentTarget;
    
    void Start()
    {
        _player = this.gameObject;
    }

    void Update()
    {
        Vector3 playerDirection = _player.transform.forward;
        Vector3 belowPlayer = Vector3.down;
        RaycastHit hit;
        
        if (Input.GetKeyUp(KeyCode.T))
        {
            Debug.Log("pressed T");
            if (Physics.SphereCast(_player.transform.position, interactionRadius, playerDirection, out hit,
                    interactionDistance))
            {
                var target = hit.collider.gameObject;
                _currentTarget = target.transform;
                Teleport(_currentTarget);
            }
        }
        
        if (Physics.SphereCast(_player.transform.position, interactionRadius, playerDirection, out hit,
                interactionDistance))
        {
            var target = hit.collider.gameObject;
            _currentTarget = target.transform;
            InteractionListener listener = target.GetComponent<InteractionListener>();
            if (listener == null) return;
            if (listener != null) { listener.RegisterInteraction(this); }

            if (Input.GetKeyUp(KeyCode.Z))
            {
                Debug.Log("Pressed Z");
                Interact1(_currentTarget); 
            }

            if (Input.GetKeyUp(KeyCode.X))
            {
                Debug.Log("Pressed X");
                Interact2(_currentTarget);
                
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                Debug.Log("Pressed C");
                EndInteract(_currentTarget); 
            }
        }
    }

    private void Teleport(Transform target)
    {
        GameObject player = this.gameObject;
        Debug.Log("Call Teleport");
        InteractionTeleport?.Invoke(player, target);
    }

    private void Interact1(Transform target)
    {
        GameObject player = this.gameObject;
        Interaction1?.Invoke(player, target);
        _currentTarget = target;
    }
    private void Interact2(Transform target)
    {
        GameObject player = this.gameObject;
        Interaction2?.Invoke(player, target);
        _currentTarget = target;
    }
    private void EndInteract(Transform target)
    {
        GameObject player = this.gameObject;
        EndInteraction?.Invoke(player, target);
        _currentTarget = null;
    }
    
    
    //DEBUGGING
    void OnDrawGizmos()
    {
        if (_player == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_player.transform.position, interactionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(_player.transform.position, _player.transform.position + _player.transform.forward * interactionDistance);
    }
}
