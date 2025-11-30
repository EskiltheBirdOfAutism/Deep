using Unity.VisualScripting;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private FixedJoint joint;
    private Rigidbody rb;
    private GameObject grabbedObject;
    [HideInInspector] public bool grabAllowed = false;
    private void Awake()
    {
        joint = GetComponent<FixedJoint>();
        rb = GetComponent<Rigidbody>();
    }
    public void Release()
    {
        if (grabbedObject != null)
        {
            DestroyImmediate(grabbedObject.GetComponent<FixedJoint>());
            grabbedObject = null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (grabAllowed)
        {
            grabbedObject = other.gameObject;
            if (grabbedObject.layer != 10)
            {
                FixedJoint fj = grabbedObject.AddComponent<FixedJoint>();
                fj.connectedBody = rb;
                fj.breakForce = 200;
            }
        }
    }
}
