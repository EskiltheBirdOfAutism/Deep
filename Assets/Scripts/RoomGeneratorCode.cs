using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoomGeneratorCode : MonoBehaviour
{
    [SerializeField] private GameObject room;
    [SerializeField] private GameObject roomdown;
    [SerializeField] private GameObject roomside;
    [SerializeField] private GameObject roomsideup;
    [SerializeField] private GameObject roomsidedown;
    [SerializeField] private GameObject roomdownup;
    [SerializeField] private GameObject room_parent;
    private Vector3[] room_pos = new Vector3[64];
    private bool[] room_change = new bool[64];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameObject _room_parent = Instantiate(room_parent);
        _room_parent.GetComponent<NetworkObject>().Spawn();

        for (int _i = 0; _i < room_pos.Length; _i++)
        {
            room_pos[_i] = new Vector3(0, 0, 0);
            room_change[_i] = false;
            if (_i > 0)
            {
                room_change[_i] = false;
                room_pos[_i] = room_pos[_i - 1] + new Vector3(0, -25, 0);
                if (Random.Range(0, 100) < 70 && room_change[_i - 1] == false)
                {
                    float _offset = 1;
                    if (Random.Range(0, 100) <= 50) _offset = -1;

                    room_pos[_i] = room_pos[_i - 1] + new Vector3(_offset * 50, 0, 0);
                    if(Random.Range(0, 100) <= 50) room_pos[_i] = room_pos[_i - 1] + new Vector3(0, 0, _offset * 50);
                    room_change[_i] = true;
                }
            }
        }

        for (int _i = 0; _i < room_pos.Length; _i++)
        {
            if(_i > 0)
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
                    _room.transform.SetParent(_room_parent.transform, false);
                }
                else
                {
                    if (room_pos[_i - 1].y > room_pos[_i].y)
                    {
                        GameObject _room = Instantiate(roomdownup, room_pos[_i], Quaternion.identity);
                        _room.GetComponent<NetworkObject>().Spawn();
                        _room.transform.SetParent(_room_parent.transform, false);
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
                        _room.transform.SetParent(_room_parent.transform, false);
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
                    _room.transform.SetParent(_room_parent.transform, false);
                }
                else
                {
                    GameObject _room = Instantiate(roomdown, room_pos[_i], Quaternion.identity);
                    _room.GetComponent<NetworkObject>().Spawn();
                    _room.transform.SetParent(_room_parent.transform, false);
                }
            }
        }
    }
}
