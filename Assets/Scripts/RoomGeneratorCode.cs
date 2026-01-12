using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoomGeneratorCode : NetworkBehaviour
{
    [SerializeField] private GameObject room;
    [SerializeField] private GameObject roomdown;
    [SerializeField] private GameObject roomside;
    [SerializeField] private GameObject roomsideup;
    [SerializeField] private GameObject roomsidedown;
    [SerializeField] private GameObject roomdownup;
    private Vector3[] room_pos = new Vector3[65];
    private bool room_change = false;
    private bool room_change_previous = false;
    private GameObject[] room_id = new GameObject[65];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (OwnerClientId == 0)
        {
            for (int _i = 0; _i < 64; _i++)
            {
                room_pos[_i] = new Vector3(0, 0, 0);
                room_change_previous = room_change;
                room_change = false;
                if (_i > 0)
                {
                    room_change = false;
                    room_pos[_i] = room_pos[_i - 1] + new Vector3(0, -25, 0);
                    if (Random.Range(0, 100) < 70 && room_change_previous == false)
                    {
                        float _offset = 1;
                        if (Random.Range(0, 100) <= 50) _offset = -1;

                        room_pos[_i] = room_pos[_i - 1] + new Vector3(_offset * 50, 0, 0);
                        if (Random.Range(0, 100) <= 50) room_pos[_i] = room_pos[_i - 1] + new Vector3(0, 0, _offset * 50);
                        room_change = true;
                    }
                }
            }

            for (int _i = 0; _i < 64; _i++)
            {
                if (_i > 0)
                {
                    if (room_pos[_i + 1].y == room_pos[_i].y)
                    {
                        GameObject _room = Instantiate(roomsideup, room_pos[_i], Quaternion.identity);
                        _room.transform.rotation = Quaternion.Euler(0, 0, 0);
                        if (room_pos[_i + 1].x - room_pos[_i].x < 0)
                        {
                            _room.transform.rotation = Quaternion.Euler(0, 180, 0);
                        }
                        if (room_pos[_i + 1].z - room_pos[_i].z < 0)
                        {
                            _room.transform.rotation = Quaternion.Euler(0, 90, 0);
                        }
                        if (room_pos[_i + 1].z - room_pos[_i].z > 0)
                        {
                            _room.transform.rotation = Quaternion.Euler(0, 270, 0);
                        }
                        _room.GetComponent<NetworkObject>().Spawn();
                        room_id[_i] = _room.gameObject;
                    }
                    else
                    {
                        if (room_pos[_i - 1].y > room_pos[_i].y)
                        {
                            GameObject _room = Instantiate(roomdownup, room_pos[_i], Quaternion.identity);
                            _room.GetComponent<NetworkObject>().Spawn();
                            room_id[_i] = _room.gameObject;
                        }
                        else
                        {
                            GameObject _room = Instantiate(roomsidedown, room_pos[_i], Quaternion.identity);
                            _room.transform.rotation = Quaternion.Euler(0, 0, 0);
                            if (room_pos[_i - 1].x - room_pos[_i].x < 0)
                            {
                                _room.transform.rotation = Quaternion.Euler(0, 180, 0);
                            }
                            if (room_pos[_i - 1].z - room_pos[_i].z < 0)
                            {
                                _room.transform.rotation = Quaternion.Euler(0, 90, 0);
                            }
                            if (room_pos[_i - 1].z - room_pos[_i].z > 0)
                            {
                                _room.transform.rotation = Quaternion.Euler(0, 270, 0);
                            }
                            _room.GetComponent<NetworkObject>().Spawn();
                            room_id[_i] = _room.gameObject;
                        }
                    }
                }
                else
                {
                    if (room_pos[_i + 1].y == room_pos[_i].y)
                    {
                        GameObject _room = Instantiate(roomside, room_pos[_i], Quaternion.identity);
                        _room.transform.rotation = Quaternion.Euler(0, 0, 0);
                        if (room_pos[_i + 1].x - room_pos[_i].x < 0)
                        {
                            _room.transform.rotation = Quaternion.Euler(0, 180, 0);
                        }
                        if (room_pos[_i + 1].z - room_pos[_i].z < 0)
                        {
                            _room.transform.rotation = Quaternion.Euler(0, 90, 0);
                        }
                        if (room_pos[_i + 1].z - room_pos[_i].z > 0)
                        {
                            _room.transform.rotation = Quaternion.Euler(0, 270, 0);
                        }
                        _room.GetComponent<NetworkObject>().Spawn();
                        room_id[_i] = _room.gameObject;
                    }
                    else
                    {
                        GameObject _room = Instantiate(roomdown, room_pos[_i], Quaternion.identity);
                        _room.GetComponent<NetworkObject>().Spawn();
                        room_id[_i] = _room.gameObject;
                    }
                }
                SendRoomIdServerRpc(room_id[_i].GetComponent<NetworkObject>(), _i);
            }
        }
    }

    void Update()
    {
        ulong _client_id = OwnerClientId;
        GameObject _player = GameObject.Find("Hip " + (_client_id + 1)).gameObject;
        if (_player != null)
        {
            Vector3 _pos = _player.transform.position;
            for(int _i = 0; _i < 64; _i++) room_id[_i].SetActive(false);

            for (int _i = 0; _i < 64; _i++)
            {
                if (_pos.x < room_pos[_i].x + 25 && _pos.x > room_pos[_i].x - 25
                && (_pos.y + 0.5) < room_pos[_i].y + 12.5 && (_pos.y + 0.5) > room_pos[_i].y - 12.5
                && _pos.z < room_pos[_i].z + 25 && _pos.z > room_pos[_i].z - 25)
                {
                    for(int _j = 0; _j < 5; _j++)
                    {
                        int _index = _i + _j - 2;
                        if(_index >= 0 && _index < 64) room_id[_index].SetActive(true);
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendRoomIdServerRpc(NetworkObjectReference _room_id, int _index)
    {
        UpdateClientRoomIdClientRpc(_room_id, _index);
    }

    [ClientRpc]
    private void UpdateClientRoomIdClientRpc(NetworkObjectReference _room_id, int _index)
    {
        if(IsOwner && room_id[_index] == null)
        {
            if(!_room_id.TryGet(out NetworkObject _net_obj))
            {
                return;
            }

            room_id[_index] = _net_obj.gameObject;
        }
    }
}
