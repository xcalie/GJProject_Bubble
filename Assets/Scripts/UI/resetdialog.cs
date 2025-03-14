using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 重置对话状态按钮控制器
/// </summary>
public class resetdialog : MonoBehaviour
{
    [Header("按钮设置")]
    [SerializeField] private Button resetButton; // 重置按钮引用

    private void Start()
    {
        // 如果没有手动赋值按钮，尝试获取当前物体上的按钮组件
        if (resetButton == null)
        {
            resetButton = GetComponent<Button>();
        }

        // 为按钮添加点击事件监听
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetDialogueStatus);
        }
        else
        {
            Debug.LogWarning("重置对话按钮未找到Button组件！");
        }
    }

    /// <summary>
    /// 重置对话状态
    /// </summary>
    private void ResetDialogueStatus()
    {
        // 调用SpeakSystem中的静态方法重置对话状态
        SpeakSystem.ResetDialogueStatus();
        Debug.Log("对话状态已重置");
    }

    private void OnDestroy()
    {
        // 移除按钮点击事件监听，防止内存泄漏
        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(ResetDialogueStatus);
        }
    }
}
