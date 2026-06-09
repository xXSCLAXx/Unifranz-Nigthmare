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
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 0f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(1212f, 494f);
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
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 0f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(605f, 452.5f);
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

        if (pcWindow.activeSelf || RouterController.IsOpen || TaskNotesController.IsOpen || ComputerRoomController.IsOpen || Aula306Controller.IsOpen || WireMinigameController.IsOpen) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouse = Input.mousePosition;
            Debug.Log("Click: " + mouse);

            // Si un panel de pasillo est? abierto, solo procesar su cierre o boton PC4
            if (pasilloDerechaPanel.activeSelf)
            {
                if (mouse.x > 1190 && mouse.x < 1240 && mouse.y > 470 && mouse.y < 520)
                    ComputerRoomController.Show();
                else if (mouse.x > 35 && mouse.x < 355 && mouse.y > 40 && mouse.y < 585)
                    pasilloDerechaPanel.SetActive(false);
                return;
            }

            if (pasilloIzquierdaPanel.activeSelf)
            {
                if (mouse.x > 490 && mouse.x < 720 && mouse.y > 290 && mouse.y < 560)
                    Aula306Controller.Show();
                else if (mouse.x > 131 && mouse.x < 318 && mouse.y > 46 && mouse.y < 618)
                    pasilloIzquierdaPanel.SetActive(false);
                return;
            }

            // PC
            if (mouse.x > 720 && mouse.x < 825 && mouse.y > 270 && mouse.y < 370)
            {
                AudioManager am = FindObjectOfType<AudioManager>();
                if (am != null) am.PlayClickPC();
                pcWindow.SetActive(true);
                return;
            }

            // Router / WiFi
            if (mouse.x > 970 && mouse.x < 1070 && mouse.y > 260 && mouse.y < 360)
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
            if (mouse.x > 830 && mouse.x < 995 && mouse.y > 310 && mouse.y < 450)
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
                if (mouse.x > 1500 && mouse.x < 1740 && mouse.y > 200 && mouse.y < 620)
                {
                    pasilloDerechaPanel.SetActive(true);
                    return;
                }
            }

            // Puerta izquierda
            if (!pasilloIzquierdaPanel.activeSelf && !pasilloDerechaPanel.activeSelf)
            {
                if (mouse.x > 131 && mouse.x < 318 && mouse.y > 46 && mouse.y < 618)
                {
                    pasilloIzquierdaPanel.SetActive(true);
                    return;
                }
            }
        }
    }
}
