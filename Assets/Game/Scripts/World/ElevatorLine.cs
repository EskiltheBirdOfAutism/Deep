using UnityEngine;

public class ElevatorLine : MonoBehaviour
{
    private Vector3 startPoint; // Assign your first object here
    private Vector3 endPoint;   // Assign your second object here
    private LineRenderer lineRenderer;
    
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // We need two points for a line
        lineRenderer.startWidth = 0.1f; // Set line thickness
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = true; // Use world coordinates

        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit))
        {
            endPoint = hit.point;
        }

    }

    // Update is called once per frame
    void Update()
    {
        startPoint = transform.position;
        
        if (startPoint != null && endPoint != null)
        {
            // Set the positions of the line
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);
        }
    }
}
