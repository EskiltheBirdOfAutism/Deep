using UnityEngine;

public class Hand : MonoBehaviour
{
    [HideInInspector] public ConfigurableJoint joint;
    private void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
    }
}
