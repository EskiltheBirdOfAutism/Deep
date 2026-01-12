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
    Jumping
}
public class EnemyMovement : MonoBehaviour
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
    private BoxCollider collider;

    private void Start()
    {
        lastPosition = transform.position;
        collider = GetComponent<BoxCollider>();
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
                CheckIfStuck();

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
    public float stuckThreshold = 0.05f; // minimal movement to consider "moving"
    public float stuckTime = 1.0f; // seconds before considered stuck

    private Vector3 lastPosition;
    private float timeStill = 0f;
    private void CheckIfStuck()
    {
        // How far did we move since last frame
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved < stuckThreshold)
        {
            // Not moving enough → increment timer
            timeStill += Time.deltaTime;

            if (timeStill >= stuckTime)
            {
                // Enemy is stuck
                OnStuck();
                timeStill = 0f; // reset timer if needed
            }
        }
        else
        {
            // Enemy is moving → reset timer
            timeStill = 0f;
        }

        lastPosition = transform.position;
    }
    private void OnStuck()
    {
        Debug.Log(name + " is stuck!");
        FindJumpableBlock();
    }

    private void FindJumpableBlock()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit))
        {
            GameObject currentBlock = hit.collider.gameObject;
            jumpTarget = GetTopBlock(currentBlock);

            Debug.Log("Final jump target: " + (jumpTarget != null ? jumpTarget.name : "None"));
        }

        // Debug line to see the ray
        Debug.DrawRay(transform.position, direction * 100f, Color.red);

        currentState = MovementState.Jumping;
    }

    public GameObject HasBlockAbove(GameObject block)
    {
        // Start slightly below the top of the block
        Vector3 origin = block.transform.position + Vector3.up * 0.5f;
        float rayDistance = 1.1f;

        // Cast ray upwards and collect all hits
        List<GameObject> blocksAbove = new List<GameObject>(Physics.RaycastAll(origin, Vector3.up, rayDistance).Select(h => h.collider.gameObject));
        Debug.DrawRay(origin, Vector3.up * 0.5f, Color.red);
        // Debug
        Debug.Log("Blocks above count: " + blocksAbove.Count);

        if (blocksAbove.Count > 0)
            return blocksAbove[blocksAbove.Count - 1]; // top-most block above

        return null; // no block above
    }


    private GameObject GetTopBlock(GameObject startingBlock)
    {
        GameObject currentBlock = startingBlock;
        GameObject aboveBlock;

        int maxIterations = 50; // safety limit
        int count = 0;

        while (count < maxIterations)
        {
            count++;

            aboveBlock = HasBlockAbove(currentBlock);
            Debug.Log("above block" + aboveBlock);

            if (aboveBlock == null)
                return currentBlock; // top of stack reached

            // Optional: check for roof blocking jump
            float maxJumpHeight = jumpHeight + 0.5f; // buffer
            Vector3 roofCheckOrigin = aboveBlock.transform.position + Vector3.up * (aboveBlock.transform.localScale.y / 2 + 0.01f);

            if (Physics.Raycast(roofCheckOrigin, Vector3.up, maxJumpHeight))
            {
                // roof above prevents jumping higher
                return currentBlock;
            }

            currentBlock = aboveBlock;
        }

        Debug.LogWarning("GetTopBlock exceeded max iterations!");
        return currentBlock;
    }


    IEnumerator JumpRoutine()
    {
        Destroy(grid);
        isJumping = true;

        Vector3 startPos = transform.position;
        Vector3 targetTop = jumpTarget.transform.position + Vector3.up * (jumpTarget.transform.localScale.y / 2 + 0.01f);
        
        float timer = 0f;

        // Stop physics interference
        rigid_body.linearVelocity = Vector3.zero;
        rigid_body.isKinematic = true;
        collider.enabled = false;

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
        float yPos = Mathf.Round(transform.position.y + 0.5f);

        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);

        rigid_body.isKinematic = false;
        collider.enabled = true;

        RestartMoving();
        isJumping = false;
    }

    private void RestartMoving()
    {
        StartCoroutine(timer(1f));
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
        gridCode.path_pos = grid.transform.position.normalized;

        currentState = MovementState.Moving;
    }

    private IEnumerator timer(float time)
    {
        yield return new WaitForSeconds(time);
    }
}