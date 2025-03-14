using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 场景管理器：负责管理游戏场景的状态和流程
/// </summary>
public class SceneManagers : MonoBehaviour
{
    [Header("对话系统")]
    [SerializeField] private SpeakSystem speakSystem; // 对话系统引用

    [Header("对话组设置")]
    [SerializeField] private List<DialogueGroup> dialogueGroups; // 对话组列表
    [SerializeField] private float dialogueDelay = 0.5f; // 对话延迟时间

    private Dictionary<string, List<DialogueData>> dialogueDict; // 存储不同ID的对话组
    private string currentDialogueId; // 当前正在播放的对话组ID
    private int currentDialogueIndex = 0; // 当前对话索引
    
    private void Awake()
    {
        InitializeDialogueDictionary();
    }

    private void Start()
    {
        // 确保对话系统已经被正确引用
        if (speakSystem == null)
        {
            speakSystem = FindObjectOfType<SpeakSystem>();
            if (speakSystem == null)
            {
                Debug.LogError("未找到对话系统！请确保场景中存在SpeakSystem组件。");
                return;
            }
        }

        // 注册对话完成事件
        speakSystem.onDialogueComplete.AddListener(OnDialogueComplete);
    }

    /// <summary>
    /// 初始化对话字典
    /// </summary>
    private void InitializeDialogueDictionary()
    {
        dialogueDict = new Dictionary<string, List<DialogueData>>();
        foreach (var group in dialogueGroups)
        {
            if (!string.IsNullOrEmpty(group.groupId))
            {
                dialogueDict[group.groupId] = group.dialogues;
            }
        }
    }

    /// <summary>
    /// 触发指定ID的对话序列
    /// </summary>
    /// <param name="dialogueId">对话组ID</param>
    public void TriggerDialogue(string dialogueId)
    {
        if (dialogueDict.ContainsKey(dialogueId))
        {
            currentDialogueId = dialogueId;
            currentDialogueIndex = 0;
            Invoke(nameof(StartDialogueSequence), dialogueDelay);
        }
        else
        {
            Debug.LogWarning($"未找到ID为 {dialogueId} 的对话组");
        }
    }

    /// <summary>
    /// 开始播放对话序列
    /// </summary>
    private void StartDialogueSequence()
    {
        if (dialogueDict.ContainsKey(currentDialogueId) && dialogueDict[currentDialogueId].Count > 0)
        {
            PlayNextDialogue();
        }
    }

    /// <summary>
    /// 播放下一段对话
    /// </summary>
    private void PlayNextDialogue()
    {
        var currentDialogues = dialogueDict[currentDialogueId];
        if (currentDialogueIndex < currentDialogues.Count)
        {
            var dialogue = currentDialogues[currentDialogueIndex];
            speakSystem.StartDialogue(dialogue.message, dialogue.speakerSprite);
        }
    }

    /// <summary>
    /// 对话完成回调
    /// </summary>
    private void OnDialogueComplete()
    {
        currentDialogueIndex++;
        var currentDialogues = dialogueDict[currentDialogueId];
        if (currentDialogueIndex < currentDialogues.Count)
        {
            PlayNextDialogue();
        }
        else
        {
            OnDialogueGroupComplete(currentDialogueId);
        }
    }

    /// <summary>
    /// 对话组完成回调
    /// </summary>
    /// <param name="completedGroupId">完成的对话组ID</param>
    private void OnDialogueGroupComplete(string completedGroupId)
    {
        // TODO: 可以在这里添加对话组完成后的特定逻辑
        Debug.Log($"对话组 {completedGroupId} 已播放完毕");
    }
}

/// <summary>
/// 对话组数据结构
/// </summary>
[System.Serializable]
public class DialogueGroup
{
    public string groupId; // 对话组ID
    public string groupDescription; // 对话组描述
    public List<DialogueData> dialogues; // 对话列表
}

/// <summary>
/// 对话数据结构
/// </summary>
[System.Serializable]
public class DialogueData
{
    [TextArea(3, 10)]
    public string message; // 对话内容
    public Sprite speakerSprite; // 说话者头像
}
