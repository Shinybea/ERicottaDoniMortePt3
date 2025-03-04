using System;
using UnityEngine;

public class InteractionCommand : MonoBehaviour
{
    public event Action<GameObject, Transform> InteractionStart;
    public event Action<Transform> InteractionEnd;

    [SerializeField] private float interactionDistance;
    [SerializeField] private float interactionRadius;

    private GameObject _player;
    private Transform _currentTarget;
    
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Vector3 playerDirection = _player.transform.forward;
        Vector3 belowPlayer = Vector3.down;
        RaycastHit hit;
        
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (Physics.SphereCast(_player.transform.position, interactionRadius, playerDirection, out hit,
                    interactionDistance))
            {
                var target = hit.collider.gameObject;
                _currentTarget = target.transform;
                Interact(_currentTarget);
            }
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            if (Physics.SphereCast(_player.transform.position, interactionRadius, playerDirection, out hit,
                    interactionDistance))
            {
                var target = hit.collider.gameObject;
                _currentTarget = target.transform;
                StopInteraction(_currentTarget);
            }
        }
    }

    private void Interact(Transform target)
    {
        GameObject player = this.gameObject;
        Debug.Log("Interact with target");
        InteractionStart?.Invoke(player, target);
    }

    private void StopInteraction(Transform target)
    {
        Debug.Log("Stopped interaction");
        InteractionEnd?.Invoke(target);
        _currentTarget = null;
    }
}
