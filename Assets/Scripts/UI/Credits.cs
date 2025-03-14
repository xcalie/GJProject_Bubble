using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

/// <summary>
/// Credits组件 - 处理鼠标悬浮事件
/// </summary>
public class Credits : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("需要激活的目标组件")]
    [SerializeField] private GameObject targetObject;  // 改为GameObject以支持多个组件

    [Header("是否在开始时禁用目标组件")]
    [SerializeField] private bool disableOnStart = true;

    [Header("动画设置")]
    [SerializeField] private float hoverScale = 1.1f;        // 悬浮时的放大倍数
    [SerializeField] private float animationDuration = 0.3f; // 动画持续时间
    [SerializeField] private Ease easeType = Ease.OutQuad;   // 动画缓动类型
    [SerializeField] private float panelAnimationDuration = 0.5f; // 面板动画持续时间
    [SerializeField] private Ease panelEaseType = Ease.InOutQuart; // 面板动画缓动类型
    [SerializeField] private float fadeOutDelay = 0.1f;      // 淡出延迟时间

    private Vector3 originalScale;
    private Tween currentTween;
    private Tween panelTween;
    private MonoBehaviour[] allComponents;  // 存储所有需要控制的组件
    private CanvasGroup targetCanvasGroup; // 目标面板的CanvasGroup组件
    private bool isHovering = false;       // 是否正在悬浮

    private void Start()
    {
        // 记录原始缩放值
        originalScale = transform.localScale;

        // 获取目标对象及其子物体上的所有MonoBehaviour组件
        if (targetObject != null)
        {
            allComponents = targetObject.GetComponentsInChildren<MonoBehaviour>(true);

            // 获取或添加CanvasGroup组件
            targetCanvasGroup = targetObject.GetComponent<CanvasGroup>();
            if (targetCanvasGroup == null)
            {
                targetCanvasGroup = targetObject.AddComponent<CanvasGroup>();
            }

            // 如果设置为true，则在开始时禁用所有组件
            if (disableOnStart)
            {
                SetPanelActive(false, false);
            }
        }
    }

    /// <summary>
    /// 设置所有组件的启用状态
    /// </summary>
    private void SetComponentsState(bool state)
    {
        if (allComponents == null) return;

        foreach (var component in allComponents)
        {
            if (component != null && component != this)  // 避免禁用自身
            {
                component.enabled = state;
            }
        }
    }

    /// <summary>
    /// 设置面板激活状态
    /// </summary>
    private void SetPanelActive(bool active, bool animate)
    {
        if (targetObject == null || targetCanvasGroup == null) return;

        // 确保游戏对象是激活的以便播放动画
        targetObject.SetActive(true);

        // 取消当前正在进行的面板动画
        panelTween?.Kill();

        if (animate)
        {
            // 设置交互状态（仅在显示时立即启用）
            if (active)
            {
                targetCanvasGroup.interactable = true;
                targetCanvasGroup.blocksRaycasts = true;
                SetComponentsState(true);
            }

            // 使用DOTween执行透明度动画
            var sequence = DOTween.Sequence();

            if (!active)
            {
                // 添加延迟，使淡出更自然
                sequence.AppendInterval(fadeOutDelay);
            }

            sequence.Append(targetCanvasGroup.DOFade(active ? 1f : 0f, panelAnimationDuration)
                .SetEase(panelEaseType))
                .OnComplete(() =>
                {
                    if (!active && !isHovering) // 确保鼠标真的离开了才禁用
                    {
                        targetCanvasGroup.interactable = false;
                        targetCanvasGroup.blocksRaycasts = false;
                        SetComponentsState(false);
                        targetObject.SetActive(false);
                    }
                });

            panelTween = sequence;
        }
        else
        {
            // 直接设置状态，不播放动画
            targetCanvasGroup.alpha = active ? 1f : 0f;
            targetCanvasGroup.interactable = active;
            targetCanvasGroup.blocksRaycasts = active;
            SetComponentsState(active);
            if (!active) targetObject.SetActive(false);
        }
    }

    /// <summary>
    /// 当鼠标进入UI元素时调用
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        // 激活所有目标组件并播放动画
        SetPanelActive(true, true);

        // 取消当前正在进行的动画
        currentTween?.Kill();

        // 播放放大动画
        currentTween = transform.DOScale(originalScale * hoverScale, animationDuration)
            .SetEase(easeType);
    }

    /// <summary>
    /// 当鼠标离开UI元素时调用
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        // 禁用所有目标组件并播放动画
        SetPanelActive(false, true);

        // 取消当前正在进行的动画
        currentTween?.Kill();

        // 播放恢复原始大小的动画
        currentTween = transform.DOScale(originalScale, animationDuration)
            .SetEase(easeType);
    }

    private void OnDestroy()
    {
        // 清理动画
        currentTween?.Kill();
        panelTween?.Kill();
    }
}
