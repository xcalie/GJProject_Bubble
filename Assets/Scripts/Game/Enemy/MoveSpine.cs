using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpine : Spine
{
    public Transform start;
    public Transform end;

    public float speed = 1;

    private void Start()
    {
        transform.position = start.position;
    }

    private void Update()
    {
        transform.position = Mathf.PingPong(Time.time * speed, 1) * (end.position - start.position) + start.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(start.position, 0.1f);
        Gizmos.DrawSphere(end.position, 0.1f);
        Gizmos.DrawLine(start.position, end.position);
    }
}

