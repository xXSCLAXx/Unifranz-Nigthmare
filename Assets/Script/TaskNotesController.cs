using UnityEngine;
using UnityEngine.UI;

public class TaskNotesController : MonoBehaviour
{
    public static bool IsOpen { get; private set; }
    public static bool wifiFixed = false;
    public static bool documentFixed = false;
    public static bool pc4Fixed = false;
    public static bool informeFixed = false;

    private GameObject blocker;
    private GameObject panel;
    private GameObject frontSide;
    private GameObject backSide;
    private Text taskListText;
    private bool showingBack = false;

    void Awake()
    {
        CreateBlocker();
        CreatePanel();
        panel.SetActive(false);
        blocker.SetActive(false);
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
        bg.color = new Color(0f, 0f, 0f, 0.35f);
        bg.raycastTarget = true;
    }

    void CreatePanel()
    {
        panel = new GameObject("NotesWindow");
        panel.transform.SetParent(transform, false);

        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(420, 520);
        rt.anchoredPosition = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(1f, 0.95f, 0.5f, 0.95f);

        CreateFrontSide();
        CreateBackSide();
        CreateFlipButton();

        frontSide.SetActive(true);
        backSide.SetActive(false);
    }

    void CreateFrontSide()
    {
        frontSide = new GameObject("FrontSide");
        frontSide.transform.SetParent(panel.transform, false);
        RectTransform fRt = frontSide.AddComponent<RectTransform>();
        fRt.anchorMin = Vector2.zero;
        fRt.anchorMax = Vector2.one;
        fRt.sizeDelta = Vector2.zero;

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(frontSide.transform, false);
        Text title = titleObj.AddComponent<Text>();
        title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        title.fontSize = 22;
        title.fontStyle = FontStyle.Bold;
        title.color = new Color(0.3f, 0.15f, 0f);
        title.text = "Noche 1 - Tareas";
        title.alignment = TextAnchor.UpperCenter;
        RectTransform tRt = titleObj.GetComponent<RectTransform>();
        tRt.anchorMin = new Vector2(0f, 0.85f);
        tRt.anchorMax = new Vector2(1f, 1f);
        tRt.sizeDelta = Vector2.zero;
        tRt.anchoredPosition = Vector2.zero;

        GameObject listObj = new GameObject("TaskList");
        listObj.transform.SetParent(frontSide.transform, false);
        taskListText = listObj.AddComponent<Text>();
        taskListText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        taskListText.fontSize = 14;
        taskListText.color = new Color(0.3f, 0.15f, 0f);
        taskListText.alignment = TextAnchor.UpperLeft;
        taskListText.supportRichText = true;
        RectTransform lRt = listObj.GetComponent<RectTransform>();
        lRt.anchorMin = new Vector2(0.05f, 0.05f);
        lRt.anchorMax = new Vector2(0.95f, 0.8f);
        lRt.sizeDelta = Vector2.zero;
        lRt.anchoredPosition = Vector2.zero;

        GameObject closeBtn = CreateCloseButton(frontSide.transform);
        closeBtn.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
        closeBtn.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
        closeBtn.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
        closeBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-5f, -5f);
    }

    void CreateBackSide()
    {
        backSide = new GameObject("BackSide");
        backSide.transform.SetParent(panel.transform, false);
        RectTransform bRt = backSide.AddComponent<RectTransform>();
        bRt.anchorMin = Vector2.zero;
        bRt.anchorMax = Vector2.one;
        bRt.sizeDelta = Vector2.zero;

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(backSide.transform, false);
        Text title = titleObj.AddComponent<Text>();
        title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        title.fontSize = 18;
        title.fontStyle = FontStyle.Bold;
        title.color = new Color(0.3f, 0.15f, 0f);
        title.text = "Secundarias";
        title.alignment = TextAnchor.MiddleCenter;
        RectTransform tRt = titleObj.GetComponent<RectTransform>();
        tRt.anchorMin = new Vector2(0f, 0.88f);
        tRt.anchorMax = new Vector2(1f, 0.96f);
        tRt.sizeDelta = Vector2.zero;

        GameObject infoObj = new GameObject("InfoText");
        infoObj.transform.SetParent(backSide.transform, false);
        Text info = infoObj.AddComponent<Text>();
        info.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        info.fontSize = 13;
        info.color = new Color(0.3f, 0.15f, 0f);
        info.alignment = TextAnchor.UpperLeft;
        info.supportRichText = true;
        info.text = "<b>WiFi:</b>\n"
            + "  - 9% de romperse al navegar\n"
            + "  - 15s para reiniciarlo\n"
            + "  - Si falla: screamer\n"
            + "  - Recompensa: +1s al timer\n\n"
            + "<b>PC4 (Sala PCs):</b>\n"
            + "  - 12% de romperse al navegar\n"
            + "  - 20s para arreglarla\n"
            + "  - Si falla: screamer\n"
            + "  - Recompensa: +2s al timer\n\n"
            + "<b>Documento:</b>\n"
            + "  - Sin riesgo\n"
            + "  - Solo corregir errores\n\n"
            + "<b>Informe:</b>\n"
            + "  - 1 error cada 75s\n"
            + "  - Elegir opci�n correcta\n"
            + "  - Si fallas: -15s al timer\n"
            + "  - Recompensa: +2.5s al timer";
        RectTransform iRt = infoObj.GetComponent<RectTransform>();
        iRt.anchorMin = new Vector2(0.05f, 0.05f);
        iRt.anchorMax = new Vector2(0.95f, 0.83f);
        iRt.sizeDelta = Vector2.zero;

        GameObject closeBtn = CreateCloseButton(backSide.transform);
        closeBtn.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
        closeBtn.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
        closeBtn.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
        closeBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-5f, -5f);
    }

    GameObject CreateCloseButton(Transform parent)
    {
        GameObject closeBtn = new GameObject("CloseBtn");
        closeBtn.transform.SetParent(parent, false);
        Image btnImg = closeBtn.AddComponent<Image>();
        btnImg.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        Button btn = closeBtn.AddComponent<Button>();
        btn.onClick.AddListener(Close);
        RectTransform bRt = closeBtn.GetComponent<RectTransform>();
        bRt.sizeDelta = new Vector2(30f, 30f);

        GameObject xTxt = new GameObject("XText");
        xTxt.transform.SetParent(closeBtn.transform, false);
        Text xLabel = xTxt.AddComponent<Text>();
        xLabel.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        xLabel.fontSize = 16;
        xLabel.alignment = TextAnchor.MiddleCenter;
        xLabel.color = Color.white;
        xLabel.text = "X";
        RectTransform xRt = xTxt.GetComponent<RectTransform>();
        xRt.anchorMin = Vector2.zero;
        xRt.anchorMax = Vector2.one;
        xRt.sizeDelta = Vector2.zero;

        return closeBtn;
    }

    void CreateFlipButton()
    {
        GameObject flipBtn = new GameObject("FlipBtn");
        flipBtn.transform.SetParent(panel.transform, false);
        RectTransform fRt = flipBtn.AddComponent<RectTransform>();
        fRt.anchorMin = new Vector2(0f, 0f);
        fRt.anchorMax = new Vector2(0f, 0f);
        fRt.pivot = new Vector2(0.5f, 0.5f);
        fRt.sizeDelta = new Vector2(50f, 26f);
        fRt.anchoredPosition = new Vector2(30f, 15f);
        Image fImg = flipBtn.AddComponent<Image>();
        fImg.color = new Color(0.6f, 0.4f, 0.1f, 0.8f);
        Button fBtn = flipBtn.AddComponent<Button>();
        fBtn.targetGraphic = fImg;
        fBtn.onClick.AddListener(ToggleFlip);

        GameObject fTextObj = new GameObject("Text");
        fTextObj.transform.SetParent(flipBtn.transform, false);
        RectTransform fTextRt = fTextObj.AddComponent<RectTransform>();
        fTextRt.anchorMin = Vector2.zero;
        fTextRt.anchorMax = Vector2.one;
        fTextRt.sizeDelta = Vector2.zero;
        Text fText = fTextObj.AddComponent<Text>();
        fText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        fText.fontSize = 12;
        fText.fontStyle = FontStyle.Bold;
        fText.alignment = TextAnchor.MiddleCenter;
        fText.color = Color.white;
        fText.text = "VOLTEAR";
    }

    void ToggleFlip()
    {
        showingBack = !showingBack;
        frontSide.SetActive(!showingBack);
        backSide.SetActive(showingBack);
    }

    void Update()
    {
        if (panel != null && panel.activeSelf && frontSide.activeSelf)
            UpdateTaskList();
    }

    public void Show()
    {
        IsOpen = true;
        showingBack = false;
        frontSide.SetActive(true);
        backSide.SetActive(false);
        blocker.SetActive(true);
        UpdateTaskList();
        panel.SetActive(true);
    }

    public void Close()
    {
        IsOpen = false;
        blocker.SetActive(false);
        panel.SetActive(false);
    }

    void UpdateTaskList()
    {
        string s = "";

        int count = PCWindowController.GetModuleCount();
        s += "<b>Modulos del proyecto:</b>\n";
        for (int i = 0; i < count; i++)
        {
            string icon = PCWindowController.IsModuleCompleted(i) ? "[X]" : "[ ]";
            s += "  " + icon + " " + PCWindowController.GetModuleName(i) + "\n";
        }

        s += "\n<b>Tareas extra:</b>\n";
        s += "  " + (wifiFixed ? "[X]" : "[ ]") + " Reinicia el WiFi\n";
        s += "  " + (documentFixed ? "[X]" : "[ ]") + " Corrige el documento\n";
        s += "  " + (pc4Fixed ? "[X]" : "[ ]") + " Arregla la conexion de la PC 4\n";
        s += "  " + (informeFixed ? "[X]" : "[ ]") + " Corrige el informe\n";

        taskListText.text = s;
    }
}
