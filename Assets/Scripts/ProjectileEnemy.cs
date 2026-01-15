using System.Collections;
using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(WaitAndDestroy());
    }
    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(Random.Range(5, 16));
        Destroy(gameObject);
    }
}
