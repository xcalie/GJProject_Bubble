using UnityEngine;

/// <summary>
/// NPC对话触发器 - 当玩家靠近时触发对话
/// </summary>
public class MeetSpeak : MonoBehaviour
{
    [Header("对话设置")]
    [Tooltip("对话组ID,需要与SceneManagers中的ID对应")]
    [SerializeField] private string dialogueId; // 对话ID
    
    private SceneManagers sceneManager; // 场景管理器引用
    private bool hasTriggeredDialogue = false; // 是否已触发对话
    
    private void Start()
    {
        // 获取场景管理器引用
        sceneManager = FindObjectOfType<SceneManagers>();
        if (sceneManager == null)
        {
            Debug.LogError("未找到SceneManagers组件!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查是否与玩家碰撞且未触发过对话
        if (!hasTriggeredDialogue && collision.CompareTag("Player"))
        {
            // 触发对话
            sceneManager.TriggerDialogue(dialogueId);
            hasTriggeredDialogue = true;
        }
    }

    // TODO: 可以添加重置对话状态的方法，允许重复触发对话
    public void ResetDialogueTrigger()
    {
        hasTriggeredDialogue = false;
    }
}
