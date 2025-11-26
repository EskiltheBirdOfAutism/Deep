using System.Threading;
using UnityEngine;

public class destroyRuble : MonoBehaviour
{
    float time = 7f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void FixedUpdate()
    {
        time -= Time.deltaTime;
        if (time <= 0.0f)
        {
            Destroy(gameObject);
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
