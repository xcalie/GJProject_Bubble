using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnterScene : MonoBehaviour
{
    [Header("入场效果设置")]
    [SerializeField]
    private float transitionDuration = 2.0f; // 入场动画持续时间
    [SerializeField]
    private float letterboxHeight = 0.12f; // Letterbox高度（屏幕高度的比例）
    [SerializeField]
    private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 入场动画曲线
    [SerializeField]
    private float cameraZoomFactor = 1.2f; // 相机缩放系数

    [Header("玩家控制")]
    [SerializeField]
    private Player player; // 玩家引用

    private Canvas mainCanvas; // 主Canvas引用
    private Image fadePanel; // 淡入淡出面板
    private Image topLetterbox; // 上方黑边
    private Image bottomLetterbox; // 下方黑边
    private Camera mainCamera; // 主相机引用
    private bool isAnimating = false; // 是否正在播放动画
    private SceneManagers sceneManager;
    void Start()
    {
        sceneManager = FindObjectOfType<SceneManagers>();
        if(sceneManager != null)
        {
            sceneManager.TriggerDialogue("1");
        }
        mainCamera = Camera.main;
        // 查找场景中的Player
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        SetupTransitionElements();
        StartCoroutine(PlayEntranceAnimation());
    }

    // 设置入场效果所需的UI元素
    private void SetupTransitionElements()
    {
        // 创建一个新的Canvas作为入场效果的容器
        GameObject canvasObj = new GameObject("EntranceCanvas");
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 999; // 确保显示在最上层
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 创建淡入淡出面板
        GameObject fadePanelObj = new GameObject("FadePanel");
        fadePanelObj.transform.SetParent(mainCanvas.transform, false);
        fadePanel = fadePanelObj.AddComponent<Image>();
        fadePanel.color = new Color(0, 0, 0, 1);
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

        // 初始时显示所有入场元素
        mainCanvas.gameObject.SetActive(true);
    }

    // 播放入场动画
    private IEnumerator PlayEntranceAnimation()
    {
        isAnimating = true;
        if (player != null)
        {
            // 禁用玩家的Rigidbody2D组件
            var playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.simulated = false;
            }
            // 禁用Player脚本
            player.enabled = false;
        }

        float screenHeight = Screen.height;
        float letterboxPixelHeight = screenHeight * letterboxHeight;

        float elapsedTime = 0f;
        Vector3 originalCameraPos = mainCamera.transform.position;
        float originalOrthoSize = mainCamera.orthographicSize;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / transitionDuration;
            float curvedT = transitionCurve.Evaluate(t);

            // Letterbox动画
            float currentHeight = Mathf.Lerp(letterboxPixelHeight, 0, curvedT);
            topLetterbox.rectTransform.sizeDelta = new Vector2(0, currentHeight);
            bottomLetterbox.rectTransform.sizeDelta = new Vector2(0, currentHeight);

            // 相机缩放效果
            if (mainCamera.orthographic)
            {
                mainCamera.orthographicSize = Mathf.Lerp(originalOrthoSize / cameraZoomFactor, originalOrthoSize, curvedT);
            }
            else
            {
                mainCamera.fieldOfView = Mathf.Lerp(60f / cameraZoomFactor, 60f, curvedT);
            }

            // 淡出效果
            fadePanel.color = new Color(0, 0, 0, Mathf.Lerp(1f, 0, curvedT));

            yield return null;
        }

        // 重新启用玩家控制
        if (player != null)
        {
            // 启用玩家的Rigidbody2D组件
            var playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.simulated = true;
            }
            // 启用Player脚本
            player.enabled = true;
        }

        isAnimating = false;
        // 隐藏入场元素
        mainCanvas.gameObject.SetActive(false);
    }
}
