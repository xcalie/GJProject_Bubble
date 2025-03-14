using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCombine : BubbleCombine
{
    public float Distance = 0.5f; // 爆炸距离

    private Vector2 BeforeDead; // 爆炸前碰撞的位置

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Update()
    {
        base.Update();
        this.transform.SetParent(null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BubbleCombine"))
        {
            BaseBubble bubble = collision.GetComponent<BaseBubble>();

            if (bubble.type == BubbleType.Yellow)
            {
                BeforeDead = collision.transform.position;
                StartCoroutine(Hited(BeforeDead));
                this.Attacked();
            }
            else
            {
                BeforeDead = collision.transform.position;
                StartCoroutine(Hited(BeforeDead));
                bubble.Attacked();
                this.Attacked();
            }
        }
        else if (collision.CompareTag("Player"))
        {
            // TODO：玩家死亡
            this.Attacked();
            this.PlayerStick.GetComponent<Player>().Dead();
        }
    }

    private IEnumerator Hited(Vector2 pos)
    {
        float time = 0;
        Vector2 dir = ((Vector2)this.transform.position - pos).normalized;
        float HasMove = 0;
        while (true)
        {
            HasMove += ((Vector3)dir * Time.deltaTime * 10).magnitude;
            this.transform.position += (Vector3)dir * Time.deltaTime * 10;
            if (HasMove >= Distance)
            {
                break;
            }
            yield return null;
        }
    }
}
