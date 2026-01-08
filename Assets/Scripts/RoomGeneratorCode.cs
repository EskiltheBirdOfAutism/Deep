using Unity.VisualScripting;
using UnityEngine;

public class RoomGeneratorCode : MonoBehaviour
{
    [SerializeField] private GameObject room;
    private Vector3[] room_pos = new Vector3[64];
    private bool[] room_change = new bool[64];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            Instantiate(room, room_pos[_i], Quaternion.identity);
        }
    }
}
