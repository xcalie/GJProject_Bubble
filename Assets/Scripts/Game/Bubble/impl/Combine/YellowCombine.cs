using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowCombine : BubbleCombine
{
    public bool isFinihsed = false;
    public float radius = 5.5f;

    protected override void OnEnable()
    {
        base.OnEnable();
        isFinihsed = false;
    }

    protected override void Update()
    {
        base.Update();

        if (!isFinihsed)
        {
            isFinihsed = true;
            CombineEffect();
        }
        CoverPlayerAndAllBubbles();
    }

    public void CoverPlayerAndAllBubbles()
    {
        this.transform.position = PlayerStick.transform.position;
        /* 废弃
        foreach (BubbleCombine bubble in PlayerStick.GetComponent<Player>().BubbleList)
        {
            if (bubble != this)
            {
                float distance = Vector2.Distance(this.transform.position, bubble.transform.position);
                if (distance > radius)
                {
                    radius = distance + 0.2f;
                }
            }
        }
        */
        this.transform.localScale = new Vector3(radius, radius, 1);
    }

    public void CombineEffect()
    {
        foreach (BubbleCombine bubble in PlayerStick.GetComponentsInChildren<BubbleCombine>())
        {
            bubble.isContainByYellow = true;
        }
        PlayerStick.gameObject.GetComponent<Player>().isContainByYellow = true; 
        Invoke(nameof(Finish), ContentBubble.YellowBubbleTime);
    }

    public void Finish()
    {
        isFinihsed = true;
        PlayerStick.gameObject.GetComponent<Player>().isContainByYellow = false;
        foreach (BubbleCombine bubble in PlayerStick.GetComponentsInChildren<BubbleCombine>())
        {
            bubble.isContainByYellow = false;
        }

        this.Attacked();
    }
}
