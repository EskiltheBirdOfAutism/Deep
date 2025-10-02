using UnityEngine;

public class CameraCode : MonoBehaviour
{
    public GameObject follow;

    // Update is called once per frame
    void Update()
    {
        if (follow != null)
        {
            if (GameObject.Find(follow.name))
            {
                transform.position = follow.transform.position + new Vector3(0, 1, -10);
            }
        }
    }
}
