using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Setting : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("按钮设置")]
    [SerializeField]
    private Image buttonImage; // 按钮图片引用

    [SerializeField]
    private Button closeButton; // 关闭按钮引用

    [SerializeField]
    private float clickAnimationScale = 0.9f; // 点击时的缩放比例

    [SerializeField]
    private float hoverAnimationScale = 1.1f; // 悬停时的缩放比例

    [SerializeField]
    private float animationDuration = 0.1f; // 动画持续时间

    [Header("面板设置")]
    [SerializeField]
    private GameObject settingPanel; // 设置面板引用

    [SerializeField]
    private float panelAnimationDuration = 0.3f; // 面板动画持续时间

    private Vector3 originalScale; // 记录按钮原始大小
    private bool isAnimating = false; // 是否正在播放动画
    private CanvasGroup panelCanvasGroup; // 面板的CanvasGroup组件

    private void Start()
    {
        // 初始化按钮组件
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
        }

        // 保存按钮原始大小
        originalScale = transform.localScale;

        // 初始化面板组件
        if (settingPanel != null)
        {
            panelCanvasGroup = settingPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
            {
                panelCanvasGroup = settingPanel.AddComponent<CanvasGroup>();
            }
            // 初始化面板为隐藏状态
            SetPanelActive(false, false);
        }

        // 添加关闭按钮的点击事件监听
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() =>
            {
                SetPanelActive(false, true);
            });
        }
    }

    // 实现鼠标进入接口
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            StartCoroutine(ScaleAnimation(hoverAnimationScale));
        }
    }

    // 实现鼠标离开接口
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            StartCoroutine(ScaleAnimation(1f));
        }
    }

    // 实现点击接口
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayClickAnimation();
        TogglePanel();
    }

    // 点击动画效果
    private void PlayClickAnimation()
    {
        transform.localScale = originalScale * clickAnimationScale;
        Invoke(nameof(ResetScale), 0.1f);
    }

    // 重置按钮大小
    private void ResetScale()
    {
        transform.localScale = originalScale;
    }

    // 按钮缩放动画协程
    private IEnumerator ScaleAnimation(float targetScale)
    {
        isAnimating = true;
        Vector3 startScale = transform.localScale;
        Vector3 targetScaleVector = originalScale * targetScale;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScaleVector, t);
            yield return null;
        }

        transform.localScale = targetScaleVector;
        isAnimating = false;
    }

    // 切换面板显示状态
    private void TogglePanel()
    {
        if (settingPanel != null)
        {
            bool isActive = settingPanel.activeSelf;
            SetPanelActive(!isActive, true);
        }
    }

    // 设置面板激活状态
    private void SetPanelActive(bool active, bool animate)
    {
        if (settingPanel == null) return;

        settingPanel.SetActive(true); // 确保游戏对象是激活的以便播放动画

        if (animate)
        {
            StartCoroutine(AnimatePanelAlpha(active));
        }
        else
        {
            // 直接设置状态，不播放动画
            panelCanvasGroup.alpha = active ? 1f : 0f;
            panelCanvasGroup.interactable = active;
            panelCanvasGroup.blocksRaycasts = active;
            if (!active) settingPanel.SetActive(false);
        }
    }

    // 面板透明度动画协程
    private IEnumerator AnimatePanelAlpha(bool fadeIn)
    {
        float startAlpha = panelCanvasGroup.alpha;
        float targetAlpha = fadeIn ? 1f : 0f;
        float elapsedTime = 0f;

        panelCanvasGroup.interactable = fadeIn;
        panelCanvasGroup.blocksRaycasts = fadeIn;

        while (elapsedTime < panelAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / panelAnimationDuration;
            panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        panelCanvasGroup.alpha = targetAlpha;
        if (!fadeIn)
        {
            settingPanel.SetActive(false);
        }
    }
}
