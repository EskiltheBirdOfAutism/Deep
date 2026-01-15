using Unity.VisualScripting;
using UnityEngine;

public class ExplotionScript : MonoBehaviour
{
    public int explotionDamage;
    public int radius;

    public SphereCollider collider;
    private float time = 0;
    private void Start()
    {
        Destroy(gameObject, 0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerContoller>())
        {
            PlayerContoller playerContoller = other.GetComponent<PlayerContoller>();
            if (playerContoller != null)
            {
                playerContoller.TakeDamage(explotionDamage);
            }

            Vector3 direction = (transform.position - other.transform.position) * -1;
            Rigidbody rigidbody = other.GetComponentInParent<Hip>().GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddForce(direction * (explotionDamage * 4), ForceMode.Impulse);
            }
        } else if (other.GetComponent<EnemyAttack>())
        {
            EnemyAttack enemyAttack = other.GetComponent<EnemyAttack>();
            if (enemyAttack != null)
            {
                enemyAttack.TakeDamage(explotionDamage);
            }

            Vector3 direction = (transform.position - other.transform.position) * -1;
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddForce(direction * (explotionDamage * 1.2f), ForceMode.Impulse);
            }
        }
    }
}
