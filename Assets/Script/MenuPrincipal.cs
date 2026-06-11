using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuPrincipal : MonoBehaviour
{
    private GameObject instructionsPanel;
    private Font uiFont;

    void Start()
    {
        uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (uiFont == null)
            uiFont = Font.CreateDynamicFontFromOSFont("Arial", 16);
        if (uiFont == null)
            uiFont = Font.CreateDynamicFontFromOSFont("Times New Roman", 16);
        CreateInstructionsButton();
    }

    void CreateInstructionsButton()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject salirObj = GameObject.Find("BotonSalir");
        float offsetY = -140f;
        if (salirObj != null)
        {
            RectTransform salirRt = salirObj.GetComponent<RectTransform>();
            offsetY = salirRt.anchoredPosition.y - 140f;
        }

        GameObject btnObj = new GameObject("BtnInstructions");
        btnObj.transform.SetParent(canvas.transform, false);
        RectTransform btnRt = btnObj.AddComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(0.5f, 0.5f);
        btnRt.anchorMax = new Vector2(0.5f, 0.5f);
        btnRt.pivot = new Vector2(0.5f, 0.5f);
        btnRt.anchoredPosition = new Vector2(0f, offsetY);
        btnRt.sizeDelta = new Vector2(280f, 46f);

        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.3f, 0.2f, 0.05f, 0.8f);

        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(btnObj.transform, false);
        RectTransform iconRt = iconObj.AddComponent<RectTransform>();
        iconRt.anchorMin = Vector2.zero;
        iconRt.anchorMax = Vector2.one;
        iconRt.sizeDelta = Vector2.zero;
        Text iconText = iconObj.AddComponent<Text>();
        iconText.font = uiFont;
        iconText.fontSize = 24;
        iconText.fontStyle = FontStyle.Bold;
        iconText.alignment = TextAnchor.MiddleCenter;
        iconText.color = Color.white;
        iconText.text = "INSTRUCCIONES";

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        btn.onClick.AddListener(() => {
            AudioManager am = FindObjectOfType<AudioManager>();
            if (am != null) am.PlayClickPC();
            ShowInstructions();
        });

        ColorBlock cb = new ColorBlock();
        cb.normalColor = new Color(0.3f, 0.2f, 0.05f, 0.8f);
        cb.highlightedColor = new Color(0.5f, 0.35f, 0.1f, 1f);
        cb.pressedColor = new Color(0.2f, 0.15f, 0.03f, 1f);
        cb.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        cb.colorMultiplier = 1f;
        cb.fadeDuration = 0.1f;
        btn.colors = cb;
    }

    void ShowInstructions()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        Transform root = canvas.transform;

        GameObject blocker = new GameObject("InstructionsBlocker");
        blocker.transform.SetParent(root, false);
        RectTransform bRt = blocker.AddComponent<RectTransform>();
        bRt.anchorMin = Vector2.zero;
        bRt.anchorMax = Vector2.one;
        bRt.sizeDelta = Vector2.zero;
        Image bImg = blocker.AddComponent<Image>();
        bImg.color = new Color(0f, 0f, 0f, 0.3f);
        bImg.raycastTarget = true;

        instructionsPanel = new GameObject("InstructionsPanel");
        instructionsPanel.transform.SetParent(root, false);
        instructionsPanel.transform.SetAsLastSibling();

        RectTransform pRt = instructionsPanel.AddComponent<RectTransform>();
        pRt.anchorMin = new Vector2(0.5f, 0.5f);
        pRt.anchorMax = new Vector2(0.5f, 0.5f);
        pRt.pivot = new Vector2(0.5f, 0.5f);
        pRt.sizeDelta = new Vector2(550, 560);
        pRt.anchoredPosition = Vector2.zero;

        Image bg = instructionsPanel.AddComponent<Image>();
        bg.color = Color.white;

        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(instructionsPanel.transform, false);
        borderObj.transform.SetAsFirstSibling();
        RectTransform borderRt = borderObj.AddComponent<RectTransform>();
        borderRt.anchorMin = Vector2.zero;
        borderRt.anchorMax = Vector2.one;
        borderRt.sizeDelta = new Vector2(-4, -4);
        borderRt.anchoredPosition = Vector2.zero;
        Image borderImg = borderObj.AddComponent<Image>();
        borderImg.color = Color.black;
        borderImg.raycastTarget = false;

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(instructionsPanel.transform, false);
        RectTransform titleRt = titleObj.AddComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0f, 1f);
        titleRt.anchorMax = new Vector2(1f, 1f);
        titleRt.pivot = new Vector2(0.5f, 1f);
        titleRt.sizeDelta = new Vector2(-40, 50);
        titleRt.anchoredPosition = new Vector2(0f, -15f);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.font = uiFont;
        titleText.fontSize = 20;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.black;
        titleText.text = "INSTRUCCIONES";

        GameObject closeObj = new GameObject("BtnClose");
        closeObj.transform.SetParent(instructionsPanel.transform, false);
        RectTransform cRt = closeObj.AddComponent<RectTransform>();
        cRt.anchorMin = new Vector2(1f, 1f);
        cRt.anchorMax = new Vector2(1f, 1f);
        cRt.pivot = new Vector2(1f, 1f);
        cRt.sizeDelta = new Vector2(51f, 51f);
        cRt.anchoredPosition = new Vector2(-5f, -5f);
        Image cImg = closeObj.AddComponent<Image>();
        cImg.color = new Color(0.8f, 0.15f, 0.15f, 0.9f);
        Button cBtn = closeObj.AddComponent<Button>();
        cBtn.targetGraphic = cImg;
        cBtn.onClick.AddListener(() => {
            AudioManager am = FindObjectOfType<AudioManager>();
            if (am != null) am.PlayClickPC();
            CloseInstructions(blocker);
        });
        GameObject cTextObj = new GameObject("Text");
        cTextObj.transform.SetParent(closeObj.transform, false);
        RectTransform cTextRt = cTextObj.AddComponent<RectTransform>();
        cTextRt.anchorMin = Vector2.zero;
        cTextRt.anchorMax = Vector2.one;
        cTextRt.sizeDelta = Vector2.zero;
        Text cText = cTextObj.AddComponent<Text>();
        cText.font = uiFont;
        cText.fontSize = 24;
        cText.fontStyle = FontStyle.Bold;
        cText.alignment = TextAnchor.MiddleCenter;
        cText.color = Color.white;
        cText.text = "X";

        GameObject scrollObj = new GameObject("ScrollView");
        scrollObj.transform.SetParent(instructionsPanel.transform, false);
        RectTransform sRt = scrollObj.AddComponent<RectTransform>();
        sRt.anchorMin = new Vector2(0f, 0f);
        sRt.anchorMax = new Vector2(1f, 1f);
        sRt.sizeDelta = new Vector2(-30, -90);
        sRt.anchoredPosition = new Vector2(0f, -5f);

        Image sBg = scrollObj.AddComponent<Image>();
        sBg.color = new Color(0.97f, 0.94f, 0.8f);

        GameObject maskObj = new GameObject("Mask");
        maskObj.transform.SetParent(scrollObj.transform, false);
        RectTransform maskRt = maskObj.AddComponent<RectTransform>();
        maskRt.anchorMin = Vector2.zero;
        maskRt.anchorMax = Vector2.one;
        maskRt.sizeDelta = Vector2.zero;
        Image maskImg = maskObj.AddComponent<Image>();
        maskImg.color = Color.white;
        maskImg.raycastTarget = false;
        Mask mask = maskObj.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(maskObj.transform, false);
        RectTransform cRt2 = contentObj.AddComponent<RectTransform>();
        cRt2.anchorMin = new Vector2(0f, 1f);
        cRt2.anchorMax = new Vector2(1f, 1f);
        cRt2.pivot = new Vector2(0.5f, 1f);
        cRt2.sizeDelta = new Vector2(-20f, 0f);

        ContentSizeFitter csf = contentObj.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.content = cRt2;
        scrollRect.viewport = maskRt;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 30f;

        Text contentText = contentObj.AddComponent<Text>();
        contentText.font = uiFont;
        contentText.fontSize = 15;
        contentText.color = Color.black;
        contentText.supportRichText = true;
        contentText.text = GetInstructionsText();
    }

    string GetInstructionsText()
    {
        return
            "<b>FIVE NIGHTS IN UNIFRANZ — INSTRUCCIONES</b>\n\n" +
            "<b>OBJETIVO</b>\n" +
            "Sobrevivir la noche completando tareas en la PC (módulos 1-5). " +
            "Tenés 3 vidas. Si perdés las 3 → Game Over.\n\n" +
            "<b>VIDA</b>\n" +
            "• 3 vidas. Perder todas = Game Over con pantalla roja y scream.\n" +
            "• Solo el Informe puede matarte de 1 solo golpe (3 vidas de una = instant game over).\n" +
            "• Las demás fuentes de daño (PC4, WiFi, PCTimer) quitan 1 vida cada una.\n\n" +
            "<b>TAREAS (PC — módulos 1 a 5)</b>\n" +
            "1. Abrí la PC (click en la computadora de la sala principal).\n" +
            "2. Cada módulo es una pantalla con código HTML/CSS/JS/Python. Resolvelo.\n" +
            "3. Cada tarea dura ~10 segundos y se completa sola mientras el timer corre.\n" +
            "4. Completar las 5 tareas = Victoria (pantalla de win).\n\n" +
            "<b>PENALIZACIONES POR FALLA</b>\n" +
            "• PC4 (Sala de Cómputo): Cada vez que hacés click en la sala principal hay 5-12% de " +
            "chance de que PC4 se rompa. Si se rompe, entrá a la Sala de Cómputo y reparala " +
            "antes de que explote. Si explota → screamer + 1 vida.\n" +
            "• WiFi (Router): Si te olvidás del router, te agarra un screamer de WiFi → 1 vida.\n" +
            "• Timer de la PC (PCTimer): Mientras estás en la tarea de la PC, un timer interno " +
            "de 10s corre (se reduce con cada scare consecutivo, mínimo 1s). Si llega a 0 → " +
            "screamer + pérdida de la tarea actual + 1 vida.\n" +
            "• Cada scare consecutivo en la PC reduce el timer (penalidad acumulativa).\n" +
            "• Podés extender el timer haciendo tareas secundarias.\n\n" +
            "<b>TAREAS SECUNDARIAS (BONUS)</b>\n" +
            "Hacelas para ganar tiempo extra en el PCTimer:\n" +
            "• Informe: Corregir error → +2.5s\n" +
            "• PC4: Reparar → +2s\n" +
            "• Router (WiFi): Reparar → +1s\n\n" +
            "<b>INFORME — ¡CUIDADO!</b>\n" +
            "El informe tiene su propio ciclo global que siempre corre, incluso cuando cerrás " +
            "la ventana.\n" +
            "• Primera vez que abrís el Informe: Solo se marca el error, no arranca el timer.\n" +
            "• Ciclo normal (arranca después de corregir el primer error):\n" +
            "  1. 40 segundos de cooldown — No pasa nada.\n" +
            "  2. 45 segundos de peligro — Aparece un error nuevo para corregir. Se escucha " +
            "música de alerta y se ve un overlay rojo en toda la pantalla.\n" +
            "• No corregís a tiempo el error en esos 45s → Game Over instantáneo (pantalla de " +
            "sangre, scream, perdés todas las vidas).\n" +
            "• Corregís el error → Se reinicia el ciclo (vuelve a 40s de cooldown).\n" +
            "• El timer del Informe sigue corriendo aunque tengas el Informe cerrado. Si estás " +
            "en fase de peligro, lo escuchás y ves el overlay rojo desde cualquier pantalla.\n" +
            "• No hay contador visible — tenés que acordarte de los tiempos.\n\n" +
            "<b>ESTRATEGIA</b>\n" +
            "1. Priorizá completar los módulos de la PC rápido.\n" +
            "2. No te olvides del Informe: si ves el overlay rojo o escuchás la alarma, " +
            "corregí el error YA.\n" +
            "3. Revisá PC4 de vez en cuando para que no explote.\n" +
            "4. Escuchá el router: si suena raro, andá a revisarlo.\n" +
            "5. Usá las tareas secundarias para extender el timer de la PC y no morir en el intento.\n\n" +
            "<b>CONTROLES</b>\n" +
            "• Click izquierdo en la sala principal para interactuar con objetos.\n" +
            "• Puerta derecha: click en la zona derecha de la pantalla → Se abre el panel del " +
            "pasillo derecho (Sala de Cómputo).\n" +
            "• Puerta izquierda: click en la zona izquierda de la pantalla → Se abre el panel " +
            "del pasillo izquierdo (Aula 306).\n" +
            "• PC: click en la computadora (centro).\n" +
            "• Router/WiFi: click en el router (al lado de la PC).\n" +
            "• Notas (Post-it): click en el post-it al lado de la PC.\n" +
            "• Cerrar paneles: click fuera del área del botón.\n" +
            "• X con sonido de click en todas las ventanas.";
    }

    void CloseInstructions(GameObject blocker)
    {
        if (instructionsPanel != null) Destroy(instructionsPanel);
        if (blocker != null) Destroy(blocker);
        instructionsPanel = null;
    }

    public void Jugar()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Salir()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
