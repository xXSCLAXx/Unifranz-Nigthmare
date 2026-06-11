using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InformeController : MonoBehaviour
{
    public static bool IsOpen { get; private set; }
    public static event System.Action OnClose;
    private static InformeController instance;
    private static Transform canvasRoot;

    private GameObject panel;
    private GameObject blocker;
    private GameObject contentArea;

    private List<ErrorDef> errors = new List<ErrorDef>();
    private const float COOLDOWN_TIME = 40f;
    private const float DANGER_TIME = 45f;
    private bool taskCompleted = false;
    private int currentActiveErrorIndex = -1;
    private bool isCooldown = false;
    private float timerCooldown = 0f;
    private float timerDanger = 0f;

    private GameObject popupPanel;
    private int currentPopupIndex = -1;

    private GameObject alertOverlay;

    private AudioSource warningSource;
    private AudioClip warningClip;

    private static bool firstErrorFixed = false;

    struct ErrorDef
    {
        public string id;
        public string wrongText;
        public string correctText;
        public string wrongOption;
        public bool isActive;
        public bool isFixed;
        public int paragraphIndex;
    }

    public static void Show()
    {
        if (instance == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null) return;
            canvasRoot = canvas.transform;
            GameObject go = new GameObject("InformeController");
            go.transform.SetParent(canvas.transform, false);
            instance = go.AddComponent<InformeController>();
        }
        if (instance == null) return;
        instance.Open();
    }

    void Open()
    {
        if (panel == null) CreatePanel();
        IsOpen = true;
        panel.SetActive(true);
        blocker.SetActive(true);

        blocker.transform.SetAsLastSibling();
        panel.transform.SetAsLastSibling();

        if (TaskNotesController.informeFixed)
        {
            ShowCompletedState();
            return;
        }

        if (!firstErrorFixed && currentActiveErrorIndex < 0)
        {
            PickNextError();
        }
    }

    void CreatePanel()
    {
        Transform root = transform.parent;

        blocker = new GameObject("InformeBlocker");
        blocker.transform.SetParent(root, false);
        RectTransform bRt = blocker.AddComponent<RectTransform>();
        bRt.anchorMin = Vector2.zero;
        bRt.anchorMax = Vector2.one;
        bRt.sizeDelta = Vector2.zero;
        Image bImg = blocker.AddComponent<Image>();
        bImg.color = new Color(0f, 0f, 0f, 0.7f);
        bImg.raycastTarget = true;

        panel = new GameObject("InformePanel");
        panel.transform.SetParent(root, false);
        RectTransform pRt = panel.AddComponent<RectTransform>();
        pRt.anchorMin = Vector2.zero;
        pRt.anchorMax = Vector2.one;
        pRt.sizeDelta = Vector2.zero;

        Image pBg = panel.AddComponent<Image>();
        pBg.color = new Color(0.95f, 0.95f, 0.97f, 1f);

        GameObject headerObj = new GameObject("Header");
        headerObj.transform.SetParent(panel.transform, false);
        RectTransform hRt = headerObj.AddComponent<RectTransform>();
        hRt.anchorMin = new Vector2(0f, 0.92f);
        hRt.anchorMax = new Vector2(1f, 1f);
        hRt.sizeDelta = Vector2.zero;
        Image hBg = headerObj.AddComponent<Image>();
        hBg.color = new Color(0.15f, 0.15f, 0.2f, 1f);

        GameObject hTextObj = new GameObject("HeaderText");
        hTextObj.transform.SetParent(headerObj.transform, false);
        RectTransform hTextRt = hTextObj.AddComponent<RectTransform>();
        hTextRt.anchorMin = Vector2.zero;
        hTextRt.anchorMax = Vector2.one;
        hTextRt.sizeDelta = Vector2.zero;
        Text hText = hTextObj.AddComponent<Text>();
        hText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        hText.fontSize = 18;
        hText.fontStyle = FontStyle.Bold;
        hText.alignment = TextAnchor.MiddleCenter;
        hText.color = Color.white;
        hText.text = "INFORME DEL PROYECTO INTEGRADOR";

        GameObject closeObj = new GameObject("BtnClose");
        closeObj.transform.SetParent(panel.transform, false);
        RectTransform cRt = closeObj.AddComponent<RectTransform>();
        cRt.anchorMin = new Vector2(1f, 1f);
        cRt.anchorMax = new Vector2(1f, 1f);
        cRt.pivot = new Vector2(1f, 1f);
        cRt.sizeDelta = new Vector2(51f, 51f);
        cRt.anchoredPosition = new Vector2(-5f, -5f);
        Image cImg = closeObj.AddComponent<Image>();
        cImg.color = new Color(0.8f, 0.15f, 0.15f, 0.9f);
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

        GameObject scrollObj = new GameObject("ScrollView");
        scrollObj.transform.SetParent(panel.transform, false);
        RectTransform sRt = scrollObj.AddComponent<RectTransform>();
        sRt.anchorMin = new Vector2(0f, 0f);
        sRt.anchorMax = new Vector2(1f, 0.92f);
        sRt.sizeDelta = new Vector2(-20f, -10f);
        sRt.anchoredPosition = new Vector2(10f, 5f);

        ScrollRect scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 30f;
        Image sBg = scrollObj.AddComponent<Image>();
        sBg.color = new Color(1f, 1f, 1f, 1f);

        contentArea = new GameObject("Content");
        contentArea.transform.SetParent(scrollObj.transform, false);
        RectTransform cRt2 = contentArea.AddComponent<RectTransform>();
        cRt2.anchorMin = new Vector2(0f, 1f);
        cRt2.anchorMax = new Vector2(1f, 1f);
        cRt2.pivot = new Vector2(0.5f, 1f);
        cRt2.sizeDelta = new Vector2(0f, 1200f);

        scrollRect.content = cRt2;
        scrollRect.viewport = sRt;

        VerticalLayoutGroup vlg = contentArea.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 6;
        vlg.padding = new RectOffset(20, 20, 15, 15);
        ContentSizeFitter csf = contentArea.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        CreateDocumentContent();
        CreatePopup();
        CreateGlobalAlert();
        InitErrors();
        isCooldown = false;
        timerCooldown = 0f;
        timerDanger = 0f;
    }

    void CreateDocumentContent()
    {
        string[] paragraphs = new string[] {
            "Proyecto Integrador: Sistema Integral de Gestion Empresarial",
            "Objetivo General:",
            "Desarrollar un sistema de gestion empresarial que permita optimizar los procesos administrativos de una empresa mediana, con el fin de mejorar la eficencia operativa y garantizar la calidad del servicio prestado.",
            "Objetivos Especificos:",
            "1. Analizar los procesos actuales de la empresa para identificar areas de mejora.",
            "2. Disenar una arquitectura de software que soporte los requerimientos del sistema.",
            "3. Implementar los modulos funcionales del sistema de gestion.",
            "4. Evaluar el rendimiento del sistema mediante pruebas de usario.",
            "Formulacion del Problema:",
            "Como se puede mejorar la gestion empresarial a traves de un sistema informatico que integre los procesos clave de una organizacion? La respuesta a esta pregunta implica el desarrollo de una solucion tecnologica que aborde las necesadades especificas de la empresa."
        };

        for (int p = 0; p < paragraphs.Length; p++)
        {
            int paragraphIndex = p;
            GameObject parObj = new GameObject("Paragraph_" + p);
            parObj.transform.SetParent(contentArea.transform, false);
            Text parText = parObj.AddComponent<Text>();
            parText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            parText.fontSize = (p == 0 || p == 1 || p == 3 || p == 8) ? 16 : 14;
            parText.fontStyle = (p == 0 || p == 1 || p == 3 || p == 8) ? FontStyle.Bold : FontStyle.Normal;
            parText.color = new Color(0.1f, 0.1f, 0.12f);
            parText.text = paragraphs[p];
            parText.supportRichText = true;

            GameObject clickArea = new GameObject("ClickArea");
            clickArea.transform.SetParent(parObj.transform, false);
            RectTransform caRt = clickArea.AddComponent<RectTransform>();
            caRt.anchorMin = Vector2.zero;
            caRt.anchorMax = Vector2.one;
            caRt.sizeDelta = Vector2.zero;
            Image caImg = clickArea.AddComponent<Image>();
            caImg.color = new Color(0f, 0f, 0f, 0f);
            caImg.raycastTarget = true;
            Button caBtn = clickArea.AddComponent<Button>();
            caBtn.targetGraphic = caImg;
            ColorBlock cb2 = new ColorBlock();
            cb2.normalColor = new Color(0f, 0f, 0f, 0f);
            cb2.highlightedColor = new Color(1f, 1f, 0f, 0.08f);
            cb2.pressedColor = new Color(1f, 1f, 0f, 0.15f);
            cb2.disabledColor = new Color(0f, 0f, 0f, 0f);
            cb2.colorMultiplier = 1f;
            cb2.fadeDuration = 0f;
            caBtn.colors = cb2;
            caBtn.onClick.AddListener(() => OnParagraphClick(paragraphIndex));

            LayoutElement le = parObj.AddComponent<LayoutElement>();
            le.preferredHeight = -1f;
            le.flexibleWidth = 1f;
        }
    }

    void CreatePopup()
    {
        popupPanel = new GameObject("ErrorPopup");
        popupPanel.transform.SetParent(panel.transform, false);
        popupPanel.SetActive(false);
        RectTransform ppRt = popupPanel.AddComponent<RectTransform>();
        ppRt.anchorMin = new Vector2(0.3f, 0.35f);
        ppRt.anchorMax = new Vector2(0.7f, 0.65f);
        ppRt.sizeDelta = Vector2.zero;
        Image ppBg = popupPanel.AddComponent<Image>();
        ppBg.color = new Color(0.18f, 0.18f, 0.22f, 0.97f);

        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(popupPanel.transform, false);
        RectTransform bRt = borderObj.AddComponent<RectTransform>();
        bRt.anchorMin = Vector2.zero;
        bRt.anchorMax = Vector2.one;
        bRt.sizeDelta = Vector2.zero;
        Image bImg = borderObj.AddComponent<Image>();
        bImg.color = new Color(0.35f, 0.35f, 0.45f, 1f);
        bImg.raycastTarget = false;

        GameObject qObj = new GameObject("Question");
        qObj.transform.SetParent(popupPanel.transform, false);
        RectTransform qRt = qObj.AddComponent<RectTransform>();
        qRt.anchorMin = new Vector2(0.05f, 0.6f);
        qRt.anchorMax = new Vector2(0.95f, 0.95f);
        qRt.sizeDelta = Vector2.zero;
        Text qText = qObj.AddComponent<Text>();
        qText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        qText.fontSize = 14;
        qText.fontStyle = FontStyle.Bold;
        qText.alignment = TextAnchor.MiddleCenter;
        qText.color = new Color(1f, 0.85f, 0.3f);
        qText.text = "Corrige el error:";

        for (int i = 0; i < 2; i++)
        {
            int idx = i;
            GameObject optObj = new GameObject("Option" + i);
            optObj.transform.SetParent(popupPanel.transform, false);
            RectTransform oRt = optObj.AddComponent<RectTransform>();
            oRt.anchorMin = new Vector2(0.08f, 0.05f + i * 0.22f);
            oRt.anchorMax = new Vector2(0.92f, 0.05f + i * 0.22f + 0.18f);
            oRt.sizeDelta = Vector2.zero;
            Image oImg = optObj.AddComponent<Image>();
            oImg.color = new Color(0.28f, 0.28f, 0.35f, 1f);
            Button oBtn = optObj.AddComponent<Button>();
            oBtn.targetGraphic = oImg;
            ColorBlock cb = new ColorBlock();
            cb.normalColor = new Color(0.28f, 0.28f, 0.35f, 1f);
            cb.highlightedColor = new Color(0.4f, 0.45f, 0.55f, 1f);
            cb.pressedColor = new Color(0.2f, 0.2f, 0.25f, 1f);
            cb.disabledColor = new Color(0.28f, 0.28f, 0.35f, 0.5f);
            cb.colorMultiplier = 1f;
            cb.fadeDuration = 0.1f;
            oBtn.colors = cb;

            GameObject oTextObj = new GameObject("Text");
            oTextObj.transform.SetParent(optObj.transform, false);
            RectTransform oTextRt = oTextObj.AddComponent<RectTransform>();
            oTextRt.anchorMin = Vector2.zero;
            oTextRt.anchorMax = Vector2.one;
            oTextRt.sizeDelta = Vector2.zero;
            Text oText = oTextObj.AddComponent<Text>();
            oText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            oText.fontSize = 13;
            oText.alignment = TextAnchor.MiddleCenter;
            oText.color = Color.white;

            int optionIndex = i;
            oBtn.onClick.AddListener(() => OnOptionSelected(optionIndex));
        }
    }

    void InitErrors()
    {
        errors = new List<ErrorDef>();

        AddError(0, "Gestion", "Administracion", "Direccion");
        AddError(2, "optimizar", "automatizar", "implementar");
        AddError(2, "eficencia", "eficiencia", "eficacia");
        AddError(2, "garantizar", "asegurar", "descuidar");
        AddError(3, "Especificos", "Particulares", "Generales");
        AddError(4, "areas", "sectores", "zonas");
        AddError(6, "modulos", "componentes", "modales");
        AddError(7, "usario", "usuario", "usador");
        AddError(9, "mejorar", "potenciar", "empeorar");
        AddError(9, "informatico", "digital", "manual");
        AddError(9, "necesadades", "necesidades", "necesariades");
        AddError(9, "organizacion", "empresa", "institucion");
        AddError(9, "implica", "conlleva", "elimina");
        AddError(9, "desarrollo", "implementacion", "eliminacion");
        AddError(4, "identificar", "detectar", "ocultar");
    }

    void AddError(int paragraph, string wrong, string correct, string wrongOpt)
    {
        ErrorDef e = new ErrorDef();
        e.id = "err_" + errors.Count;
        e.wrongText = wrong;
        e.correctText = correct;
        e.wrongOption = wrongOpt;
        e.paragraphIndex = paragraph;
        e.isActive = false;
        e.isFixed = false;
        errors.Add(e);
    }

    void Update()
    {
        if (taskCompleted || TaskNotesController.informeFixed) return;
        if (!firstErrorFixed) return;

        if (isCooldown)
        {
            timerCooldown -= Time.deltaTime;
            if (timerCooldown <= 0f)
            {
                isCooldown = false;
                PickNextError();
                if (!taskCompleted)
                {
                    timerDanger = DANGER_TIME;
                    ShowGlobalAlert(true);
                    PlayWarningMusic();
                }
            }
        }
        else if (currentActiveErrorIndex >= 0)
        {
            timerDanger -= Time.deltaTime;
            if (timerDanger <= 0f && !errors[currentActiveErrorIndex].isFixed)
            {
                ShowGlobalAlert(false);
                StopWarningMusic();
                StartCoroutine(TimeoutGameOver());
            }
        }
        else if (currentActiveErrorIndex < 0 && !taskCompleted)
        {
            PickNextError();
            if (!taskCompleted)
            {
                timerDanger = DANGER_TIME;
                ShowGlobalAlert(true);
                PlayWarningMusic();
            }
        }
    }

    void PlayWarningMusic()
    {
        if (warningSource == null)
        {
            warningSource = gameObject.AddComponent<AudioSource>();
            warningSource.loop = true;
            warningSource.volume = 0.4f;
        }
        if (warningClip == null)
            StartCoroutine(LoadWarningClip());
        else if (warningSource != null && !warningSource.isPlaying)
        {
            warningSource.clip = warningClip;
            warningSource.Play();
        }
    }

    void StopWarningMusic()
    {
        if (warningSource != null && warningSource.isPlaying)
            warningSource.Stop();
    }

    IEnumerator LoadWarningClip()
    {
        string path = "file:///" + Application.streamingAssetsPath + "/Audio/warning.wav";
        using (WWW www = new WWW(path))
        {
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                warningClip = www.GetAudioClip(false, false, AudioType.WAV);
                if (warningClip != null && warningSource != null)
                {
                    warningSource.clip = warningClip;
                    warningSource.Play();
                }
            }
        }
    }

    void CreateGlobalAlert()
    {
        Transform root = transform.parent;

        alertOverlay = new GameObject("InformeAlert");
        alertOverlay.transform.SetParent(root, false);
        alertOverlay.SetActive(false);

        RectTransform rt = alertOverlay.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        Image img = alertOverlay.AddComponent<Image>();
        img.color = new Color(1f, 0f, 0f, 0.12f);
        img.raycastTarget = false;
    }

    void ShowGlobalAlert(bool show)
    {
        if (alertOverlay == null) CreateGlobalAlert();
        alertOverlay.SetActive(show);
    }

    IEnumerator TimeoutGameOver()
    {
        IsOpen = false;
        StopWarningMusic();
        if (panel != null) panel.SetActive(false);
        if (blocker != null) blocker.SetActive(false);

        GameObject bloodFilter = GameObject.Find("BloodFilter");
        if (bloodFilter != null) bloodFilter.SetActive(true);

        AudioManager am = FindObjectOfType<AudioManager>();
        if (am != null) am.PlayScream();

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 3; i++)
        {
            GameOverController.LoseLife();
            if (GameOverController.isGameOver) yield break;
            yield return new WaitForSeconds(0.3f);
        }
    }

    void PickNextError()
    {
        if (currentActiveErrorIndex >= 0)
        {
            var oldErr = errors[currentActiveErrorIndex];
            oldErr.isActive = false;
            errors[currentActiveErrorIndex] = oldErr;
            ClearHighlight(currentActiveErrorIndex);
            currentActiveErrorIndex = -1;
        }

        List<int> candidates = new List<int>();
        for (int i = 0; i < errors.Count; i++)
        {
            if (!errors[i].isFixed)
                candidates.Add(i);
        }

        if (candidates.Count == 0)
        {
            taskCompleted = true;
            TaskNotesController.informeFixed = true;
            ShowCompletedState();
            return;
        }

        int pick = candidates[Random.Range(0, candidates.Count)];
        currentActiveErrorIndex = pick;
        var err = errors[pick];
        err.isActive = true;
        errors[pick] = err;
        HighlightError(pick);
    }

    void ClearHighlight(int index)
    {
        if (index < 0 || index >= errors.Count) return;
        var err = errors[index];
        GameObject parObj = contentArea.transform.Find("Paragraph_" + err.paragraphIndex)?.gameObject;
        if (parObj == null) return;
        Text parText = parObj.GetComponent<Text>();
        if (parText == null) return;
        string t = parText.text;
        t = t.Replace("<color=#FF0000>" + err.wrongText + "</color>", err.wrongText);
        t = t.Replace("<color=#00AA00>" + err.correctText + "</color>", err.wrongText);
        parText.text = t;
    }

    void HighlightError(int index)
    {
        if (index < 0 || index >= errors.Count) return;
        var err = errors[index];
        GameObject parObj = contentArea.transform.Find("Paragraph_" + err.paragraphIndex)?.gameObject;
        if (parObj == null) return;

        Text parText = parObj.GetComponent<Text>();
        string original = parText.text;

        string cleanText = original.Replace("<color=#FF0000>", "").Replace("</color>", "").Replace("<color=#00AA00>", "");

        int idx = cleanText.IndexOf(err.wrongText);
        if (idx < 0) return;

        string before = cleanText.Substring(0, idx);
        string after = cleanText.Substring(idx + err.wrongText.Length);

        parText.text = before + "<color=#FF0000>" + err.wrongText + "</color>" + after;
    }

    void OnParagraphClick(int paragraphIndex)
    {
        if (popupPanel.activeSelf) return;
        if (currentActiveErrorIndex < 0) return;

        if (errors[currentActiveErrorIndex].paragraphIndex == paragraphIndex)
        {
            currentPopupIndex = currentActiveErrorIndex;
            ShowPopup(currentActiveErrorIndex);
        }
    }

    void ShowPopup(int index)
    {
        var err = errors[index];
        popupPanel.SetActive(true);
        popupPanel.transform.SetAsLastSibling();

        Text qText = popupPanel.transform.Find("Question")?.GetComponent<Text>();
        if (qText != null)
            qText.text = "Corrige: \"" + err.wrongText + "\"\nCual es la opcion correcta?";

        Text opt0 = popupPanel.transform.Find("Option0/Text")?.GetComponent<Text>();
        Text opt1 = popupPanel.transform.Find("Option1/Text")?.GetComponent<Text>();
        if (opt0 != null) opt0.text = "A) " + err.correctText;
        if (opt1 != null) opt1.text = "B) " + err.wrongOption;
    }

    void OnOptionSelected(int option)
    {
        if (!popupPanel.activeSelf || currentPopupIndex < 0) return;
        popupPanel.SetActive(false);

        var err = errors[currentPopupIndex];

        if (option == 0)
        {
            CorrectError(currentPopupIndex);
        }
        else
        {
            PCTimer timer = FindObjectOfType<PCTimer>();
            if (timer != null) timer.AddPenalty(-15f);
        }

        currentPopupIndex = -1;
    }

    void CorrectError(int index)
    {
        if (index < 0 || index >= errors.Count) return;
        var err = errors[index];
        err.isFixed = true;
        err.isActive = false;
        errors[index] = err;

        currentActiveErrorIndex = -1;

        TaskNotesController.documentFixed = true;

        if (!firstErrorFixed)
        {
            firstErrorFixed = true;
            PCTimer.AddBonusTime(2.5f);
        }

        GameObject parObj = contentArea.transform.Find("Paragraph_" + err.paragraphIndex)?.gameObject;
        if (parObj != null)
        {
            Text parText = parObj.GetComponent<Text>();
            if (parText != null)
            {
                string t = parText.text;
                t = t.Replace("<color=#FF0000>" + err.wrongText + "</color>", "<color=#00AA00>" + err.correctText + "</color>");
                parText.text = t;
            }
        }

        isCooldown = true;
        timerCooldown = COOLDOWN_TIME;
        if (PCWindowController.IsModuleCompleted(3))
            timerCooldown = COOLDOWN_TIME / 2f;
        StopWarningMusic();
        ShowGlobalAlert(false);
    }

    void ShowCompletedState()
    {
        GameObject msgObj = new GameObject("CompleteMsg");
        msgObj.transform.SetParent(panel.transform, false);
        msgObj.transform.SetAsLastSibling();
        RectTransform mRt = msgObj.AddComponent<RectTransform>();
        mRt.anchorMin = new Vector2(0.2f, 0.4f);
        mRt.anchorMax = new Vector2(0.8f, 0.6f);
        mRt.sizeDelta = Vector2.zero;
        Image mBg = msgObj.AddComponent<Image>();
        mBg.color = new Color(0f, 0.5f, 0f, 0.9f);

        GameObject mTextObj = new GameObject("Text");
        mTextObj.transform.SetParent(msgObj.transform, false);
        RectTransform mTextRt = mTextObj.AddComponent<RectTransform>();
        mTextRt.anchorMin = Vector2.zero;
        mTextRt.anchorMax = Vector2.one;
        mTextRt.sizeDelta = Vector2.zero;
        Text mText = mTextObj.AddComponent<Text>();
        mText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        mText.fontSize = 22;
        mText.fontStyle = FontStyle.Bold;
        mText.alignment = TextAnchor.MiddleCenter;
        mText.color = Color.white;
        mText.text = "INFORME COMPLETADO";
    }

    public void Close()
    {
        IsOpen = false;
        if (panel != null) panel.SetActive(false);
        if (blocker != null) blocker.SetActive(false);
        if (OnClose != null) OnClose();
    }

    void OnDestroy()
    {
        if (panel != null) Destroy(panel);
        if (blocker != null) Destroy(blocker);
        if (popupPanel != null) Destroy(popupPanel);
        if (alertOverlay != null) Destroy(alertOverlay);
        if (instance == this) instance = null;
    }
}