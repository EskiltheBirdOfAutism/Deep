using UnityEngine;

public class EnemyFalling : MonoBehaviour
{
    private EnemyMovement enemyMovement;
    private Rigidbody rigid_body;
    public float timer = 10;
    private void Update()
    {
        timer += Time.deltaTime;
    }
    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        rigid_body = GetComponent<Rigidbody>();
    }
    public void Fall()
    {
        Debug.Log(rigid_body.linearVelocity + "Fall is called");

        if (enemyMovement.isFalling && rigid_body.linearVelocity == Vector3.zero)
        {
            enemyMovement.isFalling = false;
            if (enemyMovement.grid == null)
            {
                timer = 0;
            }
        }

        if (enemyMovement.isFalling) return;

        enemyMovement.isFalling = true;

        enemyMovement.currentState = MovementState.Falling;
        Destroy(enemyMovement.grid);

        /*while (!enemyMovement.IsGrounded())
        {
            Debug.Log("While IsGrounded");
            yield return new WaitForSeconds(0.3f);
        }*/

        OnLanded();
        enemyMovement.isFalling = false;
    }

    private void OnLanded()
    {
        Debug.Log("OnLanded");
        enemyMovement.RestartMoving();
    }
}
