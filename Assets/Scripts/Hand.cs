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
    private void Update()
    {
        if (grabbedObject != null && grabAllowed!)
        {
            DestroyImmediate(grabbedObject.GetComponent<FixedJoint>());
            grabbedObject = null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        grabbedObject = other.gameObject;
        if (grabAllowed)
        {
            FixedJoint fj = grabbedObject.AddComponent<FixedJoint>();
            fj.connectedBody = rb;
            fj.breakForce = 200;
        }
    }
}
