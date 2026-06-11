using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WireMinigameController : MonoBehaviour
{
    public static bool IsOpen { get; private set; }
    private static WireMinigameController instance;
    private GameObject panel;
    private System.Action onComplete;
    private List<WireSlot> leftWires = new List<WireSlot>();
    private List<WireSlot> rightWires = new List<WireSlot>();
    private WireSlot selectedLeft = null;
    private int matchedCount = 0;
    private bool locked = false;

    class WireSlot
    {
        public GameObject obj;
        public Image img;
        public Button btn;
        public Color color;
        public bool matched;
    }

    public static void Show(System.Action onDone)
    {
        if (instance != null) return;
        IsOpen = true;
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        GameObject go = new GameObject("WireMinigameController");
        go.transform.SetParent(canvas.transform, false);
        instance = go.AddComponent<WireMinigameController>();
        instance.onComplete = onDone;
        instance.CreatePanel();
    }

    void CreatePanel()
    {
        Transform root = transform.parent;

        panel = new GameObject("WireMinigamePanel");
        panel.transform.SetParent(root, false);
        panel.transform.SetAsLastSibling();
        RectTransform pRt = panel.AddComponent<RectTransform>();
        pRt.anchorMin = new Vector2(0.2f, 0.15f);
        pRt.anchorMax = new Vector2(0.8f, 0.85f);
        pRt.sizeDelta = Vector2.zero;
        Image pBg = panel.AddComponent<Image>();
        pBg.color = new Color(0.08f, 0.08f, 0.1f, 0.97f);

        GameObject title = new GameObject("Title");
        title.transform.SetParent(panel.transform, false);
        RectTransform tRt = title.AddComponent<RectTransform>();
        tRt.anchorMin = new Vector2(0f, 0.85f);
        tRt.anchorMax = new Vector2(1f, 0.95f);
        tRt.sizeDelta = Vector2.zero;
        Text tText = title.AddComponent<Text>();
        tText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        tText.fontSize = 20;
        tText.fontStyle = FontStyle.Bold;
        tText.alignment = TextAnchor.MiddleCenter;
        tText.color = new Color(1f, 0.8f, 0.2f);
        tText.text = "CONECTA LOS CABLES DEL MISMO COLOR";

        Color[] colors = { Color.red, new Color(0.2f, 0.6f, 1f), Color.yellow, Color.white };
        int[] leftOrder = { 0, 1, 2, 3 };
        int[] rightOrder = { 0, 1, 2, 3 };
        Shuffle(leftOrder);
        Shuffle(rightOrder);

        for (int i = 0; i < 4; i++)
        {
            int idx = leftOrder[i];
            float y = 0.65f - i * 0.16f;
            CreateWireSlot(panel.transform, true, colors[idx], new Vector2(0.05f, y), idx);
        }

        for (int i = 0; i < 4; i++)
        {
            int idx = rightOrder[i];
            float y = 0.65f - i * 0.16f;
            CreateWireSlot(panel.transform, false, colors[idx], new Vector2(0.75f, y), idx);
        }
    }

    void CreateWireSlot(Transform parent, bool isLeft, Color color, Vector2 anchor, int colorIndex)
    {
        GameObject obj = new GameObject((isLeft ? "L" : "R") + colorIndex);
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(anchor.x, anchor.y - 0.06f);
        rt.anchorMax = new Vector2(anchor.x + 0.2f, anchor.y + 0.06f);
        rt.sizeDelta = Vector2.zero;

        Image img = obj.AddComponent<Image>();
        img.color = DimColor(color);

        Button btn = obj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(() => OnWireClicked(isLeft, colorIndex, obj, img, btn));

        GameObject lbl = new GameObject("Label");
        lbl.transform.SetParent(obj.transform, false);
        RectTransform lRt = lbl.AddComponent<RectTransform>();
        lRt.anchorMin = Vector2.zero;
        lRt.anchorMax = Vector2.one;
        lRt.sizeDelta = Vector2.zero;
        Text lText = lbl.AddComponent<Text>();
        lText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        lText.fontSize = 14;
        lText.fontStyle = FontStyle.Bold;
        lText.alignment = TextAnchor.MiddleCenter;
        lText.color = Color.white;
        lText.text = isLeft ? ((char)('A' + colorIndex)).ToString() : (colorIndex + 1).ToString();

        WireSlot slot = new WireSlot { obj = obj, img = img, btn = btn, color = color, matched = false };
        if (isLeft) leftWires.Add(slot);
        else rightWires.Add(slot);
    }

    void OnWireClicked(bool isLeft, int colorIndex, GameObject obj, Image img, Button btn)
    {
        if (locked) return;

        if (isLeft)
        {
            if (selectedLeft != null)
            {
                selectedLeft.img.color = DimColor(selectedLeft.color);
            }
            selectedLeft = leftWires.Find(s => s.obj == obj);
            if (selectedLeft != null && !selectedLeft.matched)
                selectedLeft.img.color = Color.Lerp(selectedLeft.color, Color.white, 0.4f);
        }
        else
        {
            if (selectedLeft == null) return;
            WireSlot rightWire = rightWires.Find(s => s.obj == obj);
            if (rightWire == null || rightWire.matched) return;

            if (selectedLeft.color == rightWire.color)
            {
                locked = true;
                selectedLeft.img.color = selectedLeft.color;
                rightWire.img.color = rightWire.color;
                selectedLeft.matched = true;
                rightWire.matched = true;
                selectedLeft.btn.interactable = false;
                rightWire.btn.interactable = false;
                DrawLine(selectedLeft.obj.GetComponent<RectTransform>(), rightWire.obj.GetComponent<RectTransform>(), selectedLeft.color);
                matchedCount++;
                selectedLeft = null;
                locked = false;

                if (matchedCount >= 4)
                {
                    Invoke("Complete", 0.5f);
                }
            }
            else
            {
                locked = true;
                selectedLeft.img.color = Color.red;
                rightWire.img.color = Color.red;
                selectedLeft = null;
                Invoke("ResetWrongColors", 0.4f);
            }
        }
    }

    void DrawLine(RectTransform from, RectTransform to, Color color)
    {
        GameObject lineObj = new GameObject("CableLine");
        lineObj.transform.SetParent(panel.transform, false);

        RectTransform lineRt = lineObj.AddComponent<RectTransform>();

        Vector3[] fromCorners = new Vector3[4];
        Vector3[] toCorners = new Vector3[4];
        from.GetWorldCorners(fromCorners);
        to.GetWorldCorners(toCorners);

        Vector3 startPos = (fromCorners[2] + fromCorners[3]) * 0.5f;
        Vector3 endPos = (toCorners[0] + toCorners[1]) * 0.5f;
        Vector3 mid = (startPos + endPos) * 0.5f;

        lineRt.position = mid;
        float dist = Vector3.Distance(startPos, endPos);
        lineRt.sizeDelta = new Vector2(dist, 4);

        Vector3 dir = endPos - startPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lineRt.localEulerAngles = new Vector3(0, 0, angle);

        Image lineImg = lineObj.AddComponent<Image>();
        Color lineColor = color;
        if (lineColor == Color.white) lineColor = new Color(0.8f, 0.8f, 0.8f);
        lineImg.color = lineColor;
    }

    void ResetWrongColors()
    {
        foreach (var s in leftWires) if (!s.matched) s.img.color = DimColor(s.color);
        foreach (var s in rightWires) if (!s.matched) s.img.color = DimColor(s.color);
        locked = false;
    }

    Color DimColor(Color c)
    {
        if (c == Color.white) return new Color(0.6f, 0.6f, 0.6f, 0.9f);
        return new Color(c.r * 0.6f, c.g * 0.6f, c.b * 0.6f, 0.9f);
    }

    public static void ResetStatics()
    {
        IsOpen = false;
        instance = null;
    }

    void Complete()
    {
        IsOpen = false;
        if (onComplete != null) onComplete();
        Destroy(panel);
        Destroy(gameObject);
        instance = null;
    }

    void Shuffle(int[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = arr[i]; arr[i] = arr[j]; arr[j] = tmp;
        }
    }
}
