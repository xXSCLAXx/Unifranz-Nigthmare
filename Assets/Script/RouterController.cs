using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class RouterController : MonoBehaviour
{
    public static bool IsOpen { get; private set; }
    public static bool wifiFixed = false;

    private GameObject panel;
    private GameObject blocker;
    private GameObject frontView;
    private GameObject backView;
    private Button btnPower;
    private Text btnPowerText;
    private Image btnPowerImg;
    private Text statusText;
    private Button btnSwitch;
    private Text switchText;
    private Image switchImg;
    private bool showingBack = false;
    private bool powerOn = false;
    private bool wifiRestarted = false;
    private float blinkTimer = 0f;

    private static bool firstTimeRewardGiven = false;

    private static bool wifiBrokenAgain = false;
    private static float brokenTimer = 0f;
    private GameObject alertOverlay;
    private GameObject screamerObj;
    private RawImage screamerRaw;
    private AudioSource alarmSource;
    private AudioClip alarmClip;
    private VideoPlayer videoPlayer;
    private bool screamerShowing = false;
    private float screamerTimer = 0f;

    private Transform canvasRoot;

    void Awake()
    {
        canvasRoot = transform.parent;
        alarmClip = Resources.Load<AudioClip>("Audio/wifi_alarm");

        CreateBlocker();
        CreatePanel();
        CreateAlertOverlay();
        CreateScreamer();
        panel.SetActive(false);
        blocker.SetActive(false);
        alertOverlay.SetActive(false);
        screamerObj.SetActive(false);
    }

    void CreateBlocker()
    {
        blocker = new GameObject("Blocker");
        blocker.transform.SetParent(transform, false);
        RectTransform bRt = blocker.AddComponent<RectTransform>();
        bRt.anchorMin = Vector2.zero;
        bRt.anchorMax = Vector2.one;
        bRt.sizeDelta = Vector2.zero;
        bRt.anchoredPosition = Vector2.zero;
        Image bg = blocker.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.4f);
        bg.raycastTarget = true;
    }

    void CreateAlertOverlay()
    {
        alertOverlay = new GameObject("AlertOverlay");
        alertOverlay.transform.SetParent(canvasRoot, false);
        RectTransform aRt = alertOverlay.AddComponent<RectTransform>();
        aRt.anchorMin = Vector2.zero;
        aRt.anchorMax = Vector2.one;
        aRt.sizeDelta = Vector2.zero;
        aRt.anchoredPosition = Vector2.zero;
        Image alertImg = alertOverlay.AddComponent<Image>();
        alertImg.color = new Color(0.6f, 0f, 0f, 0f);
        alertImg.raycastTarget = false;
    }

    void CreateScreamer()
    {
        screamerObj = new GameObject("ScreamerVideo");
        screamerObj.transform.SetParent(canvasRoot, false);
        RectTransform sRt = screamerObj.AddComponent<RectTransform>();
        sRt.anchorMin = Vector2.zero;
        sRt.anchorMax = Vector2.one;
        sRt.sizeDelta = Vector2.zero;
        sRt.anchoredPosition = Vector2.zero;

        screamerRaw = screamerObj.AddComponent<RawImage>();
        screamerRaw.color = Color.white;
        screamerRaw.raycastTarget = false;

        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        rt.Create();

        videoPlayer = screamerObj.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.skipOnDrop = true;
        videoPlayer.source = VideoSource.VideoClip;
        VideoClip vc = Resources.Load<VideoClip>("Video/screamer_wifi");
        if (vc == null)
            vc = Resources.Load<VideoClip>("screamer_wifi");
        videoPlayer.clip = vc;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = rt;

        screamerRaw.texture = rt;
    }

    void CreatePanel()
    {
        panel = new GameObject("RouterPanel");
        panel.transform.SetParent(transform, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(640, 380);
        rt.anchoredPosition = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.12f, 0.12f, 0.14f, 0.98f);
        bg.raycastTarget = true;

        frontView = CreateFrontView();
        backView = CreateBackView();
        backView.SetActive(false);

        CreateCloseButton();
    }

    GameObject CreateFrontView()
    {
        GameObject view = new GameObject("FrontView");
        view.transform.SetParent(panel.transform, false);
        RectTransform vRt = view.AddComponent<RectTransform>();
        vRt.anchorMin = Vector2.zero;
        vRt.anchorMax = Vector2.one;
        vRt.sizeDelta = new Vector2(-16, -36);
        vRt.anchoredPosition = Vector2.zero;

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(view.transform, false);
        Text title = titleObj.AddComponent<Text>();
        title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        title.fontSize = 13;
        title.fontStyle = FontStyle.Bold;
        title.alignment = TextAnchor.UpperCenter;
        title.color = new Color(0.7f, 0.7f, 0.75f);
        title.text = "Router TP-Link WR940N - Panel frontal";
        RectTransform tRt = titleObj.GetComponent<RectTransform>();
        tRt.anchorMin = new Vector2(0f, 0.88f);
        tRt.anchorMax = new Vector2(1f, 1f);
        tRt.sizeDelta = Vector2.zero;
        tRt.anchoredPosition = Vector2.zero;

        GameObject routerBody = new GameObject("RouterBody");
        routerBody.transform.SetParent(view.transform, false);
        Image body = routerBody.AddComponent<Image>();
        body.color = new Color(0.22f, 0.22f, 0.25f, 1f);
        RectTransform bRt = routerBody.GetComponent<RectTransform>();
        bRt.anchorMin = new Vector2(0.02f, 0.06f);
        bRt.anchorMax = new Vector2(0.98f, 0.85f);
        bRt.sizeDelta = Vector2.zero;
        bRt.anchoredPosition = Vector2.zero;

        GameObject antenaLeft = new GameObject("AntenaLeft");
        antenaLeft.transform.SetParent(view.transform, false);
        Image antL = antenaLeft.AddComponent<Image>();
        antL.color = new Color(0.18f, 0.18f, 0.2f, 1f);
        RectTransform alRt = antenaLeft.GetComponent<RectTransform>();
        alRt.anchorMin = new Vector2(0.1f, 0.86f);
        alRt.anchorMax = new Vector2(0.22f, 0.96f);
        alRt.sizeDelta = Vector2.zero;
        alRt.anchoredPosition = Vector2.zero;

        GameObject antenaRight = new GameObject("AntenaRight");
        antenaRight.transform.SetParent(view.transform, false);
        Image antR = antenaRight.AddComponent<Image>();
        antR.color = new Color(0.18f, 0.18f, 0.2f, 1f);
        RectTransform arRt = antenaRight.GetComponent<RectTransform>();
        arRt.anchorMin = new Vector2(0.78f, 0.86f);
        arRt.anchorMax = new Vector2(0.9f, 0.96f);
        arRt.sizeDelta = Vector2.zero;
        arRt.anchoredPosition = Vector2.zero;

        GameObject vent = new GameObject("VentLine");
        vent.transform.SetParent(routerBody.transform, false);
        Image ventImg = vent.AddComponent<Image>();
        ventImg.color = new Color(0.18f, 0.18f, 0.2f, 1f);
        RectTransform veRt = vent.GetComponent<RectTransform>();
        veRt.anchorMin = new Vector2(0.02f, 0.5f);
        veRt.anchorMax = new Vector2(0.98f, 0.52f);
        veRt.sizeDelta = Vector2.zero;
        veRt.anchoredPosition = Vector2.zero;
        for (int i = 0; i < 12; i++)
        {
            GameObject slot = new GameObject("Slot" + i);
            slot.transform.SetParent(routerBody.transform, false);
            Image slotImg = slot.AddComponent<Image>();
            slotImg.color = new Color(0.16f, 0.16f, 0.18f, 1f);
            RectTransform slotRt = slot.GetComponent<RectTransform>();
            float x = 0.08f + i * 0.075f;
            slotRt.anchorMin = new Vector2(x, 0.5f);
            slotRt.anchorMax = new Vector2(x + 0.045f, 0.62f);
            slotRt.sizeDelta = Vector2.zero;
            slotRt.anchoredPosition = Vector2.zero;
        }

        GameObject ledStrip = new GameObject("LEDStrip");
        ledStrip.transform.SetParent(routerBody.transform, false);
        HorizontalLayoutGroup hlg = ledStrip.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.spacing = 18;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        RectTransform lsRt = ledStrip.GetComponent<RectTransform>();
        lsRt.anchorMin = new Vector2(0.1f, 0.78f);
        lsRt.anchorMax = new Vector2(0.9f, 0.95f);
        lsRt.sizeDelta = Vector2.zero;
        lsRt.anchoredPosition = Vector2.zero;

        string[] labels = { "PWR", "WAN", "LAN1", "LAN2", "LAN3", "WLAN" };
        for (int i = 0; i < labels.Length; i++)
        {
            GameObject led = new GameObject("LED_" + labels[i]);
            led.transform.SetParent(ledStrip.transform, false);
            VerticalLayoutGroup vlg = led.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.childForceExpandWidth = false;
            vlg.childForceExpandHeight = false;

            GameObject dot = new GameObject("Dot");
            dot.transform.SetParent(led.transform, false);
            Image dotImg = dot.AddComponent<Image>();
            dotImg.color = labels[i] == "PWR" ? (powerOn ? new Color(0f, 1f, 0f) : new Color(0.25f, 0.25f, 0.25f)) : new Color(0.25f, 0.25f, 0.25f);
            dotImg.rectTransform.sizeDelta = new Vector2(14, 14);
            GameObject lTxt = new GameObject("Label");
            lTxt.transform.SetParent(led.transform, false);
            Text lText = lTxt.AddComponent<Text>();
            lText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            lText.fontSize = 8;
            lText.alignment = TextAnchor.UpperCenter;
            lText.color = new Color(0.5f, 0.5f, 0.55f);
            lText.text = labels[i];
            RectTransform lTxtRt = lTxt.GetComponent<RectTransform>();
            lTxtRt.anchorMin = new Vector2(0f, 1f);
            lTxtRt.anchorMax = new Vector2(1f, 1f);
            lTxtRt.sizeDelta = new Vector2(0f, 14f);
            lTxtRt.anchoredPosition = Vector2.zero;
        }

        GameObject powerBtnObj = new GameObject("BtnPower");
        powerBtnObj.transform.SetParent(routerBody.transform, false);
        btnPowerImg = powerBtnObj.AddComponent<Image>();
        btnPowerImg.color = new Color(0.5f, 0.05f, 0.05f, 1f);
        btnPowerImg.rectTransform.sizeDelta = new Vector2(90, 30);
        btnPowerImg.rectTransform.anchoredPosition = new Vector2(0, -55);
        btnPower = powerBtnObj.AddComponent<Button>();
        btnPower.onClick.AddListener(OnPowerButtonClick);
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.None;
        btnPower.navigation = nav;

        GameObject pTxt = new GameObject("Text");
        pTxt.transform.SetParent(powerBtnObj.transform, false);
        btnPowerText = pTxt.AddComponent<Text>();
        btnPowerText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        btnPowerText.fontSize = 11;
        btnPowerText.fontStyle = FontStyle.Bold;
        btnPowerText.alignment = TextAnchor.MiddleCenter;
        btnPowerText.color = Color.white;
        btnPowerText.text = "REINICIAR";
        RectTransform pTxtRt = pTxt.GetComponent<RectTransform>();
        pTxtRt.anchorMin = Vector2.zero;
        pTxtRt.anchorMax = Vector2.one;
        pTxtRt.sizeDelta = Vector2.zero;
        pTxtRt.anchoredPosition = Vector2.zero;

        GameObject statusObj = new GameObject("StatusText");
        statusObj.transform.SetParent(view.transform, false);
        statusText = statusObj.AddComponent<Text>();
        statusText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        statusText.fontSize = 11;
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.color = new Color(1f, 0.85f, 0.2f);
        RectTransform sRt = statusObj.GetComponent<RectTransform>();
        sRt.anchorMin = new Vector2(0f, 0f);
        sRt.anchorMax = new Vector2(1f, 0f);
        sRt.sizeDelta = new Vector2(0f, 22f);
        sRt.anchoredPosition = new Vector2(0f, 6f);

        CreateFlipButton(view);
        return view;
    }

    GameObject CreateBackView()
    {
        GameObject view = new GameObject("BackView");
        view.transform.SetParent(panel.transform, false);
        RectTransform vRt = view.AddComponent<RectTransform>();
        vRt.anchorMin = Vector2.zero;
        vRt.anchorMax = Vector2.one;
        vRt.sizeDelta = new Vector2(-16, -36);
        vRt.anchoredPosition = Vector2.zero;

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(view.transform, false);
        Text title = titleObj.AddComponent<Text>();
        title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        title.fontSize = 13;
        title.fontStyle = FontStyle.Bold;
        title.alignment = TextAnchor.UpperCenter;
        title.color = new Color(0.7f, 0.7f, 0.75f);
        title.text = "Router TP-Link WR940N - Panel trasero";
        RectTransform tRt = titleObj.GetComponent<RectTransform>();
        tRt.anchorMin = new Vector2(0f, 0.88f);
        tRt.anchorMax = new Vector2(1f, 1f);
        tRt.sizeDelta = Vector2.zero;
        tRt.anchoredPosition = Vector2.zero;

        GameObject backBody = new GameObject("BackBody");
        backBody.transform.SetParent(view.transform, false);
        Image body = backBody.AddComponent<Image>();
        body.color = new Color(0.2f, 0.2f, 0.22f, 1f);
        RectTransform bRt = backBody.GetComponent<RectTransform>();
        bRt.anchorMin = new Vector2(0.02f, 0.06f);
        bRt.anchorMax = new Vector2(0.98f, 0.85f);
        bRt.sizeDelta = Vector2.zero;
        bRt.anchoredPosition = Vector2.zero;

        GameObject portGroup = new GameObject("PortGroup");
        portGroup.transform.SetParent(backBody.transform, false);
        HorizontalLayoutGroup hlg = portGroup.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.spacing = 10;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        RectTransform pgRt = portGroup.GetComponent<RectTransform>();
        pgRt.anchorMin = new Vector2(0.1f, 0.55f);
        pgRt.anchorMax = new Vector2(0.7f, 0.9f);
        pgRt.sizeDelta = Vector2.zero;
        pgRt.anchoredPosition = Vector2.zero;

        string[] ports = { "WAN", "LAN1", "LAN2", "LAN3" };
        foreach (string p in ports)
        {
            GameObject port = new GameObject("Port_" + p);
            port.transform.SetParent(portGroup.transform, false);
            VerticalLayoutGroup pvlg = port.AddComponent<VerticalLayoutGroup>();
            pvlg.childAlignment = TextAnchor.MiddleCenter;
            pvlg.childForceExpandWidth = false;
            pvlg.childForceExpandHeight = false;

            GameObject portSlot = new GameObject("Slot");
            portSlot.transform.SetParent(port.transform, false);
            Image portImg = portSlot.AddComponent<Image>();
            portImg.color = new Color(0.35f, 0.3f, 0.25f, 1f);
            portImg.rectTransform.sizeDelta = new Vector2(24, 16);
            GameObject portLabel = new GameObject("Label");
            portLabel.transform.SetParent(port.transform, false);
            Text pText = portLabel.AddComponent<Text>();
            pText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            pText.fontSize = 7;
            pText.alignment = TextAnchor.UpperCenter;
            pText.color = new Color(0.5f, 0.5f, 0.55f);
            pText.text = p;
            RectTransform pTxtRt = portLabel.GetComponent<RectTransform>();
            pTxtRt.anchorMin = new Vector2(0f, 1f);
            pTxtRt.anchorMax = new Vector2(1f, 1f);
            pTxtRt.sizeDelta = new Vector2(0f, 12f);
            pTxtRt.anchoredPosition = Vector2.zero;
        }

        GameObject switchObj = new GameObject("BtnSwitch");
        switchObj.transform.SetParent(backBody.transform, false);
        switchImg = switchObj.AddComponent<Image>();
        switchImg.color = new Color(0.3f, 0.3f, 0.35f, 1f);
        switchImg.rectTransform.sizeDelta = new Vector2(130, 44);
        switchImg.rectTransform.anchoredPosition = new Vector2(0, -55);
        btnSwitch = switchObj.AddComponent<Button>();
        btnSwitch.onClick.AddListener(OnSwitchToggle);
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.None;
        btnSwitch.navigation = nav;

        GameObject sTxt = new GameObject("Text");
        sTxt.transform.SetParent(switchObj.transform, false);
        switchText = sTxt.AddComponent<Text>();
        switchText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        switchText.fontSize = 16;
        switchText.fontStyle = FontStyle.Bold;
        switchText.alignment = TextAnchor.MiddleCenter;
        switchText.color = new Color(1f, 0.3f, 0.3f);
        switchText.text = "OFF";
        RectTransform sTxtRt = sTxt.GetComponent<RectTransform>();
        sTxtRt.anchorMin = Vector2.zero;
        sTxtRt.anchorMax = Vector2.one;
        sTxtRt.sizeDelta = Vector2.zero;
        sTxtRt.anchoredPosition = Vector2.zero;

        GameObject pwLabel = new GameObject("PowerLabel");
        pwLabel.transform.SetParent(backBody.transform, false);
        Text pwText = pwLabel.AddComponent<Text>();
        pwText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        pwText.fontSize = 8;
        pwText.alignment = TextAnchor.MiddleCenter;
        pwText.color = new Color(0.5f, 0.5f, 0.55f);
        pwText.text = "FUENTE DE PODER";
        RectTransform pwRt = pwLabel.GetComponent<RectTransform>();
        pwRt.anchorMin = new Vector2(0.4f, 0.35f);
        pwRt.anchorMax = new Vector2(0.6f, 0.42f);
        pwRt.sizeDelta = Vector2.zero;
        pwRt.anchoredPosition = Vector2.zero;

        CreateFlipButton(view);
        return view;
    }

    void CreateFlipButton(GameObject parent)
    {
        GameObject flipBtn = new GameObject("BtnFlip");
        flipBtn.transform.SetParent(parent.transform, false);
        Image img = flipBtn.AddComponent<Image>();
        img.color = new Color(0.25f, 0.3f, 0.5f, 1f);
        Button btn = flipBtn.AddComponent<Button>();
        btn.onClick.AddListener(FlipView);
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.None;
        btn.navigation = nav;
        RectTransform rt = flipBtn.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(120, 24);
        rt.anchoredPosition = new Vector2(0, -12);

        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(flipBtn.transform, false);
        Text txt = txtObj.AddComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = 11;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.text = "DAR LA VUELTA";
        RectTransform txtRt = txtObj.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero;
        txtRt.anchoredPosition = Vector2.zero;
    }

    void CreateCloseButton()
    {
        GameObject closeBtn = new GameObject("CloseBtn");
        closeBtn.transform.SetParent(panel.transform, false);
        Image btnImg = closeBtn.AddComponent<Image>();
        btnImg.color = new Color(0.8f, 0.15f, 0.15f, 1f);
        Button btn = closeBtn.AddComponent<Button>();
        btn.onClick.AddListener(Close);
        RectTransform bRt = closeBtn.GetComponent<RectTransform>();
        bRt.anchorMin = new Vector2(1f, 1f);
        bRt.anchorMax = new Vector2(1f, 1f);
        bRt.pivot = new Vector2(1f, 1f);
        bRt.sizeDelta = new Vector2(28f, 28f);
        bRt.anchoredPosition = new Vector2(-4f, -4f);
        GameObject xTxt = new GameObject("XText");
        xTxt.transform.SetParent(closeBtn.transform, false);
        Text xLabel = xTxt.AddComponent<Text>();
        xLabel.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        xLabel.fontSize = 16;
        xLabel.fontStyle = FontStyle.Bold;
        xLabel.alignment = TextAnchor.MiddleCenter;
        xLabel.color = Color.white;
        xLabel.text = "X";
        RectTransform xRt = xTxt.GetComponent<RectTransform>();
        xRt.anchorMin = Vector2.zero;
        xRt.anchorMax = Vector2.one;
        xRt.sizeDelta = Vector2.zero;
        xRt.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        if (panel == null) return;

        if (screamerShowing)
        {
            screamerTimer -= Time.deltaTime;
            if (screamerTimer <= 0f)
            {
                screamerShowing = false;
                if (videoPlayer != null && videoPlayer.isPlaying)
                    videoPlayer.Stop();
                screamerObj.SetActive(false);
            }
        }

        if (wifiBrokenAgain)
        {
            float pulseAlpha = Mathf.Abs(Mathf.Sin(Time.time * 2f)) * 0.3f;
            Image alertImg = alertOverlay.GetComponent<Image>();
            if (alertImg != null)
                alertImg.color = new Color(0.6f, 0f, 0f, pulseAlpha);

            brokenTimer -= Time.deltaTime;

            if (brokenTimer <= 0f)
            {
                wifiBrokenAgain = false;
                TriggerScreamer();
            }
        }

        if (wifiRestarted && !wifiBrokenAgain && !panel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (Random.value < 0.5f)
            {
                wifiBrokenAgain = true;
                wifiFixed = false;
                TaskNotesController.wifiFixed = false;
                brokenTimer = 15f;
                alertOverlay.transform.SetAsLastSibling();
                alertOverlay.SetActive(true);
                StartAlarm();
            }
        }

        if (panel.activeSelf && !showingBack && !wifiRestarted)
        {
            if (powerOn)
            {
                blinkTimer += Time.deltaTime * 4f;
                float alpha = Mathf.Abs(Mathf.Sin(blinkTimer));
                btnPowerImg.color = new Color(1f, 0.1f, 0.05f, alpha);
            }
            else
            {
                btnPowerImg.color = new Color(0.3f, 0.05f, 0.05f, 0.5f);
            }
        }
        else if (panel.activeSelf && !showingBack && wifiRestarted)
        {
            btnPowerImg.color = new Color(0.05f, 0.6f, 0.05f, 1f);
            btnPowerText.text = "LISTO";
        }
    }

    void FlipView()
    {
        showingBack = !showingBack;
        frontView.SetActive(!showingBack);
        backView.SetActive(showingBack);
    }

    void OnPowerButtonClick()
    {
        if (wifiRestarted) return;
        if (!powerOn)
        {
            statusText.text = "Sin corriente! Revisa la parte de atras.";
            return;
        }
        wifiRestarted = true;
        wifiFixed = true;
        TaskNotesController.wifiFixed = true;

        if (!firstTimeRewardGiven)
        {
            firstTimeRewardGiven = true;
            PCTimer.bonusTime += 1f;
        }

        if (wifiBrokenAgain)
        {
            wifiBrokenAgain = false;
            StopAlarm();
            alertOverlay.SetActive(false);
        }

        statusText.text = "WiFi reiniciado correctamente!";
        btnPowerText.text = "LISTO";
        btnPowerImg.color = new Color(0.05f, 0.6f, 0.05f, 1f);
    }

    void OnSwitchToggle()
    {
        powerOn = !powerOn;
        switchText.text = powerOn ? "ON" : "OFF";
        switchText.color = powerOn ? new Color(0.2f, 1f, 0.2f) : new Color(1f, 0.3f, 0.3f);
    }

    void StartAlarm()
    {
        if (alarmSource == null)
        {
            alarmSource = gameObject.AddComponent<AudioSource>();
            alarmSource.loop = true;
            alarmSource.volume = 0.6f;
        }
        if (alarmClip == null)
        {
            AudioClip loaded = Resources.Load<AudioClip>("Audio/wifi_alarms");
            if (loaded != null)
            {
                int sr = loaded.frequency;
                int channels = loaded.channels;
                int startSample = sr * 1;
                int endSample = sr * 9;
                int len = endSample - startSample;
                float[] fullData = new float[loaded.samples * channels];
                loaded.GetData(fullData, 0);
                float[] trimData = new float[len * channels];
                for (int i = 0; i < trimData.Length; i++)
                    trimData[i] = fullData[startSample * channels + i];
                alarmClip = AudioClip.Create("alarm_trim", len, channels, sr, false);
                alarmClip.SetData(trimData, 0);
            }
            if (alarmClip == null)
            {
                int sr = 44100;
                int len = sr * 8;
                float[] wave = new float[len];
                for (int i = 0; i < len; i++)
                    wave[i] = Mathf.Sin(2 * Mathf.PI * 880 * i / sr);
                alarmClip = AudioClip.Create("beep", len, 1, sr, false);
                alarmClip.SetData(wave, 0);
            }
        }
        alarmSource.clip = alarmClip;
        alarmSource.Play();
    }

    void StopAlarm()
    {
        if (alarmSource != null && alarmSource.isPlaying)
            alarmSource.Stop();
    }

    void TriggerScreamer()
    {
        screamerShowing = true;
        screamerTimer = 3f;
        screamerObj.transform.SetAsLastSibling();
        screamerObj.SetActive(true);

        if (videoPlayer != null && videoPlayer.clip != null)
        {
            videoPlayer.Play();
            screamerTimer = Mathf.Max(3f, (float)videoPlayer.clip.length);
        }
        else
        {
            screamerRaw.color = Color.red;
        }

        AudioManager am = FindObjectOfType<AudioManager>();
        if (am != null) am.PlayScream();

        StopAlarm();
    }

    public void Show()
    {
        IsOpen = true;
        blocker.SetActive(true);

        if (wifiBrokenAgain)
        {
            powerOn = false;
            showingBack = false;
            wifiRestarted = false;
            frontView.SetActive(true);
            backView.SetActive(false);
            switchText.text = "OFF";
            switchText.color = new Color(1f, 0.3f, 0.3f);
            btnPowerText.text = "REINICIAR";
            btnPowerImg.color = new Color(0.3f, 0.05f, 0.05f, 0.5f);
            statusText.text = "Conecta la corriente en la parte de atras";
            panel.SetActive(true);
            return;
        }

        if (wifiRestarted)
        {
            panel.SetActive(true);
            statusText.text = "WiFi funcionando!";
            btnPowerText.text = "LISTO";
            btnPowerImg.color = new Color(0.05f, 0.6f, 0.05f, 1f);
            frontView.SetActive(true);
            backView.SetActive(false);
            showingBack = false;
            return;
        }

        powerOn = false;
        showingBack = false;
        frontView.SetActive(true);
        backView.SetActive(false);
        switchText.text = "OFF";
        switchText.color = new Color(1f, 0.3f, 0.3f);
        btnPowerText.text = "REINICIAR";
        btnPowerImg.color = new Color(0.3f, 0.05f, 0.05f, 0.5f);
        statusText.text = "Conecta la corriente en la parte de atras";
        panel.SetActive(true);
    }

    public void Close()
    {
        IsOpen = false;
        blocker.SetActive(false);
        panel.SetActive(false);
    }
}
