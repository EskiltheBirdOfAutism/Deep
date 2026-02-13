using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ToolType
{
    Pickaxe,
    Gun,
    Hand,
    Flashlight
}

public class Tool : NetworkBehaviour
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

    [Header("Flashlight")]
    [SerializeField] private GameObject light;
    private bool isFlashOn = false;
    [SerializeField] private AudioSource flashButton;

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

    public void OnToolTrigger(InputAction.CallbackContext context)
    {
        if (isEquiped && gunDelay <= 0 && tool == ToolType.Gun)
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
        else if (tool == ToolType.Flashlight && isEquiped)
        {
            isFlashOn = !isFlashOn;
            Flashlight(isFlashOn);
        }

    }

    public void Flashlight(bool on)
    {

        light.SetActive(on);
        flashButton.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isEquiped && collision != null && tool == ToolType.Pickaxe)
        {
            if (pickAxeDelay <= 0)
            {
                pickAxeDelay = 3f;
                pickAxeHit.Play();
            }


            Vector3 _col_point = collision.contacts[0].point;
            BlockMeshDestroy _destroy = collision.gameObject.GetComponent<BlockMeshDestroy>();

            if (NetworkManager.Singleton.IsHost == true)
            {
                DestroyBlock(_col_point, collision.transform.position, collision.gameObject.GetComponent<BlockMeshDestroy>().index);
            }
            else
            {
                DestroyBlockServerRpc(_col_point, collision.transform.position, collision.gameObject.GetComponent<BlockMeshDestroy>().index);
                Debug.Log(collision.gameObject.GetComponent<BlockMeshDestroy>().index);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyBlockServerRpc(Vector3 _col_point, Vector3 collision, int _id)
    {
        DestroyBlock(_col_point, collision, _id);
    }

    void DestroyBlock(Vector3 _col_point, Vector3 collision, int _id)
    {
        BlockMeshDestroy _destroy = GameObject.Find("RoomGenerator(Clone)").GetComponent<RoomGeneratorCode>().mesh_id[_id].gameObject.GetComponent<BlockMeshDestroy>();
        for (int _i = 0; _i < _destroy.room_size / 2; _i++)
        {
            for (int _j = 0; _j < _destroy.room_size / 2; _j++)
            {
                Vector3 _pos = collision + new Vector3(_i, 0, _j);
                CombineInstance[] _block_id = new CombineInstance[49];
                Material _material = _destroy.roomblock.GetComponent<MeshRenderer>().sharedMaterial;

                if (_col_point.x >= _pos.x - 0.1f && _col_point.x < _pos.x + 1.1f
                && _col_point.z >= _pos.z - 0.1f && _col_point.z < _pos.z + 1.1f)
                {
                    // Debug.Log("HELLO");
                    if (_destroy.block_exist[_i + (_j * (int)_destroy.room_size / 2)] == true)
                    {
                        _destroy.block_exist[_i + (_j * (int)_destroy.room_size / 2)] = false;
                        for (int _k = 0; _k < _destroy.room_size / 2; _k++)
                        {
                            for (int _l = 0; _l < _destroy.room_size / 2; _l++)
                            {
                                if (_destroy.block_exist[_k + (_l * (int)_destroy.room_size / 2)] == true)
                                {
                                    _destroy.roomblock.transform.position = new Vector3(0.5f + _k, 0, 0.5f + _l);
                                    _block_id[_k + (_l * (int)_destroy.room_size / 2)].mesh = _destroy.roomblock.GetComponent<MeshFilter>().sharedMesh;
                                    _block_id[_k + (_l * (int)_destroy.room_size / 2)].transform = _destroy.roomblock.transform.localToWorldMatrix;
                                    if (Random.Range(0, 100) < 3)
                                    {
                                        //GameObject _enemy = Instantiate(_destroy.enemy, _destroy.roomblock.transform.position + new Vector3(-0.5f, -0.5f, -0.5f), Quaternion.identity);
                                        //_enemy.gameObject.GetComponent<NetworkObject>().Spawn();
                                    }
                                }
                            }
                        }

                        Mesh _new_mesh = new Mesh();
                        _new_mesh.CombineMeshes(_block_id);
                        _destroy.GetComponent<MeshFilter>().sharedMesh = _new_mesh;
                        _destroy.GetComponent<MeshRenderer>().sharedMaterial = _material;
                        _destroy.GetComponent<MeshCollider>().sharedMesh = _new_mesh;

                        foreach (ulong _client_id in NetworkManager.Singleton.ConnectedClientsIds)
                        {
                            Mesh _mesh = _destroy.GetComponent<MeshFilter>().mesh;
                            if (GameObject.Find("RoomGenerator(Clone)")) GameObject.Find("RoomGenerator(Clone)").GetComponent<RoomGeneratorCode>().UpdateClientMeshIdClientRpc(_client_id,
                            _destroy.gameObject.GetComponent<NetworkObject>(), _destroy.index, _mesh.triangles, _mesh.normals, _mesh.vertices);
                        }
                    }
                }
            }
        }
    }
}