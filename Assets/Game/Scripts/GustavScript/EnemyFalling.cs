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
        //Debug.Log(rigid_body.linearVelocity + "Fall is called");

        if (enemyMovement.isFalling && Vector3.Distance(rigid_body.linearVelocity, Vector3.zero) < 0.1f)
        {
            enemyMovement.isFalling = false;
        }

        enemyMovement.isFalling = true;
        if (enemyMovement.grid != null) Destroy(enemyMovement.grid);

        //OnLanded();
    }

    /*private void OnLanded()
    {
        Debug.Log("OnLanded");
        enemyMovement.RestartMoving();
    }*/
}
