using UnityEngine;

public class LockKnees : MonoBehaviour
{
    void Start()
    {
        ConfigurableJoint kneeJoint = GetComponent<ConfigurableJoint>();

        // Lock the axes you want to restrict
        kneeJoint.angularXMotion = ConfigurableJointMotion.Limited;
        kneeJoint.angularYMotion = ConfigurableJointMotion.Locked;
        kneeJoint.angularZMotion = ConfigurableJointMotion.Locked;

        // Set limits to prevent backward bending
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = 120f; // Max bend angle (forward only)
        kneeJoint.highAngularXLimit = limit;

        limit.limit = 0f; // Prevent backward bending
        kneeJoint.lowAngularXLimit = limit;
    }
}
