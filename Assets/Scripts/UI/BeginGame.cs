using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using Unity.VisualScripting;

public class ImageButtonSceneChange : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("场景设置")]
    [SerializeField]
    private string nextSceneName; // 下一个场景的名称

    [Header("按钮设置")]
    [SerializeField]
    private Image buttonImage; // 图片按钮引用
    [SerializeField]
    private float clickAnimationScale = 0.9f; // 点击时的缩放比例
    [SerializeField]
    private float hoverAnimationScale = 1.1f; // 悬停时的缩放比例
    [SerializeField]
    private float animationDuration = 0.1f; // 动画持续时间

    [Header("转场效果设置")]
    [SerializeField]
    private float transitionDuration = 2.0f; // 转场动画持续时间
    [SerializeField]
    private float letterboxHeight = 0.12f; // Letterbox高度（屏幕高度的比例）
    [SerializeField]
    private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 转场动画曲线
    [SerializeField]
    private float cameraZoomFactor = 1.2f; // 相机缩放系数

    [Header("音效设置")]
    [SerializeField]
    private AudioClip transitionSound; // 转场音效
    [SerializeField]
    private AudioClip buttonClickSound; // 按钮点击音效
    [SerializeField]
    private float soundVolume = 1f; // 音效音量

    private Vector3 originalScale; // 记录原始大小
    private bool isAnimating = false; // 是否正在播放动画
    private Canvas mainCanvas; // 主Canvas引用
    private Image fadePanel; // 淡入淡出面板
    private Image topLetterbox; // 上方黑边
    private Image bottomLetterbox; // 下方黑边
    private Camera mainCamera; // 主相机引用
    private AudioSource audioSource; // 音频源组件

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter");
        if (other.CompareTag("Player"))
        {
            ChangeScene();
        }
    }
    void Start()
    {
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
        }

        originalScale = transform.localScale;
        mainCamera = Camera.main;

        // 设置音频源
        SetupAudioSource();

        // 创建转场效果所需的UI元素
        SetupTransitionElements();
    }

    // 设置音频源
    AudioSource asForBegin;
    private void SetupAudioSource()
    {
        /*
        // 创建音频源
        GameObject audioObj = new GameObject("TransitionAudio");
        audioObj.transform.SetParent(transform);
        audioSource = audioObj.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = soundVolume;
        */
    }

    // 设置转场效果所需的UI元素
    private void SetupTransitionElements()
    {
        // 创建一个新的Canvas作为转场效果的容器
        GameObject canvasObj = new GameObject("TransitionCanvas");
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 999; // 确保显示在最上层
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 创建淡入淡出面板
        GameObject fadePanelObj = new GameObject("FadePanel");
        fadePanelObj.transform.SetParent(mainCanvas.transform, false);
        fadePanel = fadePanelObj.AddComponent<Image>();
        fadePanel.color = new Color(0, 0, 0, 0);
        fadePanel.rectTransform.anchorMin = Vector2.zero;
        fadePanel.rectTransform.anchorMax = Vector2.one;
        fadePanel.rectTransform.sizeDelta = Vector2.zero;

        // 创建上方letterbox
        GameObject topLetterboxObj = new GameObject("TopLetterbox");
        topLetterboxObj.transform.SetParent(mainCanvas.transform, false);
        topLetterbox = topLetterboxObj.AddComponent<Image>();
        topLetterbox.color = Color.black;
        topLetterbox.rectTransform.anchorMin = new Vector2(0, 1);
        topLetterbox.rectTransform.anchorMax = new Vector2(1, 1);
        topLetterbox.rectTransform.sizeDelta = new Vector2(0, 0);
        topLetterbox.rectTransform.anchoredPosition = new Vector2(0, 0);

        // 创建下方letterbox
        GameObject bottomLetterboxObj = new GameObject("BottomLetterbox");
        bottomLetterboxObj.transform.SetParent(mainCanvas.transform, false);
        bottomLetterbox = bottomLetterboxObj.AddComponent<Image>();
        bottomLetterbox.color = Color.black;
        bottomLetterbox.rectTransform.anchorMin = new Vector2(0, 0);
        bottomLetterbox.rectTransform.anchorMax = new Vector2(1, 0);
        bottomLetterbox.rectTransform.sizeDelta = new Vector2(0, 0);
        bottomLetterbox.rectTransform.anchoredPosition = new Vector2(0, 0);

        // 初始时隐藏所有转场元素
        mainCanvas.gameObject.SetActive(false);
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
        //清理对象池
        PoolManager.Instance.ClearPool();

        // 确保时间恢复正常运行
        Time.timeScale = 1f;

        if (buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound, soundVolume);
        }
        PlayClickAnimation();
        Invoke("ChangeScene", 0.2f);
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
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / animationDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScaleVector, t);
            yield return null;
        }

        transform.localScale = targetScaleVector;
        isAnimating = false;
    }

    // 切换场景
    private void ChangeScene()
    {
        StartCoroutine(PlayTransitionAndLoadScene());
    }

    // 播放转场动画并加载场景
    private IEnumerator PlayTransitionAndLoadScene()
    {
        mainCanvas.gameObject.SetActive(true);
        float screenHeight = Screen.height;
        float letterboxPixelHeight = screenHeight * letterboxHeight;

        if (transitionSound != null)
        {
            audioSource.PlayOneShot(transitionSound, soundVolume);
        }

        // 第一阶段：Letterbox出现和初始淡入
        float elapsedTime = 0f;
        Vector3 originalCameraPos = mainCamera.transform.position;
        float originalOrthoSize = mainCamera.orthographicSize;

        while (elapsedTime < transitionDuration * 0.4f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / (transitionDuration * 0.4f);
            float curvedT = transitionCurve.Evaluate(t);

            // Letterbox动画
            float currentHeight = Mathf.Lerp(0, letterboxPixelHeight, curvedT);
            topLetterbox.rectTransform.sizeDelta = new Vector2(0, currentHeight);
            bottomLetterbox.rectTransform.sizeDelta = new Vector2(0, currentHeight);

            // 相机缩放效果
            if (mainCamera.orthographic)
            {
                mainCamera.orthographicSize = Mathf.Lerp(originalOrthoSize, originalOrthoSize / cameraZoomFactor, curvedT);
            }
            else
            {
                mainCamera.fieldOfView = Mathf.Lerp(60f, 60f / cameraZoomFactor, curvedT);
            }

            // 淡入效果
            fadePanel.color = new Color(0, 0, 0, Mathf.Lerp(0, 0.3f, curvedT));

            yield return null;
        }

        // 开始加载新场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoad.allowSceneActivation = false;

        // 等待场景加载完成
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // 第二阶段：完全淡入
        elapsedTime = 0f;
        while (elapsedTime < transitionDuration * 0.6f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / (transitionDuration * 0.6f);
            float curvedT = transitionCurve.Evaluate(t);

            // 继续缩放相机
            if (mainCamera.orthographic)
            {
                mainCamera.orthographicSize = Mathf.Lerp(originalOrthoSize / cameraZoomFactor, originalOrthoSize / (cameraZoomFactor * 1.2f), curvedT);
            }
            else
            {
                mainCamera.fieldOfView = Mathf.Lerp(60f / cameraZoomFactor, 60f / (cameraZoomFactor * 1.2f), curvedT);
            }

            // 加深淡入效果
            fadePanel.color = new Color(0, 0, 0, Mathf.Lerp(0.3f, 1f, curvedT));

            yield return null;
        }

        // 激活新场景
        asyncLoad.allowSceneActivation = true;
    }
}
