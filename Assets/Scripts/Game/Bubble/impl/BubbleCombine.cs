using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BubbleCombine : BaseBubble
{
    public GameObject PlayerStick;
    public bool isContainByYellow = false;

    #region 压缩相关参数

    [Header("压缩相关参数")]
    [SerializeField] private float burstThreshold = 0.7f; // 爆炸的阈值
    [SerializeField] private float maxCompressionDistance = 1f; // 最大压缩距离
    [SerializeField] private float expansionMultiplier = 0.5f; // 垂直方向膨胀系数

    private Vector3 originalScale; // 原始缩放值
    private bool isCompressing = false; // 是否正在压缩
    private Vector2 compressionDirection; // 压缩方向
    private Vector2 initialCollisionPoint; // 初始碰撞点
    private float currentCompressionRatio = 0f; // 当前压缩比例

    #endregion

    #region 生命周期函数

    protected override void Awake()
    {
        base.Awake();
        originalScale = transform.localScale;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Update()
    {
        base.Update();
    }

    #endregion

    public void StickOnPlayer(GameObject player, Vector3 CrashPos, bool isOld = false)
    {
        /*
        if (this.type == BubbleType.Red)
        {
            isOld = true;
        }
        */

        PlayerStick = player;
        this.transform.SetParent(player.transform);
        //player.GetComponent<Player>().BubbleList.Add(this);

        AudioManager.Instance.addAfterLoad += () =>
        {
            Debug.Log("载入音效");
            AudioManager.Instance.AddChildWithAudioSource(player.gameObject, AudioType.Effect, 2).Play();
        };

        //碰撞点
        Vector3 hitPos = CrashPos;
        //碰撞点到玩家的方向
        Vector3 dir = (hitPos - player.transform.position).normalized;
        //碰撞点到玩家的距离
        float dis;
        if (isOld)
        {
            dis = Vector3.Distance(hitPos, player.transform.position);
        }
        else
        {
            dis = Vector3.Distance(hitPos, player.transform.position) * ContentBubble.BubbleClosestDistance;
        }
        //设置位置
        this.transform.localPosition = dir * dis;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查是否与标签为Ground的物体碰撞
        if (collision.gameObject.CompareTag("Ground"))
        {
            // 获取碰撞点的法线和位置
            compressionDirection = collision.contacts[0].normal;
            initialCollisionPoint = collision.contacts[0].point;
            isCompressing = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 检查是否与标签为Ground的物体碰撞
        if (collision.gameObject.CompareTag("Ground"))
        {
            // 获取当前碰撞点
            Vector2 currentCollisionPoint = collision.contacts[0].point;

            // 计算压缩距离
            float compressionDistance = Vector2.Distance(initialCollisionPoint, currentCollisionPoint);

            // 计算压缩比例
            currentCompressionRatio = Mathf.Clamp01(compressionDistance / maxCompressionDistance);

            // 检查是否达到爆炸阈值
            if (currentCompressionRatio >= burstThreshold)
            {
                Burst();
                return;
            }

            // 更新物体缩放
            UpdateCompression();
        }
    }

    private void UpdateCompression()
    {
        // 根据碰撞法线方向计算新的缩放值
        Vector3 newScale = originalScale;

        // 在碰撞方向上直接压缩
        newScale.x = originalScale.x * (1f - Mathf.Abs(compressionDirection.x) * currentCompressionRatio);
        newScale.y = originalScale.y * (1f - Mathf.Abs(compressionDirection.y) * currentCompressionRatio);

        // 在垂直于碰撞方向上略微膨胀以保持视觉体积
        float expansionAmount = currentCompressionRatio * expansionMultiplier;
        newScale.x += originalScale.x * Mathf.Abs(compressionDirection.y) * expansionAmount;
        newScale.y += originalScale.y * Mathf.Abs(compressionDirection.x) * expansionAmount;

        // 应用缩放
        transform.localScale = newScale;
    }

    private IEnumerator RecoverShape()
    {
        float recoverySpeed = 2f; // 恢复速度

        while (currentCompressionRatio > 0 && gameObject.activeInHierarchy)
        {
            currentCompressionRatio -= Time.deltaTime * recoverySpeed;
            currentCompressionRatio = Mathf.Max(0, currentCompressionRatio);

            UpdateCompression();

            yield return null;
        }

        // 确保完全恢复到原始大小
        transform.localScale = originalScale;
        currentCompressionRatio = 0f;
    }

    private void Burst()
    {
        // 调用基类的Attacked方法处理销毁逻辑
        Attacked();

        // TODO: 在这里添加爆炸特效
        // TODO: 在这里添加爆炸音效
    }

    public override void Attacked()
    {
        //PlayerStick.GetComponent<Player>().BubbleList.Remove(this);
        //清理协程
        StopAllCoroutines();
        base.Attacked();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isCompressing = false;
            // 只有在游戏对象激活时才启动恢复形状的协程
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(RecoverShape());
            }
            else
            {
                // 如果对象已经不活跃，直接重置状态
                currentCompressionRatio = 0f;
                transform.localScale = originalScale;
            }
        }
    }
}
