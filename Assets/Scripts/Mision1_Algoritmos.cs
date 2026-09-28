using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Mision1_Algoritmos : MonoBehaviour
{
    [Header("Título de la misión")]
    public string tituloMision = "MISIÓN 1: Ordena el algoritmo";
    public string instruccion = "Arrastra las tarjetas al orden correcto";

    [Header("Colores")]
    public Color colorFondo = new Color(0.10f, 0.14f, 0.25f);
    public Color colorTarjeta = new Color(1f, 0.85f, 0.35f);
    public Color colorSlot = new Color(1f, 1f, 1f, 0.15f);
    public Color colorBoton = new Color(0.20f, 0.70f, 0.40f);
    public Color colorTexto = Color.white;

    private List<Paso> pasos = new List<Paso>()
    {
        new Paso("Tomar el cepillo",       1),
        new Paso("Poner pasta dental",     2),
        new Paso("Cepillar los dientes",   3),
        new Paso("Enjuagar la boca",       4),
        new Paso("Guardar el cepillo",     5),
    };

    private Canvas canvas;
    private RectTransform zonaDesordenada;
    private RectTransform zonaSecuencia;
    private Text textoFeedback;
    private Text textoTitulo;
    private Text textoInstruccion;
    private Font fuente;

    private List<TarjetaPaso> tarjetas = new List<TarjetaPaso>();
    private List<RectTransform> slots = new List<RectTransform>();
    private List<TarjetaPaso> tarjetasEnSecuencia = new List<TarjetaPaso>();

    [System.Serializable]
    public class Paso
    {
        public string texto;
        public int ordenCorrecto;
        public Paso(string t, int o) { texto = t; ordenCorrecto = o; }
    }

    void Start()
    {
        fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (fuente == null) fuente = Resources.GetBuiltinResource<Font>("Arial.ttf");

        CrearEventSystem();
        CrearCanvas();
        CrearFondo();
        CrearTitulo();
        CrearZonas();
        CrearSlots();
        CrearTarjetas();
        CrearBotonComprobar();
        CrearTextoFeedback();
        CrearBotonReiniciar();
    }

    void CrearEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    void CrearCanvas()
    {
        GameObject canvasGO = new GameObject("Canvas");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();
    }

    void CrearFondo()
    {
        GameObject fondo = new GameObject("Fondo");
        fondo.transform.SetParent(canvas.transform, false);

        Image img = fondo.AddComponent<Image>();
        img.color = colorFondo;

        RectTransform rt = fondo.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    void CrearTitulo()
    {
        textoTitulo = CrearTexto(
            canvas.transform,
            tituloMision,
            36,
            TextAnchor.MiddleCenter,
            new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(0, -50),
            new Vector2(1000, 60)
        );
        textoTitulo.color = colorTarjeta;
        textoTitulo.fontStyle = FontStyle.Bold;

        textoInstruccion = CrearTexto(
            canvas.transform,
            instruccion,
            20,
            TextAnchor.MiddleCenter,
            new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(0, -100),
            new Vector2(1000, 40)
        );
        textoInstruccion.color = colorTexto;
    }

    void CrearZonas()
    {
        GameObject zonaA = new GameObject("ZonaDesordenada");
        zonaA.transform.SetParent(canvas.transform, false);
        zonaDesordenada = zonaA.AddComponent<RectTransform>();
        zonaDesordenada.anchorMin = new Vector2(0.5f, 0.65f);
        zonaDesordenada.anchorMax = new Vector2(0.5f, 0.65f);
        zonaDesordenada.pivot = new Vector2(0.5f, 0.5f);
        zonaDesordenada.sizeDelta = new Vector2(1100, 160);
        zonaDesordenada.anchoredPosition = Vector2.zero;

        HorizontalLayoutGroup hlg = zonaA.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 20;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        GameObject zonaB = new GameObject("ZonaSecuencia");
        zonaB.transform.SetParent(canvas.transform, false);
        zonaSecuencia = zonaB.AddComponent<RectTransform>();
        zonaSecuencia.anchorMin = new Vector2(0.5f, 0.30f);
        zonaSecuencia.anchorMax = new Vector2(0.5f, 0.30f);
        zonaSecuencia.pivot = new Vector2(0.5f, 0.5f);
        zonaSecuencia.sizeDelta = new Vector2(1100, 160);
        zonaSecuencia.anchoredPosition = Vector2.zero;

        HorizontalLayoutGroup hlg2 = zonaB.AddComponent<HorizontalLayoutGroup>();
        hlg2.spacing = 20;
        hlg2.childAlignment = TextAnchor.MiddleCenter;
        hlg2.childForceExpandWidth = false;
        hlg2.childForceExpandHeight = false;
    }

    void CrearSlots()
    {
        int total = pasos.Count;
        for (int i = 0; i < total; i++)
        {
            GameObject slotGO = new GameObject("Slot_" + (i + 1));
            slotGO.transform.SetParent(zonaSecuencia, false);

            Image img = slotGO.AddComponent<Image>();
            img.color = colorSlot;

            RectTransform rt = slotGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(190, 140);

            Text numero = CrearTexto(
                slotGO.transform,
                (i + 1).ToString(),
                28,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(190, 140)
            );
            numero.color = new Color(1, 1, 1, 0.25f);
            numero.fontStyle = FontStyle.Bold;

            SlotSecuencia receptor = slotGO.AddComponent<SlotSecuencia>();
            receptor.indice = i;
            receptor.manager = this;

            slots.Add(rt);
        }
    }

    void CrearTarjetas()
    {
        var mezclados = pasos.OrderBy(x => Random.value).ToList();

        foreach (var paso in mezclados)
        {
            GameObject tarjetaGO = new GameObject("Tarjeta_" + paso.texto);
            tarjetaGO.transform.SetParent(zonaDesordenada, false);

            Image img = tarjetaGO.AddComponent<Image>();
            img.color = colorTarjeta;

            RectTransform rt = tarjetaGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(190, 140);

            CanvasGroup cg = tarjetaGO.AddComponent<CanvasGroup>();

            Text txt = CrearTexto(
                tarjetaGO.transform,
                paso.texto,
                16,
                TextAnchor.MiddleCenter,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero
            );
            txt.color = Color.black;
            txt.fontStyle = FontStyle.Bold;

            TarjetaPaso tarjeta = tarjetaGO.AddComponent<TarjetaPaso>();
            tarjeta.Configurar(paso, this, cg);

            tarjetas.Add(tarjeta);
        }
    }

    void CrearBotonComprobar()
    {
        CrearBoton(
            canvas.transform,
            "¡Comprobar!",
            new Vector2(0.5f, 0.10f),
            new Vector2(280, 70),
            colorBoton,
            () => Comprobar()
        );
    }

    void CrearBotonReiniciar()
    {
        CrearBoton(
            canvas.transform,
            "Reiniciar",
            new Vector2(0.85f, 0.10f),
            new Vector2(180, 70),
            new Color(0.8f, 0.3f, 0.3f),
            () => SceneManager.LoadScene(SceneManager.GetActiveScene().name)
        );
    }

    void CrearTextoFeedback()
    {
        textoFeedback = CrearTexto(
            canvas.transform,
            "",
            26,
            TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.50f),
            new Vector2(0.5f, 0.50f),
            new Vector2(0, 0),
            new Vector2(900, 60)
        );
        textoFeedback.fontStyle = FontStyle.Bold;
    }

    public void AlSoltarTarjeta(TarjetaPaso tarjeta, int indiceSlot)
    {
        if (tarjetasEnSecuencia.Contains(tarjeta))
            tarjetasEnSecuencia.Remove(tarjeta);

        TarjetaPaso anterior = tarjetasEnSecuencia.FirstOrDefault(t => t.slotActual == indiceSlot);
        if (anterior != null && anterior != tarjeta)
        {
            anterior.transform.SetParent(zonaDesordenada, false);
            anterior.slotActual = -1;
            tarjetasEnSecuencia.Remove(anterior);
        }

        tarjeta.slotActual = indiceSlot;
        tarjetasEnSecuencia.Add(tarjeta);
    }

    void Comprobar()
    {
        if (tarjetasEnSecuencia.Count != pasos.Count)
        {
            textoFeedback.text = "⚠️ Faltan tarjetas por colocar.";
            textoFeedback.color = new Color(1f, 0.8f, 0.2f);
            return;
        }

        for (int i = 0; i < pasos.Count; i++)
        {
            TarjetaPaso t = tarjetasEnSecuencia.FirstOrDefault(x => x.slotActual == i);
            if (t == null || t.paso.ordenCorrecto != i + 1)
            {
                textoFeedback.text = "❌ ¡Casi! Revisa el orden.";
                textoFeedback.color = new Color(1f, 0.4f, 0.4f);
                return;
            }
        }

        textoFeedback.text = "✅ ¡Algoritmo correcto! ¡Bien hecho!";
        textoFeedback.color = new Color(0.3f, 1f, 0.5f);

        StartCoroutine(SiguienteMisionTras(2f));
    }

    IEnumerator SiguienteMisionTras(float seg)
    {
        yield return new WaitForSeconds(seg);
        Debug.Log("¡Misión 1 completada!");
    }

    Text CrearTexto(Transform padre, string contenido, int tamano, TextAnchor anchor,
                    Vector2 anchorMin, Vector2 anchorMax, Vector2 posicion, Vector2 tamanoRT)
    {
        GameObject go = new GameObject("Texto");
        go.transform.SetParent(padre, false);

        Text txt = go.AddComponent<Text>();
        txt.text = contenido;
        txt.font = fuente;
        txt.fontSize = tamano;
        txt.alignment = anchor;
        txt.color = colorTexto;
        txt.horizontalOverflow = HorizontalWrapMode.Wrap;
        txt.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = tamanoRT;
        rt.anchoredPosition = posicion;

        return txt;
    }

    Button CrearBoton(Transform padre, string textoBoton, Vector2 posicion,
                      Vector2 tamano, Color color, UnityEngine.Events.UnityAction accion)
    {
        GameObject go = new GameObject("Boton_" + textoBoton);
        go.transform.SetParent(padre, false);

        Image img = go.AddComponent<Image>();
        img.color = color;

        Button btn = go.AddComponent<Button>();
        btn.onClick.AddListener(accion);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = posicion;
        rt.anchorMax = posicion;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = tamano;
        rt.anchoredPosition = Vector2.zero;

        Text txt = CrearTexto(
            go.transform,
            textoBoton,
            20,
            TextAnchor.MiddleCenter,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero
        );
        txt.color = Color.white;
        txt.fontStyle = FontStyle.Bold;

        return btn;
    }
}