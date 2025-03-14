using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 关卡选择系统管理器
/// </summary>
public class LevelSelection : MonoBehaviour
{
    [Header("关卡按钮设置")]
    [Tooltip("关卡按钮预制体")]
    public Button levelButtonPrefab;

    [Tooltip("关卡按钮的父物体Transform")]
    public Transform buttonContainer;

    [Header("关卡配置")]
    [Tooltip("关卡场景名称列表")]
    public List<string> levelSceneNames = new();

    [Tooltip("已解锁关卡的PlayerPrefs键名")]
    private const string UNLOCKED_LEVEL_KEY = "UnlockedLevel";

    [Header("UI设置")]
    [Tooltip("锁定状态的图片")]
    public Sprite lockedSprite;

    [Tooltip("解锁状态的图片")]
    public Sprite unlockedSprite;

    private void Start()
    {
        InitializeLevelButtons();
    }

    /// <summary>
    /// 初始化所有关卡按钮
    /// </summary>
    private void InitializeLevelButtons()
    {
        // 获取当前已解锁的最高关卡
        int unlockedLevel = PlayerPrefs.GetInt(UNLOCKED_LEVEL_KEY, 1);

        // 为每个关卡创建按钮
        for (int i = 0; i < levelSceneNames.Count; i++)
        {
            int levelIndex = i; // 创建副本用于闭包

            // 实例化按钮
            Button levelButton = Instantiate(levelButtonPrefab, buttonContainer);

            // 设置按钮文本
            Text buttonText = levelButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = $"Level {levelIndex + 1}";
            }

            // 设置按钮图片
            Image buttonImage = levelButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = levelIndex + 1 <= unlockedLevel ? unlockedSprite : lockedSprite;
            }

            // 设置按钮交互性
            levelButton.interactable = levelIndex + 1 <= unlockedLevel;

            // 添加点击事件
            levelButton.onClick.AddListener(() => LoadLevel(levelIndex));
        }
    }

    /// <summary>
    /// 加载指定关卡
    /// </summary>
    /// <param name="levelIndex">关卡索引</param>
    private void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelSceneNames.Count)
        {
            SceneManager.LoadScene(levelSceneNames[levelIndex]);
        }
        else
        {
            Debug.LogError($"关卡索引 {levelIndex} 超出范围!");
        }
    }

    /// <summary>
    /// 解锁下一关卡
    /// </summary>
    public void UnlockNextLevel()
    {
        int currentUnlockedLevel = PlayerPrefs.GetInt(UNLOCKED_LEVEL_KEY, 1);
        PlayerPrefs.SetInt(UNLOCKED_LEVEL_KEY, currentUnlockedLevel + 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 重置所有关卡解锁状态
    /// </summary>
    public void ResetLevelProgress()
    {
        PlayerPrefs.SetInt(UNLOCKED_LEVEL_KEY, 1);
        PlayerPrefs.Save();
    }
}
