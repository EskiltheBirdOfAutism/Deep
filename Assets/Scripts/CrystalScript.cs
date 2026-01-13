using UnityEngine;

public class CrystalScript : MonoBehaviour
{
    // Script for crystal item
    CapsuleCollider crystalCol;

    void Start()
    {
        crystalCol = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionEnter(Collision collisionlayer)
    {
        if(collisionlayer.gameObject.tag == "Deposit")//Detect deposit and looses collision. Adds crystal points.
        {
            crystalCol.enabled = false;
            Destroy(gameObject);
        }
    }
}
