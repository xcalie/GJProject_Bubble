using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 文字闪烁效果控制器
/// </summary>
public class TextFlicker : MonoBehaviour
{
    [Header("闪烁设置")]
    [Tooltip("闪烁间隔时间（秒）")]
    [SerializeField] private float flickerInterval = 1.0f;

    [Tooltip("淡入淡出的过渡时间（秒）")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Tooltip("最小透明度 (0-1)")]
    [SerializeField] private float minAlpha = 0.0f;

    [Tooltip("最大透明度 (0-1)")]
    [SerializeField] private float maxAlpha = 1.0f;

    // 文本组件引用
    private TMP_Text tmpText;
    private Text legacyText;

    // 当前是否正在闪烁
    private bool isFlickering = true;

    private void Awake()
    {
        // 尝试获取TextMeshPro组件
        tmpText = GetComponent<TMP_Text>();

        // 如果没有TextMeshPro组件，则尝试获取传统Text组件
        if (tmpText == null)
        {
            legacyText = GetComponent<Text>();
        }

        // 确保至少有一个文本组件存在
        if (tmpText == null && legacyText == null)
        {
            Debug.LogError("TextFlicker组件需要Text或TextMeshPro组件！");
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(FlickerRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 闪烁效果协程
    /// </summary>
    private IEnumerator FlickerRoutine()
    {
        while (isFlickering)
        {
            // 淡出效果
            yield return StartCoroutine(FadeText(maxAlpha, minAlpha));
            yield return new WaitForSeconds(flickerInterval);

            // 淡入效果
            yield return StartCoroutine(FadeText(minAlpha, maxAlpha));
            yield return new WaitForSeconds(flickerInterval);
        }
    }

    /// <summary>
    /// 文本淡入淡出效果
    /// </summary>
    /// <param name="startAlpha">起始透明度</param>
    /// <param name="endAlpha">目标透明度</param>
    private IEnumerator FadeText(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);

            if (tmpText != null)
            {
                Color newColor = tmpText.color;
                newColor.a = currentAlpha;
                tmpText.color = newColor;
            }
            else if (legacyText != null)
            {
                Color newColor = legacyText.color;
                newColor.a = currentAlpha;
                legacyText.color = newColor;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 停止闪烁效果
    /// </summary>
    public void StopFlicker()
    {
        isFlickering = false;
        StopAllCoroutines();
    }

    /// <summary>
    /// 开始闪烁效果
    /// </summary>
    public void StartFlicker()
    {
        if (!isFlickering)
        {
            isFlickering = true;
            StartCoroutine(FlickerRoutine());
        }
    }
}
