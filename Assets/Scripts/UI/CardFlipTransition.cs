using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景卡片翻转过渡效果控制器
/// </summary>
public class CardFlipTransition : MonoBehaviour
{
    [Header("过渡设置")]
    [Tooltip("翻转动画持续时间(秒)")]
    [SerializeField] private float flipDuration = 1.0f;

    [Tooltip("过渡用的Canvas")]
    [SerializeField] private Canvas transitionCanvas;

    [Tooltip("场景截图显示的Image组件")]
    [SerializeField] private Image sceneImage;

    private bool isTransitioning = false;
    private Camera mainCamera;

    private void Awake()
    {
        // 确保组件引用正确
        if (!transitionCanvas) transitionCanvas = GetComponentInChildren<Canvas>();
        if (!sceneImage) sceneImage = GetComponentInChildren<Image>();
        mainCamera = Camera.main;

        // 初始状态下隐藏过渡Canvas
        transitionCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// 开始场景切换
    /// </summary>
    /// <param name="targetSceneName">目标场景名称</param>
    public void StartSceneTransition(string targetSceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(FlipTransitionCoroutine(targetSceneName));
        }
    }

    /// <summary>
    /// 卡片翻转过渡协程
    /// </summary>
    private IEnumerator FlipTransitionCoroutine(string targetSceneName)
    {
        isTransitioning = true;

        // 创建当前场景的截图
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        mainCamera.targetTexture = rt;
        mainCamera.Render();
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();
        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // 设置截图到Image
        Sprite sprite = Sprite.Create(screenShot, new Rect(0, 0, screenShot.width, screenShot.height), new Vector2(0.5f, 0.5f));
        sceneImage.sprite = sprite;
        transitionCanvas.gameObject.SetActive(true);

        // 执行翻转动画
        float elapsed = 0;
        RectTransform rectTransform = sceneImage.rectTransform;
        Vector3 originalRotation = rectTransform.localEulerAngles;

        // 前半段动画：翻转到90度
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (flipDuration / 2);
            float currentAngle = Mathf.Lerp(0, 90, progress);
            rectTransform.localEulerAngles = new Vector3(0, currentAngle, 0);
            yield return null;
        }

        // 在90度时加载新场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // 后半段动画：从90度翻转到180度
        elapsed = 0;
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (flipDuration / 2);
            float currentAngle = Mathf.Lerp(90, 180, progress);
            rectTransform.localEulerAngles = new Vector3(0, currentAngle, 0);
            yield return null;
        }

        // 完成场景加载
        asyncLoad.allowSceneActivation = true;

        // 清理资源
        Destroy(screenShot);
        isTransitioning = false;
    }
}