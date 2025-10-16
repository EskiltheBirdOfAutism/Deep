using Unity.VisualScripting;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private FixedJoint joint;
    private Rigidbody rb;
    [HideInInspector] public bool grabAllowed = false;
    private void Awake()
    {
        joint = GetComponent<FixedJoint>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (grabAllowed!)
        {
            //???
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>())
        {
            if (grabAllowed)
            {
                joint.connectedBody = collision.gameObject.GetComponent<Rigidbody>();
            }
        }
    }
}
