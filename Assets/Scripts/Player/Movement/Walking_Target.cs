using UnityEngine;

public class Walking_Target : MonoBehaviour
{
    [SerializeField] private float maxRaycastDistance = 10f;
    [SerializeField] private GameObject movementDirection;

    void Update()
    {
        if (movementDirection == null) return;

        RaycastHit[] hits = Physics.RaycastAll(movementDirection.transform.localPosition, Vector3.down, maxRaycastDistance);

        RaycastHit validHit = new RaycastHit();
        bool foundValidHit = false;

        foreach (RaycastHit hit in hits)
        {
            if (IsChildOfSameParent(hit.transform))
            {
                continue;
            }

            if (!foundValidHit || hit.distance < validHit.distance)
            {
                validHit = hit;
                foundValidHit = true;
            }
        }

        if (foundValidHit)
        {
            transform.position = new Vector3(transform.position.x, validHit.point.y, transform.position.z);
        }
    }

    private bool IsChildOfSameParent(Transform hitTransform)
    {
        Transform rootParent = GetRootParent(transform);
        Transform hitRootParent = GetRootParent(hitTransform);
        return rootParent == hitRootParent;
    }

    private Transform GetRootParent(Transform child)
    {
        Transform current = child;
        while (current.parent != null)
        {
            current = current.parent;
        }
        return current;
    }
}