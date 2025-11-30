using UnityEngine;

public class Foot : MonoBehaviour
{

    public bool isGrounded;
    void Update()
    {
        RaycastHit hit;

        // Cast from slightly above the bottom of the player
        Vector3 rayStart = transform.position;
        float rayDistance = 0.4f; // Adjust based on your player's height

        isGrounded = Physics.Raycast(rayStart, Vector3.down, out hit, rayDistance);
        // Optional: visualize the ray in Scene view
        Debug.DrawRay(rayStart, Vector3.down * rayDistance, isGrounded ? Color.green : Color.red);
    }
}
