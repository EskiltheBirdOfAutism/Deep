using UnityEngine;
using System.Collections;
public class ViewBob : MonoBehaviour
{
    public float Speed = 0.025f;
    public bool IsFullWave = true;
    public Vector3 Amount = new Vector3(1, 0, 0);
    private float timer = 0.0f;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void Update()
    {
        float moveCyclePercent = Mathf.Sin(timer);
        timer += Speed;
        if (timer > Mathf.PI * (IsFullWave ? 2 : 1))
        {
            timer = 0.0f;
        }

        Vector3 offset = Vector3.zero;
        if (Amount.x != 0)
        {
            offset.x = Amount.x * moveCyclePercent;
        }
        if (Amount.y != 0)
        {
            offset.y = Amount.y * moveCyclePercent;
        }
        if (Amount.z != 0)
        {
            offset.z = Amount.z * moveCyclePercent;
        }

        transform.localPosition = originalPosition + offset;
    }
}