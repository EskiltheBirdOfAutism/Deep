using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
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
    private Vector3 direction;
    private float forceAmount = 30f;

    public void Attack()
    {
        if (enemyMovement.isAttacking) return;
        enemyMovement.isAttacking = true;
        if (enemyMovement.target != null)
        {
            direction = transform.position - enemyMovement.target.transform.position;
            direction = direction.normalized * -1;
        }

        float force = forceAmount += Random.RandomRange(-5, 20);
        if (rigid_body != null)
        {
            rigid_body.AddForce(direction * force, ForceMode.Impulse);
        }
        StartCoroutine(AttackDuration());
    }

    private IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(forceAmount/5);
        enemyMovement.isAttacking = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && enemyMovement.isAttacking)
        {

        }
    }
}
