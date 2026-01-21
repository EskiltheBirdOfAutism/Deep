using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DeveloperIntro : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ChangeScene());
    }

    // Update is called once per frame
    void Update()
    {

    }
    //har scenen öppen under hela videon
    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(6.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
