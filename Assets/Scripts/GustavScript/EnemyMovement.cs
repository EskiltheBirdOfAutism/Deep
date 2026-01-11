using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public enum MovementState
{
    Moving,
    Jumping
}
public class PlayerMovement : MonoBehaviour
{
    public MovementState currentState;

    Vector3 target_position = new Vector3(0f, 0f, 0f);
    public Camera camera_object;
    public Rigidbody rigid_body;
    public LayerMask ground;
    public GameObject orientation;
    public GameObject grid;
    public GameObject gridPrefab;
    public GameObject target;


    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;
    public GameObject jumpTarget;

    private bool isJumping;

    private void Start()
    {
        currentState = MovementState.Moving;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            currentState = MovementState.Jumping;
        }

        switch (currentState)
        {
            case MovementState.Jumping:
                if (!isJumping)
                    StartCoroutine(JumpRoutine());
                return; 



            case MovementState.Moving:

                float _distance = 2f;

                if (grid != null)
                {
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
                break;
        }

    }
    IEnumerator JumpRoutine()
    {
        Destroy(grid);
        isJumping = true;

        Vector3 startPos = transform.position;
        Vector3 targetTop = jumpTarget.transform.position + Vector3.up * jumpTarget.transform.localScale.y;

        float timer = 0f;

        // Stop physics interference
        rigid_body.linearVelocity = Vector3.zero;
        rigid_body.isKinematic = true;

        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;
            float t = timer / jumpDuration;

            float height = 4f * jumpHeight * t * (1 - t);

            transform.position =
                Vector3.Lerp(startPos, targetTop, t) +
                Vector3.up * height;

            yield return null;
        }

        transform.position = targetTop;

        rigid_body.isKinematic = false;

        RestartMoving();
        isJumping = false;
    }

    private void RestartMoving()
    {
        float gridSize = 1f;

        Vector3 gridSpawnPos = new Vector3(
            Mathf.Round(transform.position.x / gridSize) * gridSize,
            Mathf.Round(transform.position.y / gridSize) * gridSize,
            Mathf.Round(transform.position.z / gridSize) * gridSize
        ); 
        grid = Instantiate(gridPrefab, gridSpawnPos, Quaternion.identity);
        PathfindingCode pathFindingCode = grid.GetComponent<PathfindingCode>();
        pathFindingCode.target = target.transform;
        pathFindingCode.seeker = gameObject.transform;

        GridCode gridCode = grid.GetComponent<GridCode>();
        gridCode.player = target;

        currentState = MovementState.Moving;
    }
}