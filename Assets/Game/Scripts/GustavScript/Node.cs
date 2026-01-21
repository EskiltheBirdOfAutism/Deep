using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 world_position = new Vector3(0, 0, 0);

    public int h_cost;
    public int g_cost;
    public int x_g;
    public int y_g;
    public Node parent;

    public Node(bool _walkable, Vector3 _world_position, int _x_g, int _y_g)
    {
        walkable = _walkable;
        world_position = _world_position;
        x_g = _x_g;
        y_g = _y_g;
    }

    public int f_cost
    {
        get
        {
            return h_cost + g_cost;
        }
    }
}