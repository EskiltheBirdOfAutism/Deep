using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MineScript : MonoBehaviour
{
    public GameObject explotionPrefab;
    public GameObject audioPlayerPrefab;
    public List<AudioClip> clipList;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("NonGrabable"))
        {
            Explode();
        }
    }

    private int explotionRadius;
    private int damage = 10;
    private void Explode()
    {
        explotionRadius = Random.RandomRange(2, 5);
        damage -= ((int)explotionRadius);

        GameObject explotion = Instantiate(explotionPrefab, transform.position, transform.rotation);
        ExplotionScript explotionScript = explotion.GetComponent<ExplotionScript>();
        SphereCollider sphereCollider = explotion.GetComponent<SphereCollider>();
        //sphereCollider.radius = explotionRadius;
        explotion.transform.localScale = new Vector3(explotionRadius * 2.5f, explotionRadius * 2.5f, explotionRadius * 2.5f);
        explotionScript.explotionDamage = damage;
        explotionScript.radius = explotionRadius;

        GameObject audioPlayer = Instantiate(audioPlayerPrefab, transform.position, Quaternion.identity);
        AudioSource audioSource = audioPlayer.GetComponent<AudioSource>();
        if (clipList.Count > 0)
        {
            int index = Random.Range(0, clipList.Count);
            audioSource.clip = clipList[index];
            audioSource.Play();
        }

        Destroy(gameObject);
    }
}
