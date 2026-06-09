using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public static int lives = 3;
    public static bool isGameOver = false;

    private static GameOverController instance;
    private GameObject panel;
    private bool created = false;

    public static void LoseLife()
    {
        if (isGameOver) return;
        lives--;
        if (lives <= 0)
        {
            isGameOver = true;
            if (instance == null)
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas == null) return;
                GameObject go = new GameObject("GameOverController");
                go.transform.SetParent(canvas.transform, false);
                instance = go.AddComponent<GameOverController>();
                instance.created = false;
            }
            instance.ShowGameOver();
        }
    }

    void ShowGameOver()
    {
        if (!created)
        {
            Transform canvas = transform.parent;

            panel = new GameObject("GameOverPanel");
            panel.transform.SetParent(canvas, false);
            panel.transform.SetAsLastSibling();
            RectTransform pRt = panel.AddComponent<RectTransform>();
            pRt.anchorMin = Vector2.zero;
            pRt.anchorMax = Vector2.one;
            pRt.sizeDelta = Vector2.zero;
            pRt.anchoredPosition = Vector2.zero;

            Image bg = panel.AddComponent<Image>();
            Sprite bgSprite = null;
            string imgPath = Application.dataPath + "/Texture/gameover.png";
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
                bg.color = new Color(0.05f, 0f, 0f, 1f);
            bg.raycastTarget = true;

            int completed = 0;
            int total = 0;

            int modCount = PCWindowController.GetModuleCount();
            total += modCount;
            for (int i = 0; i < modCount; i++)
                if (PCWindowController.IsModuleCompleted(i)) completed++;

            total += 4;
            if (TaskNotesController.wifiFixed) completed++;
            if (TaskNotesController.documentFixed) completed++;
            if (TaskNotesController.pc4Fixed) completed++;
            if (TaskNotesController.informeFixed) completed++;

            GameObject infoObj = new GameObject("InfoText");
            infoObj.transform.SetParent(panel.transform, false);
            Text info = infoObj.AddComponent<Text>();
            info.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            info.fontSize = 22;
            info.fontStyle = FontStyle.Bold;
            info.alignment = TextAnchor.MiddleCenter;
            info.color = new Color(1f, 0.8f, 0.2f);
            info.text = "GAME OVER\n\nTareas completadas: " + completed + " / " + total;
            RectTransform iRt = infoObj.GetComponent<RectTransform>();
            iRt.anchorMin = new Vector2(0f, 0.55f);
            iRt.anchorMax = new Vector2(1f, 0.7f);
            iRt.sizeDelta = Vector2.zero;
            iRt.anchoredPosition = Vector2.zero;

            CreateButton("BtnReintentar", "REINTENTAR", 0.4f, 0.35f, () =>
            {
                lives = 3;
                isGameOver = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });

            CreateButton("BtnSalir", "SALIR", 0.6f, 0.35f, () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });

            created = true;
        }
        panel.SetActive(true);
    }

    void CreateButton(string name, string label, float x, float y, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(panel.transform, false);
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.25f, 0.25f, 0.3f, 0.95f);
        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(action);
        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(x - 0.1f, y);
        rt.anchorMax = new Vector2(x + 0.1f, y + 0.07f);
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        Text txt = txtObj.AddComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = 16;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.text = label;
        RectTransform txtRt = txtObj.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero;
        txtRt.anchoredPosition = Vector2.zero;
    }
}
