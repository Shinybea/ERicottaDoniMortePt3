using UnityEngine;
using System.Collections.Generic;

public class MultiTargetCamera : MonoBehaviour
{
    [SerializeField] List<Transform> targets;

    [SerializeField] Vector3 offset = new Vector3(0, 5, -10);
    [SerializeField] float smoothTime = 0.2f;

    [SerializeField] float minZoom = 5f;
    [SerializeField] float maxZoom = 20f;
    [SerializeField] float zoomLimiter = 20f;

    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (targets.Count == 0)
            return;

        Move();
        Zoom();
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    void Zoom()
    {
        float greatestDistance = GetGreatestDistance();

        float newZoom = Mathf.Lerp(minZoom, maxZoom, greatestDistance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime * 5f);
    }


    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
            return targets[0].position;

        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }

    float GetGreatestDistance()
    {
        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return Mathf.Max(bounds.size.x, bounds.size.y);
    }
}
