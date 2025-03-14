using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BubbleType
{
    Red, //红色 爆炸
    Yellow, //黄色 无敌
    Orange, //橙色 加速
    Green, //绿色 增殖，分裂，变小
    None, //无色 无效果
}

public enum MoveType
{
    Float, //浮动
    Combine, //合并
}

public abstract class BaseBubble : MonoBehaviour
{
    private BubbleType OrginType;//原始类型
    public BubbleType type;//泡泡类型
    public MoveType moveType;//移动类型
    public SpriteRenderer spr;//精灵渲染器
    public Animator anim;//动画控制器

    public bool isDead = false;

    protected virtual void Awake()
    {
        //组件获取
        spr = this.GetComponent<SpriteRenderer>();
        anim = this.GetComponent<Animator>();

        //初始化
        OrginType = this.type;
    }

    protected virtual void OnEnable()
    {
        type = OrginType;
        this.transform.localScale = Vector3.one;
        isDead = false;
    }

    protected virtual void Update()
    {
        CheckOfColor();
    }

    public virtual void Attacked()
    {
        isDead = true;
        /*
        AudioManager.Instance.addAfterLoad += () =>
        {
            try
            {
                AudioManager.Instance.AddChildWithAudioSource(this.gameObject, AudioType.Effect, 4);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("音效添加，失败对象已经失活了，不用管");
            }
        };
        */
        //等待动画播放完毕
        float time = anim.GetCurrentAnimatorStateInfo(1).length;
        anim.SetTrigger("isAttacked");
        Invoke("CheckAndPushSelf", time - 0.3f);
    }

    public void CheckAndPushSelf()
    {
        PoolManager.Instance.PushObj(this.gameObject);
        /*
        try
        {
            PoolManager.Instance.PushObj(this.gameObject);
        }
        catch (KeyNotFoundException)
        {
            GameObject obj;
            switch (this.moveType)
            {
                case MoveType.Float:
                    obj = PoolManager.Instance.GetObj(CheckColor.CheckFloatType(this.type));
                    break;
                case MoveType.Combine:
                    obj = PoolManager.Instance.GetObj(CheckColor.CheckCombineType(this.type));
                    break;
                default:
                    Destroy(this.gameObject);
                    return;
            }
            Destroy(this.gameObject);
            PoolManager.Instance.PushObj(obj);
        }
        */
    }

    public void CheckOfColor()
    {
        spr.color = CheckColor.CheckNowColor(type);
    }

    public virtual void ChangeColor(BubbleType type)
    {
        this.type = type;
    }

    public void ChangeSize(float size)
    {
        float scale = Mathf.Clamp(size, ContentBubble.BubbleSizeLeast, ContentBubble.BubbleSizeMost);
        this.transform.localScale = new Vector3(scale, scale, 1);
    }

}
