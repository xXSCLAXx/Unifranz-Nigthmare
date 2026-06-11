using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class ComputerRoomController : MonoBehaviour
{
    public static bool IsOpen { get; private set; }
    public static bool pc4IsBroken = false;
    private static ComputerRoomController instance;
    private static GameObject alertOverlay;
    private static AudioSource alarmSource;
    private static AudioClip alarmClip;
    private static float breakTimer = 0f;
    private GameObject panel;
    private GameObject blocker;
    private GameObject screamerObj;
    private RawImage screamerRaw;
    private VideoPlayer videoPlayer;
    private RenderTexture pc4ScreamerRt;
    private Text pc4ErrorText;
    private Button pc4BtnRef;
    private static bool persistentCreated = false;
    private static Transform canvasRoot;

    public static void Show()
    {
        if (instance == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null) return;
            canvasRoot = canvas.transform;
            GameObject go = new GameObject("ComputerRoomController");
            go.transform.SetParent(canvas.transform, false);
            instance = go.AddComponent<ComputerRoomController>();
            instance.InitPersistent();
        }
        instance.CreatePanel();
    }

    void InitPersistent()
    {
        if (persistentCreated) return;
        persistentCreated = true;

        GameObject rtObj = new GameObject("PC4ScreamerRT");
        rtObj.transform.SetParent(canvasRoot, false);
        pc4ScreamerRt = new RenderTexture(1920, 1080, 0);
        pc4ScreamerRt.Create();

        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = pc4ScreamerRt;
        videoPlayer.isLooping = false;

        screamerObj = new GameObject("PC4Screamer");
        screamerObj.transform.SetParent(canvasRoot, false);
        screamerObj.SetActive(false);
        RectTransform sRt = screamerObj.AddComponent<RectTransform>();
        sRt.anchorMin = Vector2.zero;
        sRt.anchorMax = Vector2.one;
        sRt.sizeDelta = Vector2.zero;
        screamerRaw = screamerObj.AddComponent<RawImage>();
        screamerRaw.texture = pc4ScreamerRt;
        screamerRaw.color = Color.white;

        alertOverlay = new GameObject("PC4AlertOverlay");
        alertOverlay.transform.SetParent(canvasRoot, false);
        alertOverlay.SetActive(false);
        RectTransform aRt = alertOverlay.AddComponent<RectTransform>();
        aRt.anchorMin = Vector2.zero;
        aRt.anchorMax = Vector2.one;
        aRt.sizeDelta = Vector2.zero;
        Image aImg = alertOverlay.AddComponent<Image>();
        aImg.color = new Color(1f, 0f, 0f, 0f);
        aImg.raycastTarget = false;

        StartCoroutine(AlertPulse(aImg));

        alarmSource = gameObject.AddComponent<AudioSource>();
        alarmSource.loop = true;
        alarmSource.volume = 0.6f;
    }

    void CreatePanel()
    {
        IsOpen = true;
        Transform root = transform.parent;

        blocker = new GameObject("ComputerRoomBlocker");
        blocker.transform.SetParent(root, false);
        RectTransform bRt = blocker.AddComponent<RectTransform>();
        bRt.anchorMin = Vector2.zero;
        bRt.anchorMax = Vector2.one;
        bRt.sizeDelta = Vector2.zero;
        Image bImg = blocker.AddComponent<Image>();
        bImg.color = new Color(0f, 0f, 0f, 0.8f);
        bImg.raycastTarget = true;

        panel = new GameObject("ComputerRoomPanel");
        panel.transform.SetParent(root, false);
        panel.transform.SetAsLastSibling();
        RectTransform pRt = panel.AddComponent<RectTransform>();
        pRt.anchorMin = Vector2.zero;
        pRt.anchorMax = Vector2.one;
        pRt.sizeDelta = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        Sprite bgSprite = null;
        Texture2D tex = Resources.Load<Texture2D>("Texture/computer_room");
        if (tex != null)
            bgSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        if (bgSprite != null)
        {
            bg.sprite = bgSprite;
            bg.type = Image.Type.Simple;
            bg.preserveAspect = true;
        }
        else
            bg.color = new Color(0.1f, 0.08f, 0.12f, 1f);
        bg.raycastTarget = true;

        GameObject closeObj = new GameObject("BtnClose");
        closeObj.transform.SetParent(panel.transform, false);
        RectTransform cRt = closeObj.AddComponent<RectTransform>();
        cRt.anchorMin = new Vector2(0f, 0f);
        cRt.anchorMax = new Vector2(0f, 0f);
        cRt.pivot = new Vector2(0.5f, 0.5f);
        cRt.anchoredPosition = new Vector2(50, 50);
        cRt.sizeDelta = new Vector2(68, 68);
        Image cImg = closeObj.AddComponent<Image>();
        cImg.color = new Color(0.8f, 0.15f, 0.15f, 0.85f);
        Button cBtn = closeObj.AddComponent<Button>();
        cBtn.targetGraphic = cImg;
        cBtn.onClick.AddListener(() => {
            AudioManager am = FindObjectOfType<AudioManager>();
            if (am != null) am.PlayClickPC();
            Close();
        });

        GameObject cTextObj = new GameObject("Text");
        cTextObj.transform.SetParent(closeObj.transform, false);
        RectTransform cTextRt = cTextObj.AddComponent<RectTransform>();
        cTextRt.anchorMin = Vector2.zero;
        cTextRt.anchorMax = Vector2.one;
        cTextRt.sizeDelta = Vector2.zero;
        Text cText = cTextObj.AddComponent<Text>();
        cText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        cText.fontSize = 18;
        cText.fontStyle = FontStyle.Bold;
        cText.alignment = TextAnchor.MiddleCenter;
        cText.color = Color.white;
        cText.text = "X";

        CreatePC4Hotspot();
    }

    void CreatePC4Hotspot()
    {
        bool alreadyFixed = TaskNotesController.pc4Fixed;

        GameObject pc4Obj = new GameObject("PC4");
        pc4Obj.transform.SetParent(panel.transform, false);
        RectTransform pc4Rt = pc4Obj.AddComponent<RectTransform>();
        pc4Rt.anchorMin = new Vector2(1040f / 1920f, 469f / 1080f);
        pc4Rt.anchorMax = new Vector2(1040f / 1920f, 469f / 1080f);
        pc4Rt.pivot = new Vector2(0.5f, 0.5f);
        pc4Rt.anchoredPosition = Vector2.zero;
        pc4Rt.sizeDelta = new Vector2(80, 40);

        if (!alreadyFixed)
        {
            GameObject errObj = new GameObject("ErrorText");
            errObj.transform.SetParent(pc4Obj.transform, false);
            RectTransform errRt = errObj.AddComponent<RectTransform>();
            errRt.anchorMin = Vector2.zero;
            errRt.anchorMax = Vector2.one;
            errRt.sizeDelta = Vector2.zero;
            pc4ErrorText = errObj.AddComponent<Text>();
            pc4ErrorText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            pc4ErrorText.fontSize = 18;
            pc4ErrorText.fontStyle = FontStyle.Bold;
            pc4ErrorText.alignment = TextAnchor.MiddleCenter;
            pc4ErrorText.color = new Color(1f, 0.3f, 0.3f);
            pc4ErrorText.text = "ERROR";

            Button pc4Btn = pc4Obj.AddComponent<Button>();
            ColorBlock cb = new ColorBlock();
            cb.normalColor = new Color(0f, 0f, 0f, 0f);
            cb.highlightedColor = new Color(0f, 0f, 0f, 0f);
            cb.pressedColor = new Color(0f, 0f, 0f, 0f);
            cb.disabledColor = new Color(0f, 0f, 0f, 0f);
            cb.colorMultiplier = 1f;
            cb.fadeDuration = 0.1f;
            pc4Btn.colors = cb;

            pc4BtnRef = pc4Btn;
            pc4Btn.onClick.AddListener(() =>
            {
                if (TaskNotesController.pc4Fixed && !pc4IsBroken) return;
                WireMinigameController.Show(OnMinigameComplete);
            });

            StartCoroutine(PC4Blink(pc4ErrorText));
        }
    }

    public static void BreakPC4()
    {
        if (pc4IsBroken || !TaskNotesController.pc4Fixed) return;
        pc4IsBroken = true;
        TaskNotesController.pc4Fixed = false;
        breakTimer = 20f;
        if (instance != null && instance.pc4ErrorText != null && instance.pc4ErrorText.gameObject != null)
        {
            instance.pc4ErrorText.text = "ERROR";
            instance.pc4ErrorText.color = new Color(1f, 0.3f, 0.3f);
        }
        if (alertOverlay != null)
        {
            alertOverlay.transform.SetAsLastSibling();
            alertOverlay.SetActive(true);
        }
        if (alarmClip == null)
        {
            int sr = 44100;
            int len = sr * 8;
            float[] wave = new float[len];
            for (int i = 0; i < len; i++)
                wave[i] = Mathf.Sin(2 * Mathf.PI * 880 * i / sr);
            alarmClip = AudioClip.Create("pc4_beep", len, 1, sr, false);
            alarmClip.SetData(wave, 0);
        }
        if (alarmSource != null)
        {
            alarmSource.clip = alarmClip;
            alarmSource.Play();
        }
    }

    public static void FixPC4()
    {
        if (!pc4IsBroken) return;
        pc4IsBroken = false;
        TaskNotesController.pc4Fixed = true;
        if (alertOverlay != null) alertOverlay.SetActive(false);
        if (alarmSource != null && alarmSource.isPlaying) alarmSource.Stop();
        if (instance != null && instance.pc4ErrorText != null && instance.pc4ErrorText.gameObject != null)
            instance.pc4ErrorText.text = "";
    }

    void MarkPC4Fixed()
    {
        TaskNotesController.pc4Fixed = true;
        if (pc4ErrorText != null) pc4ErrorText.text = "";
        if (pc4BtnRef != null)
        {
            ColorBlock cb = pc4BtnRef.colors;
            cb.normalColor = new Color(0f, 0f, 0f, 0f);
            cb.highlightedColor = new Color(0f, 0f, 0f, 0f);
            cb.pressedColor = new Color(0f, 0f, 0f, 0f);
            cb.disabledColor = new Color(0f, 0f, 0f, 0f);
            pc4BtnRef.colors = cb;
        }
    }

    private static bool firstTimeBonusClaimed = false;

    void OnMinigameComplete()
    {
        if (pc4IsBroken)
        {
            FixPC4();
        }
        else
        {
            MarkPC4Fixed();
            if (!firstTimeBonusClaimed)
            {
                firstTimeBonusClaimed = true;
                PCTimer.AddBonusTime(2f);
            }
        }
    }

    void Update()
    {
        if (pc4IsBroken)
        {
            breakTimer -= Time.deltaTime;
            if (breakTimer <= 0f)
            {
                pc4IsBroken = false;
                if (alarmSource != null && alarmSource.isPlaying) alarmSource.Stop();
                if (alertOverlay != null) alertOverlay.SetActive(false);
                TriggerPC4Screamer();
            }
        }
    }

    void TriggerPC4Screamer()
    {
        GameOverController.LoseLife();
        screamerObj.transform.SetAsLastSibling();
        screamerObj.SetActive(true);

        GameObject bloodFilter = GameObject.Find("BloodFilter");
        if (bloodFilter != null) bloodFilter.SetActive(true);

        VideoClip screamerClip = Resources.Load<VideoClip>("Video/pc4_screamer");
        if (screamerClip != null)
        {
            videoPlayer.clip = screamerClip;
            videoPlayer.Play();
            Invoke("HideScreamer", 4f);
        }
        else
        {
            screamerRaw.color = Color.red;
            Invoke("HideScreamer", 1.5f);
        }

        AudioManager am = FindObjectOfType<AudioManager>();
        if (am != null) am.PlayScream();
    }

    void HideScreamer()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();
        if (screamerObj != null)
            screamerObj.SetActive(false);
        GameObject bloodFilter = GameObject.Find("BloodFilter");
        if (bloodFilter != null) bloodFilter.SetActive(false);
    }

    System.Collections.IEnumerator PC4Blink(Text text)
    {
        while (text != null && !TaskNotesController.pc4Fixed)
        {
            float blink = Mathf.PingPong(Time.time * 2f, 1f);
            text.color = new Color(1f, 0.3f, 0.3f, Mathf.Lerp(0.4f, 1f, blink));
            yield return null;
        }
    }

    System.Collections.IEnumerator AlertPulse(Image img)
    {
        while (true)
        {
            if (img != null && alertOverlay != null && alertOverlay.activeSelf)
            {
                float pulse = Mathf.PingPong(Time.time * 3f, 1f);
                img.color = new Color(1f, 0f, 0f, Mathf.Lerp(0.1f, 0.4f, pulse));
            }
            yield return null;
        }
    }

    void Close()
    {
        IsOpen = false;
        if (panel != null) Destroy(panel);
        if (blocker != null) Destroy(blocker);
        panel = null;
        blocker = null;
    }
}
