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
    [SerializeField] private GameObject meshtemplate;
    [SerializeField] private GameObject elevator;
    private int room_amount = 8;
    private Vector3[] room_pos = new Vector3[9];
    private bool room_change = false;
    private bool room_change_previous = false;
    private GameObject[] room_id = new GameObject[9];
    private GameObject[] mesh_id = new GameObject[33 * 14];
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
                _room.gameObject.GetComponent<NetworkObject>().Spawn();
                room_id[_i] = _room.gameObject;

                for (int _o = 0; _o < 14; _o++)
                {
                    for (int _l = 0; _l < 4; _l++)
                    {
                        int _size_of_mesh = ((int)room_size.x / 2);
                        CombineInstance[] _block_id = new CombineInstance[_size_of_mesh * _size_of_mesh];
                        Material _material = roomblock.GetComponent<MeshRenderer>().sharedMaterial;
                        NetworkObject _no = roomblock.GetComponent<NetworkObject>();
                        float _add_x = 0;
                        float _add_z = 0;

                        if (_l == 1 || _l == 3) _add_x = 7;
                        if (_l >= 2) _add_z = 7;

                        for (int _j = 0; _j < _size_of_mesh; _j++)
                        {
                            for (int _k = 0; _k < _size_of_mesh; _k++)
                            {
                                roomblock.transform.position = new Vector3(0.5f + _k, 0, 0.5f + _j);
                                _block_id[_j + (_k * _size_of_mesh)].mesh = roomblock.GetComponent<MeshFilter>().sharedMesh;
                                _block_id[_j + (_k * _size_of_mesh)].transform = roomblock.transform.localToWorldMatrix;
                            }
                        }
                        Mesh _new_mesh = new Mesh();
                        _new_mesh.CombineMeshes(_block_id);
                        GameObject _mesh = Instantiate(meshtemplate, new Vector3(0, -7 + _o, 0), Quaternion.identity);
                        _mesh.GetComponent<MeshFilter>().sharedMesh = _new_mesh;
                        _mesh.GetComponent<MeshRenderer>().sharedMaterial = _material;
                        _mesh.GetComponent<MeshCollider>().sharedMesh = _new_mesh;
                        _mesh.transform.position = room_pos[_i] + new Vector3(_add_x - (room_size.x / 2), (0.5f - room_size.y / 2) + _o, _add_z - (room_size.z / 2));
                        _mesh.gameObject.GetComponent<NetworkObject>().Spawn();
                        mesh_id[_l + (_o * 4) + (_i * 56)] = _mesh.gameObject;
                        _mesh.GetComponent<BlockMeshDestroy>().index = _l + (_o * 4) + (_i * 56);
                    }
                }
            }

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
            NetworkObjectReference[] _room_ref = new NetworkObjectReference[9];
            NetworkObjectReference[] _mesh_ref = new NetworkObjectReference[33 * 14];
            for (int _i = 0; _i < room_amount; _i++)
            {
                _room_ref[_i] = room_id[_i].GetComponent<NetworkObject>();
                for (int _k = 0; _k < 14; _k++)
                {
                    for (int _j = 0; _j < 4; _j++)
                    {
                        int _index = _j + (_k * 4) + (_i * 56);
                        _mesh_ref[_index] = mesh_id[_index].GetComponent<NetworkObject>();
                    }
                }
            }
            UpdateClientRoomIdClientRpc(_client_id, _room_ref, room_pos);
            UpdateClientMeshOnceIdClientRpc(_client_id, _mesh_ref);
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
                if (_i > 0)
                {
                    room_id[_i].SetActive(false);
                    for (int _k = 0; _k < 14; _k++)
                    {
                        for (int _j = 0; _j < 4; _j++)
                        {
                            mesh_id[_j + (_k * 4) + (_i * 56)].SetActive(false);
                        }
                    }
                }
            }

            for (int _i = 0; _i < room_amount; _i++)
            {
                if (_pos.x < room_pos[_i].x + (room_size.x / 2) && _pos.x > room_pos[_i].x - (room_size.x / 2)
                && (_pos.y + 0.5) < room_pos[_i].y + (room_size.y / 2) && (_pos.y + 0.5) > room_pos[_i].y - (room_size.y / 2)
                && _pos.z < room_pos[_i].z + (room_size.x / 2) && _pos.z > room_pos[_i].z - (room_size.x / 2))
                {
                    for(int _j = 0; _j < 3; _j++)
                    {
                        int _index = _i + _j - 1;
                        if (_index >= 0 && _index < room_amount)
                        {
                            room_id[_index].SetActive(true);
                            for (int _o = 0; _o < 14; _o++)
                            {
                                for (int _k = 0; _k < 4; _k++)
                                {
                                    mesh_id[_j + (_k * 4) + (_index * 56)].SetActive(true);
                                }
                            }
                        }
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
            }
        }
    }

    [ClientRpc]
    public void UpdateClientMeshIdClientRpc(ulong _client_id, NetworkObjectReference _mesh_id, int _mesh_index, int[] _triangles, Vector3[] _normals, Vector3[] _vertices)
    {
        if (NetworkManager.Singleton.LocalClientId == _client_id)
        {
            if (!_mesh_id.TryGet(out NetworkObject _net_obj)) return;

            mesh_id[_mesh_index] = _net_obj.gameObject;
            Mesh _mesh = mesh_id[_mesh_index].GetComponent<MeshFilter>().mesh;
            _mesh.Clear();
            _mesh.vertices = new Vector3[_vertices.Length];
            _mesh.normals = new Vector3[_normals.Length];
            _mesh.triangles = new int[_triangles.Length];

            _mesh.vertices = _vertices;
            _mesh.normals = _normals;
            _mesh.triangles = _triangles;

            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();

            MeshCollider _mesh_col = mesh_id[_mesh_index].GetComponent<MeshCollider>();
            _mesh_col.sharedMesh = _mesh;
        }
    }

    [ClientRpc]
    public void UpdateClientMeshOnceIdClientRpc(ulong _client_id, NetworkObjectReference[] _mesh_id)
    {
        if (NetworkManager.Singleton.LocalClientId == _client_id)
        {
            for (int _i = 0; _i < room_amount; _i++)
            {
                for (int _o = 0; _o < 14; _o++)
                {
                    for (int _l = 0; _l < 4; _l++)
                    {
                        if (!_mesh_id[_l + (_o * 4) + (_i * 56)].TryGet(out NetworkObject _net_obj)) return;

                        mesh_id[_l + (_o * 4) + (_i * 56)] = _net_obj.gameObject;

                        int _size_of_mesh = ((int)room_size.x / 2);
                        CombineInstance[] _block_id = new CombineInstance[_size_of_mesh * _size_of_mesh];
                        Material _material = roomblock.GetComponent<MeshRenderer>().sharedMaterial;
                        NetworkObject _no = roomblock.GetComponent<NetworkObject>();
                        float _add_x = 0;
                        float _add_z = 0;
                        
                        if (_l == 1 || _l == 3) _add_x = 7;
                        if (_l >= 2) _add_z = 7;
                        
                        for (int _j = 0; _j < _size_of_mesh; _j++)
                        {
                            for (int _k = 0; _k < _size_of_mesh; _k++)
                            {
                                roomblock.transform.position = new Vector3(0.5f + _k, 0, 0.5f + _j);
                                _block_id[_j + (_k * _size_of_mesh)].mesh = roomblock.GetComponent<MeshFilter>().sharedMesh;
                                _block_id[_j + (_k * _size_of_mesh)].transform = roomblock.transform.localToWorldMatrix;
                            }
                        }
                        Mesh _new_mesh = new Mesh();
                        _new_mesh.CombineMeshes(_block_id);
                        _net_obj.GetComponent<MeshFilter>().sharedMesh = _new_mesh;
                        _net_obj.GetComponent<MeshRenderer>().sharedMaterial = _material;
                        _net_obj.GetComponent<MeshCollider>().sharedMesh = _new_mesh;
                        _net_obj.transform.position = room_pos[_i] + new Vector3(_add_x - (room_size.x / 2), (0.5f - room_size.y / 2) + _o, _add_z - (room_size.z / 2));
                    }   
                }
            }
        }
    }
}
