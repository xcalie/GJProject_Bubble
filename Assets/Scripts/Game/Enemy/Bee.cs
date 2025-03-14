using UnityEngine;
using System.Collections;

/// <summary>
/// 蜜蜂敌人的行为控制脚本
/// </summary>
public class Bee : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("向中心移动的距离")]
    [SerializeField] private float moveDistance = 3f;
    [Tooltip("移动速度")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("被相机检测到之后过了多少秒开始移动")]
    [SerializeField] private float moveAfterViewTime = 0.5f;

    [Header("射击设置")]
    [Tooltip("子弹预制体的资源路径")]
    [SerializeField] private string bulletResourcePath = "Prefabs/Bullets/BeeSpine";
    [Tooltip("子弹速度")]
    [SerializeField] private float bulletSpeed = 8f;
    [Tooltip("射击后等待返回的时间")]
    [SerializeField] private float waitAfterShootTime = 0.5f;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isMovingToTarget = true;
    private bool isInView;
    private Camera mainCamera;
    private SpriteRenderer spr;
    private bool hasShot;

    private Vector2 OldPos;

    private void Start()
    {
        // 初始化
        originalPosition = transform.position;
        mainCamera = Camera.main;
        isMovingToTarget = false;
        isInView = false;
        hasShot = false;
        OldPos = transform.position;
        spr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 检查是否在相机视野内
        CheckIfInView();

        // 处理移动
        HandleMovement();

        // 检查是否需要翻转
        CheckFilp();
    }

    private void CheckFilp()
    {
        if (OldPos.x > transform.position.x)
        {
            spr.flipX = true;
        }
        else
        {
            spr.flipX = false;
        }
        OldPos = transform.position;
    }

    /// <summary>
    /// 检查是否在相机视野内
    /// </summary>
    private void CheckIfInView()
    {
        if (!mainCamera) return;
        
        if (Time.time < moveAfterViewTime) return;

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        bool newIsInView = viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                          viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
                          viewportPoint.z > 0;

        // 当首次进入视野时
        if (newIsInView && !isInView)
        {
            OnEnterView();
        }
        // 当离开视野时
        else if (!newIsInView && isInView)
        {
            OnExitView();
        }

        isInView = newIsInView;
    }

    /// <summary>
    /// 进入视野时的处理
    /// </summary>
    private void OnEnterView()
    {
        // 设置目标位置（向屏幕中心移动）
        targetPosition = transform.position + Vector3.left * moveDistance;
        isMovingToTarget = true;
        hasShot = false;
    }

    /// <summary>
    /// 离开视野时的处理
    /// </summary>
    private void OnExitView()
    {
        // 重置状态
        hasShot = false;
    }

    /// <summary>
    /// 处理移动逻辑
    /// </summary>
    private void HandleMovement()
    {
        if (!isInView) return;

        Vector3 currentTarget = isMovingToTarget ? targetPosition : originalPosition;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);

        // 到达目标位置的处理
        if (Vector3.Distance(transform.position, currentTarget) < 0.1f)
        {
            if (currentTarget == targetPosition && !hasShot)
            {
                // 到达目标位置，射击一次
                Shoot();
                StartCoroutine(WaitAndReturn());
            }
            else if (currentTarget == originalPosition)
            {
                // 返回原位后重置状态
                isMovingToTarget = false;
            }
        }
    }

    /// <summary>
    /// 射击一次
    /// </summary>
    private void Shoot()
    {
        if (!hasShot)
        {
            // 直接使用资源路径从对象池获取子弹
            GameObject bullet = PoolManager.Instance.GetObj(bulletResourcePath);
            if (bullet != null)
            {
                // 设置子弹位置和方向
                bullet.transform.position = transform.position;
                bullet.transform.rotation = Quaternion.identity;

                // 获取或添加子弹组件
                Bullet bulletComponent = bullet.GetComponent<Bullet>();
                if (bulletComponent != null)
                {
                    // 初始化子弹属性
                    bulletComponent.Initialize(bulletSpeed, Vector2.down);
                }
            }
            AudioManager.Instance.addAfterLoad += () =>
            {
                AudioManager.Instance.AddChildWithAudioSource(gameObject, AudioType.Effect, 7).Play();
            };
            hasShot = true;
        }
    }

    /// <summary>
    /// 等待一段时间后返回
    /// </summary>
    private IEnumerator WaitAndReturn()
    {
        yield return new WaitForSeconds(waitAfterShootTime);
        isMovingToTarget = false;
    }

    private void OnDrawGizmosSelected()
    {
        // 在编辑器中显示移动范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * moveDistance);
    }
}
