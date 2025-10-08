using UnityEngine;

public class CopyMovement : MonoBehaviour
{
    [SerializeField] private Transform targetLimb ;
    private ConfigurableJoint joint;
    [SerializeField] private bool mirror = false;
    [SerializeField] private bool foot = false;
    void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
    }
    void Update()
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
}
