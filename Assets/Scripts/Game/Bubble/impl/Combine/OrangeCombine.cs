using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// 橙色泡泡的组合效果实现
/// 特性：接触后染上已有的一半泡泡，根据染色数量加速
/// </summary>
public class OrangeCombine : BubbleCombine
{
    [Header("橙色泡泡参数")]
    [SerializeField] private float dyeRadius = 2f; // 染色范围
    [SerializeField] private float speedBoostPerBubble = 0.1f; // 每个染色泡泡提供的速度加成

    private int dyedBubbleCount = 0; // 已染色的泡泡数量
    private bool isDyeing = false; // 是否正在染色

    protected override void OnEnable()
    {
        base.OnEnable();
        type = BubbleType.Orange; // 设置泡泡类型为橙色
        dyedBubbleCount = 0;
        isDyeing = false;
    }

    protected override void Update()
    {
        base.Update();
        this.transform.SetParent(null);
    }

    private void LateUpdate()
    {
        try
        {
            Player player = this.transform.parent.GetComponent<Player>();
        }
        catch (Exception)
        {
            this.Attacked();
        }
    }

    /// <summary>
    /// 当与其他泡泡或玩家接触时触发染色效果
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDyeing)
        {
            return;
        }
        if (other.CompareTag("BubbleCombine") || other.CompareTag("Player"))
        {
            StartDyeEffect();
        }
    }

    /// <summary>
    /// 开始染色效果
    /// </summary>
    private void StartDyeEffect()
    {
        // 获取范围内的所有泡泡
        Collider2D[] nearbyBubbles = Physics2D.OverlapCircleAll(transform.position, dyeRadius);
        int newDyedCount = 0; // 新染色的泡泡数量

        foreach (var bubble in nearbyBubbles)
        {
            if (bubble.gameObject != gameObject)
            {
                if (bubble.CompareTag("Player"))
                {
                    continue;
                }
                BaseBubble targetBubble = bubble.GetComponent<BaseBubble>();
                if (targetBubble != null && targetBubble.type != BubbleType.Orange)
                {
                    if (targetBubble.isDead)
                    {
                        continue;
                    }
                    // 染色
                    //targetBubble.ChangeColor(BubbleType.Orange);
                    targetBubble.Attacked();
                    newDyedCount++;
                }
            }
        }

        isDyeing = true;

        // 更新染色计数并应用速度加成
        if (newDyedCount >= 0 && PlayerStick != null)
        {
            //dyedBubbleCount += newDyedCount;
            UpdatePlayerSpeed();
        }
    }

    /// <summary>
    /// 更新玩家速度
    /// </summary>
    private void UpdatePlayerSpeed()
    {
        if (PlayerStick != null)
        {
            Player player = PlayerStick.GetComponent<Player>();
            if (player != null)
            {
                // 根据染色数量计算速度加成
                //float speedMultiplier = 1f + (dyedBubbleCount * speedBoostPerBubble);
                // TODO: 调用Player脚本中的速度调整方法
                //PlayerStick.GetComponent<Player>().Accelerate(PlayerStick.GetComponent<Player>().Dirt ,speedMultiplier * 0.1f);
                PlayerStick.GetComponent<Player>().Accelerate();
            }
        }
        this.Attacked();
    }

    /// <summary>
    /// 在编辑器中显示染色范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // 半透明橙色
        Gizmos.DrawWireSphere(transform.position, dyeRadius);
    }

    public override void Attacked()
    {
        dyedBubbleCount = 0;
        /*
        if (PlayerStick != null)
        {
            Player player = PlayerStick.GetComponent<Player>();
            if (player != null)
            {
                // TODO: 重置玩家速度
                // player.ResetSpeedMultiplier();
            }
        }
        */
        base.Attacked();
    }
}