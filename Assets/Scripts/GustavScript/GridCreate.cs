using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreate : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject grid;
    public GameObject player;
    public Transform seeker;
    public Transform target;
    int timer = 10;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer--;
        if (timer <= 0)
        {
            GameObject new_grid = Instantiate(grid, new Vector3(0f, 0f, 0f), Quaternion.identity);
            new_grid.GetComponent<GridCode>().player = player;
            new_grid.GetComponent<PathfindingCode>().seeker = seeker;
            new_grid.GetComponent<PathfindingCode>().target = target;
            Destroy(gameObject);
        }
    }
}