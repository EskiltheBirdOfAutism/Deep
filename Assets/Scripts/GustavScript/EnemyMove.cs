using Unity.VisualScripting;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float speed = 5f;
    private float distanceXZ;
    private float distanceX;
    private float distanceZ;
    private Rigidbody rigid_body;
    private GameObject orientation;
    private EnemyMovement enemyMovement;
    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        orientation = transform.GetChild(0).gameObject;
        rigid_body = GetComponent<Rigidbody>();
    }
    public void Move(Vector3 target_position, float _distance)
    {
        if (enemyMovement.grid != null)
        {
            Vector3 gridPos = enemyMovement.gridCode.path_pos;

            // Map grid (x, y)  world (x, z)
            target_position = new Vector3(
                gridPos.x,
                transform.position.y, // keep height
                gridPos.y
            );

            // Separate distances
            distanceX = Mathf.Abs(target_position.x - transform.position.x);
            distanceZ = Mathf.Abs(target_position.z - transform.position.z);

            // Optional combined distance (2D XZ plane)
            distanceXZ = Mathf.Sqrt(distanceX * distanceX + distanceZ * distanceZ);

            // Debug if needed
            // Debug.Log($"Distance X: {distanceX}, Distance Z: {distanceZ}, Distance XZ: {distanceXZ}");
        }

        if (enemyMovement.grid == null)
        {
            //Debug.Log("REstart from move");
            enemyMovement.RestartMoving();
            return;
        }

        // DIAGNOSTIC: Check why enemy isn't moving
        if ((distanceX < 1f && distanceZ < 1f) && _distance < 1f)
        {
            //Debug.Log($"Distance too small: {_distance} (needs >= 1)");
            return;
        }

        if (enemyMovement.currentState != MovementState.Moving)
        {
            Debug.Log($"Wrong state: {enemyMovement.currentState} (needs Moving)");
            return;
        }

        if (rigid_body.isKinematic)
        {
            Debug.LogWarning("Rigidbody is kinematic - cannot move!");
            return;
        }

        // Calculate movement
        Vector3 _direction = (enemyMovement.target.transform.position - transform.position);
        Vector3 _normalized_direction = _direction.normalized;

        // Check if direction is valid
        if (_normalized_direction.magnitude < 0.1f)
        {
            Debug.LogWarning("Invalid direction!");
            return;
        }

        orientation.transform.forward = _normalized_direction;


        Vector3 targetVelocity = orientation.transform.forward * speed;

        // Apply smooth acceleration
        rigid_body.linearVelocity = Vector3.Lerp(rigid_body.linearVelocity, targetVelocity, 0.1f);
    }
}
