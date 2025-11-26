using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Hip : MonoBehaviour
{
    [SerializeField] private Movement_Direction movementDirection;
    private Rigidbody rb;
    private float speed = 200f;
    private Vector3 direction;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        direction = movementDirection.gameObject.transform.position - transform.position;
        direction.Normalize();
        // Apply the force towards the target
        rb.AddForce(new Vector3(direction.x, 0, direction.z) * speed);
    }
    public void Jump()
    {
        rb.AddForce(new Vector3(0, -direction.y * 4000, 0));
    }
}
