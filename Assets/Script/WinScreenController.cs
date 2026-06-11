using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinScreenController : MonoBehaviour
{
    private static WinScreenController instance;
    private static bool winShown = false;
    private GameObject panel;
    private AudioSource audioSource;

    public static void Show()
    {
        if (winShown) return;
        winShown = true;
        if (instance == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null) return;
            GameObject go = new GameObject("WinScreenController");
            go.transform.SetParent(canvas.transform, false);
            instance = go.AddComponent<WinScreenController>();
        }
        instance.CreateWinScreen();
    }

    void CreateWinScreen()
    {
        Transform canvas = transform.parent;

        GameObject informeAlert = GameObject.Find("InformeAlert");
        if (informeAlert != null) informeAlert.SetActive(false);
        GameObject pc4Alert = GameObject.Find("PC4AlertOverlay");
        if (pc4Alert != null) pc4Alert.SetActive(false);
        GameObject routerAlert = GameObject.Find("AlertOverlay");
        if (routerAlert != null) routerAlert.SetActive(false);

        PlayMusic();

        panel = new GameObject("WinPanel");
        panel.transform.SetParent(canvas, false);
        panel.transform.SetAsLastSibling();
        RectTransform pRt = panel.AddComponent<RectTransform>();
        pRt.anchorMin = Vector2.zero;
        pRt.anchorMax = Vector2.one;
        pRt.sizeDelta = Vector2.zero;
        pRt.anchoredPosition = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        Sprite bgSprite = null;
        Texture2D tex = Resources.Load<Texture2D>("Texture/win");
        if (tex != null)
            bgSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        if (bgSprite != null)
        {
            bg.sprite = bgSprite;
            bg.type = Image.Type.Simple;
            bg.preserveAspect = true;
        }
        else
            bg.color = new Color(0f, 0.05f, 0f, 1f);
        bg.raycastTarget = true;

        int completed = 0;
        int total = 0;

        int modCount = PCWindowController.GetModuleCount();
        total += modCount;
        for (int i = 0; i < modCount; i++)
            if (PCWindowController.IsModuleCompleted(i)) completed++;

        total += 3;
        if (TaskNotesController.wifiFixed) completed++;
        if (TaskNotesController.documentFixed) completed++;
        if (TaskNotesController.pc4Fixed) completed++;

        GameObject infoObj = new GameObject("InfoText");
        infoObj.transform.SetParent(panel.transform, false);
        Text info = infoObj.AddComponent<Text>();
        info.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        info.fontSize = 24;
        info.fontStyle = FontStyle.Bold;
        info.alignment = TextAnchor.MiddleCenter;
        info.color = new Color(0.2f, 1f, 0.4f);
        info.text = "¡FELICIDADES!\nCompletaste todos los modulos del proyecto\n\nTareas completadas: " + completed + " / " + total;
        RectTransform iRt = infoObj.GetComponent<RectTransform>();
        iRt.anchorMin = new Vector2(0f, 0.55f);
        iRt.anchorMax = new Vector2(1f, 0.72f);
        iRt.sizeDelta = Vector2.zero;
        iRt.anchoredPosition = Vector2.zero;

        CreateButton("BtnReintentar", "REINTENTAR", 0.5f, 0.35f, () =>
        {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
            GameStateReset.ResetAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    void CreateButton(string name, string label, float x, float y, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(panel.transform, false);
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.25f, 0.2f, 0.95f);
        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(action);
        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(x - 0.12f, y);
        rt.anchorMax = new Vector2(x + 0.12f, y + 0.07f);
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

    public static void ResetStatics()
    {
        winShown = false;
        instance = null;
    }

    void PlayMusic()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = 0.6f;
        AudioClip clip = Resources.Load<AudioClip>("Audio/win_music");
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
