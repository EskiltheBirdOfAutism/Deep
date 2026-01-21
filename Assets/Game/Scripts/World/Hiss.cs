using System.Collections;
using UnityEngine;

public class Hiss : MonoBehaviour
{
    private bool isDown;
    public Transform topLocation;
    [SerializeField] private Transform bottomLocation;
    [SerializeField] private float speed = 2f;
    [SerializeField] private int waitTime = 15;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    IEnumerator Start()
    {
        while (true)
        {
            yield return StartCoroutine(MoveElevator());
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator MoveElevator()
    {
        isDown = !isDown;
        Vector3 target = isDown ? topLocation.position : bottomLocation.position;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
            yield return new WaitForFixedUpdate(); 
        }
        rb.MovePosition(target); 
    }
}