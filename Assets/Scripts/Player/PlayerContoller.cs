using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class PlayerContoller : NetworkBehaviour
{
    #region Variables

    [Header("Ragdoll")]
    private List<ConfigurableJoint> ragdollParts;
    private List<JointDrive> jointDrivesX;
    private List<JointDrive> jointDrivesYZ;
    private bool start = false;
    bool isRagdolled = false;

    [Header("Jump")]
    private IEnumerator Jumptime;
    private Foot foot;

    [Header("Inputs")]
    private Vector2 move;
    private Vector2 cameraRotation;
    private bool leftMouse;
    private bool rightMouse;

    [Header("Movement")]
    [HideInInspector] public Vector3 moveDirection;
    private Hip hip;
    private ConfigurableJoint hipJoint;
    private Movement_Direction movementDirection;
    [SerializeField] private int moveSpeed;
    private bool högerArm;
    private bool isMoving = false;
    [SerializeField] private Animator walkAnimation;
    [SerializeField] private List<ConfigurableJoint> LegJoints;
    private float movedirectionZ;

    [Header("Grab")]
    [SerializeField] private ConfigurableJoint leftShoulder;
    [SerializeField] private ConfigurableJoint rightShoulder;
    [SerializeField] private ConfigurableJoint leftArmbåge;
    [SerializeField] private ConfigurableJoint rightArmbåge;
    [SerializeField] private Hand leftHand;
    [SerializeField] private Hand rightHand;

    [Header("Camera")]
    private CameraHolder camHolder;
    [SerializeField]private Camera thirdPersonCamera;
    [SerializeField]private Camera firstPersonCamera;
    [SerializeField] private GameObject body;
    private AudioListener alComponent;
    private float yRotation = 0;
    private float xRotation = 0;
    public float xSens = 20f;
    public float ySens = 20f;

    [Header("Tools")]
    [SerializeField] private List<GameObject> ToolSlots = new List<GameObject>();
    [SerializeField] private List<Tool> Tools = new List<Tool>();
    

    #endregion

    private void Awake()
    {
        hip = GetComponentInChildren<Hip>();
        hipJoint = hip.GetComponent<ConfigurableJoint>();
        movementDirection = GetComponentInChildren<Movement_Direction>();
        camHolder = GetComponentInChildren<CameraHolder>();
        alComponent = GetComponentInChildren<AudioListener>();
        Cursor.lockState = CursorLockMode.Locked;
        foot = GetComponentInChildren<Foot>();
        DontDestroyOnLoad(gameObject);
        moveDirection = movementDirection.transform.localPosition;
        jointDrivesX = new List<JointDrive>();
        jointDrivesYZ = new List<JointDrive>();

        ragdollParts = GetComponentsInChildren<ConfigurableJoint>().ToList();
        for(int i = 0; i < ragdollParts.Count; i++)
        {
            jointDrivesX.Add(ragdollParts[i].angularXDrive);
            jointDrivesYZ.Add(ragdollParts[i].angularYZDrive);
        }


    }
    private void Update()
    {
        if (NetworkManager.Singleton.LocalClientId == OwnerClientId)
        {
            Move();
            RotateCamera();
            camHolder.gameObject.SetActive(true);
            alComponent.enabled = true;
        }
        else
        {
            camHolder.gameObject.SetActive(false);
            alComponent.enabled = false;
        }

        if (leftMouse) { Grab(leftShoulder, leftArmbåge,leftHand, högerArm = false); } else { EndGrab(leftShoulder, leftArmbåge, leftHand); }
        if(rightMouse) { Grab(rightShoulder, rightArmbåge,rightHand, högerArm = true); } else { EndGrab(rightShoulder, rightArmbåge, rightHand); }
    }

    #region Inputs
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
        print(move);
        if(move != Vector2.zero) { isMoving = true; } else { isMoving = false; }
    }
    public void OnCamera(InputAction.CallbackContext context)
    {
        cameraRotation = context.ReadValue<Vector2>();
    }
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() < 1) { leftMouse = false; } else { leftMouse = true; }
    }
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() < 1) { rightMouse = false; } else { rightMouse = true; }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    public void OnDisableMovement(InputAction.CallbackContext context)
    {
        start = !start;
        DisableMovement(start);
    }

    public void OnCameraSwitch(InputAction.CallbackContext context)
    {
        if (thirdPersonCamera.enabled == true) { thirdPersonCamera.enabled = false; camHolder.transform.localPosition = new Vector3(0, 0, 0); body.SetActive(false); }
        else { thirdPersonCamera.enabled = true; camHolder.transform.localPosition = new Vector3(0, 0.46f, -0.82f); body.SetActive(true); }

    }

    public void OnBelt1(InputAction.CallbackContext context)
    {
        EquipTool(0);
    }
    public void OnBelt2(InputAction.CallbackContext context)
    {
        EquipTool(1);
    }
    

    #endregion

    #region PlayerMove

    public void Move()
    {
        if (!isRagdolled)
        {
            if (isMoving) { walkAnimation.enabled = true; } else { walkAnimation.enabled = false; }
            moveDirection = Vector3.zero;
            moveDirection.x = move.x * 3;
            moveDirection.z = move.y * 3;


            Vector3 targetPosition = new Vector3(moveDirection.x, movedirectionZ, moveDirection.z);
            movementDirection.transform.localPosition = Vector3.MoveTowards(movementDirection.transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);

            foreach (ConfigurableJoint joint in LegJoints)
            {
                JointDrive positionDrive = new JointDrive();
                positionDrive.positionSpring = 10000f;
                positionDrive.positionDamper = 500f;
                positionDrive.maximumForce = 10000f; 

                joint.slerpDrive = positionDrive;

                // For angular drives
                JointDrive angularDrive = new JointDrive();
                angularDrive.positionSpring = 5000f;
                angularDrive.positionDamper = 200f;
                angularDrive.maximumForce = 5000f;

                joint.angularXDrive = angularDrive;
                joint.angularYZDrive = angularDrive;
            }
        }
    }
    public void Jump()
    {
        if (!isRagdolled)
        {
            if (foot.isGrounded)
            {
                hip.Jump();
            }
        }
    }


    public void RotateCamera()
    {
        xRotation -= (cameraRotation.x * Time.deltaTime) * xSens;
        yRotation -= (cameraRotation.y * Time.deltaTime) * xSens;
        yRotation = Mathf.Clamp(yRotation, -80, 80);

        hipJoint.targetRotation = Quaternion.Euler(0, xRotation, 0);

        camHolder.gameObject.transform.rotation = Quaternion.Euler(0, -xRotation + 90, -yRotation);
    }

    public void Grab(ConfigurableJoint shoulder, ConfigurableJoint armbåge,Hand hand, bool högerArm)
    {
        if (!isRagdolled)
        {
            Vector3 camEulerAngles = camHolder.transform.localEulerAngles;
            if (högerArm)
            {
                shoulder.targetRotation = Quaternion.Euler(camEulerAngles.z, 0, -90);
                print(camEulerAngles);
            }
            else
            {
                shoulder.targetRotation = Quaternion.Euler(camEulerAngles.z, 0, 90);
            }

            JointDrive shoulderDrive = new JointDrive();
            shoulderDrive.positionSpring = 5000;
            shoulderDrive.positionDamper = 20f;
            shoulderDrive.maximumForce = Mathf.Infinity;
            shoulder.angularYZDrive = shoulderDrive;
            shoulder.angularXDrive = shoulderDrive;

            JointDrive armbågeDrive = new JointDrive();
            armbågeDrive.positionSpring = 5000;
            armbågeDrive.positionDamper = 20f;
            armbågeDrive.maximumForce = Mathf.Infinity;
            armbåge.angularYZDrive = shoulderDrive;
            armbåge.angularXDrive = shoulderDrive;

            if (hand.GetComponentInChildren<Tool>() == false) { hand.grabAllowed = true; }
        }
    }

    private void EndGrab(ConfigurableJoint shoulder, ConfigurableJoint armbåge, Hand hand)
    {
        JointDrive shoulderDrive = new JointDrive();
        shoulderDrive.positionSpring = 0;
        shoulderDrive.positionDamper = 0f;
        shoulderDrive.maximumForce = Mathf.Infinity;
        shoulder.angularYZDrive = shoulderDrive;
        shoulder.angularXDrive = shoulderDrive;

        JointDrive armbågeDrive = new JointDrive();
        armbågeDrive.positionSpring = 0;
        armbågeDrive.positionDamper = 0f;
        armbågeDrive.maximumForce = Mathf.Infinity;
        armbåge.angularYZDrive = armbågeDrive;
        armbåge.angularXDrive = armbågeDrive;

        hand.grabAllowed = false;
        hand.Release();
    }

    private void DisableMovement(bool start)
    {
        for(int i = 0; i < ragdollParts.Count; i++)
        {
            if (start)
            {
                JointDrive angularDrive = new JointDrive();
                angularDrive.positionSpring = 0;
                angularDrive.positionDamper = 0f;
                angularDrive.maximumForce = Mathf.Infinity;
                ragdollParts[i].angularYZDrive = angularDrive;
                ragdollParts[i].angularXDrive = angularDrive;
            }
            else
            {
                ragdollParts[i].angularXDrive = jointDrivesX[i];
                ragdollParts[i].angularYZDrive = jointDrivesYZ[i];
            }
        }

        if (start) { isRagdolled = true; } else { isRagdolled = false; }
    }

    #endregion

    #region Tools

    public void EquipTool(int slot)
    {
        for (int i = 0; i < Tools.Count; i++)
        {
            if (Tools[i].isEquiped == true)
            {
                UnequipTool(i); 
            }
        }
        Tools[slot].transform.SetParent(rightHand.transform);
        Tools[slot].transform.localPosition = Tools[slot].equipedPos;
        Tools[slot].transform.localRotation = Tools[slot].equipedQuaternion;
        Tools[slot].isEquiped = true;
        Tools[slot].GetComponent<ConfigurableJoint>().connectedBody = rightHand.GetComponent<Rigidbody>();
    }
    public void UnequipTool(int slot)
    {
        Tools[slot].transform.SetParent(ToolSlots[slot].transform);
        Tools[slot].transform.localPosition = Tools[slot].unequipedPos;
        Tools[slot].transform.localRotation = Tools[slot].unequipedQuaternion;
        Tools[slot].isEquiped = false;
        Tools[slot].GetComponent<ConfigurableJoint>().connectedBody = null;
    }

    #endregion
}
