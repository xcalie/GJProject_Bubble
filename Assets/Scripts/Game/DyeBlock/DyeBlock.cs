using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyeBlock : MonoBehaviour
{
    public BubbleType type;//染色块类型
    public Collider2D cdr;//碰撞器
    public SpriteRenderer spr;//精灵渲染器

    private void Awake()
    {
        //组件获取
        cdr = GetComponent<Collider2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        //设置颜色
        spr.color = CheckColor.CheckNowColor(type);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BubbleFloat"))
        {
            //获取泡泡
            BubbleFloat bubble = collision.GetComponent<BubbleFloat>();
            //改变颜色
            bubble.ChangeColor(this.type);
        }

        if (collision.CompareTag("BubbleCombine"))
        { 
            BubbleCombine oldBubble = collision.GetComponent<BubbleCombine>();
            if (oldBubble.isContainByYellow)
            {
                return;
            }
            if (oldBubble.type != BubbleType.None)
            {
                return;
            }
            try
            {
                GameObject newObj = PoolManager.Instance.GetObj(CheckColor.CheckCombineType(this.type));
                BubbleCombine newBubble = newObj.GetComponent<BubbleCombine>();
                newBubble.StickOnPlayer(oldBubble.PlayerStick, collision.transform.position, true);

                AudioManager.Instance.addAfterLoad += () =>
                {
                    AudioManager.Instance.AddChildWithAudioSource(this.gameObject, AudioType.Effect, 5).Play();
                };

                PoolManager.Instance.PushObj(collision.gameObject);
            }
            catch (KeyNotFoundException)
            {
                GameObject obj = PoolManager.Instance.GetObj(CheckColor.CheckCombineType(oldBubble.type));
                Destroy(collision.gameObject);
                PoolManager.Instance.PushObj(obj);
            }
        }
    }
}
