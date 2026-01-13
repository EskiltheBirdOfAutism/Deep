using UnityEngine;


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

    private void Awake()
    {
        unequipedPos = transform.localPosition;
        unequipedQuaternion = transform.localRotation;

        equipedQuaternion = Quaternion.Euler(equipedRot);
    }
}
