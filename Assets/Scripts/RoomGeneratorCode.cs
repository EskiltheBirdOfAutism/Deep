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
    private Vector3[] room_pos = new Vector3[64];
    private bool room_change = false;
    private bool room_change_previous = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        for (int _i = 0; _i < room_pos.Length; _i++)
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
                    if(Random.Range(0, 100) <= 50) room_pos[_i] = room_pos[_i - 1] + new Vector3(0, 0, _offset * 50);
                    room_change = true;
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
                }
                else
                {
                    if (room_pos[_i - 1].y > room_pos[_i].y)
                    {
                        GameObject _room = Instantiate(roomdownup, room_pos[_i], Quaternion.identity);
                        _room.GetComponent<NetworkObject>().Spawn();
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
                }
                else
                {
                    GameObject _room = Instantiate(roomdown, room_pos[_i], Quaternion.identity);
                    _room.GetComponent<NetworkObject>().Spawn();
                }
            }
        }
    }
}
