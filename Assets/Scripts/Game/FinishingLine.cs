using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 终点线判定脚本
/// </summary>
public class FinishingLine : MonoBehaviour
{
    [Header("触发设置")]
    [Tooltip("是否只触发一次")]
    [SerializeField] private bool triggerOnce = true;

    [Tooltip("是否在触发后销毁此物体")]
    [SerializeField] private bool destroyAfterTrigger = false;

    [Header("事件")]
    [Tooltip("玩家到达终点时触发的事件")]
    public UnityEvent onPlayerReached;

    [Header("目标对象")]
    [Tooltip("到达终点后需要激活的游戏对象")]
    [SerializeField] private GameObject targetObject;

    private bool hasTriggered = false;

    private void OnCollisionEnter2D(Collision2D other)
    {
        // 如果已经触发过且设置为只触发一次，则返回
        if (hasTriggered && triggerOnce)
            return;

        // 检查碰撞物体是否是玩家
        if (other.gameObject.CompareTag("Player"))
        {
            // 触发到达终点事件
            onPlayerReached?.Invoke();

            hasTriggered = true;

            // 激活目标对象
            if (targetObject != null)
            {
                targetObject.SetActive(true);
            }

            Debug.Log("玩家到达终点");

            if (destroyAfterTrigger)
            {
                Destroy(gameObject);
            }
        }
    }

}
