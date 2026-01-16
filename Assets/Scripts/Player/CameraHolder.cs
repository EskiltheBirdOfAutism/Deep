using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    public Transform thirdPersonCamera; // Drag your actual camera here
    public float defaultDistance = 8f; // How far the camera normally sits from this holder
    public LayerMask collisionLayers;


    void LateUpdate()
    {
        // Raycast backwards from holder toward where camera sits
        RaycastHit hit;
        Vector3 direction = thirdPersonCamera.position - transform.position;

        if (Physics.Raycast(transform.position, direction, out hit, defaultDistance, collisionLayers))
        {
            // Hit a wall - move camera to hit point with small offset
            thirdPersonCamera.localPosition = new Vector3(-(hit.distance - 0.2f), 0, 0);
        }
        else
        {
            // No obstruction - use default distance
            thirdPersonCamera.localPosition = new Vector3(-defaultDistance, 0,0);
        }
    }
}
