using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClickablePC : MonoBehaviour
{
    public GameObject pcWindow;
    public GameObject pasilloDerechaPanel;
    public GameObject pasilloIzquierdaPanel;
    private Button btnComputerRoom;
    private Image btnComputerRoomImg;
    private Image aula306Dot;
    void Start()
    {
        CreateComputerRoomButton();
        CreateAula306Dot();
    }

    void CreateComputerRoomButton()
    {
        if (pasilloDerechaPanel == null) return;

        GameObject btnObj = new GameObject("BtnComputerRoom");
        btnObj.transform.SetParent(pasilloDerechaPanel.transform, false);

        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1212f / 1920f, 494f / 1080f);
        rt.anchorMax = new Vector2(1212f / 1920f, 494f / 1080f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(40, 40);

        Texture2D circleTex = new Texture2D(40, 40);
        for (int y = 0; y < 40; y++)
            for (int x = 0; x < 40; x++)
            {
                float dx = x - 20, dy = y - 20;
                circleTex.SetPixel(x, y, (dx * dx + dy * dy <= 18 * 18) ? Color.white : Color.clear);
            }
        circleTex.Apply();

        btnComputerRoom = btnObj.AddComponent<Button>();
        btnComputerRoomImg = btnObj.AddComponent<Image>();
        btnComputerRoom.targetGraphic = btnComputerRoomImg;
        btnComputerRoomImg.sprite = Sprite.Create(circleTex, new Rect(0, 0, 24, 24), new Vector2(0.5f, 0.5f));
        btnComputerRoomImg.color = Color.red;

        btnComputerRoom.onClick.AddListener(() =>
        {
            ComputerRoomController.Show();
        });

        StartCoroutine(GlowRoutine());
    }

    void CreateAula306Dot()
    {
        if (pasilloIzquierdaPanel == null) return;

        GameObject dotObj = new GameObject("Aula306Dot");
        dotObj.transform.SetParent(pasilloIzquierdaPanel.transform, false);

        RectTransform rt = dotObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(605f / 1920f, 452.5f / 1080f);
        rt.anchorMax = new Vector2(605f / 1920f, 452.5f / 1080f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(30, 30);

        Texture2D circleTex = new Texture2D(30, 30);
        for (int y = 0; y < 30; y++)
            for (int x = 0; x < 30; x++)
            {
                float dx = x - 15, dy = y - 15;
                circleTex.SetPixel(x, y, (dx * dx + dy * dy <= 13 * 13) ? Color.white : Color.clear);
            }
        circleTex.Apply();

        aula306Dot = dotObj.AddComponent<Image>();
        aula306Dot.sprite = Sprite.Create(circleTex, new Rect(0, 0, 24, 24), new Vector2(0.5f, 0.5f));
        aula306Dot.color = Color.red;
    }

    IEnumerator GlowRoutine()
    {
        while (true)
        {
            float pulse = Mathf.PingPong(Time.time * 1.2f, 1f);
            float alpha = Mathf.Lerp(0.3f, 0.7f, pulse);

            if (btnComputerRoomImg != null && pasilloDerechaPanel != null && pasilloDerechaPanel.activeSelf)
                btnComputerRoomImg.color = new Color(0.8f, 0.15f, 0.15f, alpha);

            if (aula306Dot != null && pasilloIzquierdaPanel != null)
                aula306Dot.color = new Color(0.8f, 0.15f, 0.15f, alpha);

            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // PC4 break check (50% cada click si PC4 est? reparada y no rota)
            if (TaskNotesController.pc4Fixed && !ComputerRoomController.pc4IsBroken)
            {
                float pc4Chance = 0.05f;
                if (PCWindowController.IsModuleCompleted(4)) pc4Chance = 0.12f;
                else if (PCWindowController.IsModuleCompleted(2)) pc4Chance = 0.07f;
                if (Random.value < pc4Chance)
                    ComputerRoomController.BreakPC4();
            }
        }

        if (pcWindow.activeSelf || RouterController.IsOpen || TaskNotesController.IsOpen || ComputerRoomController.IsOpen || Aula306Controller.IsOpen || InformeController.IsOpen || WireMinigameController.IsOpen) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouse = Input.mousePosition;
            float nx = mouse.x / Screen.width;
            float ny = mouse.y / Screen.height;
            Debug.Log("Click normalized: (" + nx.ToString("F3") + ", " + ny.ToString("F3") + ")");

            // Si un panel de pasillo est? abierto, solo procesar su cierre o boton PC4
            if (pasilloDerechaPanel.activeSelf)
            {
                if (nx > 0.620f && nx < 0.646f && ny > 0.435f && ny < 0.481f)
                    ComputerRoomController.Show();
                else if (nx > 0.018f && nx < 0.185f && ny > 0.037f && ny < 0.542f)
                    pasilloDerechaPanel.SetActive(false);
                return;
            }

            if (pasilloIzquierdaPanel.activeSelf)
            {
                if (nx > 0.255f && nx < 0.375f && ny > 0.269f && ny < 0.519f)
                    Aula306Controller.Show();
                else if (nx > 0.068f && nx < 0.166f && ny > 0.043f && ny < 0.572f)
                    pasilloIzquierdaPanel.SetActive(false);
                return;
            }

            // PC
            if (nx > 0.375f && nx < 0.430f && ny > 0.325f && ny < 0.418f)
            {
                AudioManager am = FindObjectOfType<AudioManager>();
                if (am != null) am.PlayClickPC();
                pcWindow.SetActive(true);
                return;
            }

            // Router / WiFi
            if (nx > 0.505f && nx < 0.557f && ny > 0.316f && ny < 0.408f)
            {
                AudioManager am = FindObjectOfType<AudioManager>();
                if (am != null) am.PlayClickPC();
                RouterController router = FindObjectOfType<RouterController>();
                if (router == null)
                {
                    Canvas canvas = FindObjectOfType<Canvas>();
                    if (canvas == null) return;
                    GameObject go = new GameObject("RouterController");
                    go.transform.SetParent(canvas.transform, false);
                    router = go.AddComponent<RouterController>();
                }
                router.Show();
                return;
            }

            // Notes / Task screen (post-it)
            if (nx > 0.432f && nx < 0.518f && ny > 0.362f && ny < 0.492f)
            {
                AudioManager am = FindObjectOfType<AudioManager>();
                if (am != null) am.PlayClickPC();
                TaskNotesController notes = FindObjectOfType<TaskNotesController>();
                if (notes == null)
                {
                    Canvas canvas = FindObjectOfType<Canvas>();
                    if (canvas == null) return;
                    GameObject go = new GameObject("TaskNotesController");
                    go.transform.SetParent(canvas.transform, false);
                    notes = go.AddComponent<TaskNotesController>();
                }
                notes.Show();
                return;
            }

            // Puerta derecha
            if (!pasilloDerechaPanel.activeSelf && !pasilloIzquierdaPanel.activeSelf)
            {
                if (nx > 0.781f && nx < 0.906f && ny > 0.185f && ny < 0.574f)
                {
                    pasilloDerechaPanel.SetActive(true);
                    return;
                }
            }

            // Puerta izquierda
            if (!pasilloIzquierdaPanel.activeSelf && !pasilloDerechaPanel.activeSelf)
            {
                if (nx > 0.068f && nx < 0.166f && ny > 0.043f && ny < 0.572f)
                {
                    pasilloIzquierdaPanel.SetActive(true);
                    return;
                }
            }
        }
    }
}
