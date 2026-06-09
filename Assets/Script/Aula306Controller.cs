using UnityEngine;
using UnityEngine.UI;

public class Aula306Controller : MonoBehaviour
{
    public static bool IsOpen { get; private set; }
    private static Aula306Controller instance;
    private GameObject panel;
    private GameObject blocker;
    private static bool persistentCreated = false;
    private static Transform canvasRoot;

    public static void Show()
    {
        if (instance == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null) return;
            canvasRoot = canvas.transform;
            GameObject go = new GameObject("Aula306Controller");
            go.transform.SetParent(canvas.transform, false);
            instance = go.AddComponent<Aula306Controller>();
        }
        instance.CreatePanel();
    }

    void CreatePanel()
    {
        IsOpen = true;
        Transform root = transform.parent;

        blocker = new GameObject("Aula306Blocker");
        blocker.transform.SetParent(root, false);
        RectTransform bRt = blocker.AddComponent<RectTransform>();
        bRt.anchorMin = Vector2.zero;
        bRt.anchorMax = Vector2.one;
        bRt.sizeDelta = Vector2.zero;
        Image bImg = blocker.AddComponent<Image>();
        bImg.color = new Color(0f, 0f, 0f, 0.8f);
        bImg.raycastTarget = true;

        panel = new GameObject("Aula306Panel");
        panel.transform.SetParent(root, false);
        panel.transform.SetAsLastSibling();
        RectTransform pRt = panel.AddComponent<RectTransform>();
        pRt.anchorMin = Vector2.zero;
        pRt.anchorMax = Vector2.one;
        pRt.sizeDelta = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        Sprite bgSprite = null;
        string imgPath = Application.dataPath + "/Texture/aula306.png";
        if (System.IO.File.Exists(imgPath))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(imgPath);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(bytes))
                bgSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
        if (bgSprite != null)
        {
            bg.sprite = bgSprite;
            bg.type = Image.Type.Simple;
            bg.preserveAspect = true;
        }
        else
            bg.color = new Color(0.1f, 0.05f, 0.15f, 1f);
        bg.raycastTarget = true;

        GameObject textObj = new GameObject("TitleText");
        textObj.transform.SetParent(panel.transform, false);
        RectTransform tRt = textObj.AddComponent<RectTransform>();
        tRt.anchorMin = new Vector2(0f, 0f);
        tRt.anchorMax = new Vector2(0f, 0f);
        tRt.pivot = new Vector2(0.5f, 0.5f);
        tRt.anchoredPosition = new Vector2(Screen.width / 2f, 80f);
        tRt.sizeDelta = new Vector2(300, 50);
        Text title = textObj.AddComponent<Text>();
        title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        title.fontSize = 36;
        title.fontStyle = FontStyle.Bold;
        title.alignment = TextAnchor.MiddleCenter;
        title.color = new Color(1f, 0.8f, 0.2f);
        title.text = "AULA 306";

        GameObject closeObj = new GameObject("BtnClose");
        closeObj.transform.SetParent(panel.transform, false);
        RectTransform cRt = closeObj.AddComponent<RectTransform>();
        cRt.anchorMin = new Vector2(0f, 0f);
        cRt.anchorMax = new Vector2(0f, 0f);
        cRt.pivot = new Vector2(0.5f, 0.5f);
        cRt.anchoredPosition = new Vector2(50, 50);
        cRt.sizeDelta = new Vector2(40, 40);
        Image cImg = closeObj.AddComponent<Image>();
        cImg.color = new Color(0.8f, 0.15f, 0.15f, 0.85f);
        Button cBtn = closeObj.AddComponent<Button>();
        cBtn.targetGraphic = cImg;
        cBtn.onClick.AddListener(() => Close());

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
