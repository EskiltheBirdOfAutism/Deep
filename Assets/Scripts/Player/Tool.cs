using UnityEngine;
using UnityEngine.InputSystem;


public enum ToolType
{
    Pickaxe,
    Gun
}
public class Tool : MonoBehaviour
{
    public bool isEquiped;
    public ToolType tool;
    public Vector3 equipedPos;
    public Vector3 equipedRot;
    [HideInInspector] public Quaternion equipedQuaternion;
    [HideInInspector] public Vector3 unequipedPos;
    [HideInInspector] public Quaternion unequipedQuaternion;
    private MeshCollider gunCollider;
    private bool allowHit;
    private EnemyAttack enemie;
    

    private void Awake()
    {
        unequipedPos = transform.localPosition;
        unequipedQuaternion = transform.localRotation;

        equipedQuaternion = Quaternion.Euler(equipedRot);
        if(tool == ToolType.Gun)
        {
            gunCollider = GetComponentInChildren<MeshCollider>();
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (isEquiped && allowHit && enemie != null)
        {
            enemie.TakeDamage(3);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tool == ToolType.Gun)
        {
            Vector3 direction = other.transform.position -= transform.position;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit) == other.gameObject.CompareTag("Enemie"))
            {
                allowHit = true;
                enemie = other.gameObject.GetComponent<EnemyAttack>();
            }
            else { allowHit = false; }
            enemie = null;
        }
    }

}
