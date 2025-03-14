using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 绿色泡泡的组合效果实现
/// 特性：将身上泡泡全部变绿且增殖一半，体积减半，变绿后免疫其他染色
/// </summary>
public class GreenCombine : BubbleCombine
{
    [Header("绿色泡泡参数")]
    [SerializeField] private float multiplyDelay = 0.2f; // 增殖延迟时间
    [SerializeField] private float sizeReductionRatio = 0.5f; // 体积减小比例

    private bool hasTriggeredEffect = false; // 是否已触发效果
    protected override void OnEnable()
    {
        base.OnEnable();
        type = BubbleType.Green; // 设置泡泡类型为绿色
    }

    protected override void Update()
    {
        base.Update();
        if (hasTriggeredEffect)
        {
            this.Attacked();
        }
        else
        {
            this.transform.SetParent(null);
        }
    }

    /// <summary>
    /// 当与其他泡泡或玩家接触时触发效果
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 如果已经触发过效果，则不触发
        if (hasTriggeredEffect) return;

        // 如果接触到其他泡泡或玩家，则触发效果
        if (other.CompareTag("BubbleCombine") || other.CompareTag("Player"))
        {
            StartGreenEffect();
        }
    }

    /// <summary>
    /// 开始绿色效果
    /// </summary>
    private void StartGreenEffect()
    {
        hasTriggeredEffect = true;

        if (PlayerStick != null)
        {
            Player player = PlayerStick.GetComponent<Player>();
            if (player != null)
            {
                // 获取玩家身上所有的泡泡
                List<BaseBubble> bubbles = player.gameObject.GetComponentsInChildren<BaseBubble>().ToList();
                int originalCount = bubbles.Count;

                // 将所有泡泡变绿并减小体积
                foreach (BaseBubble bubble in bubbles)
                {
                    //如果是绿色的泡泡，则不染色
                    if (bubble.type == BubbleType.Green)
                    {
                        continue;
                    }
                    if (bubble != null && bubble != this)
                    {
                        bubble.ChangeColor(BubbleType.Green);
                        bubble.ChangeSize(bubble.transform.localScale.x * sizeReductionRatio);
                    }
                }

                // 计算需要增殖的数量（向上取整）
                int multiplyCount = Mathf.CeilToInt(originalCount * 0.5f);
                //StartCoroutine(MultiplyBubbles(multiplyCount));

                #region 增殖泡泡

                for (int i = 0; i < multiplyCount; i++)
                {
                    GameObject newBubbleObj = PoolManager.Instance.GetObj(CheckColor.CheckCombineType(BubbleType.None));
                    if (newBubbleObj != null)
                    {
                        // 设置新泡泡的位置和大小
                        newBubbleObj.transform.position = transform.position + Random.insideUnitSphere * 0.5f;

                        BubbleCombine newBubble = newBubbleObj.GetComponent<BubbleCombine>();
                        if (newBubble != null)
                        {
                            // 设置新泡泡的大小为当前大小
                            newBubble.ChangeSize(transform.localScale.x);
                            // 设置新泡泡的颜色为绿色
                            newBubble.ChangeColor(BubbleType.Green);
                            // 将新泡泡粘在玩家身上
                            newBubble.StickOnPlayer(PlayerStick, transform.position);

                            /*
                            // 获取GreenCombine组件并立即设置hasTriggeredEffect为true
                            GreenCombine greenBubble = newBubbleObj.GetComponent<GreenCombine>();
                            if (greenBubble != null)
                            {
                                greenBubble.hasTriggeredEffect = true; // 禁用新生成泡泡的增殖能力
                                greenBubble.StickOnPlayer(PlayerStick, transform.position);
                            }
                            */
                        }
                    }
                }
                #endregion
            }
        }
        // 自身也要变小
        ChangeSize(transform.localScale.x * sizeReductionRatio);
    }

    /// <summary>
    /// 增殖泡泡的协程
    /// </summary>
    private IEnumerator MultiplyBubbles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newBubbleObj = PoolManager.Instance.GetObj(CheckColor.CheckCombineType(BubbleType.None));
            if (newBubbleObj != null)
            {
                // 设置新泡泡的位置和大小
                newBubbleObj.transform.position = transform.position + Random.insideUnitSphere * 0.5f;

                BubbleCombine newBubble = newBubbleObj.GetComponent<BubbleCombine>();
                if (newBubble != null)
                {
                    // 设置新泡泡的大小为当前大小
                    newBubble.ChangeSize(transform.localScale.x);
                    // 设置新泡泡的颜色为绿色
                    newBubble.ChangeColor(BubbleType.Green);
                    // 将新泡泡粘在玩家身上
                    newBubble.StickOnPlayer(PlayerStick, transform.position);

                    /*
                    // 获取GreenCombine组件并立即设置hasTriggeredEffect为true
                    GreenCombine greenBubble = newBubbleObj.GetComponent<GreenCombine>();
                    if (greenBubble != null)
                    {
                        greenBubble.hasTriggeredEffect = true; // 禁用新生成泡泡的增殖能力
                        greenBubble.StickOnPlayer(PlayerStick, transform.position);
                    }
                    */
                }
            }

            yield return new WaitForSeconds(multiplyDelay);
        }
    }

    public override void ChangeColor(BubbleType newType)
    {
        // 已经是绿色的泡泡免疫其他染色效果
        if (type == BubbleType.Green)
        {
            return;
        }
        base.ChangeColor(newType);
    }

    /// <summary>
    /// 在编辑器中显示效果范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // 半透明绿色
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    public override void Attacked()
    {
        StopAllCoroutines();
        base.Attacked();
    }
}