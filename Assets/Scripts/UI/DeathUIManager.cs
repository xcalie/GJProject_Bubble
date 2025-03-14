using UnityEngine;

/// <summary>
/// 死亡UI管理器：负责处理死亡界面的显示和隐藏
/// </summary>
public class DeathUIManager : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private GameObject deathUIPanel; // 死亡界面面板

    private void Start()
    {
        // 获取场景中的玩家引用
        var player = FindObjectOfType<Player>();
        if (player != null)
        {
            // 订阅玩家死亡事件
            player.onPlayerDeath.AddListener(OnPlayerDeath);
        }
        else
        {
            Debug.LogWarning("场景中未找到Player组件！");
        }

        // 确保初始状态下死亡界面是隐藏的
        if (deathUIPanel != null)
        {
            deathUIPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 处理玩家死亡事件
    /// </summary>
    private void OnPlayerDeath()
    {
        ShowDeathUI();

    }

    /// <summary>
    /// 显示死亡界面
    /// </summary>
    private void ShowDeathUI()
    {
        if (deathUIPanel != null)
        {
            deathUIPanel.SetActive(true);
            // 激活所有子对象
            foreach (Transform child in deathUIPanel.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("死亡界面未赋值！");
        }
    }

    /// <summary>
    /// 隐藏死亡界面
    /// </summary>
    public void HideDeathUI()
    {
        if (deathUIPanel != null)
        {
            deathUIPanel.SetActive(false);
        }
    }
}