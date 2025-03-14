using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// 对话系统管理器 - 增强版动画效果
/// </summary>
public class SpeakSystem : MonoBehaviour
{
    // 静态字典，用于记录对话是否已经播放过
    private static Dictionary<string, bool> playedDialogues = new Dictionary<string, bool>();

    [Header("UI组件")]
    [SerializeField] private Image backgroundImage; // 对话框背景图片
    [SerializeField] private TextMeshProUGUI dialogueText; // 对话文本
    [SerializeField] private Image speakerImage; // 说话者头像

    [Header("对话框设置")]
    [SerializeField] private float backgroundAlpha = 1f; // 背景图片透明度
    [SerializeField] private Vector2 showPosition = new Vector2(-50f, 50f); // 显示时的位置（相对于右下角）
    [SerializeField] private Vector2 hidePosition = new Vector2(-50f, -200f); // 隐藏时的位置（相对于右下角）
    [SerializeField] private float popupScale = 1.1f; // 弹出时的缩放值

    [Header("对话设置")]
    [SerializeField] private float typingSpeed = 0.05f; // 打字速度
    [SerializeField] private float fadeSpeed = 0.5f; // 淡入淡出速度

    [Header("动画设置")]
    [SerializeField] private float panelBounceDuration = 0.5f; // 面板弹出动画持续时间
    [SerializeField] private float speakerImageRotation = 5f; // 头像旋转角度

    [Header("文字动画设置")]
    [SerializeField] private float bounceHeight = 30f; // 文字弹跳高度
    [SerializeField] private float bounceDuration = 0.8f; // 弹跳动画持续时间
    [SerializeField] private float bounceDelay = 0.02f; // 每个字符弹跳的延迟时间
    [SerializeField] private Ease bounceEase = Ease.OutElastic; // 弹跳动画缓动类型

    private bool isDialogueActive = false;
    private bool isTyping = false;
    private string currentMessage;
    private Coroutine typingCoroutine;
    private Vector3 originalSpeakerScale;
    private RectTransform backgroundRect;

    // 对话完成事件
    public UnityEvent onDialogueComplete;

    private void Awake()
    {
        // 初始化UI元素
        if (backgroundImage != null)
        {
            backgroundImage.color = new Color(1, 1, 1, 0f);
            backgroundImage.gameObject.SetActive(false);
            backgroundRect = backgroundImage.GetComponent<RectTransform>();

            // 设置锚点为右下角
            backgroundRect.anchorMin = new Vector2(1, 0);
            backgroundRect.anchorMax = new Vector2(1, 0);
            backgroundRect.pivot = new Vector2(1, 0);

            backgroundRect.anchoredPosition = hidePosition;
        }

        if (speakerImage != null)
        {
            originalSpeakerScale = speakerImage.transform.localScale;
            speakerImage.transform.localScale = Vector3.zero;
        }
    }

    /// <summary>
    /// 开始显示对话
    /// </summary>
    public void StartDialogue(string message, Sprite speakerSprite = null)
    {
        // 检查对话是否已经播放过
        if (playedDialogues.ContainsKey(message) && playedDialogues[message])
        {
            return; // 如果对话已经播放过，直接返回
        }

        // 记录该对话已经播放
        playedDialogues[message] = true;

        currentMessage = message;

        // 显示对话框并添加动画效果
        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(true);

            // 创建动画序列
            Sequence dialogueSequence = DOTween.Sequence();

            // 淡入动画
            dialogueSequence.Join(backgroundImage.DOFade(backgroundAlpha, fadeSpeed));

            // 位置动画 - 从右下角弹出
            dialogueSequence.Join(backgroundRect.DOAnchorPos(showPosition, panelBounceDuration)
                .SetEase(Ease.OutBack));

            // 缩放动画 - 从右下角开始
            dialogueSequence.Join(backgroundRect.DOScale(Vector3.one * popupScale, panelBounceDuration * 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    backgroundRect.DOScale(Vector3.one, panelBounceDuration * 0.5f)
                        .SetEase(Ease.InOutQuad);
                }));
        }

        // 头像动画
        if (speakerImage != null)
        {
            if (speakerSprite != null)
            {
                speakerImage.sprite = speakerSprite;
            }

            // 头像弹出动画
            Sequence avatarSequence = DOTween.Sequence();
            avatarSequence.Append(speakerImage.transform.DOScale(originalSpeakerScale * 1.2f, 0.3f)
                .SetEase(Ease.OutBack))
                .Append(speakerImage.transform.DOScale(originalSpeakerScale, 0.2f)
                .SetEase(Ease.InOutQuad));

            // 添加轻微摇摆动画
            speakerImage.transform.DORotate(new Vector3(0, 0, speakerImageRotation), 1f)
                .SetEase(Ease.InOutQuad)
                .SetLoops(-1, LoopType.Yoyo);
        }

        // 开始打字效果
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(message));

        isDialogueActive = true;
    }

    /// <summary>
    /// 结束对话
    /// </summary>
    public void EndDialogue()
    {
        if (!isDialogueActive) return;

        // 创建结束动画序列
        Sequence endSequence = DOTween.Sequence();

        // 对话框收回动画 - 向右下角收回
        if (backgroundRect != null)
        {
            endSequence.Join(backgroundRect.DOAnchorPos(hidePosition, panelBounceDuration)
                .SetEase(Ease.InBack));
            endSequence.Join(backgroundRect.DOScale(Vector3.one * 0.8f, panelBounceDuration)
                .SetEase(Ease.InQuad));
            endSequence.Join(backgroundImage.DOFade(0f, fadeSpeed));
        }

        // 头像缩小消失
        if (speakerImage != null)
        {
            endSequence.Join(speakerImage.transform.DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBack));
            speakerImage.transform.DOKill(); // 停止旋转动画
        }

        endSequence.OnComplete(() =>
        {
            if (backgroundImage != null)
            {
                backgroundImage.gameObject.SetActive(false);
            }
            isDialogueActive = false;
            onDialogueComplete?.Invoke();
        });
    }

    /// <summary>
    /// 文字逐字显示协程 - 带弹跳效果
    /// </summary>
    private IEnumerator TypeText(string message)
    {
        isTyping = true;
        dialogueText.text = "";

        // 创建TMP_Text子对象来实现单字动画
        dialogueText.ForceMeshUpdate();
        TMP_TextInfo textInfo = dialogueText.textInfo;

        int charIndex = 0;
        foreach (char letter in message.ToCharArray())
        {
            // 添加新字符
            dialogueText.text += letter;
            dialogueText.ForceMeshUpdate();

            // 为新添加的字符添加弹跳动画
            if (textInfo.characterCount > 0)
            {
                StartCoroutine(AnimateCharacter(charIndex));
            }

            charIndex++;

            // 如果是标点符号，稍微停顿长一点
            if (char.IsPunctuation(letter))
            {
                yield return new WaitForSeconds(typingSpeed * 2f);
            }
            else
            {
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        isTyping = false;
    }

    /// <summary>
    /// 为单个字符添加弹跳动画
    /// </summary>
    private IEnumerator AnimateCharacter(int charIndex)
    {
        if (dialogueText != null && dialogueText.textInfo.characterCount > charIndex)
        {
            TMP_CharacterInfo charInfo = dialogueText.textInfo.characterInfo[charIndex];

            if (charInfo.isVisible)
            {
                // 获取字符的顶点信息
                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                Vector3[] vertices = dialogueText.textInfo.meshInfo[materialIndex].vertices;

                // 存储原始位置
                Vector3[] sourceVertices = new Vector3[4];
                for (int i = 0; i < 4; i++)
                {
                    sourceVertices[i] = vertices[vertexIndex + i];
                }

                // 立即将字符移到最高点
                for (int i = 0; i < 4; i++)
                {
                    vertices[vertexIndex + i] = sourceVertices[i] + new Vector3(0, bounceHeight, 0);
                }
                dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

                // 创建弹跳动画序列
                float elapsedTime = 0f;
                float startHeight = bounceHeight;

                while (elapsedTime < bounceDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float normalizedTime = elapsedTime / bounceDuration;

                    // 使用弹性函数计算当前高度
                    float currentHeight = startHeight * (1f - DOVirtual.EasedValue(0f, 1f, normalizedTime, bounceEase));

                    // 添加小幅度摆动
                    float wobble = Mathf.Sin(normalizedTime * 12f) * (1f - normalizedTime) * 5f;

                    // 更新顶点位置
                    for (int i = 0; i < 4; i++)
                    {
                        vertices[vertexIndex + i] = sourceVertices[i] + new Vector3(wobble, currentHeight, 0);
                    }

                    dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                    yield return null;
                }

                // 确保最终位置正确
                for (int i = 0; i < 4; i++)
                {
                    vertices[vertexIndex + i] = sourceVertices[i];
                }
                dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            }
        }
    }

    /// <summary>
    /// 处理用户输入
    /// </summary>
    private void Update()
    {
        // 如果正在显示对话且用户点击或按空格，则加速显示或关闭对话
        if (isDialogueActive && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.Joystick1Button0)))
        {
            if (isTyping)
            {
                // 如果正在打字，则立即显示完整文本
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentMessage;
                isTyping = false;
            }
            else
            {
                // 如果已经显示完毕，则关闭对话
                EndDialogue();
            }
        }
    }

    /// <summary>
    /// 设置打字速度
    /// </summary>
    public void SetTypingSpeed(float speed)
    {
        typingSpeed = Mathf.Max(0.01f, speed);
    }

    /// <summary>
    /// 立即显示当前对话的完整内容
    /// </summary>
    public void ShowCompleteText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        dialogueText.text = currentMessage;
        isTyping = false;
    }

    /// <summary>
    /// 设置背景图片
    /// </summary>
    /// <param name="sprite">背景图片精灵</param>
    public void SetBackgroundImage(Sprite sprite)
    {
        if (backgroundImage != null && sprite != null)
        {
            backgroundImage.sprite = sprite;
        }
    }

    /// <summary>
    /// 设置背景图片透明度
    /// </summary>
    /// <param name="alpha">透明度值(0-1)</param>
    public void SetBackgroundAlpha(float alpha)
    {
        backgroundAlpha = Mathf.Clamp01(alpha);
        if (backgroundImage != null && isDialogueActive)
        {
            backgroundImage.DOFade(backgroundAlpha, fadeSpeed);
        }
    }

    /// <summary>
    /// 添加文字强调效果
    /// </summary>
    private void EmphasisEffect(string word)
    {
        // TODO: 为特定文字添加强调效果
        // 可以通过DOTween实现放大、颜色变化等效果
    }

    /// <summary>
    /// 添加情感表现效果
    /// </summary>
    private void EmotionEffect(string emotion)
    {
        // TODO: 根据情感类型添加不同的动画效果
        // 例如：开心时上下跳动，生气时剧烈抖动等
    }

    /// <summary>
    /// 重置对话播放状态
    /// </summary>
    public static void ResetDialogueStatus()
    {
        playedDialogues.Clear();
    }

    /// <summary>
    /// 检查对话是否已经播放过
    /// </summary>
    /// <param name="message">要检查的对话内容</param>
    /// <returns>是否已经播放过</returns>
    public static bool HasDialoguePlayed(string message)
    {
        return playedDialogues.ContainsKey(message) && playedDialogues[message];
    }
}


