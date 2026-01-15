using UnityEngine;

public class MineScript : MonoBehaviour
{
    public GameObject explotionPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("NonGrabable"))
        {
            Explode();
        }
    }

    private int explotionRadius;
    private int damage = 10;
    private void Explode()
    {
        explotionRadius = Random.RandomRange(2, 5);
        damage -= ((int)explotionRadius);

        GameObject explotion = Instantiate(explotionPrefab, transform.position, transform.rotation);
        ExplotionScript explotionScript = explotion.GetComponent<ExplotionScript>();
        SphereCollider sphereCollider = explotion.GetComponent<SphereCollider>();
        sphereCollider.radius = explotionRadius;
        explotionScript.explotionDamage = damage;
        explotionScript.radius = explotionRadius;
        Destroy(gameObject);
    }
}
