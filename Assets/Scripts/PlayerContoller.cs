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
    private RagdollPart grabDirection_R;
    private RagdollPart grabDirection_L;
    [SerializeField] private ConfigurableJoint leftShoulder;
    [SerializeField] private ConfigurableJoint rightShoulder;
    [SerializeField] private ConfigurableJoint leftArmbåge;
    [SerializeField] private ConfigurableJoint rightArmbåge;
    private bool högerArm;
    private bool isMoving = false;
    [SerializeField] private Animator walkAnimation;

    [Header("Camera")]
    private CameraHolder camHolder;
    private Camera camComponent;
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
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        Move();
        RotateCamera();
        camComponent.enabled = true;
        
        if(leftMouse) { Grab(leftShoulder, leftArmbåge, högerArm = false); } else { EndGrab(leftShoulder, leftArmbåge); }
        if(rightMouse) { Grab(rightShoulder, rightArmbåge, högerArm = true); } else { EndGrab(rightShoulder, rightArmbåge); }
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

    public void Grab(ConfigurableJoint shoulder, ConfigurableJoint armbåge, bool högerArm)
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

    }

    private void EndGrab(ConfigurableJoint shoulder, ConfigurableJoint armbåge)
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
    }

    #endregion
}
