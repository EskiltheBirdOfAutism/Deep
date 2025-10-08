using UnityEngine;

public class Foot : MonoBehaviour
{
    public bool isGrounded;
    void Update()
    {
        RaycastHit hit;

        Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, 0.1f);
        if(hit.collider == null)
        {
            isGrounded = false;
        }else
        {
            isGrounded = true;
        }
    }
}
