using Unity.Netcode;
using UnityEngine;

public class TowerEnemy : NetworkBehaviour
{

    bool destroyState = false;

    float realSize;
    float targetSize;
    float shrinkSpeed;

    public GameObject projectile;
    float countDown;
    float coolDownTimer;

    AudioSource soundSource;

    void Start()
    {
        destroyState = false;
        realSize = 3;
        targetSize = 0.1f;
        shrinkSpeed = 0.4f;
        coolDownTimer = 10;
        countDown = coolDownTimer;
        soundSource = GetComponent<AudioSource>();
        soundSource.mute = true;
    }

    private void Update()
    {
        if (destroyState)
        {
            realSize -= 1 * Time.deltaTime * shrinkSpeed;
            gameObject.transform.localScale = new Vector3(realSize, realSize, realSize);
            if (realSize < targetSize)
            {
                CurrencyManager.crystalPoints += 1;
                CurrencyManager.currencyPoints += 2;
                CmdDestroyThis();//Adds crystal points and destroys object when small enough.
            }
            countDown = coolDownTimer;
        }
        if(countDown < 0)
        {
            Instantiate(projectile, transform.position,Quaternion.identity);
            coolDownTimer = Random.Range(3, 7);
            countDown = coolDownTimer;
        }
        countDown -= Time.deltaTime;
    }

    private void OnCollisionStay(Collision collisionlayer)
    {
        if (collisionlayer.gameObject.tag == "Deposit")//Detect deposit and triggers destruction state.
        {
            destroyState = true;
            soundSource.mute = false;
        }
        else
        {
            destroyState = false;
            soundSource.mute = true;
        }
    }
    void CmdDestroyThis()
    {
        if (NetworkManager.Singleton.IsHost == true)
        {
            NetworkObject.Despawn();
        }
        else
        {
            CmdDestroyThisServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void CmdDestroyThisServerRpc()
    {
        NetworkObject.Despawn();
    }
}
