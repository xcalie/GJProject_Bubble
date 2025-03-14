using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleFloat : BaseBubble
{
    #region 基础属性

    private float _speed;//游走速度
    public float _moveRangeX = 5f; // X轴移动范围
    public float _moveRangeY = 3f; // Y轴移动范围
    public Vector2 _initialPosition; // 初始位置
    public bool _showDebugArea = true; // 是否显示调试区域

    [Header("正弦运动参数")]
    public float _horizontalSpeed = 2f; // 水平移动速度
    public float _verticalFrequency = 2f; // 垂直振动频率
    public float _verticalPhase = 0f; // 垂直振动相位
    public float _time; // 累计时间

    protected bool isMove = true;
    protected bool isCD = false;

    #region Getters & Setters

    public float Speed
    {
        get => _speed;
        set
        {
            if (value < ContentBubble.BubbleSpeedLeast)
            {
                _speed = ContentBubble.BubbleSpeedLeast;
            }
            else if (value > ContentBubble.BubbleSpeedMost)
            {
                _speed = ContentBubble.BubbleSpeedMost;
            }
            else
            {
                _speed = value;
            }
        }
    }

    /// <summary>
    /// X轴移动范围
    /// </summary>
    public float MoveRangeX
    {
        get => _moveRangeX;
        set => _moveRangeX = Mathf.Max(0.1f, value);
    }

    /// <summary>
    /// Y轴移动范围
    /// </summary>
    public float MoveRangeY
    {
        get => _moveRangeY;
        set => _moveRangeY = Mathf.Max(0.1f, value);
    }

    /// <summary>
    /// 水平移动速度
    /// </summary>
    public float HorizontalSpeed
    {
        get => _horizontalSpeed;
        set => _horizontalSpeed = Mathf.Max(0.1f, value);
    }

    /// <summary>
    /// 垂直振动频率
    /// </summary>
    public float VerticalFrequency
    {
        get => _verticalFrequency;
        set => _verticalFrequency = Mathf.Max(0.1f, value);
    }

    /// <summary>
    /// 垂直振动相位
    /// </summary>
    public float VerticalPhase
    {
        get => _verticalPhase;
        set => _verticalPhase = value;
    }

    /// <summary>
    /// 是否显示调试区域
    /// </summary>
    public bool ShowDebugArea
    {
        get => _showDebugArea;
        set => _showDebugArea = value;
    }

    #endregion

    #endregion

    #region 生命周期

    protected override void Awake()
    {
        base.Awake();
        // 记录初始位置
        _initialPosition = transform.position;
        // 设置初始速度
        Speed = Random.Range(ContentBubble.BubbleSpeedLeast, ContentBubble.BubbleSpeedMost);
        // 随机初始相位，使得多个气泡的运动不同步
        _verticalPhase = Random.Range(0f, 2f * Mathf.PI);
        // 随机初始时间，使得多个气泡的起始位置不同
        _time = Random.Range(0f, 100f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isMove = true;
        isCD = false;
        type = BubbleType.None;
    }

    protected override void Update()
    {
        base.Update();
        // 更新时间
        _time += Time.deltaTime;
        // 应用正弦运动
        ApplySineMovement();
    }


    #endregion

    #region 私有方法

    #region 碰撞器

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCD) return;
        if (collision.CompareTag("Player") || collision.CompareTag("BubbleCombine"))
        {
            //遇到黄色气泡透过 红色气泡销毁
            if (collision.CompareTag("BubbleCombine"))
            {
                if (collision.GetComponent<BubbleCombine>().type == BubbleType.Yellow && this.type == BubbleType.Red)
                {
                    this.Attacked();
                    return;
                }
                if (collision.GetComponent<BubbleCombine>().type == BubbleType.Yellow)
                {
                    return;
                }
            }

            GameObject combine = PoolManager.Instance.GetObj(CheckColor.CheckCombineType(this.type));
            BubbleCombine bubble = combine.GetComponent<BubbleCombine>();
            bubble.StickOnPlayer(PlayerManager.Instance.Player1, this.transform.position);

            //PlayerManager.Instance.Player1.GetComponent<Player>().BubbleList.Add(bubble);

            isCD = true;

            PoolManager.Instance.PushObj(this.gameObject);

            /*
            try
            {
                PoolManager.Instance.PushObj(this.gameObject);
            }
            catch (KeyNotFoundException)
            {
                GameObject obj = PoolManager.Instance.GetObj(CheckColor.CheckFloatType(this.type));
                Destroy(this.gameObject);
                PoolManager.Instance.PushObj(obj);
            }
            */
        }
    }

    #endregion

    #region 移动

    /// <summary>
    /// 在Scene视图中绘制移动范围
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!_showDebugArea) return;

        // 设置绘制颜色为半透明青色
        Gizmos.color = new Color(0, 1, 1, 0.3f);

        // 计算矩形的中心点和大小
        Vector3 center = Application.isPlaying ? _initialPosition : transform.position;
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

    /// <summary>
    /// 应用正弦运动
    /// </summary>
    private void ApplySineMovement()
    {
        if (!isMove) return;

        // 计算水平位置（使用PingPong使其在范围内来回移动）
        float x = Mathf.PingPong(_time * _horizontalSpeed, _moveRangeX * 2) - _moveRangeX;

        // 计算垂直位置（使用正弦函数）
        float y = Mathf.Sin((_time * _verticalFrequency + _verticalPhase) * Mathf.PI) * _moveRangeY;

        // 应用新位置
        Vector2 newPosition = _initialPosition + new Vector2(x, y);
        transform.position = newPosition;
    }

    #endregion

    #region 破裂

    public override void Attacked()
    {
        isMove = false;
        base.Attacked();
    }

    #endregion

    #endregion
}
