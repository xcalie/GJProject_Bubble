using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EasyCamera : MonoBehaviour
{
    public Transform target;
    public Vector2 Old;
    public Vector2 Start;
    public Vector2 offset;

    public float smoothSpeed = 0.125f;
    public float timer = 0;
    public float distance = 1;
    private void FixedUpdate()
    { 
        Vector2 desiredPosition = new Vector2(target.position.x, target.position.y);
        if (Old != desiredPosition)
        {
            Old = target.position;
            Start = this.transform.position;
            distance = Vector2.Distance(Start, Old);
        }
        this.transform.position = Vector2.Lerp(Start + offset, Old + offset, smoothSpeed * timer * distance);
        this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, -10);
        timer += Time.deltaTime;
    }

}
