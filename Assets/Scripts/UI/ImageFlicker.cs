using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 控制UI图片闪烁效果的组件
/// </summary>
public class ImageFlicker : MonoBehaviour
{
    [Header("闪烁设置")]
    [Tooltip("闪烁的总次数")]
    [SerializeField] private int flickerCount = 3;

    [Tooltip("每次闪烁的时间间隔（秒）")]
    [SerializeField] private float flickerInterval = 0.2f;

    [Tooltip("闪烁时的最小透明度")]
    [Range(0f, 1f)]
    [SerializeField] private float minAlpha = 0.0f;

    [Tooltip("闪烁时的最大透明度")]
    [Range(0f, 1f)]
    [SerializeField] private float maxAlpha = 1.0f;

    // 图片组件引用
    private Image imageComponent;
    // 当前是否正在闪烁
    private bool isFlickering = false;

    private void Awake()
    {
        // 获取Image组件
        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogError("ImageFlicker脚本需要附加在带有Image组件的GameObject上");
            enabled = false;
            return;
        }
    }

    /// <summary>
    /// 开始闪烁效果
    /// </summary>
    public void StartFlicker()
    {
        if (!isFlickering)
        {
            StartCoroutine(FlickerCoroutine());
        }
    }

    /// <summary>
    /// 闪烁效果的协程
    /// </summary>
    private IEnumerator FlickerCoroutine()
    {
        isFlickering = true;
        Color originalColor = imageComponent.color;

        for (int i = 0; i < flickerCount; i++)
        {
            // 淡出
            imageComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, minAlpha);
            yield return new WaitForSeconds(flickerInterval * 0.5f);

            // 淡入
            imageComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, maxAlpha);
            yield return new WaitForSeconds(flickerInterval * 0.5f);
        }

        // 确保最后显示状态是完全显示
        imageComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, maxAlpha);
        isFlickering = false;
    }
}
