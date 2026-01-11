using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathfindingCode : MonoBehaviour
{
    public Transform seeker;
    public Transform target;
    GridCode grid;

    void Awake()
    {
        grid = GetComponent<GridCode>();
    }

void Update()
{
    if (seeker == null || target == null || grid == null)
        return;

    FindPath(seeker.position, target.position);
}

    void FindPath(Vector3 _start_pos, Vector3 _target_pos)
    {
        Node _start_node = grid.NodeFromWorldPoint(_start_pos);
        Node _target_node = grid.NodeFromWorldPoint(_target_pos);

        List<Node> _open_set = new List<Node>();
        HashSet<Node> _closed_set = new HashSet<Node>();
        _open_set.Add(_start_node);

        while (_open_set.Count > 0)
        {
            Node _current_node = _open_set[0];

            for (int i = 0; i < _open_set.Count; i++)
            {
                if (_open_set[i].f_cost < _current_node.f_cost || _open_set[i].f_cost == _current_node.f_cost || _open_set[i].h_cost < _current_node.h_cost)
                {
                    _current_node = _open_set[i];
                }
            }

            _open_set.Remove(_current_node);
            _closed_set.Add(_current_node);

            if (_current_node == _target_node)
            {
                RetracePath(_start_node, _target_node);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(_current_node))
            {
                if (!neighbour.walkable || _closed_set.Contains(neighbour))
                {
                    continue;
                }

                int _new_cost = _current_node.g_cost + GetDistance(_current_node, neighbour);
                if (_new_cost < neighbour.g_cost || !_open_set.Contains(neighbour))
                {
                    neighbour.g_cost = _new_cost;
                    neighbour.h_cost = GetDistance(neighbour, _target_node);
                    neighbour.parent = _current_node;

                    if (!_open_set.Contains(neighbour))
                    {
                        _open_set.Add(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(Node _start_node, Node _end_node)
    {
        List<Node> path = new List<Node>();
        Node _current_node = _end_node;

        while (_current_node != _start_node)
        {
            path.Add(_current_node);
            _current_node = _current_node.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    int GetDistance(Node _node_a, Node _node_b)
    {
        int _dist_x = Mathf.Abs(_node_a.x_g - _node_b.x_g);
        int _dist_y = Mathf.Abs(_node_a.y_g - _node_b.y_g);

        if (_dist_x > _dist_y)
        {
            return 14 * _dist_x + 10 * (_dist_x - _dist_y);
        }
        else
        {
            return 14 * _dist_y + 10 * (_dist_y - _dist_x);
        }
    }
}