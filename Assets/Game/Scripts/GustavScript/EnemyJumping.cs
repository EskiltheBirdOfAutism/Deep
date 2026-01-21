using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyJumping : MonoBehaviour
{
    //private GameObject grid;
    private EnemyMovement enemyMovement;
    private Rigidbody rigid_body;
    private Collider collider;
    private void Start()
    {
        collider = GetComponent<Collider>();
        enemyMovement = GetComponent<EnemyMovement>();
        rigid_body = GetComponent<Rigidbody>();
    }
    private void FindJumpableBlock()
    {
        //Debug.Log("FindJumpableBlock");
        Vector3 direction = (enemyMovement.target.transform.position - transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit))
        {
            GameObject currentBlock = hit.collider.gameObject;
            enemyMovement.jumpTarget = GetTopBlock(currentBlock);

            //Debug.Log("Final jump target: " + (enemyMovement.jumpTarget != null ? enemyMovement.jumpTarget.name : "None"));
        }

        // Debug line to see the ray
        //Debug.DrawRay(transform.position, direction * 100f, Color.red);
    }

    public GameObject HasBlockAbove(GameObject block)
    {
        //Debug.Log("HasBlockAbove");

        Vector3 origin = block.transform.position + Vector3.up * 0.51f;
        float rayDistance = 2f;

        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            Vector3.up,
            rayDistance,
            enemyMovement.wall,
            QueryTriggerInteraction.Collide
        );

        //Debug.DrawRay(origin, Vector3.up * rayDistance, Color.red, 2f);
        //Debug.Log("Hits: " + hits.Length);

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject != block)
                return hit.collider.gameObject;
        }

        return null;
    }



    private GameObject GetTopBlock(GameObject startingBlock)
    {
        //Debug.Log("GetTopBlock");

        GameObject currentBlock = startingBlock;
        GameObject aboveBlock;

        int maxIterations = 50;
        int count = 0;

        while (count < maxIterations)
        {
            count++;

            aboveBlock = HasBlockAbove(currentBlock);
            //Debug.Log("above block" + aboveBlock);

            if (aboveBlock == null)
                return currentBlock; // top of stack reached

            // Optional: check for roof blocking jump
            float maxJumpHeight = enemyMovement.jumpHeight + 0.5f; // buffer
            Vector3 roofCheckOrigin = aboveBlock.transform.position + Vector3.up * (aboveBlock.transform.localScale.y / 2 + 0.01f);

            if (Physics.Raycast(roofCheckOrigin, Vector3.up, maxJumpHeight))
            {
                // roof above prevents jumping higher
                return currentBlock;
            }

            currentBlock = aboveBlock;
        }

        //Debug.LogWarning("GetTopBlock exceeded max iterations!");
        return currentBlock;
    }


    public IEnumerator JumpRoutine()
    {
        //Debug.Log("JumpRoutine");
        if (enemyMovement.grid != null) Destroy(enemyMovement.grid);

        FindJumpableBlock();
        if (enemyMovement.jumpTarget == null)
        {
            Debug.LogWarning("No jump target found, aborting jump");
            enemyMovement.isJumping = false;
            enemyMovement.RestartMoving();
            yield break;
        }

        enemyMovement.isJumping = true;
        Vector3 startPos = transform.position;
        Vector3 targetTop = enemyMovement.jumpTarget.transform.position + Vector3.up * (enemyMovement.jumpTarget.transform.localScale.y / 2 + 0.01f);

        float timer = 0f;

        // Stop physics interference
        if (!rigid_body.isKinematic)
        {
            rigid_body.linearVelocity = Vector3.zero;
        }
        rigid_body.isKinematic = true;
        collider.enabled = false;


        while (timer < enemyMovement.jumpDuration)
        {
            timer += Time.deltaTime;
            float t = timer / enemyMovement.jumpDuration;

            float height = 4f * enemyMovement.jumpHeight * t * (1 - t);

            transform.position =
                Vector3.Lerp(startPos, targetTop, t) +
                Vector3.up * height;
            yield return null;
        }
        float yPos = Mathf.Round(transform.position.y + 0.5f);

        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);

        rigid_body.isKinematic = false;
        collider.enabled = true;
        enemyMovement.isJumping = false;

        enemyMovement.RestartMoving();

        enemyMovement.isJumping = false;
    }
}
