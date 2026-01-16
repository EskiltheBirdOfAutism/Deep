using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ProjectileEnemy : MonoBehaviour
{
    public List<AudioClip> sfxClips = new List<AudioClip>();
    public float timeBetweenSounds = 2f;
    private AudioSource audioSource;
    private float soundTimer = 0f;
    private bool waitingForNextSound = false;

    private void Awake()
    {
        StartCoroutine(WaitAndDestroy());
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        HandleSound();
    }
    private void HandleSound()
    {
        if (!audioSource.isPlaying && !waitingForNextSound)
        {
            waitingForNextSound = true;
            soundTimer = Random.Range(60, 180);
        }

        if (waitingForNextSound)
        {
            soundTimer -= Time.deltaTime;

            if (soundTimer <= 0f)
            {
                PlayNextSound();
            }
        }
    }
    private void PlayNextSound()
    {
        if (sfxClips.Count == 0)
            return;

        AudioClip next = sfxClips[Random.Range(0, sfxClips.Count)];

        audioSource.clip = next;
        audioSource.Play();

        waitingForNextSound = false;
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(Random.Range(6, 18));
        Destroy(gameObject);
    }
}
