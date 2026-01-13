using UnityEngine;

public class CopyMovement : MonoBehaviour
{
    [SerializeField] private Transform targetLimb ;
    private ConfigurableJoint joint;
    [SerializeField] private bool mirror = false;
    [SerializeField] private bool foot = false;
    private Foot footObject;
    void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
        footObject = GetComponentInParent<PlayerContoller>().GetComponentInChildren<Foot>();
    }
    void Update()
    {
        if(footObject.isGrounded)
        {
            if (foot)
            {
                joint.targetPosition = targetLimb.position;
            }
            else
            {
                if (!mirror)
                {
                    joint.targetRotation = targetLimb.rotation;
                }
                else
                {
                    joint.targetRotation = Quaternion.Inverse(targetLimb.rotation);
                }
            }
        }
        else
        {
            if (foot)
            {
                joint.targetPosition = Vector3.zero;
            }
            else
            {
                joint.targetRotation = Quaternion.identity;
            }
        }


    }
}
