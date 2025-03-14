using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主音量控制器脚本
/// </summary>
public class VolumeSlider : MonoBehaviour
{
    #region 组件引用

    private Slider volumeSlider; // Slider组件引用

    #endregion

    #region 生命周期

    private void Awake()
    {
        // 获取Slider组件
        volumeSlider = GetComponent<Slider>();

        // 确保Slider组件存在
        if (volumeSlider == null)
        {
            Debug.LogError("VolumeSlider脚本必须挂载在带有Slider组件的GameObject上");
            return;
        }

        // 初始化Slider的值为当前主音量
        volumeSlider.value = AudioManager.Instance.AllVolume;

        // 添加值改变的监听事件
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnDestroy()
    {
        // 移除监听事件，防止内存泄漏
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        }
    }

    #endregion

    #region 事件处理

    /// <summary>
    /// 当音量滑动条值改变时调用
    /// </summary>
    /// <param name="value">新的音量值 (0-1)</param>
    private void OnVolumeChanged(float value)
    {
        AudioManager.Instance.AudjestVolume(value);
    }

    #endregion
}