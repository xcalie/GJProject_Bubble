using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 气泡生成器类
/// 负责在场景中生成和管理浮动气泡
/// </summary>
public class Producer : MonoBehaviour
{
    /// <summary>
    /// 当前管理的所有浮动气泡列表
    /// </summary>
    public List<BubbleFloat> Bubbles = new List<BubbleFloat>();

    [Header("最大生成数量")]
    [Tooltip("场景中同时存在的最大气泡数量")]
    public int NumToProduce = 3;

    /// <summary>
    /// 当前已生成的气泡数量
    /// </summary>
    public int numHasProduced = 0;

    private bool isFirst = true;
    private int firstProduceNum = 0;

    [Header("生成间隔")]
    [Tooltip("每次生成气泡的时间间隔(秒)")]
    public float ProduceInterval = 10f;
    public float timer = 0;

    [Header("移动范围")]
    [SerializeField] private float _moveRangeX = 5f; // X轴移动范围
    [SerializeField] private float _moveRangeY = 3f; // Y轴移动范围
    [SerializeField]private Transform _initialPosition; // 初始位置
    [SerializeField] private bool _showDebugArea = true; // 是否显示调试区域

    [Header("正弦运动参数")]
    [SerializeField] private float _horizontalSpeed = 2f; // 水平移动速度
    [SerializeField] private float _verticalFrequency = 2f; // 垂直振动频率
    [SerializeField] private float _verticalPhase = 0f; // 垂直振动相位

    [Header("随机数范围+-诺干")]
    [SerializeField] private float _randomRangeOfHorizontalSpeed = 0.5f;
    [SerializeField] private float _randomRangeOfVerticalFrequency = 0.5f;
    [SerializeField] private float _randomRangeOfVerticalPhase = 0.5f;



    /// <summary>
    /// 每帧更新检查和生成气泡
    /// </summary>
    private void Update()
    {
        timer += Time.deltaTime;
        CheckChildrenWithBubble();
        ProduceBubble();
    }

    /// <summary>
    /// 检查当前子物体中的气泡数量
    /// 更新已生成的气泡计数
    /// </summary>
    public void CheckChildrenWithBubble()
    {
        BubbleFloat[] children = GetComponentsInChildren<BubbleFloat>();
        numHasProduced = children.Length;
    }

    /// <summary>
    /// 生成新的气泡
    /// 当当前气泡数量小于最大生成数量时,从对象池获取新气泡并设置父物体
    /// </summary>
    public void ProduceBubble()
    {
        if ((timer > ProduceInterval && numHasProduced < NumToProduce) || (isFirst && firstProduceNum < NumToProduce))
        {
            if (isFirst)
            {
                firstProduceNum++;
                if (firstProduceNum >= NumToProduce)
                {
                    isFirst = false;
                }
            }

            // 从对象池获取气泡对象
            GameObject obj = PoolManager.Instance.GetObj(CheckColor.CheckFloatType(BubbleType.None));
            BubbleFloat bubble = obj.GetComponent<BubbleFloat>();
            Bubbles.Add(bubble);
            obj.transform.SetParent(this.transform);

            AudioManager.Instance.addAfterLoad += () =>
            {
                AudioManager.Instance.AddChildWithAudioSource(this.gameObject, AudioType.Effect, 3).Play();
            };

            // 设置气泡的初始位置    
            bubble._initialPosition = this._initialPosition.position;
            // 设置气泡的移动范围
            bubble._moveRangeX = this._moveRangeX;
            bubble._moveRangeY = this._moveRangeY;
            // 随机设置气泡的移动速度
            bubble._horizontalSpeed = this._horizontalSpeed + Random.Range(-_randomRangeOfHorizontalSpeed, _randomRangeOfHorizontalSpeed);
            bubble._verticalFrequency = this._verticalFrequency + Random.Range(-_randomRangeOfVerticalFrequency, _randomRangeOfVerticalFrequency);
            bubble._verticalPhase = this._verticalPhase + Random.Range(-_randomRangeOfVerticalPhase, _randomRangeOfVerticalPhase);

            timer = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (!_showDebugArea) return;

        // 设置绘制颜色为半透明青色
        Gizmos.color = new Color(0, 1, 1, 0.3f);

        // 计算矩形的中心点和大小
        Vector3 center = Application.isPlaying ? _initialPosition.position : transform.position;
        Vector3 size = new Vector3(_moveRangeX * 2, _moveRangeY * 2, 0);

        // 绘制矩形区域
        Gizmos.DrawCube(center, size);

        // 绘制边框
        Gizmos.color = new Color(0, 1, 1, 1f);
        Gizmos.DrawWireCube(center, size);

        // 如果在编辑器中，绘制正弦路径预览
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Vector3 prevPos = Vector3.zero;
            for (float t = 0; t <= 2f; t += 0.1f)
            {
                float x = Mathf.PingPong(t * _horizontalSpeed, _moveRangeX * 2) - _moveRangeX;
                float y = Mathf.Sin((t * _verticalFrequency + _verticalPhase) * Mathf.PI) * _moveRangeY;
                Vector3 pos = center + new Vector3(x, y, 0);

                if (t > 0)
                {
                    Gizmos.DrawLine(prevPos, pos);
                }
                prevPos = pos;
            }
        }
    }
}
