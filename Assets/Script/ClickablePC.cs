using UnityEngine;

public class ClickablePC : MonoBehaviour
{
    public GameObject pcWindow;
    public GameObject pasilloDerechaPanel;
    public GameObject pasilloIzquierdaPanel;

    void Update()
    {
        if (pcWindow.activeSelf || RouterController.IsOpen || TaskNotesController.IsOpen) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouse = Input.mousePosition;
            Debug.Log("Click: " + mouse);

            // PC
            if (mouse.x > 720 && mouse.x < 825 && mouse.y > 270 && mouse.y < 370)
            {
                AudioManager am = FindObjectOfType<AudioManager>();
                if (am != null) am.PlayClickPC();
                pcWindow.SetActive(true);
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
            }

            // Puerta derecha
            if (!pasilloDerechaPanel.activeSelf && !pasilloIzquierdaPanel.activeSelf)
            {
                if (mouse.x > 1199 && mouse.x < 1344 && mouse.y > 61 && mouse.y < 631)
                    pasilloDerechaPanel.SetActive(true);
            }
            else if (pasilloDerechaPanel.activeSelf)
            {
                if (mouse.x > 35 && mouse.x < 355 && mouse.y > 40 && mouse.y < 585)
                    pasilloDerechaPanel.SetActive(false);
            }

            // Puerta izquierda
            if (!pasilloIzquierdaPanel.activeSelf && !pasilloDerechaPanel.activeSelf)
            {
                if (mouse.x > 131 && mouse.x < 318 && mouse.y > 46 && mouse.y < 618)
                    pasilloIzquierdaPanel.SetActive(true);
            }
            else if (pasilloIzquierdaPanel.activeSelf)
            {
                if (mouse.x > 131 && mouse.x < 318 && mouse.y > 46 && mouse.y < 618)
                    pasilloIzquierdaPanel.SetActive(false);
            }
        }
    }
}