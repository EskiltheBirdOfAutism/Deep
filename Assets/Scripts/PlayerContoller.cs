using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class PlayerContoller : NetworkBehaviour
{
    #region Variables

    [Header("Inputs")]
    private Vector2 move;
    private Vector2 cameraRotation;
    private bool leftMouse;
    private bool rightMouse;

    [HideInInspector] public Vector3 moveDirection;
    private Hip hip;
    private ConfigurableJoint hipJoint;
    private Movement_Direction movementDirection;
    [SerializeField] private int moveSpeed;
    private bool högerArm;
    private bool isMoving = false;
    [SerializeField] private Animator walkAnimation;

    [Header("Grab")]
    [SerializeField] private ConfigurableJoint leftShoulder;
    [SerializeField] private ConfigurableJoint rightShoulder;
    [SerializeField] private ConfigurableJoint leftArmbåge;
    [SerializeField] private ConfigurableJoint rightArmbåge;
    [SerializeField] private Hand leftHand;
    [SerializeField] private Hand rightHand;

    [Header("Camera")]
    private CameraHolder camHolder;
    private Camera camComponent;
    private AudioListener alComponent;
    private float yRotation = 0;
    private float xRotation = 0;
    public float xSens = 30f;
    public float ySens = 30f;
    #endregion

    private void Awake()
    {
        hip = GetComponentInChildren<Hip>();
        hipJoint = hip.GetComponent<ConfigurableJoint>();
        movementDirection = GetComponentInChildren<Movement_Direction>();
        camHolder = GetComponentInChildren<CameraHolder>();
        camComponent = GetComponentInChildren<Camera>();
        alComponent = GetComponentInChildren<AudioListener>();
        Cursor.lockState = CursorLockMode.Locked;

        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (NetworkManager.Singleton.LocalClientId == OwnerClientId)
        {
            Move();
            RotateCamera();
            camComponent.enabled = true;
            alComponent.enabled = true;
        }
        else
        {
            camComponent.enabled = false;
            alComponent.enabled = false;
        }

        if (leftMouse) { Grab(leftShoulder, leftArmbåge,leftHand, högerArm = false); } else { EndGrab(leftShoulder, leftArmbåge, leftHand); }
        if(rightMouse) { Grab(rightShoulder, rightArmbåge,rightHand, högerArm = true); } else { EndGrab(rightShoulder, rightArmbåge, rightHand); }
    }

    #region Inputs
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
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

    #endregion

    #region PlayerMove

    public void Move()
    {
        if (isMoving) { walkAnimation.enabled = true; } else { walkAnimation.enabled = false; }
        moveDirection = Vector3.zero;
        moveDirection.x = move.x * 3;
        moveDirection.z = move.y * 3;

        Vector3 targetPosition = new Vector3(moveDirection.x, movementDirection.transform.localPosition.y, moveDirection.z);
        movementDirection.transform.localPosition = Vector3.MoveTowards(movementDirection.transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);

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
        Vector3 camEulerAngles = camHolder.transform.localEulerAngles;
        if (högerArm)
        {
            shoulder.targetRotation = Quaternion.Euler(-camEulerAngles.z - 10, 90, 0);
        }
        else
        {
            shoulder.targetRotation = Quaternion.Euler(-camEulerAngles.z - 10, -90, 0);
        }

        JointDrive shoulderDrive = new JointDrive();
        shoulderDrive.positionSpring = 500;
        shoulderDrive.positionDamper = 5f;
        shoulderDrive.maximumForce = Mathf.Infinity; 
        shoulder.angularYZDrive = shoulderDrive;
        shoulder.angularXDrive = shoulderDrive;

        JointDrive armbågeDrive = new JointDrive();
        armbågeDrive.positionSpring = 500;
        armbågeDrive.positionDamper = 5f;
        armbågeDrive.maximumForce = Mathf.Infinity;
        armbåge.angularYZDrive = shoulderDrive;
        armbåge.angularXDrive = shoulderDrive;

        hand.grabAllowed = true;
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
    }

    #endregion
}
