using UnityEngine;

public class ClickablePC : MonoBehaviour
{
    public GameObject pcWindow;
    public GameObject pasilloDerechaPanel;
    public GameObject pasilloIzquierdaPanel;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouse = Input.mousePosition;
            Debug.Log("Click: " + mouse);

            // PC
            if (mouse.x > 620 && mouse.x < 720 && mouse.y > 240 && mouse.y < 320)
            {
                AudioManager am = FindObjectOfType<AudioManager>();
                if (am != null) am.PlayClickPC();
                pcWindow.SetActive(true);
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