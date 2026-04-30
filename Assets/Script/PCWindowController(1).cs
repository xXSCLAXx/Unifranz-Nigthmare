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

    private static string historialChat = "";

    private Dictionary<string, string> respuestas = new Dictionary<string, string>()
    {
        { "panel admin", "Buena elección. ¿Qué necesitás del panel admin?\n<color=#569CD6>Opciones:</color> login | usuarios | reportes | volver" },
        { "panel cliente", "Ok, panel cliente. ¿Por dónde arrancamos?\n<color=#569CD6>Opciones:</color> registro | carrito | historial | volver" },
        { "base de datos", "Base de datos, clásico. ¿Qué parte?\n<color=#569CD6>Opciones:</color> tablas | consultas | relaciones | volver" },
        { "api", "API REST con FastAPI. ¿Qué endpoint?\n<color=#569CD6>Opciones:</color> get | post | autenticacion | volver" },
        { "login", "Para el login usá JWT con FastAPI:\n<color=#4EC9B0>POST</color> /api/login\nBody: { email, password }\nResponse: { access_token, token_type }\n\n<color=#569CD6>Opciones:</color> mas detalle | volver" },
        { "usuarios", "Tabla usuarios en PostgreSQL:\nid SERIAL PRIMARY KEY\nnombre VARCHAR(100)\nemail VARCHAR(100) UNIQUE\nrol VARCHAR(20)\nfecha_alta TIMESTAMP\n\n<color=#569CD6>Opciones:</color> si dame el crud | no gracias | volver" },
        { "reportes", "Generá reportes con pandas + matplotlib en Python, exportá a PDF con reportlab.\n\n<color=#569CD6>Opciones:</color> ejemplo | volver" },
        { "registro", "Validá email único en PostgreSQL, hasheá con bcrypt en Python:\npwd_context.hash(password)\n\n<color=#569CD6>Opciones:</color> mas detalle | volver" },
        { "carrito", "El carrito puede ser en estado de Next.js (Zustand/Redux) o en PostgreSQL.\n\n<color=#569CD6>Opciones:</color> zustand | postgresql | volver" },
        { "historial", "Historial con JOIN en PostgreSQL:\nSELECT o.id, p.nombre FROM orders o\nJOIN productos p ON o.producto_id = p.id\nWHERE o.usuario_id = $1\n\n<color=#569CD6>Opciones:</color> volver" },
        { "tablas", "Tablas principales:\n- usuarios\n- productos\n- pedidos\n- detalle_pedido\n- categorias\n\n<color=#569CD6>Opciones:</color> ver diagrama | volver" },
        { "consultas", "¿Qué tipo de consulta?\n\n<color=#569CD6>Opciones:</color> select | insert | update | volver" },
        { "relaciones", "usuarios 1-N pedidos\npedidos N-N productos via detalle_pedido\ncategorias 1-N productos\n\n<color=#569CD6>Opciones:</color> ver diagrama | volver" },
        { "get", "En FastAPI:\n@app.get('/productos')\nasync def get_productos(db: Session):\n    return db.query(Producto).all()\n\n<color=#569CD6>Opciones:</color> volver" },
        { "post", "En FastAPI:\n@app.post('/productos')\nasync def crear(p: ProductoSchema, db: Session):\n    nuevo = Producto(**p.dict())\n    db.add(nuevo)\n    db.commit()\n    return nuevo\n\n<color=#569CD6>Opciones:</color> volver" },
        { "autenticacion", "JWT con python-jose:\nfrom jose import jwt\ntoken = jwt.encode(data, SECRET_KEY)\n\nEn Next.js usá middleware para proteger rutas.\n\n<color=#569CD6>Opciones:</color> volver" },
        { "si dame el crud", "CRUD en FastAPI:\nGET /productos\nGET /productos/{id}\nPOST /productos\nPUT /productos/{id}\nDELETE /productos/{id}\n\n<color=#569CD6>Opciones:</color> volver" },
        { "zustand", "Instalá: npm install zustand\nconst useCart = create((set) => ({\n  items: [],\n  add: (item) => set((s) => ({items: [...s.items, item]}))\n}))\n\n<color=#569CD6>Opciones:</color> volver" },
        { "postgresql", "Tabla cart_items:\nid, usuario_id, producto_id, cantidad\nAl checkout movés todo a pedidos.\n\n<color=#569CD6>Opciones:</color> volver" },
        { "ver diagrama", "usuarios --< pedidos --< detalle_pedido >-- productos\ncategorias --< productos\n\n<color=#569CD6>Opciones:</color> volver" },
        { "select", "SELECT * FROM productos\nWHERE categoria_id = $1\nORDER BY nombre\nLIMIT 10 OFFSET $2;\n\n<color=#569CD6>Opciones:</color> volver" },
        { "insert", "INSERT INTO productos\n(nombre, precio, categoria_id)\nVALUES ($1, $2, $3)\nRETURNING id;\n\n<color=#569CD6>Opciones:</color> volver" },
        { "update", "UPDATE productos\nSET precio = $1\nWHERE id = $2\nRETURNING *;\n\n<color=#569CD6>Opciones:</color> volver" },
        { "ejemplo", "from reportlab.pdfgen import canvas\nc = canvas.Canvas('reporte.pdf')\nc.drawString(100, 750, 'Reporte de ventas')\nc.save()\n\n<color=#569CD6>Opciones:</color> volver" },
        { "mas detalle", "¿Qué parte querés profundizar?\n\n<color=#569CD6>Opciones:</color> volver" },
        { "no gracias", "Ok, ¿qué más necesitás?\n\n<color=#569CD6>Opciones:</color> panel admin | panel cliente | base de datos | api" },
        { "volver", "¿Con qué módulo seguimos?\n<color=#569CD6>Opciones:</color> panel admin | panel cliente | base de datos | api" },
    };

    void OnEnable()
    {
        codeDisplay.text =
            "<color=#569CD6>📁</color> <color=#DCDCAA>NexoMarket</color>\n" +
            "<color=#569CD6>├──</color> <color=#569CD6>📁</color> <color=#4EC9B0>backend</color>\n" +
            "<color=#569CD6>│   ├──</color> <color=#569CD6>📁</color> <color=#4EC9B0>app</color>\n" +
            "<color=#569CD6>│   │   ├──</color> <color=#DCDCAA>main.py</color>\n" +
            "<color=#569CD6>│   │   ├──</color> <color=#569CD6>📁</color> <color=#4EC9B0>routers</color>\n" +
            "<color=#569CD6>│   │   │   ├──</color> <color=#9CDCFE>auth.py</color>\n" +
            "<color=#569CD6>│   │   │   ├──</color> <color=#9CDCFE>usuarios.py</color>\n" +
            "<color=#569CD6>│   │   │   ├──</color> <color=#9CDCFE>productos.py</color>\n" +
            "<color=#569CD6>│   │   │   └──</color> <color=#9CDCFE>pedidos.py</color>\n" +
            "<color=#569CD6>│   │   ├──</color> <color=#569CD6>📁</color> <color=#4EC9B0>models</color>\n" +
            "<color=#569CD6>│   │   │   ├──</color> <color=#9CDCFE>usuario.py</color>\n" +
            "<color=#569CD6>│   │   │   ├──</color> <color=#9CDCFE>producto.py</color>\n" +
            "<color=#569CD6>│   │   │   └──</color> <color=#9CDCFE>pedido.py</color>\n" +
            "<color=#569CD6>│   │   ├──</color> <color=#569CD6>📁</color> <color=#4EC9B0>schemas</color>\n" +
            "<color=#569CD6>│   │   │   ├──</color> <color=#9CDCFE>usuario.py</color>\n" +
            "<color=#569CD6>│   │   │   └──</color> <color=#9CDCFE>producto.py</color>\n" +
            "<color=#569CD6>│   │   └──</color> <color=#569CD6>📁</color> <color=#4EC9B0>database</color>\n" +
            "<color=#569CD6>│   │       └──</color> <color=#9CDCFE>db.py</color>\n" +
            "<color=#569CD6>│   ├──</color> <color=#DCDCAA>requirements.txt</color>\n" +
            "<color=#569CD6>│   └──</color> <color=#DCDCAA>.env</color>\n" +
            "<color=#569CD6>└──</color> <color=#569CD6>📁</color> <color=#4EC9B0>frontend</color>\n"+
            "<color=#569CD6>    └──</color> <color=#DCDCAA>next.config.js</color>";

        if (historialChat != "")
        {
            chatDisplay.text = historialChat;
        }
        else
        {
            chatDisplay.text =
                "<color=#569CD6>● NexoMarket AI Assistant</color>\n" +
                "<color=#6A9955>──────────────────────────</color>\n\n" +
                "<color=#9CDCFE>AI:</color> Hola! Soy tu asistente del proyecto <color=#4EC9B0>NexoMarket</color>.\n" +
                "Stack: <color=#DCDCAA>Python</color> + <color=#4EC9B0>FastAPI</color> | <color=#DCDCAA>Next.js</color> | <color=#4EC9B0>PostgreSQL</color>\n\n" +
                "<color=#9CDCFE>AI:</color> ¿Con qué módulo arrancamos?\n" +
                "<color=#569CD6>Opciones:</color> panel admin | panel cliente | base de datos | api\n";
            historialChat = chatDisplay.text;
        }
    }

    public void OnSendMessage()
    {
        if (string.IsNullOrEmpty(playerInput.text)) return;

        string msg = playerInput.text.ToLower().Trim();
        chatDisplay.text += "\n<color=#DCDCAA>Vos:</color> " + msg;
        playerInput.text = "";

        StartCoroutine(Responder(msg));
    }

    IEnumerator Responder(string msg)
    {
        chatDisplay.text += "\n<color=#9CDCFE>AI:</color> <color=#6A9955>escribiendo...</color>";
        yield return new WaitForSeconds(0.8f);

        int idx = chatDisplay.text.LastIndexOf("\n<color=#9CDCFE>AI:</color> <color=#6A9955>escribiendo...</color>");
        chatDisplay.text = chatDisplay.text.Substring(0, idx);

        if (respuestas.ContainsKey(msg))
            chatDisplay.text += "\n<color=#9CDCFE>AI:</color> " + respuestas[msg] + "\n";
        else
            chatDisplay.text += "\n<color=#9CDCFE>AI:</color> No entendí eso. Escribí una de las opciones disponibles.\n";

        historialChat = chatDisplay.text;
    }

    public void Cerrar()
    {
        PCTimer timer = GetComponent<PCTimer>();
        if (timer != null) timer.DetenerTimer();
        GetComponent<Image>().raycastTarget = false;
        gameObject.SetActive(false);
    }
}