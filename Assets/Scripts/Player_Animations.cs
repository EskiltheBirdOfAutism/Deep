using UnityEngine;

public class Player_Animations : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] private ConfigurableJoint R_Hip;
    [SerializeField] private ConfigurableJoint L_Hip;
    [SerializeField] private ConfigurableJoint R_Knee;
    [SerializeField] private ConfigurableJoint L_Knee;

    private Animations_Parts animParts_;
    [HideInInspector] public bool startWalkAnimation = true;
    void Start()
    {
        animParts_ = GetComponentInChildren<Animations_Parts>();
    }

    void Update()
    {
        if (startWalkAnimation)
        {
            print("Startet Walk");
            R_Hip.targetRotation = animParts_.R_Hip.transform.localRotation;
            L_Hip.targetRotation = animParts_.L_Hip.transform.localRotation;
            R_Knee.targetRotation = animParts_.R_Knee.transform.localRotation;
            L_Knee.targetRotation = animParts_.L_Knee.transform.localRotation;
        }
    }
}
