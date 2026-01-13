using UnityEngine;

public class Kristall : MonoBehaviour
{
    // Script for a rarer crystal item
    CapsuleCollider crystalCol;

    void Start()
    {
        crystalCol = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionEnter(Collision collisionlayer)
    {
        if (collisionlayer.gameObject.tag == "Deposit")//Detect deposit and looses collision. Adds extra crystal points.
        {
            crystalCol.enabled = false;
            Destroy(gameObject);
        }
    }
}
