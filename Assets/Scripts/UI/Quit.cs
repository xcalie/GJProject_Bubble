using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class QuitButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image buttonImage; // 图片按钮引用

    [SerializeField]
    private float clickAnimationScale = 0.9f; // 点击时的缩放比例

    [SerializeField]
    private float hoverAnimationScale = 1.1f; // 悬停时的缩放比例

    [SerializeField]
    private float animationDuration = 0.1f; // 动画持续时间

    private Vector3 originalScale; // 记录原始大小
    private bool isAnimating = false; // 是否正在播放动画

    void Start()
    {
        // 如果没有手动赋值，自动获取Image组件
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
        }

        // 保存原始大小
        originalScale = transform.localScale;
    }

    // 实现鼠标进入接口
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            // 播放悬停动画
            StartCoroutine(ScaleAnimation(hoverAnimationScale));
        }
    }

    // 实现鼠标离开接口
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            // 恢复原始大小
            StartCoroutine(ScaleAnimation(1f));
        }
    }

    // 实现接口方法，处理点击事件
    public void OnPointerClick(PointerEventData eventData)
    {
        // 播放点击动画
        PlayClickAnimation();

        // 延迟一小段时间后退出游戏，让动画效果能够播放
        Invoke("QuitGame", 0.2f);
    }

    // 点击动画效果
    private void PlayClickAnimation()
    {
        // 缩小
        transform.localScale = originalScale * clickAnimationScale;

        // 恢复原始大小
        Invoke("ResetScale", 0.1f);
    }

    // 重置缩放
    private void ResetScale()
    {
        transform.localScale = originalScale;
    }

    // 缩放动画协程
    private System.Collections.IEnumerator ScaleAnimation(float targetScale)
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

    // 退出游戏
    private void QuitGame()
    {
#if UNITY_EDITOR
        // 如果在Unity编辑器中运行，停止播放模式
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // 在实际构建的游戏中退出应用
            Application.Quit();
#endif
    }
}
