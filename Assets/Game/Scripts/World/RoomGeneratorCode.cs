using System.Linq;
using TMPro.Examples;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering.Universal;

public class RoomGeneratorCode : NetworkBehaviour
{
    [SerializeField] private GameObject room;
    [SerializeField] private GameObject roomdown;
    [SerializeField] private GameObject roomside;
    [SerializeField] private GameObject roomsideup;
    [SerializeField] private GameObject roomsidedown;
    [SerializeField] private GameObject roomdownup;
    [SerializeField] private GameObject roomblock;
    [SerializeField] private GameObject elevator;
    private int room_amount = 32;
    private Vector3[] room_pos = new Vector3[33];
    private bool room_change = false;
    private bool room_change_previous = false;
    private GameObject[] room_id = new GameObject[33];
    private GameObject[] mesh_id = new GameObject[33];
    [SerializeField] private Vector3 room_size = new Vector3(15, 7.5f, 15);
    [SerializeField] private int blocks_per_room = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            for (int _i = 0; _i < room_amount; _i++)
            {
                room_pos[_i] = new Vector3(0, 0, 0);
                room_change_previous = room_change;
                room_change = false;
                if (_i > 0)
                {
                    room_change = false;
                    room_pos[_i] = room_pos[_i - 1] + new Vector3(0, -room_size.y, 0);
                    if (room_change_previous == false)
                    {
                        float _offset = 1;
                        if (Random.Range(0, 100) <= 50) _offset = -1;
                        bool _z_axis = false;
                        if (Random.Range(0, 100) <= 50) _z_axis = true;

                        if(_i == 1)
                        {
                            _offset = 1;
                            _z_axis = false;
                        }

                        room_pos[_i] = room_pos[_i - 1] + new Vector3(_offset * room_size.x, 0, 0);
                        if (_z_axis == true) room_pos[_i] = room_pos[_i - 1] + new Vector3(0, 0, _offset * room_size.z);
                        room_change = true;
                    }
                }
            }

            for (int _i = 0; _i < room_amount; _i++)
            {
                GameObject _room;
                if (_i > 0)
                {
                    if (room_pos[_i + 1].y == room_pos[_i].y)
                    {
                        _room = Instantiate(roomsideup, room_pos[_i], Quaternion.identity);
                        _room.transform.rotation = RotateRoom(_room.transform.rotation, _i, 1);

                        GameObject _elevator = Instantiate(elevator, room_pos[_i] + new Vector3(0, (-room_size.y / 2) + 1, 0), Quaternion.identity);
                        Quaternion _rot = room_id[_i - 1].transform.rotation;
                        _elevator.transform.rotation = _rot;
                        _elevator.GetComponentInChildren<Hiss>().topLocation.gameObject.transform.position = room_pos[_i - 1] + new Vector3(0, (-room_size.y / 2) + 1, 0);
                        _elevator.gameObject.GetComponent<NetworkObject>().Spawn();
                    }
                    else
                    {
                        if (room_pos[_i - 1].y > room_pos[_i].y)
                        {
                            _room = Instantiate(roomdownup, room_pos[_i], Quaternion.identity);
                        }
                        else
                        {
                            _room = Instantiate(roomsidedown, room_pos[_i], Quaternion.identity);
                            _room.transform.rotation = RotateRoom(_room.transform.rotation, _i, -1);
                        }
                    }
                }
                else
                {
                    if (room_pos[_i + 1].y == room_pos[_i].y)
                    {
                        _room = Instantiate(roomside, room_pos[_i], Quaternion.identity);
                        _room.transform.rotation = RotateRoom(_room.transform.rotation, _i, 1);
                    }
                    else
                    {
                        _room = Instantiate(roomdown, room_pos[_i], Quaternion.identity);
                    }
                }
                _room.GetComponent<NetworkObject>().Spawn();
                room_id[_i] = _room.gameObject;
                /*
                for (int _j = 0; _j < blocks_per_room; _j++)
                {
                    Vector3 _pos;
                    Vector3 _add = _room.transform.forward;
                    if(Random.Range(0, 100) <= 33) _add = -_room.transform.right;
                    if(Random.Range(0, 100) <= 33) _add = -_room.transform.forward;
                    _pos = _add * 7;
                    if(_add == _room.transform.forward)
                    {
                        _pos -= _room.transform.right * 7;
                        _pos += _room.transform.right * Random.Range(0, 14);
                    }
                    else if(_add == -_room.transform.right)
                    {
                        _pos -= _room.transform.forward * 7;
                        _pos += _room.transform.forward * Random.Range(0, 14);
                    }
                    else if(_add == -_room.transform.forward)
                    {
                        _pos += _room.transform.right * 7;
                        _pos -= _room.transform.right * Random.Range(0, 14);
                    }

                    _pos = new Vector3(_pos.x, _pos.y - Random.Range(room_size.y / 4, (room_size.y / 2)) + 0.5f, _pos.z);

                    GameObject _block = Instantiate(roomblock, room_pos[_i] + _pos, Quaternion.identity);
                    _block.gameObject.GetComponent<NetworkObject>().Spawn();
                    _block.gameObject.transform.SetParent(room_id[_i].gameObject.transform, true);
                }
                */
            }
            CombineInstance[] _block_id = new CombineInstance[225];
            Material _material = roomblock.GetComponent<MeshRenderer>().sharedMaterial;
            for (int _i = 0; _i < 15; _i++)
            {
                for (int _j = 0; _j < 15; _j++)
                {
                    GameObject _block = Instantiate(roomblock, new Vector3(_i - 7, 0, _j - 7), Quaternion.identity);
                    _block_id[_i + (_j * 15)].mesh = _block.GetComponent<MeshFilter>().sharedMesh;
                    _block_id[_i + (_j * 15)].transform = _block.transform.localToWorldMatrix;
                    Destroy(_block);
                }
            }
            Mesh _new_mesh = new Mesh();
            _new_mesh.CombineMeshes(_block_id);
            GameObject _mesh = new GameObject("RoomBlockMesh");
            _mesh.transform.position = new Vector3(0, -7, 0);
            _mesh.AddComponent<MeshFilter>();
            _mesh.AddComponent<MeshRenderer>();
            _mesh.AddComponent<MeshCollider>();
            _mesh.AddComponent<NetworkObject>();
            _mesh.GetComponent<MeshFilter>().sharedMesh = _new_mesh;
            _mesh.GetComponent<MeshRenderer>().sharedMaterial = _material;
            _mesh.GetComponent<MeshCollider>().sharedMesh = _new_mesh;
            _mesh.GetComponent<NetworkObject>().Spawn();

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private Quaternion RotateRoom(Quaternion _rot, int _index, int _add)
    {
        _rot = Quaternion.Euler(0, 0, 0);
        if (room_pos[_index + _add].x - room_pos[_index].x < 0)
        {
            _rot = Quaternion.Euler(0, 180, 0);
        }
        if (room_pos[_index + _add].z - room_pos[_index].z < 0)
        {
            _rot = Quaternion.Euler(0, 90, 0);
        }
        if (room_pos[_index + _add].z - room_pos[_index].z > 0)
        {
            _rot = Quaternion.Euler(0, 270, 0);
        }

        return _rot;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong _client_id)
    {
        if (NetworkManager.Singleton.IsHost == true)
        {
            NetworkObjectReference[] _room_ref = new NetworkObjectReference[64];
            for (int _i = 0; _i < room_amount; _i++)
            {
                _room_ref[_i] = room_id[_i].GetComponent<NetworkObject>();
            }
            UpdateClientRoomIdClientRpc(_client_id, _room_ref, room_pos);
        }
    }

    void Update()
    {
        ulong _client_id = NetworkManager.Singleton.LocalClientId;
        GameObject _player;
        if (GameObject.Find("Hip " + ((int)_client_id + 1)) != false) 
        {
            _player = GameObject.Find("Hip " + ((int)_client_id + 1)).gameObject;
            Vector3 _pos = _player.transform.position;
            for (int _i = 0; _i < room_amount; _i++)
            {
                if(_i > 0) room_id[_i].SetActive(false);
            }

            for (int _i = 0; _i < room_amount; _i++)
            {
                if (_pos.x < room_pos[_i].x + (room_size.x / 2) && _pos.x > room_pos[_i].x - (room_size.x / 2)
                && (_pos.y + 0.5) < room_pos[_i].y + (room_size.y / 2) && (_pos.y + 0.5) > room_pos[_i].y - (room_size.y / 2)
                && _pos.z < room_pos[_i].z + (room_size.x / 2) && _pos.z > room_pos[_i].z - (room_size.x / 2))
                {
                    for(int _j = 0; _j < 5; _j++)
                    {
                        int _index = _i + _j - 2;
                        if(_index >= 0 && _index < room_amount) room_id[_index].SetActive(true);
                    }
                }
            }
        }
    }

    [ClientRpc]
    private void UpdateClientRoomIdClientRpc(ulong _client_id, NetworkObjectReference[] _room_id, Vector3[] _room_pos)
    {
        if (NetworkManager.Singleton.LocalClientId == _client_id)
        {
            for (int _i = 0; _i < room_amount; _i++)
            {
                if (!_room_id[_i].TryGet(out NetworkObject _net_obj)) continue;

                room_id[_i] = _net_obj.gameObject;
                room_pos[_i] = _room_pos[_i];

                /*
                for (int _j = 0; _j < blocks_per_room; _j++)
                {
                    if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(_net_obj.NetworkObjectId + 1 + (ulong)_j, out NetworkObject _block))
                    {
                        _block.gameObject.transform.SetParent(room_id[_i].gameObject.transform, true);
                        _block.gameObject.transform.localScale = new Vector3(_block.gameObject.transform.localScale.x * room_size.x,
                            _block.gameObject.transform.localScale.y * room_size.y,
                            _block.gameObject.transform.localScale.z * room_size.z);
                    }
                }
                */
            }
        }
    }
}
