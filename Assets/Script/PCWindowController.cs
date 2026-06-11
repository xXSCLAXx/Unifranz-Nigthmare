using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PCWindowController : MonoBehaviour
{
    [Header("Refs")]
    public Text chatDisplay;
    public Text codeDisplay;
    public InputField playerInput;

    [Header("Config")]
    public float taskDuration = 25f;

    private GameObject taskButtonsContainer;
    private InputField codeEditor;
    private GameObject codeEditorObj;
    private bool inCodeView = false;
    private int viewingTaskIndex = -1;
    private int viewingFileIndex = -1;

    private GameObject backButton;
    private static string lastChatText = "";

    private static List<ProjectTask> tasks;
    private static int currentTaskIndex = -1;
    private static float currentProgress = 0f;
    private static bool isLoading = false;
    private static string chatHistory = "";

    private Dictionary<string, string> responses;
    private static bool initialized = false;

    private class ProjectTask
    {
        public string name;
        public string keyword;
        public string responseText;
        public string[] files;
        public bool completed;
        public int linesOfCode;
        public string[] codeContents;
    }

    void Awake()
    {
        if (!initialized) InitTasks();
        InitResponses();
        CreateTaskButtons();
        CreateCodeEditor();
        CreateBackButton();
    }

    void InitTasks()
    {
        initialized = true;
        tasks = new List<ProjectTask>
        {
            new ProjectTask
            {
                name = "Dashboard Cliente",
                keyword = "dashboard",
                responseText = "Creando estructura del Dashboard...\nHTML semantico con sidebar, header y grid de cards.",
                files = new string[] { "index.html", "dashboard.html", "components/sidebar.html" },
                codeContents = new string[] {
                    "<!DOCTYPE html>\n<html lang=\"es\">\n<head>\n  <meta charset=\"UTF-8\">\n  <title>Mi Proyecto</title>\n</head>\n<body>\n  <h1>Dashboard Cliente</h1>\n  <div id=\"app\"></div>\n</body>\n</html>",
                    "<div class=\"dashboard\">\n  <aside class=\"sidebar\">\n    <nav>\n      <a href=\"#\">Inicio</a>\n      <a href=\"#\">Pedidos</a>\n      <a href=\"#\">Perfil</a>\n    </nav>\n  </aside>\n  <main>\n    <header>\n      <h2>Bienvenido, Cliente</h2>\n    </header>\n    <section class=\"cards\">\n      <div class=\"card\">Total Pedidos: 12</div>\n      <div class=\"card\">Pendientes: 3</div>\n    </section>\n  </main>\n</div>",
                    "<nav class=\"sidebar\">\n  <ul>\n    <li><a href=\"index.html\">Inicio</a></li>\n    <li><a href=\"dashboard.html\">Dashboard</a></li>\n    <li><a href=\"#\">Configuracion</a></li>\n  </ul>\n</nav>"
                },
                linesOfCode = 180
            },
            new ProjectTask
            {
                name = "Estilos CSS",
                keyword = "estilos",
                responseText = "Aplicando estilos modernos...\nVariables CSS, flexbox/grid, diseño responsive.",
                files = new string[] { "styles/main.css", "styles/dashboard.css", "styles/responsive.css" },
                codeContents = new string[] {
                    ":root {\n  --primary: #2563eb;\n  --dark: #1e293b;\n  --bg: #f8fafc;\n  --radius: 8px;\n}\n* { margin: 0; padding: 0; box-sizing: border-box; }\nbody {\n  font-family: 'Segoe UI', sans-serif;\n  background: var(--bg);\n  color: var(--dark);\n}",
                    ".dashboard {\n  display: grid;\n  grid-template-columns: 250px 1fr;\n  min-height: 100vh;\n}\n.sidebar {\n  background: var(--dark);\n  padding: 2rem;\n}\n.sidebar a { color: white; text-decoration: none; }\n.cards {\n  display: grid;\n  grid-template-columns: repeat(2, 1fr);\n  gap: 1rem;\n  padding: 2rem;\n}\n.card {\n  background: white;\n  padding: 1.5rem;\n  border-radius: var(--radius);\n  box-shadow: 0 2px 4px rgba(0,0,0,0.1);\n}",
                    "@media (max-width: 768px) {\n  .dashboard {\n    grid-template-columns: 1fr;\n  }\n  .sidebar {\n    display: none;\n  }\n  .cards {\n    grid-template-columns: 1fr;\n  }\n}"
                },
                linesOfCode = 320
            },
            new ProjectTask
            {
                name = "Interactividad JS",
                keyword = "javascript",
                responseText = "Agregando dinamismo con JS...\nManejo de DOM, eventos, fetch API.",
                files = new string[] { "scripts/app.js", "scripts/api.js", "scripts/utils.js" },
                codeContents = new string[] {
                    "const App = {\n  init() {\n    this.loadDashboard();\n    this.bindEvents();\n  },\n  loadDashboard() {\n    fetch('/api/dashboard')\n      .then(r => r.json())\n      .then(data => this.render(data));\n  },\n  render(data) {\n    document.getElementById('app').innerHTML =\n      `<h2>Bienvenido</h2><p>${data.message}</p>`;\n  },\n  bindEvents() {\n    document.querySelectorAll('.card').forEach(c =>\n      c.addEventListener('click', this.onCardClick));\n  },\n  onCardClick(e) {\n    console.log('Card clicked:', e.target);\n  }\n};\ndocument.addEventListener('DOMContentLoaded', () => App.init());",
                    "const API = {\n  base: '/api',\n  async get(endpoint) {\n    const res = await fetch(this.base + endpoint);\n    return res.json();\n  },\n  async post(endpoint, data) {\n    const res = await fetch(this.base + endpoint, {\n      method: 'POST',\n      headers: { 'Content-Type': 'application/json' },\n      body: JSON.stringify(data)\n    });\n    return res.json();\n  }\n};",
                    "export function formatDate(date) {\n  return new Date(date).toLocaleDateString('es-ES');\n}\nexport function classNames(...classes) {\n  return classes.filter(Boolean).join(' ');\n}"
                },
                linesOfCode = 250
            },
            new ProjectTask
            {
                name = "API REST",
                keyword = "api",
                responseText = "Configurando endpoints REST...\nFastAPI con rutas, schemas y middleware.",
                files = new string[] { "api/main.py", "api/routes.py", "api/schemas.py" },
                codeContents = new string[] {
                    "from fastapi import FastAPI\nfrom fastapi.middleware.cors import CORSMiddleware\n\napp = FastAPI(title='Mi Proyecto API')\n\napp.add_middleware(\n    CORSMiddleware,\n    allow_origins=['*'],\n    allow_methods=['*'],\n    allow_headers=['*'],\n)\n\n@app.get('/')\ndef root():\n    return {'message': 'API funcionando'}",
                    "from fastapi import APIRouter, Depends\nfrom .schemas import *\n\nrouter = APIRouter()\n\n@router.get('/productos')\nasync def get_productos():\n    return [{'id': 1, 'name': 'Producto A'}]\n\n@router.post('/productos')\nasync def create_producto(p: ProductoSchema):\n    return {'id': 2, **p.dict()}",
                    "from pydantic import BaseModel\n\nclass ProductoSchema(BaseModel):\n    nombre: str\n    precio: float\n    categoria: str\n\nclass UsuarioSchema(BaseModel):\n    email: str\n    password: str\n    rol: str = 'cliente'"
                },
                linesOfCode = 200
            },
            new ProjectTask
            {
                name = "Base de Datos",
                keyword = "base de datos",
                responseText = "Modelando la base de datos...\nPostgreSQL con SQLAlchemy ORM.",
                files = new string[] { "database/models.py", "database/connection.py", "database/migrations/" },
                codeContents = new string[] {
                    "from sqlalchemy import Column, Integer, String, Float, ForeignKey\nfrom sqlalchemy.orm import relationship\nfrom .connection import Base\n\nclass Usuario(Base):\n    __tablename__ = 'usuarios'\n    id = Column(Integer, primary_key=True)\n    nombre = Column(String(100))\n    email = Column(String(100), unique=True)\n    pedidos = relationship('Pedido', back_populates='usuario')",
                    "from sqlalchemy import create_engine\nfrom sqlalchemy.orm import sessionmaker, declarative_base\n\nDATABASE_URL = 'postgresql://user:pass@localhost:5432/mi_db'\nengine = create_engine(DATABASE_URL)\nSessionLocal = sessionmaker(bind=engine)\nBase = declarative_base()\n\ndef get_db():\n    db = SessionLocal()\n    try:\n        yield db\n    finally:\n        db.close()",
                    "# Migraciones con Alembic\n# Comandos:\n# alembic init alembic\n# alembic revision --autogenerate -m 'init'\n# alembic upgrade head"
                },
                linesOfCode = 150
            },
            new ProjectTask
            {
                name = "Despliegue",
                keyword = "desplegar",
                responseText = "Preparando deploy...\nDocker, CI/CD y configuracion de produccion.",
                files = new string[] { "Dockerfile", "docker-compose.yml", ".github/workflows/deploy.yml" },
                codeContents = new string[] {
                    "FROM python:3.11-slim\nWORKDIR /app\nCOPY requirements.txt .\nRUN pip install -r requirements.txt\nCOPY . .\nCMD ['uvicorn', 'api.main:app', '--host', '0.0.0.0']",
                    "version: '3.8'\nservices:\n  api:\n    build: .\n    ports:\n      - '8000:8000'\n    environment:\n      - DATABASE_URL=postgresql://postgres:pass@db:5432/mi_db\n  db:\n    image: postgres:15\n    environment:\n      - POSTGRES_PASSWORD=pass",
                    "name: Deploy\non:\n  push:\n    branches: [main]\njobs:\n  deploy:\n    runs-on: ubuntu-latest\n    steps:\n      - uses: actions/checkout@v3\n      - run: docker compose up -d"
                },
                linesOfCode = 90
            }
        };
    }

    void InitResponses()
    {
        responses = new Dictionary<string, string>
        {
            { "dashboard", "Genial! Vamos con el <color=#4EC9B0>Dashboard del Cliente</color>.\n"
                + "Arquitectura:\n"
                + "- Sidebar con navegacion\n"
                + "- Header con datos del usuario\n"
                + "- Grid de cards con metricas\n"
                + "- Tabla de pedidos recientes\n\n"
                + "<color=#6A9955>Progreso: 0% → Iniciando carga...</color>\n\n"
                + "<color=#569CD6>Opciones:</color> estado | cancelar" },
            { "estilos", "Aplicando <color=#4EC9B0>Estilos CSS</color> al proyecto.\n"
                + "Variables globales, layout flexbox,\n"
                + "animaciones y diseño mobile-first.\n\n"
                + "<color=#6A9955>Progreso: 0% → Iniciando carga...</color>\n\n"
                + "<color=#569CD6>Opciones:</color> estado | cancelar" },
            { "javascript", "Agregando <color=#4EC9B0>JavaScript</color> al proyecto.\n"
                + "Manejo del DOM, llamadas a la API,\n"
                + "validacion de formularios y graficos.\n\n"
                + "<color=#6A9955>Progreso: 0% → Iniciando carga...</color>\n\n"
                + "<color=#569CD6>Opciones:</color> estado | cancelar" },
            { "api", "Configurando <color=#4EC9B0>API REST</color> con FastAPI.\n"
                + "Endpoints CRUD, autenticacion JWT,\n"
                + "validacion con Pydantic.\n\n"
                + "<color=#6A9955>Progreso: 0% → Iniciando carga...</color>\n\n"
                + "<color=#569CD6>Opciones:</color> estado | cancelar" },
            { "base de datos", "Conectando <color=#4EC9B0>Base de Datos</color> PostgreSQL.\n"
                + "Modelos SQLAlchemy, migraciones Alembic,\n"
                + "consultas optimizadas con indices.\n\n"
                + "<color=#6A9955>Progreso: 0% → Iniciando carga...</color>\n\n"
                + "<color=#569CD6>Opciones:</color> estado | cancelar" },
            { "desplegar", "Preparando <color=#4EC9B0>Despliegue</color> del proyecto.\n"
                + "Docker multi-stage, CI/CD con GitHub Actions,\n"
                + "nginx como reverse proxy.\n\n"
                + "<color=#6A9955>Progreso: 0% → Iniciando carga...</color>\n\n"
                + "<color=#569CD6>Opciones:</color> estado | cancelar" },
        };
    }

    void CreateCodeEditor()
    {
        Transform panelCodigo = transform.Find("PanelCodigo");
        if (panelCodigo == null) return;

        codeEditorObj = new GameObject("CodeEditor");
        codeEditorObj.transform.SetParent(panelCodigo, false);

        RectTransform rt = codeEditorObj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        Image bg = codeEditorObj.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.05f, 1f);

        codeEditor = codeEditorObj.AddComponent<InputField>();

        GameObject textArea = new GameObject("TextArea");
        textArea.transform.SetParent(codeEditorObj.transform, false);
        Text displayText = textArea.AddComponent<Text>();
        displayText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        displayText.fontSize = 10;
        displayText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        displayText.supportRichText = false;
        displayText.alignment = TextAnchor.UpperLeft;
        displayText.horizontalOverflow = HorizontalWrapMode.Wrap;
        displayText.verticalOverflow = VerticalWrapMode.Truncate;
        RectTransform taRt = textArea.GetComponent<RectTransform>();
        taRt.anchorMin = Vector2.zero;
        taRt.anchorMax = Vector2.one;
        taRt.sizeDelta = new Vector2(-10, -10);
        taRt.anchoredPosition = Vector2.zero;

        codeEditor.textComponent = displayText;
        codeEditor.lineType = InputField.LineType.MultiLineNewline;

        codeEditorObj.SetActive(false);
    }

    void CreateBackButton()
    {
        Transform panelCodigo = transform.Find("PanelCodigo");
        if (panelCodigo == null) return;

        backButton = new GameObject("BtnVolver");
        backButton.transform.SetParent(panelCodigo, false);
        RectTransform brt = backButton.AddComponent<RectTransform>();
        brt.anchorMin = new Vector2(0f, 0f);
        brt.anchorMax = new Vector2(0f, 0f);
        brt.pivot = new Vector2(0f, 0f);
        brt.anchoredPosition = new Vector2(10f, 10f);
        brt.sizeDelta = new Vector2(80f, 24f);
        Image bImg = backButton.AddComponent<Image>();
        bImg.color = new Color(0.3f, 0.3f, 0.4f, 0.9f);
        Button btn = backButton.AddComponent<Button>();
        btn.onClick.AddListener(() => ShowProjectOverview());
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.None;
        btn.navigation = nav;
        GameObject bTxt = new GameObject("Text");
        bTxt.transform.SetParent(backButton.transform, false);
        Text bLabel = bTxt.AddComponent<Text>();
        bLabel.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        bLabel.fontSize = 11;
        bLabel.alignment = TextAnchor.MiddleCenter;
        bLabel.color = Color.white;
        bLabel.text = "← Volver";
        RectTransform btrt = bTxt.GetComponent<RectTransform>();
        btrt.anchorMin = Vector2.zero;
        btrt.anchorMax = Vector2.one;
        btrt.sizeDelta = Vector2.zero;
        btrt.anchoredPosition = Vector2.zero;

        backButton.SetActive(false);
    }

    void SetupChatScroll()
    {
    }

    void ScrollChatToBottom()
    {
        ScrollRect sr = chatDisplay.GetComponentInParent<ScrollRect>();
        if (sr == null) return;
        Canvas.ForceUpdateCanvases();
        sr.verticalNormalizedPosition = 0f;
    }

    void CreateTaskButtons()
    {
        Transform panelChat = transform.Find("PanelChat");
        if (panelChat == null) return;

        taskButtonsContainer = new GameObject("TaskButtons");
        taskButtonsContainer.transform.SetParent(panelChat, false);
        RectTransform tbcRt = taskButtonsContainer.AddComponent<RectTransform>();
        tbcRt.anchorMin = new Vector2(0, 0);
        tbcRt.anchorMax = new Vector2(0.7f, 0);
        tbcRt.pivot = new Vector2(0, 0);
        tbcRt.anchoredPosition = new Vector2(10, 10);
        tbcRt.sizeDelta = new Vector2(0, 40);

        VerticalLayoutGroup vlg = taskButtonsContainer.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 4;
        vlg.childAlignment = TextAnchor.LowerLeft;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        ContentSizeFitter csf = taskButtonsContainer.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    void OnEnable()
    {
        PCTimer timer = GetComponent<PCTimer>();
        if (timer != null && isLoading)
        {
            timer.ResetTimer();
            timer.IniciarTimer();
        }

        if (inCodeView)
            ShowCodeView(viewingTaskIndex, viewingFileIndex);
        else
            UpdateProjectDisplay();
        UpdateTaskButtons();

        if (chatHistory != "")
        {
            chatDisplay.text = chatHistory;
        }
        else
        {
            string availableTasks = GetAvailableTasksText();
            chatDisplay.text =
                "<color=#569CD6>● Web Builder AI Assistant</color>\n" +
                "<color=#6A9955>──────────────────────────</color>\n\n" +
                "<color=#9CDCFE>AI:</color> Bienvenido al constructor de proyectos web!\n" +
                "Elegi un modulo para desarrollar:\n\n" +
                "<color=#569CD6>Opciones:</color> " + availableTasks + "\n" +
                "\n<color=#6A9955>Tip: Tambien podes tocar los botones de abajo.</color>\n";
            chatHistory = chatDisplay.text;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    void OnDisable()
    {
        PCTimer timer = GetComponent<PCTimer>();
        if (timer != null) timer.PausarTimer();
    }

    void LateUpdate()
    {
        if (chatDisplay == null) return;
        if (chatDisplay.text == lastChatText) return;

        lastChatText = chatDisplay.text;

        if (chatDisplay.text.Length > 1500)
        {
            string t = chatDisplay.text;
            int cut = t.Length - 1500;
            int nl = t.IndexOf('\n', cut);
            if (nl >= 0)
                t = t.Substring(nl + 1);
            else
                t = t.Substring(cut);
            if (t.Length > 0)
            {
                int searchLen = Mathf.Min(30, t.Length);
                int anyOpen = t.LastIndexOf('<', searchLen - 1, searchLen);
                if (anyOpen >= 0 && t.IndexOf('>', anyOpen) < 0)
                {
                    int lineEnd = t.IndexOf('\n');
                    if (lineEnd >= 0)
                        t = t.Substring(lineEnd + 1);
                    else
                        t = "";
                }
            }
            chatDisplay.text = t;
            chatHistory = t;
            lastChatText = t;
        }

        ScrollChatToBottom();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleCodeDisplayClick();

        if (!isLoading) return;

        currentProgress += Time.unscaledDeltaTime;
        float pct = Mathf.Clamp01(currentProgress / taskDuration);

        UpdateProjectDisplay();

        if (pct >= 1f)
            CompleteCurrentTask();
    }

    void HandleCodeDisplayClick()
    {
        if (codeDisplay == null || !codeDisplay.gameObject.activeInHierarchy || inCodeView)
            return;

        RectTransform rt = codeDisplay.rectTransform;
        if (!RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition, null))
            return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out localPoint);

        float yFromTop = rt.rect.height / 2f - localPoint.y;

        Canvas.ForceUpdateCanvases();
        var generator = codeDisplay.cachedTextGenerator;
        if (generator == null || generator.lineCount == 0) return;

        int clickedLineIndex = -1;
        for (int i = 0; i < generator.lines.Count; i++)
        {
            var line = generator.lines[i];
            float lineTop = line.topY;
            float lineBottom = lineTop - line.height;

            if (yFromTop >= lineBottom && yFromTop <= lineTop)
            {
                clickedLineIndex = i;
                break;
            }
        }

        if (clickedLineIndex < 0) return;

        string GetLineText(int idx)
        {
            var l = generator.lines[idx];
            int s = l.startCharIdx;
            int e = (idx < generator.lines.Count - 1)
                ? generator.lines[idx + 1].startCharIdx
                : codeDisplay.text.Length;
            return codeDisplay.text.Substring(s, e - s);
        }

        string context = GetLineText(clickedLineIndex);
        if (clickedLineIndex > 0)
            context = GetLineText(clickedLineIndex - 1) + context;
        if (clickedLineIndex < generator.lines.Count - 1)
            context += GetLineText(clickedLineIndex + 1);

        if (!context.Contains("[ver]")) return;

        for (int i = 0; i < tasks.Count; i++)
        {
            if (!tasks[i].completed) continue;
            if (context.Contains(tasks[i].name))
            {
                ShowCodeView(i, 0);
                return;
            }
        }

        for (int i = 0; i < tasks.Count; i++)
        {
            if (!tasks[i].completed) continue;
            for (int j = 0; j < tasks[i].files.Length; j++)
            {
                if (context.Contains(tasks[i].files[j]))
                {
                    ShowCodeView(i, j);
                    return;
                }
            }
        }
    }

    public void CancelTask()
    {
        if (!isLoading || currentTaskIndex < 0) return;
        isLoading = false;
        currentTaskIndex = -1;
        currentProgress = 0f;
        PCTimer timer = GetComponent<PCTimer>();
        if (timer != null) timer.ResetTimer();
    }

    void CompleteCurrentTask()
    {
        if (currentTaskIndex < 0 || currentTaskIndex >= tasks.Count) return;

        ProjectTask task = tasks[currentTaskIndex];
        task.completed = true;
        isLoading = false;
        currentTaskIndex = -1;
        currentProgress = 0f;

        PCTimer timer = GetComponent<PCTimer>();
        if (timer != null) timer.ResetTimer();

        UpdateProjectDisplay();
        UpdateTaskButtons();

        string fileList = string.Join("\n  ", task.files);
        chatDisplay.text += "\n<color=#4EC9B0>AI:</color> " + task.name + " completado!\n"
            + "<color=#6A9955>Archivos creados:</color>\n  " + fileList + "\n\n"
            + "<color=#9CDCFE>AI:</color> " + GetRandomCompleteMessage() + "\n\n"
            + "<color=#569CD6>Opciones:</color> " + GetAvailableTasksText() + "\n"
            + "<color=#6A9955>Tip: Escribi 'ver codigo' para explorar los archivos.</color>\n";
        chatHistory = chatDisplay.text;

        if (tasks.TrueForAll(t => t.completed))
            WinScreenController.Show();
    }

    string GetRandomCompleteMessage()
    {
        string[] msgs = {
            "Modulo listo! Que sigue?",
            "Perfecto, una tarea menos. Siguiente?",
            "Codigo compilado sin errores. Continuamos?",
            "Listo y funcionando. Proximo modulo?",
            "Excelente! Ya falta menos para terminar."
        };
        return msgs[Random.Range(0, msgs.Length)];
    }

    string GetProgressBarString(float pct)
    {
        int filled = Mathf.RoundToInt(pct * 20);
        string bar = "";
        for (int i = 0; i < 20; i++)
            bar += i < filled ? "<color=#4EC9B0>█</color>" : "<color=#3E3E3E>█</color>";
        return bar + string.Format(" <color=#DCDCAA>{0:F0}%</color>", pct * 100f);
    }

    void ShowCodeView(int taskIndex, int fileIndex)
    {
        if (taskIndex < 0 || taskIndex >= tasks.Count) return;
        ProjectTask task = tasks[taskIndex];
        if (!task.completed) return;
        if (fileIndex < 0 || fileIndex >= task.files.Length) return;

        inCodeView = true;
        viewingTaskIndex = taskIndex;
        viewingFileIndex = fileIndex;

        codeDisplay.gameObject.SetActive(false);
        codeEditorObj.SetActive(true);
        if (backButton != null) backButton.SetActive(true);

        codeEditor.text = task.codeContents[fileIndex];

        PCTimer timer = GetComponent<PCTimer>();
        if (timer != null) timer.PausarTimer();
    }

    void ShowProjectOverview()
    {
        inCodeView = false;
        viewingTaskIndex = -1;
        viewingFileIndex = -1;

        codeEditorObj.SetActive(false);
        codeDisplay.gameObject.SetActive(true);
        if (backButton != null) backButton.SetActive(false);

        UpdateProjectDisplay();
    }

    void NavegarArchivo(bool siguiente)
    {
        if (viewingTaskIndex < 0 || viewingTaskIndex >= tasks.Count) return;
        ProjectTask task = tasks[viewingTaskIndex];
        int maxIdx = task.files.Length;

        int nuevoIdx = viewingFileIndex + (siguiente ? 1 : -1);
        if (nuevoIdx < 0) nuevoIdx = maxIdx - 1;
        if (nuevoIdx >= maxIdx) nuevoIdx = 0;

        ShowCodeView(viewingTaskIndex, nuevoIdx);

        chatDisplay.text += "<color=#9CDCFE>AI:</color> Ahora viendo: <color=#4EC9B0>" + task.files[nuevoIdx] + "</color> (" + (nuevoIdx + 1) + "/" + maxIdx + ")\n";
        chatHistory = chatDisplay.text;
    }

    void UpdateProjectDisplay()
    {
        if (codeDisplay == null) return;

        string display = "";
        display += "<color=#569CD6>╔══════════════════════════╗</color>\n";
        display += "<color=#569CD6>║</color>  <color=#4EC9B0>📁 Proyecto Web - Cliente</color>  <color=#569CD6>║</color>\n";
        display += "<color=#569CD6>╚══════════════════════════╝</color>\n\n";

        int completedCount = 0;
        foreach (var t in tasks) { if (t.completed) completedCount++; }
        float totalPct = tasks.Count > 0 ? (float)completedCount / tasks.Count : 0f;

        if (isLoading && currentTaskIndex >= 0)
        {
            float taskPct = Mathf.Clamp01(currentProgress / taskDuration);
            display += "<color=#9CDCFE>Cargando:</color> " + tasks[currentTaskIndex].name + "\n";
            display += "  " + GetProgressBarString(taskPct) + "\n\n";
        }
        else
        {
            display += GetProgressBarString(totalPct) + "\n\n";
        }

        display += string.Format("<color=#DCDCAA>Progreso total:</color> {0}%  ({1}/{2} modulos)\n\n",
            Mathf.RoundToInt(totalPct * 100f), completedCount, tasks.Count);

        display += "<color=#569CD6>📋 Modulos:</color>\n";
        for (int i = 0; i < tasks.Count; i++)
        {
            string icon = tasks[i].completed ? "<color=#4EC9B0>✅</color>" : "⬜";
            string name = tasks[i].completed
                ? "<color=#6A9955>" + tasks[i].name + "</color>"
                : "<color=#DCDCAA>" + tasks[i].name + "</color>";

            if (isLoading && i == currentTaskIndex)
                name = "<color=#4EC9B0>" + tasks[i].name + "</color>";

            display += "  " + icon + " " + name;
            if (tasks[i].completed)
                display += " <color=#569CD6>[ver]</color>";
            display += "\n";
        }

        display += "\n<color=#569CD6>📂 Estructura del proyecto:</color>\n";
        display += "  <color=#DCDCAA>mi-proyecto/</color>\n";
        display += "  ├── <color=#569CD6>📁</color> <color=#4EC9B0>frontend/</color>\n";

        bool hasFrontend = false;
        for (int i = 0; i < tasks.Count; i++)
        {
            if (!tasks[i].completed) continue;
            if (i <= 2) hasFrontend = true;
            foreach (string f in tasks[i].files)
            {
                if (f.StartsWith("styles/") || f.StartsWith("scripts/") || f.StartsWith("components/") || f == "index.html" || f == "dashboard.html")
                {
                    display += "  │   ├── <color=#9CDCFE>" + f + "</color>";
                    display += " <color=#569CD6>[ver]</color>\n";
                }
            }
        }
        if (!hasFrontend)
            display += "  │   └── <color=#6A9955>(pendiente)</color>\n";

        display += "  ├── <color=#569CD6>📁</color> <color=#4EC9B0>backend/</color>\n";
        bool hasBackend = false;
        for (int i = 0; i < tasks.Count; i++)
        {
            if (!tasks[i].completed) continue;
            if (i >= 3) hasBackend = true;
            foreach (string f in tasks[i].files)
            {
                if (f.StartsWith("api/") || f.StartsWith("database/"))
                {
                    display += "  │   ├── <color=#9CDCFE>" + f + "</color>";
                    display += " <color=#569CD6>[ver]</color>\n";
                }
            }
        }
        if (!hasBackend)
            display += "  │   └── <color=#6A9955>(pendiente)</color>\n";

        display += "  └── <color=#569CD6>📁</color> <color=#4EC9B0>deploy/</color>\n";
        bool hasDeploy = false;
        for (int i = 0; i < tasks.Count; i++)
        {
            if (!tasks[i].completed) continue;
            if (i == 5) hasDeploy = true;
            foreach (string f in tasks[i].files)
            {
                if (f.StartsWith("Dockerfile") || f.StartsWith("docker-compose") || f.StartsWith(".github"))
                {
                    display += "      └── <color=#9CDCFE>" + f + "</color>";
                    display += " <color=#569CD6>[ver]</color>\n";
                }
            }
        }
        if (!hasDeploy)
            display += "      └── <color=#6A9955>(pendiente)</color>\n";

        if (completedCount == tasks.Count)
        {
            display += "\n<color=#4EC9B0>━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>\n";
            display += "<color=#4EC9B0>  🎉 PROYECTO COMPLETADO!</color>\n";
            display += "<color=#4EC9B0>━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>\n";
        }

        codeDisplay.text = display;
    }

    void UpdateTaskButtons()
    {
        if (taskButtonsContainer == null) return;

        foreach (Transform child in taskButtonsContainer.transform)
            Destroy(child.gameObject);

        foreach (ProjectTask task in tasks)
        {
            if (task.completed) continue;

            GameObject btn = new GameObject("Btn_" + task.name);
            btn.transform.SetParent(taskButtonsContainer.transform, false);

            Button button = btn.AddComponent<Button>();
            Image img = btn.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.25f, 1f);
            img.type = Image.Type.Sliced;

            RectTransform rt = btn.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, 24);

            GameObject txt = new GameObject("Text");
            txt.transform.SetParent(btn.transform, false);
            Text label = txt.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.fontSize = 10;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(0.7f, 0.7f, 1f, 1f);
            label.text = "+ " + task.name;
            RectTransform trt = txt.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.sizeDelta = Vector2.zero;
            trt.anchoredPosition = Vector2.zero;

            string kw = task.keyword;
            button.onClick.AddListener(() => OnTaskButtonClick(kw));

            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            button.navigation = nav;
        }
    }

    void OnTaskButtonClick(string keyword)
    {
        playerInput.text = keyword;
        OnSendMessage();
    }

    public void OnSendMessage()
    {
        if (string.IsNullOrEmpty(playerInput.text)) return;

        if (inCodeView)
        {
            string cmd = playerInput.text.ToLower().Trim();
            if (cmd == "volver")
            {
                playerInput.text = "";
                ShowProjectOverview();
                return;
            }
            if (cmd == "siguiente" || cmd == "anterior")
            {
                chatDisplay.text += "\n<color=#DCDCAA>Vos:</color> " + cmd + "\n";
                playerInput.text = "";
                NavegarArchivo(cmd == "siguiente");
                return;
            }
            chatDisplay.text += "\n<color=#DCDCAA>Vos:</color> " + cmd + "\n";
            chatDisplay.text += "\n<color=#9CDCFE>AI:</color> Estas en el editor. Escribi <color=#569CD6>volver</color> para salir, <color=#569CD6>siguiente</color>/<color=#569CD6>anterior</color> para cambiar de archivo.\n";
            playerInput.text = "";
            chatHistory = chatDisplay.text;
            return;
        }

        if (isLoading)
        {
            chatDisplay.text += "\n<color=#DCDCAA>Vos:</color> " + playerInput.text + "\n";
            chatDisplay.text += "\n<color=#9CDCFE>AI:</color> Ya hay una tarea en progreso! Escribi <color=#569CD6>estado</color> para ver el progreso o <color=#569CD6>cancelar</color> para detenerla.\n";
            playerInput.text = "";
            chatHistory = chatDisplay.text;
            return;
        }

        string msg = playerInput.text.ToLower().Trim();
        chatDisplay.text += "\n<color=#DCDCAA>Vos:</color> " + msg + "\n";
        playerInput.text = "";

        StartCoroutine(Responder(msg));
    }

    IEnumerator Responder(string msg)
    {
        if (msg.StartsWith("ver ") || msg == "ver")
        {
            string rest = msg == "ver" ? "" : msg.Substring(4).Trim();
            string result = HandleVerCommand(rest);
            chatDisplay.text += "<color=#9CDCFE>AI:</color> " + result + "\n";
            chatHistory = chatDisplay.text;
            yield break;
        }

        if (msg == "volver" && inCodeView)
        {
            ShowProjectOverview();
            chatDisplay.text += "<color=#9CDCFE>AI:</color> Volviendo al panel principal.\n";
            chatHistory = chatDisplay.text;
            yield break;
        }

        chatDisplay.text += "<color=#9CDCFE>AI:</color> <color=#6A9955>escribiendo...</color>\n";
        yield return new WaitForSeconds(0.6f);

        int idx = chatDisplay.text.LastIndexOf("<color=#9CDCFE>AI:</color> <color=#6A9955>escribiendo...</color>");
        if (idx >= 0)
            chatDisplay.text = chatDisplay.text.Substring(0, idx);

        if (msg == "estado" || msg == "cancelar" || msg == "tareas")
        {
            string r;
            if (msg == "estado") r = GetProjectStatusResponse();
            else if (msg == "cancelar") r = "Tarea cancelada. ¿Que modulo sigue?\n<color=#569CD6>Opciones:</color> " + GetAvailableTasksText();
            else r = "Estas son las tareas disponibles:\n<color=#569CD6>Opciones:</color> " + GetAvailableTasksText();
            chatDisplay.text += "<color=#9CDCFE>AI:</color> " + r + "\n";
            chatHistory = chatDisplay.text;
            yield break;
        }

        ProjectTask matchedTask = null;
        foreach (ProjectTask t in tasks)
        {
            if (msg == t.keyword)
            {
                matchedTask = t;
                break;
            }
        }

        if (matchedTask != null)
        {
            if (matchedTask.completed)
            {
                chatDisplay.text += "<color=#9CDCFE>AI:</color> Ese modulo ya esta completado! Eleji otro.\n<color=#569CD6>Opciones:</color> " + GetAvailableTasksText() + "\n";
            }
            else
            {
                StartTask(matchedTask);
                if (responses.ContainsKey(msg))
                    chatDisplay.text += responses[msg] + "\n";
                else
                    chatDisplay.text += matchedTask.responseText + "\n";
            }
        }
        else if (responses.ContainsKey(msg))
        {
            chatDisplay.text += responses[msg] + "\n";
        }
        else
        {
            chatDisplay.text += "<color=#9CDCFE>AI:</color> No entendi. Estas son las opciones:\n" + GetAvailableTasksText() + "\n";
        }

        chatHistory = chatDisplay.text;

        ScrollChatToBottom();
    }

    string HandleVerCommand(string target)
    {
        if (string.IsNullOrEmpty(target))
        {
            string result = "Que queres ver?\n";
            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].completed)
                    result += "- <color=#569CD6>" + tasks[i].keyword + "</color> (codigo)\n";
            }
            result += "Escribi 'ver [modulo]' para ver el codigo.\n<color=#569CD6>Opciones:</color> " + GetAvailableTasksText();
            return result;
        }

        for (int i = 0; i < tasks.Count; i++)
        {
            if (target == tasks[i].keyword)
            {
                if (!tasks[i].completed)
                    return "Ese modulo aun no esta completado. Terminalo primero!\n<color=#569CD6>Opciones:</color> " + GetAvailableTasksText();

                ShowCodeView(i, 0);
                return "Mostrando: <color=#4EC9B0>" + tasks[i].files[0] + "</color>\n"
                    + "Escribi <color=#569CD6>siguiente</color> / <color=#569CD6>anterior</color> para cambiar de archivo.\n"
                    + "Escribi <color=#569CD6>volver</color> para salir del editor.\n"
                    + "<color=#6A9955>Tip: Podes editar el codigo directamente.</color>";
            }
        }

        return "No encontre '" + target + "'. Usa 'ver [modulo]'.\n<color=#569CD6>Opciones:</color> ver codigo | " + GetAvailableTasksText();
    }

    void StartTask(ProjectTask task)
    {
        int taskIndex = tasks.IndexOf(task);
        if (taskIndex < 0) return;

        currentTaskIndex = taskIndex;
        currentProgress = 0f;
        isLoading = true;

        PCTimer timer = GetComponent<PCTimer>();
        if (timer != null)
        {
            timer.ResetTimer();
            timer.IniciarTimer();
        }
    }

    string GetAvailableTasksText()
    {
        List<string> available = new List<string>();
        foreach (ProjectTask t in tasks)
        {
            if (!t.completed)
                available.Add("<color=#569CD6>" + t.keyword + "</color>");
        }
        if (available.Count == 0)
            return "<color=#4EC9B0>🎉 Todas las tareas completadas!</color>";
        return string.Join(" | ", available);
    }

    string GetProjectStatusResponse()
    {
        int completed = 0;
        foreach (var t in tasks) { if (t.completed) completed++; }

        string status = "<color=#DCDCAA>Estado del proyecto:</color>\n";
        status += string.Format("Completado: {0}/{1} modulos ({2}%)\n",
            completed, tasks.Count, Mathf.RoundToInt((float)completed / tasks.Count * 100f));

        if (isLoading && currentTaskIndex >= 0)
        {
            float pct = Mathf.Clamp01(currentProgress / taskDuration);
            status += string.Format("Cargando: {0} - {1}%\n", tasks[currentTaskIndex].name, Mathf.RoundToInt(pct * 100f));
        }

        status += "\n<color=#569CD6>Opciones:</color> " + GetAvailableTasksText();
        if (completed > 0)
            status += " | <color=#569CD6>ver codigo</color>";
        return status;
    }

    public void Cerrar()
    {
        PCTimer timer = GetComponent<PCTimer>();
        if (timer != null) timer.DetenerTimer();
        GetComponent<Image>().raycastTarget = false;
        gameObject.SetActive(false);
    }

    public static int GetModuleCount() { return tasks?.Count ?? 0; }
    public static bool IsModuleCompleted(int index) { return tasks != null && index >= 0 && index < tasks.Count && tasks[index].completed; }
    public static string GetModuleName(int index) { return tasks != null && index >= 0 && index < tasks.Count ? tasks[index].name : ""; }

    public static void ResetStatics()
    {
        initialized = false;
        tasks = null;
        currentTaskIndex = -1;
        currentProgress = 0f;
        isLoading = false;
        chatHistory = "";
        lastChatText = "";
    }
}
