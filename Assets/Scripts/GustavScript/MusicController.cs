using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public float pullSpeed = 5f;

    private List<Transform> players = new List<Transform>();

    [Header("Music")]
    public List<AudioClip> musicTracks = new List<AudioClip>();
    public float timeBetweenSongs = 10f;
    private AudioSource audioSource;
    private float songTimer = 0f;
    private bool waitingForNextSong = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        FindPlayers();
        PlayNextSong();
    }

    void Update()
    {
        if (players.Count == 0)
            return;

        HandleMusic();

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

    private void HandleMusic()
    {
        // Song finished start cooldown
        if (!audioSource.isPlaying && !waitingForNextSong)
        {
            waitingForNextSong = true;
            songTimer = Random.Range(60, 180);
        }

        // Countdown between songs
        if (waitingForNextSong)
        {
            songTimer -= Time.deltaTime;

            if (songTimer <= 0f)
            {
                PlayNextSong();
            }
        }
    }

    private void PlayNextSong()
    {
        if (musicTracks.Count == 0)
            return;

        AudioClip next = musicTracks[Random.Range(0, musicTracks.Count)];

        audioSource.clip = next;
        audioSource.pitch += Random.Range(-0.2f, 0.2f);
        audioSource.volume -= Random.Range(0.1f, 0.35f);
        audioSource.Play();

        waitingForNextSong = false;
    }
}
