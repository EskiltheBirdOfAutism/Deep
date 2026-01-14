using UnityEngine;

public class TowerEnemy : MonoBehaviour
{

    bool destroyState = false;

    float realSize;
    float targetSize;
    float shrinkSpeed;

    public GameObject projectile;
    float countDown;
    float coolDownTimer;

    void Start()
    {
        destroyState = false;
        realSize = 3;
        targetSize = 0.1f;
        shrinkSpeed = 0.4f;
        coolDownTimer = 1;
    }

    private void Update()
    {
        Instantiate(projectile);
        if (destroyState)
        {
            realSize -= 1 * Time.deltaTime * shrinkSpeed;
            gameObject.transform.localScale = new Vector3(realSize, realSize, realSize);
            if (realSize < targetSize)
            {
                CurrencyManager.crystalPoints += 5;
                CurrencyManager.currencyPoints += 14;
                Destroy(gameObject);
            }
            countDown = coolDownTimer;
        }
        else if(countDown < 0)
        {
            Instantiate(projectile);
            coolDownTimer = Random.Range(3, 7);
            countDown = coolDownTimer;
        }
        countDown -= 1 * Time.deltaTime;
    }

    private void OnCollisionStay(Collision collisionlayer)
    {
        if (collisionlayer.gameObject.tag == "Deposit")
        {
            destroyState = true;
        }
        else
        {
            destroyState = false;
        }
    }
}
