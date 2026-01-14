using UnityEngine;

public class CrystalScript : MonoBehaviour
{
    // Script for crystal item

    bool destroyState = false;

    float realSize;
    float targetSize;
    float shrinkSpeed;

    void Start()
    {
        destroyState = false;
        realSize = 1;
        targetSize = 0.1f;
        shrinkSpeed = 0.4f;
    }

    private void Update()
    {
        if(destroyState)
        {
            realSize -= 1 * Time.deltaTime * shrinkSpeed;
            gameObject.transform.localScale = new Vector3(realSize, realSize, realSize);
            if(realSize < targetSize)
            {
                CurrencyManager.crystalPoints += 1;
                CurrencyManager.currencyPoints += 2;
                Destroy(gameObject);//Adds crystal points and destroys object when small enough.
            }
        }
    }

    private void OnCollisionStay(Collision collisionlayer)
    {
        if(collisionlayer.gameObject.tag == "Deposit")//Detect deposit and triggers destruction state.
        {
            destroyState = true;
        }
        else
        {
            destroyState = false;
        }
    }
}
