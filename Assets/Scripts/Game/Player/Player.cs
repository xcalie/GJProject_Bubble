using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    #region 属性

    public Animator anim;

    // 添加死亡事件
    [Header("事件")]
    public UnityEvent onPlayerDeath = new UnityEvent();

    // 气泡相关属性
    //public int CombineBubbleNum = 0;
    //public List<BubbleCombine> BubbleList = new List<BubbleCombine>();

    public GameObject Drone;//子对象无人机方向

    // 移动相关属性
    [Header("移动设置")]
    [Tooltip("移动速度")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float OriginalMoveSpeed;
    /*
    [Tooltip("加速度")]
    [SerializeField] private float acceleration = 50f;

    [Tooltip("减速度")]
    [SerializeField] private float deceleration = 50f;
    */

    [Tooltip("加速持续时间")]
    public float accelerateTime = 5f;

    [Tooltip("加速倍率")]
    public float Multiplier = 1f;


    // 组件引用
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 currentVelocity;

    public Vector2 Dirt => rb.velocity.normalized;

    //是否被黄色泡泡包裹(无敌)
    public bool isContainByYellow = false;

    #endregion 

    #region 生命周期函数

    private void Awake()
    {
        // 获取刚体组件
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // 关闭重力
            rb.drag = 0.5f; // 添加一些阻力
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 锁定旋转
        }

        if (Drone == null)
        {
            Drone = this.transform.GetChild(0).gameObject;
        }

        PlayAudioOnStart();
    }

    private void Start()
    {
        OriginalMoveSpeed = moveSpeed;
        anim = this.GetComponent<Animator>();
    }

    private void Update()
    {
        // 获取输入
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // 标准化输入向量，确保斜向移动速度不会更快
        if (moveInput.magnitude > 1f)
        {
            moveInput.Normalize();
        }

        // 无人机方向
        JusticeDirtOfDrone(moveInput.x, moveInput.y);
    }

    private void FixedUpdate()
    {
        if (enabled)
        {
            Move();
        }
    }

    #endregion

    #region 私有方法

    AudioSource asForStart;
    private void PlayAudioOnStart()
    {
        AudioManager.Instance.addAfterLoad += () =>
        {
            asForStart = AudioManager.Instance.AddChildWithAudioSource(gameObject, AudioType.Effect, 1, 0.5f);
            asForStart.loop = true;
            //asForStart.volume = 0.8f;
            asForStart.Play();
            Invoke(nameof(StopAudioOnStart), 2.5f);
        };
    }

    public void StopAudioOnStart()
    {
        StartCoroutine(StopAudioOnStartCoroutine());
    }

    private IEnumerator StopAudioOnStartCoroutine()
    {
        AudioManager.Instance.VolumeOfObjs[AudioManager.Instance.ObjsOfAudioSource[this.gameObject]][asForStart] = 0.5f;
        while (asForStart.volume > 0.8f)
        {
            asForStart.volume += Time.deltaTime * 0.1f;
            yield return null;
        }
        AudioManager.Instance.VolumeOfObjs[AudioManager.Instance.ObjsOfAudioSource[this.gameObject]][asForStart] = 0.8f;
        while (asForStart.volume > 0.18f)
        {
            asForStart.volume -= Time.deltaTime * 0.1f;
            yield return null;
        }
        AudioManager.Instance.VolumeOfObjs[AudioManager.Instance.ObjsOfAudioSource[this.gameObject]][asForStart] = 0.18f;
    }

    private void Move()
    {
        this.transform.Translate(moveInput * moveSpeed * Time.fixedDeltaTime);
    }


    public void JusticeDirtOfDrone(float horizontal, float vertical)
    {
        float angle = Mathf.Atan2(horizontal, -vertical) * Mathf.Rad2Deg;
        Quaternion quaternion = Quaternion.Euler(new Vector3(0, 0, angle));

        //插值运算
        Drone.transform.rotation = Quaternion.Slerp(Drone.transform.rotation, quaternion, Time.deltaTime * 6);
    }

    /*
    public void AddBubble(BubbleCombine bubble)
    {
        BubbleList.Add(bubble);
        CombineBubbleNum++;
    }
    */

    public void Accelerate()
    {
        moveSpeed = OriginalMoveSpeed * (1 + Multiplier);
        Invoke(nameof(DecAccelerate), accelerateTime);
    }

    public void DecAccelerate()
    {
        moveSpeed = OriginalMoveSpeed;
    }

    public void Dead()
    {
        if (isContainByYellow)
        {
            return;
        }
        anim.SetBool("isDead", true);

        AudioManager.Instance.addAfterLoad += () =>
        {
            AudioManager.Instance.AddChildWithAudioSource(gameObject, AudioType.Effect, 6).Play();
        };

        // 禁用玩家输入和移动
        enabled = false;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        float time = anim.GetCurrentAnimatorStateInfo(1).length;
        Invoke(nameof(DeadInvoke), time);
    }

    private void DeadInvoke()
    {
        // 触发死亡事件
        onPlayerDeath?.Invoke();
    }

    #endregion

}
