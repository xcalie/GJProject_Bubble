using UnityEngine;

/// <summary>
/// 刺的行为脚本
/// </summary>
public class Spine : MonoBehaviour
{

    /// <summary>
    /// 当发生碰撞时调用
    /// </summary>
    /// <param name="collision">碰撞信息</param>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    /// <summary>
    /// 当发生触发器碰撞时调用
    /// </summary>
    /// <param name="other">触发器碰撞对象</param>
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }

    /// <summary>
    /// 处理碰撞逻辑
    /// </summary>
    /// <param name="collisionObject">碰撞物体</param>
    protected virtual void HandleCollision(GameObject collisionObject)
    {
        // 检查碰撞物体是否是泡泡
        if (collisionObject.CompareTag("BubbleFloat") || collisionObject.CompareTag("BubbleCombine"))
        {
            //直接破裂
            collisionObject.GetComponent<BaseBubble>().Attacked();
        }

        if (collisionObject.CompareTag("Player"))
        {
            // 玩家受伤
            collisionObject.GetComponent<Player>().Dead();
        }
    }
}

