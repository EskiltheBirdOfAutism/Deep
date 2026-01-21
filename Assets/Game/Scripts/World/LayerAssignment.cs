using Unity.Netcode;
using UnityEngine;

public class LayerAssignment : NetworkBehaviour
{
    [SerializeField] private LayerMask[] player_layer = new LayerMask[4];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        ulong _client_id = OwnerClientId;

        int _layer = (int)Mathf.Log(player_layer[_client_id].value, 2);
        Debug.Log(_client_id.ToString() + " - " + _layer.ToString());
        SetLayer(gameObject.transform, _layer);

        GameObject _hip = GetComponentInChildren<Hip>().gameObject;
        if (_hip != null)
        {
            _hip.name = "Hip " + (_layer - 5);
        }
    }

    private void SetLayer(Transform _parent, int _layer)
    {
        _parent.gameObject.layer = _layer;
        foreach (Transform _child in _parent)
        {
            SetLayer(_child, _layer);
        }
    }
}
