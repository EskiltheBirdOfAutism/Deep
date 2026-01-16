using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
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
    public GridCode gridCode;
    public GameObject gridPrefab;
    public GameObject target;


    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;
    public GameObject jumpTarget;

    public bool isAttacking = false;
    public bool isGrounded = false;
    public bool isStuck = false;
    public bool isJumping = false;
    private BoxCollider collider;

    public List<AudioClip> gnomeSounds = new List<AudioClip>();
    public AudioSource secondSource;
    private EnemyFalling enemyFalling;
    private EnemyJumping enemyJumping;
    private EnemyMove enemyMove;
    private EnemyAttack enemyAttack;

    private IEnumerator Speedup()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.RandomRange(10, 30));
            enemyMove.speed *= 1.2f;
        }
    }

    public List<PlayerContoller> players = new List<PlayerContoller>();
    private void FindAndSort()
    {
        // Find all PlayerMovement scripts in the scene
        PlayerContoller[] foundPlayers = FindObjectsOfType<PlayerContoller>();

        // Clear list and add them
        players.Clear();
        players.AddRange(foundPlayers);

        // Sort by distance to THIS object
        players.Sort((a, b) =>
        {
            float distA = Vector3.Distance(transform.position, a.transform.position);
            float distB = Vector3.Distance(transform.position, b.transform.position);
            return distA.CompareTo(distB);
        });
    }
    private void Start()
    {
        enemyAttack = GetComponent<EnemyAttack>();
        enemyFalling = GetComponent<EnemyFalling>();
        enemyJumping = GetComponent<EnemyJumping>();
        enemyMove = GetComponent<EnemyMove>();

        StartCoroutine(CheckIfStuck());
        lastPosition = transform.position;
        collider = GetComponent<BoxCollider>();
        rigid_body = GetComponent<Rigidbody>();
        currentState = MovementState.Moving;
        StartCoroutine(Speedup());
    }
    private float time = 0f;
    public float _distance = 2f;
    void Update()
    {
        GnomeSounds();

        if (target == null)
        {
            FindAndSort();
            if (players.Count > 0 && players[0].transform.parent != null)
            {
                target = players[0].transform.parent.gameObject;
            }
            return;
        }
        else
        {
            transform.GetChild(1).transform.LookAt(target.transform.position); 
        }

        

        _distance = Vector3.Distance(target.transform.position, transform.position);

        if (_distance < 3 && !isAttacking)
        {
            enemyAttack.Attack();
            return;
        }

        bool grounded = IsGrounded();

        if (enemyFalling.timer <= 1 && !isJumping)
        {
            RestartMoving();
        }
        else if (grounded && !isStuck && !isJumping)
        {
            currentState = MovementState.Moving;
        }
        else if (grounded && isStuck)
        {
            currentState = MovementState.Jumping;
        }
        else if (!grounded && !isJumping) 
        {
            currentState = MovementState.Falling; 
        }
        if (isStuck && !isJumping)
        {
            transform.position = new Vector3(
                Mathf.Round(transform.position.x),
                Mathf.Round(transform.position.y),
                Mathf.Round(transform.position.z)
            );
            //transform.rotation = Quaternion.identity;
        }
        
        switch (currentState) 
            {
          case MovementState.Moving:
            // SIMPLIFIED: Just follow the target directly
            if (target != null)
            {
                target_position = target.transform.position;
                
                //Debug.Log($"Following target at: {target_position}, Distance: {_distance}");
                enemyMove.Move(target_position, _distance);
            }
            else
            {
                Debug.LogError("No target set!");
            }
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
            //Debug.DrawRay(origin, Vector3.down * maxDistance, Color.green);

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


    public float stuckThreshold = 0.075f; // minimal movement to consider "moving"
    public float stuckTime = 0.5f; // seconds before considered stuck

    private Vector3 lastPosition;
    private float timeStill = 0f;
    private IEnumerator CheckIfStuck()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            // How far did we move since last check (0.25 seconds ago)
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            if (distanceMoved < stuckThreshold)
            {
                // Not moving enough → increment timer
                timeStill += 0.25f;  // ✓ Add the actual wait time, not frame time!

                if (timeStill >= stuckTime)
                {
                    // Enemy is stuck
                    isStuck = true;
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

    public void RestartMoving()
    {
        /*if (time < 1f) { return; }
        time = 0f;*/
        //if (currentState == MovementState.Moving) return;
        //Debug.Log("RestartMoving");
        currentState = MovementState.Moving;
        //StartCoroutine(timer(1f));
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
        _distance = Vector3.Distance(grid.transform.position, transform.position);

        gridCode = grid.GetComponent<GridCode>();
        gridCode.player = target;
        gridCode.path_pos = gridSpawnPos; 
        isFalling = false;
    }

    private IEnumerator timer(float time)
    {
        yield return new WaitForSeconds(time);
    }
    private float songTimer = 0f;
    private bool waitingForNextSong = false;
    public float timeBetweenSongs = 10f;

    private void GnomeSounds()
    {        // Song finished start cooldown
        if (secondSource == null) return;
        if (!secondSource.isPlaying && !waitingForNextSong)
        {
            waitingForNextSong = true;
            songTimer = Random.Range(3, 10);
        }

        if (waitingForNextSong)
        {
            songTimer -= Time.deltaTime;

            if (songTimer <= 0f)
            {
                PlayNextSound();
            }
        }
    }
    private void PlayNextSound()
    {
        if (gnomeSounds.Count == 0 && secondSource != null)
            return;

        AudioClip next = gnomeSounds[Random.Range(0, gnomeSounds.Count)];

        secondSource.clip = next;
        secondSource.pitch += Random.Range(0.2f, 0.8f);
        secondSource.volume -= Random.Range(0.15f, 0.35f);
        secondSource.Play();
        Debug.Log("Playing gnome sound");

        waitingForNextSong = false;
    }
}