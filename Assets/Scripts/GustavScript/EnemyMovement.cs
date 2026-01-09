using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    Vector3 target_position = new Vector3(0f, 0f, 0f);
    public Camera camera_object;
    public Rigidbody rigid_body;
    public LayerMask ground;
    public GameObject orientation;
    public GameObject grid;
    public GameObject target;

    void Update()
    {
        float _distance = 2f;

        if (GameObject.Find("Grid") != null)
        {
            grid = GameObject.Find("Grid");

            target_position = grid.GetComponent<GridCode>().path_pos;

            _distance = Vector3.Distance(grid.GetComponent<PathfindingCode>().target.position, transform.position);
        }

        if (_distance >= 1f)
        {
            Vector3 _direction = (target_position - transform.position);
            Vector3 _normalized_direction = _direction.normalized;
            orientation.transform.forward = _normalized_direction;

            rigid_body.linearVelocity += ((orientation.transform.forward * 400f * Time.deltaTime) - rigid_body.linearVelocity) * 0.5f;
        }
        else
        {
            rigid_body.linearVelocity += ((new Vector3(0f, 0f, 0f)) - rigid_body.linearVelocity) * 0.5f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = camera_object.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Ground")
            {
                target.transform.position = new Vector3(hit.collider.transform.position.x, 0f, hit.collider.transform.position.z);
            }
        }
    }
}