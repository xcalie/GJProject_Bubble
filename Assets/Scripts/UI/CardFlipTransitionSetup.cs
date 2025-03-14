using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 卡片翻转过渡效果UI组件自动设置脚本
/// </summary>
[RequireComponent(typeof(CardFlipTransition))]
public class CardFlipTransitionSetup : MonoBehaviour
{
    private void Awake()
    {
        SetupTransitionUI();
    }

    /// <summary>
    /// 设置过渡UI组件
    /// </summary>
    private void SetupTransitionUI()
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("TransitionCanvas");
        canvasObj.transform.SetParent(transform);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // 确保显示在最上层

        // 添加CanvasScaler组件
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // 添加GraphicRaycaster组件
        canvasObj.AddComponent<GraphicRaycaster>();

        // 创建Image
        GameObject imageObj = new GameObject("TransitionImage");
        imageObj.transform.SetParent(canvasObj.transform);

        Image image = imageObj.AddComponent<Image>();
        image.color = Color.white;

        // 设置Image的RectTransform
        RectTransform rectTransform = image.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // 获取CardFlipTransition组件并设置引用
        CardFlipTransition transition = GetComponent<CardFlipTransition>();
        var field = typeof(CardFlipTransition).GetField("transitionCanvas",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(transition, canvas);

        field = typeof(CardFlipTransition).GetField("sceneImage",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(transition, image);

        // 初始状态下隐藏
        canvasObj.SetActive(false);
    }
}