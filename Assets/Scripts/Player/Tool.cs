using UnityEngine;
using UnityEngine.InputSystem;

public enum ToolType
{
    Pickaxe,
    Gun
}

public class Tool : MonoBehaviour
{
    [Header("Tools")]
    public bool isEquiped;
    public ToolType tool;
    public Vector3 equipedPos;
    public Vector3 equipedRot;
    [HideInInspector] public Quaternion equipedQuaternion;
    [HideInInspector] public Vector3 unequipedPos;
    [HideInInspector] public Quaternion unequipedQuaternion;

    [Header("Pickaxe")]
    [SerializeField] private AudioSource pickAxeHit;
    private float pickAxeDelay;

    [Header("Gun")]
    public ParticleSystem MuzzleFlash;
    [SerializeField] AudioSource gunShot;
    public float detectionRange = 30f;
    public float detectionAngle = 30f; // Cone angle
    public LayerMask enemyLayer; // Set to Enemy layer
    public LayerMask obstacleLayer; // Set to everything that blocks shots
    private float gunDelay;

    private bool allowHit;
    private EnemyAttack enemie;

    private void Awake()
    {
        unequipedPos = transform.localPosition;
        unequipedQuaternion = transform.localRotation;
        equipedQuaternion = Quaternion.Euler(equipedRot);
    }

    private void Update()
    {
        if (tool == ToolType.Gun && isEquiped)
        {
            CheckForEnemies();
        }
        gunDelay -= Time.deltaTime;
        pickAxeDelay -= Time.deltaTime;
    }

    private void CheckForEnemies()
    {
        // Gun's actual forward direction
        Vector3 gunForward = -transform.up;

        // Find all enemies in sphere range
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);

        allowHit = false;
        enemie = null;

        // Debug: Draw gun's forward direction
        Debug.DrawRay(transform.position, gunForward * detectionRange, Color.blue);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Vector3 directionToEnemy = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(gunForward, directionToEnemy);

            //print($"Angle to enemy: {angle:F1}");

            // Check if enemy is within cone angle
            if (angle < detectionAngle)
            {
                // Raycast to check for obstructions
                if (Physics.Raycast(transform.position, directionToEnemy, out RaycastHit rayHit, detectionRange, obstacleLayer))
                {
                    // Check if we hit the enemy (not an obstruction)
                    if (rayHit.collider == hit)
                    {
                        allowHit = true;
                        enemie = hit.GetComponent<EnemyAttack>();
                        Debug.DrawLine(transform.position, hit.transform.position, Color.green);
                        return;
                    }
                    else
                    {
                        Debug.DrawLine(transform.position, rayHit.point, Color.red);
                        print($"Blocked by: {rayHit.collider.name}");
                    }
                }
            }
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (isEquiped && gunDelay <= 0)
        {
            gunDelay = 0.5f;
            MuzzleFlash.Play();
            gunShot.Play();
            if (allowHit && enemie != null)
            {
                enemie.TakeDamage(3);
            }
            else
            {
                print($"Can't shoot - isEquiped: {isEquiped}, allowHit: {allowHit}, enemie: {enemie}");
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isEquiped && collision != null && tool == ToolType.Pickaxe && pickAxeDelay <= 0)
        {
            pickAxeDelay = 3f;
            pickAxeHit.Play();
        }
    }
}