using UnityEngine;
using UnityEngine.UI;

public class TaskNotesController : MonoBehaviour
{
    public static bool wifiFixed = false;
    public static bool documentFixed = false;
    public static bool pc4Fixed = false;

    private GameObject panel;
    private Text taskListText;

    void Awake()
    {
        CreatePanel();
        panel.SetActive(false);
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

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panel.transform, false);
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
        listObj.transform.SetParent(panel.transform, false);
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

        GameObject closeBtn = new GameObject("CloseBtn");
        closeBtn.transform.SetParent(panel.transform, false);
        Image btnImg = closeBtn.AddComponent<Image>();
        btnImg.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        Button btn = closeBtn.AddComponent<Button>();
        btn.onClick.AddListener(Close);
        RectTransform bRt = closeBtn.GetComponent<RectTransform>();
        bRt.anchorMin = new Vector2(1f, 1f);
        bRt.anchorMax = new Vector2(1f, 1f);
        bRt.pivot = new Vector2(1f, 1f);
        bRt.sizeDelta = new Vector2(30f, 30f);
        bRt.anchoredPosition = new Vector2(-5f, -5f);
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
        xRt.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        if (panel != null && panel.activeSelf)
            UpdateTaskList();
    }

    public void Show()
    {
        UpdateTaskList();
        panel.SetActive(true);
    }

    public void Close()
    {
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

        taskListText.text = s;
    }
}
