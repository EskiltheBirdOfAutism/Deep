using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    bool id_check = false;

    void Update()
    {
        if (NetworkManager.Singleton.LocalClientId == OwnerClientId && SceneManager.GetActiveScene().name == "SampleScene")
        {
            id_check = true;
            GameObject.Find("Main Camera").GetComponent<CameraCode>().follow = gameObject;
        }

        if (id_check == false) return;

        float _h_input = Input.GetAxisRaw("Horizontal");
        float _v_input = Input.GetAxisRaw("Vertical");

        Vector3 move_dir = transform.forward * _v_input + transform.right * _h_input;
        transform.position += move_dir * 0.05f;
    }
}
