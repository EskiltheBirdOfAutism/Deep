using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public enum MovementState
{
    Moving,
    Jumping,
    Falling
}
public class EnemyMovement : MonoBehaviour
{
    public MovementState currentState;

    Vector3 target_position = new Vector3(0f, 0f, 0f);
    public Camera camera_object;
    public Rigidbody rigid_body;
    public LayerMask wall;
    public GameObject orientation;
    public GameObject grid;
    public GameObject gridPrefab;
    public GameObject target;


    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;
    public GameObject jumpTarget;

    public bool isGrounded = false;
    public bool isStuck = false;
    public bool isJumping = false;
    private BoxCollider collider;

    private EnemyFalling enemyFalling;
    private EnemyJumping enemyJumping;
    private EnemyMove enemyMove;

    private void Start()
    {
        enemyFalling = GetComponent<EnemyFalling>();
        enemyJumping = GetComponent<EnemyJumping>();
        enemyMove = GetComponent<EnemyMove>();

        StartCoroutine(CheckIfStuck());
        lastPosition = transform.position;
        collider = GetComponent<BoxCollider>();
        currentState = MovementState.Moving;
    }
    private float time = 0f;
    public float _distance = 2f;
    void Update()
    {

        if (enemyFalling.timer <= 1 && !isJumping)
        {
            RestartMoving();
        }
        else if (IsGrounded() && !isStuck && !isJumping)
        {
            currentState = MovementState.Moving;
        }
        else if (IsGrounded() && isStuck)
        {
            currentState = MovementState.Jumping;
        }
        else if (!IsGrounded() && !isJumping) 
        {
            currentState = MovementState.Falling; 
        }

            switch (currentState)
            {
                case MovementState.Moving:
                if (grid != null)
                {
                    target_position = grid.GetComponent<GridCode>().path_pos;

                    _distance = Vector3.Distance(grid.GetComponent<PathfindingCode>().target.position, transform.position);
                }
                else RestartMoving();
                enemyMove.Move(target_position, _distance);
                    break;
                case MovementState.Jumping:
                if (!isJumping)
                {
                    StartCoroutine(enemyJumping.JumpRoutine());
                }
                break;
                case MovementState.Falling:
                enemyFalling.Fall();
                    break;
            }
    }
    private bool IsGrounded()
    {
        //Debug.Log("IsGrounded");
        Vector3 origin = collider.bounds.center;
        float maxDistance = collider.bounds.extents.y + 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, maxDistance, wall, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(origin, Vector3.down * maxDistance, Color.green);

            //float distanceToGround = hit.distance - collider.bounds.extents.y;
            if (hit.distance <= 0.75f)
            {
                isGrounded = true;
                return true;
            }
        }

        isGrounded = false;
        return false;
    }
    //  ||
    public bool isFalling;


    public float stuckThreshold = 0.05f; // minimal movement to consider "moving"
    public float stuckTime = 0.5f; // seconds before considered stuck

    private Vector3 lastPosition;
    private float timeStill = 0f;
    private IEnumerator CheckIfStuck()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("CheckIfStuck");
            // How far did we move since last frame
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            /*if (distanceMoved > 0.25f && isFalling && !isJumping)
            {
                Debug.LogError("WTF");
                rigid_body.linearVelocity = Vector3.zero;
                transform.position = new Vector3(jumpTarget.transform.position.x, jumpTarget.transform.position.y + 1.2f, jumpTarget.transform.position.z);
            }*/

            if (distanceMoved < stuckThreshold)
            {
                // Not moving enough → increment timer
                timeStill += Time.deltaTime;

                if (timeStill >= stuckTime)
                {
                    // Enemy is stuck
                    isStuck = true;
                    //OnStuck();
                    timeStill = 0f; // reset timer if needed
                }
            }
            else
            {
                isStuck = false;
                // Enemy is moving → reset timer
                timeStill = 0f;
            }

            lastPosition = transform.position;
        }
    }
    /*private void OnStuck()
    {
        Debug.Log(name + " is stuck!");
        FindJumpableBlock();
    }*/

    public void RestartMoving()
    {
        /*if (time < 1f) { return; }
        time = 0f;*/
        //if (currentState == MovementState.Moving) return;
        Debug.Log("RestartMoving");
        currentState = MovementState.Moving;
        StartCoroutine(timer(1f));
        float gridSize = 1f;

        Vector3 gridSpawnPos = new Vector3(
            Mathf.Round(transform.position.x / gridSize) * gridSize,
            Mathf.Round(transform.position.y / gridSize) * gridSize,
            Mathf.Round(transform.position.z / gridSize) * gridSize
        ); 
        if (grid != null) Destroy(grid);
        grid = Instantiate(gridPrefab, gridSpawnPos, Quaternion.identity);
        PathfindingCode pathFindingCode = grid.GetComponent<PathfindingCode>();
        pathFindingCode.target = target.transform;
        pathFindingCode.seeker = gameObject.transform;

        GridCode gridCode = grid.GetComponent<GridCode>();
        gridCode.player = target;
        gridCode.path_pos = grid.transform.position.normalized;
        isFalling = false;
    }

    private IEnumerator timer(float time)
    {
        yield return new WaitForSeconds(time);
    }
}

/*time += Time.deltaTime;
if (!IsGrounded() && currentState != MovementState.Jumping && currentState != MovementState.Falling && !isFalling)
{
    Debug.Log("Call Fall");
    isMoving = false;
    StartCoroutine(Fall());
}

switch (currentState)
{
    case MovementState.Jumping:
        if (!isJumping)
            Debug.Log("Call Jump");
        isMoving = false;
        StartCoroutine(JumpRoutine());
        return;


    case MovementState.Moving:
        //Debug.Log("Call Moving");

        if (grid != null)
        {
            target_position = grid.GetComponent<GridCode>().path_pos;

            _distance = Vector3.Distance(grid.GetComponent<PathfindingCode>().target.position, transform.position);
        }
        else RestartMoving();

        StartCoroutine(Move());
        break;

        /*case MovementState.Falling:
            Debug.LogError("FALLING CALLAS FRÅN STATE");
            StartCoroutine(Fall());
            return;*/