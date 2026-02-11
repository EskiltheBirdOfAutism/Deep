using UnityEngine;

public class CameraZoomer : MonoBehaviour
{
    void Update()
    {
        transform.localPosition += new Vector3(0, 0, 1) * Time.deltaTime;
    }
}
