using UnityEngine;

public class FollowHand : MonoBehaviour
{
    [SerializeField] private Hand rightHand;

    void Update()
    {
        transform.position = rightHand.transform.position;
        transform.rotation = rightHand.transform.rotation;
    }
}
