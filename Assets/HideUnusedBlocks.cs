using UnityEngine;
using System.Collections.Generic;

public class HideUnusedBlocks : MonoBehaviour
{
    private destroyCube[] blocks = new destroyCube[0];
    [SerializeField] private float cubeSize = 1f;
    [SerializeField] private LayerMask blockLayer; // Set this in Inspector

    private HashSet<destroyCube> hiddenCubes = new HashSet<destroyCube>();

    private void Start()
    {
        blocks = GetComponentsInChildren<destroyCube>();
        HideInteriorCubes();
    }

    public void OnCubeDestroyed(Vector3 destroyedPosition)
    {
        Vector3[] directions = {
            Vector3.forward, Vector3.back,
            Vector3.up, Vector3.down,
            Vector3.left, Vector3.right
        };

        foreach (var direction in directions)
        {
            Vector3 neighborPos = destroyedPosition + (direction * cubeSize);

            destroyCube neighbor = FindCubeAtPosition(neighborPos);

            if (neighbor != null && !neighbor.gameObject.activeSelf)
            {
                if (!IsCubeFullySurrounded(neighbor))
                {
                    neighbor.gameObject.SetActive(true);
                    hiddenCubes.Remove(neighbor);
                }
            }
        }
    }

    private void HideInteriorCubes()
    {
        List<destroyCube> cubesToHide = new List<destroyCube>();

        foreach (var block in blocks)
        {
            if (block == null) continue;

            if (IsCubeFullySurrounded(block))
            {
                cubesToHide.Add(block);
            }
        }

        foreach (var block in cubesToHide)
        {
            block.gameObject.SetActive(false);
            hiddenCubes.Add(block);
        }

        Debug.Log($"Hidden {cubesToHide.Count} interior cubes out of {blocks.Length} total");
    }

    private bool IsCubeFullySurrounded(destroyCube block)
    {
        Vector3[] directions = {
            Vector3.forward, Vector3.back,
            Vector3.up, Vector3.down,
            Vector3.left, Vector3.right
        };

        Vector3 startPos = block.transform.position;
        int blockedSides = 0;

        foreach (var direction in directions)
        {
            // Only check against objects on the blockLayer
            if (Physics.Raycast(startPos, direction, cubeSize, blockLayer))
            {
                blockedSides++;
            }
        }

        return blockedSides == 6;
    }

    private destroyCube FindCubeAtPosition(Vector3 position)
    {
        foreach (var block in blocks)
        {
            if (block != null && Vector3.Distance(block.transform.position, position) < 0.1f)
            {
                return block;
            }
        }
        return null;
    }
}