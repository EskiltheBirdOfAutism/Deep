using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridCode : MonoBehaviour
{
    public GameObject player;
    public LayerMask unwalk_mask;
    public Vector2 grid_size;
    [SerializeField] private Vector2 grid_pos;
    public float node_radius;
    Node[,] grid;

    float node_d;
    public int grid_x, grid_y;
    public Vector3 path_pos = new Vector3(0, 0, 0);

    void Start()
    {
        node_d = node_radius * 2;

        grid_x = Mathf.RoundToInt(grid_size.x / node_d);
        grid_y = Mathf.RoundToInt(grid_size.y / node_d);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[grid_x, grid_y];

        Vector3 _bottom_left = transform.position - Vector3.right * grid_size.x / 2 - Vector3.forward * grid_size.y / 2;

        for (int x = 0; x < grid_x; x++)
        {
            for (int y = 0; y < grid_y; y++)
            {
                Vector3 _world_point = _bottom_left + Vector3.right * (x * node_d + node_radius) + Vector3.forward * (y * node_d + node_radius);
                bool _walkable = !(Physics.CheckSphere(_world_point, node_radius, unwalk_mask));
                grid[x, y] = new Node(_walkable, _world_point, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int _check_x = node.x_g + x;
                int _check_y = node.y_g + y;

                if (_check_x >= 0 && _check_x < grid_x && _check_y >= 0 && _check_y < grid_y)
                {
                    neighbours.Add(grid[_check_x, _check_y]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 _world_position)
    {
        float _percent_x = (_world_position.x + grid_size.x / 2) / grid_size.x;
        float _percent_y = (_world_position.z + grid_size.y / 2) / grid_size.y;
        _percent_x = Mathf.Clamp01(_percent_x);
        _percent_y = Mathf.Clamp01(_percent_y);

        int x = Mathf.RoundToInt((grid_x - 1) * _percent_x);
        int y = Mathf.RoundToInt((grid_y - 1) * _percent_y);

        return grid[x, y];
    }

    public List<Node> path;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(grid_size.x, 1, grid_size.y));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                Gizmos.DrawCube(n.world_position, Vector3.one * (node_d - 0.1f));
            }
        }
    }

    private void Update()
    {
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        if (Vector3.Distance(n.world_position, new Vector3(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y), Mathf.RoundToInt(player.transform.position.z))) < 1.5f && Vector3.Distance(n.world_position, player.transform.position) > 1f)
                        {
                            path_pos = n.world_position + new Vector3(0f, 1f, 0f);
                        }
                    }
                }
            }
        }
    }
}