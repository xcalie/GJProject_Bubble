using UnityEngine;

/// <summary>
/// 子弹行为控制脚本
/// </summary>
public class Bullet : MonoBehaviour
{

    [Tooltip("子弹击中特效预制体")]
    [SerializeField] private GameObject hitEffectPrefab;

    // 子弹速度和方向将由发射者设置
    private Vector2 velocity;
    private bool isInitialized;

    /// <summary>
    /// 初始化子弹
    /// </summary>
    /// <param name="speed">子弹速度</param>
    /// <param name="direction">子弹方向</param>
    public void Initialize(float speed, Vector2 direction)
    {
        velocity = direction.normalized * speed;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        // 更新子弹位置
        transform.Translate(velocity * Time.deltaTime);

        // 检查子弹是否超出屏幕范围
        CheckBounds();
    }

    /// <summary>
    /// 检查子弹是否超出屏幕范围
    /// </summary>
    private void CheckBounds()
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPoint.x < -0.1f || viewportPoint.x > 1.1f ||
            viewportPoint.y < -0.1f || viewportPoint.y > 1.1f)
        {
            // 回收子弹
            PoolManager.Instance.PushObj(gameObject);
        }
    }

    /// <summary>
    /// 当子弹触发器碰撞时调用
    /// </summary>
    /// <param name="other">触发器碰撞对象</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞对象的标签
        if (other.CompareTag("BubbleFloat") || other.CompareTag("BubbleCombine"))
        {
            if (other.GetComponent<BaseBubble>().type == BubbleType.Yellow)
            {
                PoolManager.Instance.PushObj(gameObject);
                return;
            }
            other.GetComponent<BaseBubble>().Attacked();
            PoolManager.Instance.PushObj(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            // 玩家受到伤害
            other.GetComponent<Player>().Dead();
        }
        // 回收子弹
        PoolManager.Instance.PushObj(gameObject);
    }

    // 在编辑器中绘制子弹的碰撞范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
