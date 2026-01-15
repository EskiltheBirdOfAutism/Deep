using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public float pullSpeed = 5f;

    private List<Transform> players = new List<Transform>();

    void Start()
    {
        FindPlayers();
    }

    void Update()
    {
        if (players.Count == 0)
            return;

        Vector3 center = GetPlayersCenter();

        // Pull this object toward the center
        transform.position = Vector3.MoveTowards(
            transform.position,
            center,
            pullSpeed * Time.deltaTime
        );
    }

    void FindPlayers()
    {
        players.Clear();

        PlayerContoller[] foundPlayers = FindObjectsOfType<PlayerContoller>();

        foreach (PlayerContoller p in foundPlayers)
        {
            players.Add(p.transform);
        }
    }

    Vector3 GetPlayersCenter()
    {
        Vector3 sum = Vector3.zero;

        foreach (Transform t in players)
        {
            sum += t.position;
        }

        return sum / players.Count;
    }
}
