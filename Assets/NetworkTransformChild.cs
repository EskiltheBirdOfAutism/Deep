using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkTransformChild : NetworkBehaviour
{
    [SerializeField] private Transform[] targetTransform;

    private Vector3[] targetPosition = new Vector3[99];
    private Quaternion[] targetRotation = new Quaternion[99];

    private void Start()
    {
        for (int _i = 0; _i < targetTransform.Length; _i++)
        {
            targetPosition[_i] = targetTransform[_i].position;
            targetRotation[_i] = targetTransform[_i].rotation;
        }

        Rigidbody[] _rb = GetComponentsInChildren<Rigidbody>();

        if (!IsOwner)
        {
            foreach (var _r in _rb)
            {
                _r.isKinematic = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            for (int _i = 0; _i < targetTransform.Length; _i++)
            {
                SendTransformServerRpc(targetTransform[_i].position, targetTransform[_i].rotation, _i);
            }
        }
        else
        {
            for (int _i = 0; _i < targetTransform.Length; _i++)
            {
                targetTransform[_i].position = Vector3.Lerp(targetTransform[_i].position, targetPosition[_i], Time.fixedDeltaTime * 10f);
                targetTransform[_i].rotation = Quaternion.Slerp(targetTransform[_i].rotation, targetRotation[_i], Time.fixedDeltaTime * 10f);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendTransformServerRpc(Vector3 _pos, Quaternion _rot, int _index)
    {
        UpdateTransformClientRpc(_pos, _rot, _index);
    }

    [ClientRpc]
    private void UpdateTransformClientRpc(Vector3 _pos, Quaternion _rot, int _index)
    {
        if (!IsOwner)
        {
            targetPosition[_index] = _pos;
            targetRotation[_index] = _rot;
        }
    }
}