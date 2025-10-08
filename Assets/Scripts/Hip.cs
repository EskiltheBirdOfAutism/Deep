using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Hip : MonoBehaviour
{
    [SerializeField] private Movement_Direction movementDirection;
    private Rigidbody rb;
    private float speed = 100f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        Vector3 direction = movementDirection.gameObject.transform.position - transform.position;
        direction.Normalize();

        // Apply the force towards the target
        rb.AddForce(direction * speed);
    }
}
