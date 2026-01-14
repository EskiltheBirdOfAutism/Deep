using UnityEngine;

public class AddFloors : MonoBehaviour
{
    [SerializeField] private GameObject topFloorPrefab;
    [SerializeField] private GameObject bottomFloorPrefab;
    [SerializeField] private GameObject room;
    private Vector3 floorPosition;
    void Awake()
    {
        for (int i = 0; i < 5; i++)
        {
            floorPosition = new Vector3(room.transform.position.x - 4, -i + room.transform.position.y - 16, room.transform.position.z);
            Instantiate(bottomFloorPrefab, floorPosition, Quaternion.identity, room.transform);

        }
        for (int i = 0; i < 4; i++)
        {
            floorPosition = new Vector3(room.transform.position.x - 4, i + room.transform.position.y + 1, room.transform.position.z);
            Instantiate(topFloorPrefab, floorPosition, Quaternion.identity, room.transform);
        }
    }
}
