using Unity.VisualScripting;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
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
        if (enemyMovement.grid == null)
        {
            enemyMovement.RestartMoving();
            return;
        }
        //if (isMoving) yield break;
        //isMoving = true;
        if (_distance >= 1f && enemyMovement.currentState == MovementState.Moving)
        {
            //CheckIfStuck();
            Debug.Log("While Move");
            Vector3 _direction = (target_position - transform.position);
            Vector3 _normalized_direction = _direction.normalized;
            orientation.transform.forward = _normalized_direction;

            if (!rigid_body.isKinematic)
            {
                rigid_body.linearVelocity += ((orientation.transform.forward * 400f * Time.deltaTime) - rigid_body.linearVelocity) * 0.5f;
            }
        }
        //CheckIfStuck();

        if (!rigid_body.isKinematic)
        {
            rigid_body.linearVelocity += ((new Vector3(0f, 0f, 0f)) - rigid_body.linearVelocity) * 0.5f;
        }
    }
}
