using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Mundo Pujllay - Detectar errores - Mision 3: Paso sobrante
// Guardar como JuegoPasoSobrante.cs
public class JuegoPasoSobrante : MonoBehaviour
{
    // =========================================================
    // 01. PREGUNTA E INSTRUCCIONES
    // =========================================================
    [Header("01 - PREGUNTA E INSTRUCCIONES")]
    public TMP_Text txtPregunta;
    [Range(1f, 5f)] public float tiempoPregunta = 2f;

    // =========================================================
    // 02. FONDOS Y TITULOS
    // =========================================================
    [Header("02 - FONDOS Y TITULOS")]
    public Image imagenFondo;
    public Image imagenTitulo;
    public Sprite fondoBanda, fondoOceano, fondoParque;
    public Sprite tituloBanda, tituloOceano, tituloParque;

    // =========================================================
    // 03. CUATRO TARJETAS Y ZONA DE DESCARTE
    // =========================================================
    [Header("03 - TARJETAS Y DESCARTE")]
    [Tooltip("Asignar Tarjeta_1, Tarjeta_2, Tarjeta_3, Tarjeta_4 (Button)")]
    public Button[] tarjetas = new Button[4];
    [Tooltip("Image del area azul de descarte. Puede tener un Button en el mismo objeto.")]
    public Image zonaDescarte;
    [Tooltip("Texto opcional dentro del area de descarte")]
    public TMP_Text textoDescarte;

    // =========================================================
    // 04. EJERCICIO 1: BANDA MUSICAL
    // =========================================================
    [Header("04 - EJERCICIO 1: BANDA MUSICAL")]
    public Sprite bandaAfinarInstrumentos;
    public Sprite bandaPrepararBateria;
    public Sprite bandaProbarMicrofonos;
    public Sprite bandaLavarAutomovil; // Sobrante

    // =========================================================
    // 05. EJERCICIO 2: EXPLORAR EL OCEANO
    // =========================================================
    [Header("05 - EJERCICIO 2: EXPLORAR EL OCEANO")]
    public Sprite oceanoRevisarOxigeno;
    public Sprite oceanoPrepararCamara;
    public Sprite oceanoRevisarTraje;
    public Sprite oceanoHornearPastel; // Sobrante

    // =========================================================
    // 06. EJERCICIO 3: PARQUE DE DIVERSIONES
    // =========================================================
    [Header("06 - EJERCICIO 3: PARQUE DE DIVERSIONES")]
    public Sprite parquePrepararTerreno;
    public Sprite parqueMontarAtracciones;
    public Sprite parqueRevisarSeguridad;
    public Sprite parquePescarEnRio; // Sobrante

    // =========================================================
    // 07. BOTONES
    // =========================================================
    [Header("07 - BOTONES")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // =========================================================
    // 08. CIRCULOS DE PROGRESO
    // =========================================================
    [Header("08 - CIRCULOS DE PROGRESO")]
    public RectTransform panelProgreso;
    [Min(10f)] public float tamanoCirculo = 28f;
    [Min(0f)] public float separacionCirculos = 38f;
    public float posicionVerticalCirculos = -20f;
    public Color colorActivo = new Color(1f, .78f, .24f, 1f);
    public Color colorPendiente = new Color(.78f, .76f, .72f, 1f);
    public Color colorBorde = new Color(.48f, .30f, .65f, 1f);
    [Range(0f, 12f)] public float grosorBorde = 3f;
    public Color colorNumero = Color.white;

    // =========================================================
    // 09. COLORES Y VARIACIONES
    // =========================================================
    [Header("09 - COLORES DE TARJETAS")]
    public Color colorNormal = Color.white;
    public Color colorSeleccionado = new Color(.73f, .89f, 1f, 1f);
    public Color colorCorrecto = new Color(.68f, 1f, .73f, 1f);
    public Color colorError = new Color(1f, .76f, .76f, 1f);
    [Header("10 - VARIACIONES")]
    public bool mezclarTarjetas = true;

    private class Ejercicio
    {
        public string pregunta;
        public Sprite fondo, titulo;
        public Sprite[] pasos;
        public int sobranteOriginal;
    }

    private readonly List<Ejercicio> ejercicios = new List<Ejercicio>();
    private readonly int[] indicesTarjetas = new int[4];
    private Image[] circulos;
    private Sprite circuloSprite;
    private int ejercicioActual;
    private int tarjetaSeleccionada = -1;
    private int tarjetaDescartada = -1;
    private bool completado;
    private Coroutine rutinaPregunta;

    void Start()
    {
        if (!Validar()) { enabled = false; return; }
        CrearEjercicios();
        PrepararInteracciones();
        CrearIndicadorProgreso();
        CargarEjercicio();
    }

    bool Validar()
    {
        if (txtPregunta == null || zonaDescarte == null || btnComprobar == null || btnSiguiente == null || panelProgreso == null)
        {
            Debug.LogError("JuegoPasoSobrante: asigna Txt_Pregunta, Zona_Descarte, botones y Panel_Progreso.");
            return false;
        }
        if (tarjetas == null || tarjetas.Length != 4)
        {
            Debug.LogError("JuegoPasoSobrante: asigna exactamente 4 tarjetas Button.");
            return false;
        }
        foreach (Button tarjeta in tarjetas)
        {
            if (tarjeta == null) { Debug.LogError("JuegoPasoSobrante: falta una tarjeta."); return false; }
        }
        return true;
    }

    void CrearEjercicios()
    {
        ejercicios.Clear();
        ejercicios.Add(new Ejercicio {
            pregunta = "¿Qué paso sobra al organizar una banda musical?",
            fondo = fondoBanda, titulo = tituloBanda,
            pasos = new Sprite[] { bandaAfinarInstrumentos, bandaPrepararBateria, bandaProbarMicrofonos, bandaLavarAutomovil },
            sobranteOriginal = 3
        });
        ejercicios.Add(new Ejercicio {
            pregunta = "¿Qué paso no pertenece a la exploración del océano?",
            fondo = fondoOceano, titulo = tituloOceano,
            pasos = new Sprite[] { oceanoRevisarOxigeno, oceanoPrepararCamara, oceanoRevisarTraje, oceanoHornearPastel },
            sobranteOriginal = 3
        });
        ejercicios.Add(new Ejercicio {
            pregunta = "¿Qué paso sobra al construir el parque de diversiones?",
            fondo = fondoParque, titulo = tituloParque,
            pasos = new Sprite[] { parquePrepararTerreno, parqueMontarAtracciones, parqueRevisarSeguridad, parquePescarEnRio },
            sobranteOriginal = 3
        });
    }

    void PrepararInteracciones()
    {
        for (int i = 0; i < tarjetas.Length; i++)
        {
            int indice = i;
            tarjetas[i].onClick.AddListener(() => SeleccionarTarjeta(indice));
            tarjetas[i].transition = Selectable.Transition.None;
            ArrastrarTarjetaSobrante drag = tarjetas[i].GetComponent<ArrastrarTarjetaSobrante>();
            if (drag == null) drag = tarjetas[i].gameObject.AddComponent<ArrastrarTarjetaSobrante>();
            drag.Configurar(this, indice);
        }
        zonaDescarte.raycastTarget = true;
        SoltarPasoSobrante drop = zonaDescarte.GetComponent<SoltarPasoSobrante>();
        if (drop == null) drop = zonaDescarte.gameObject.AddComponent<SoltarPasoSobrante>();
        drop.Configurar(this);
        Button botonZona = zonaDescarte.GetComponent<Button>();
        if (botonZona == null) botonZona = zonaDescarte.gameObject.AddComponent<Button>();
        botonZona.transition = Selectable.Transition.None;
        botonZona.onClick.AddListener(DescartarSeleccionada);
        btnComprobar.onClick.AddListener(Comprobar);
        btnSiguiente.onClick.AddListener(SiguienteEjercicio);
    }

    void CargarEjercicio()
    {
        DetenerPregunta();
        completado = false;
        tarjetaSeleccionada = -1;
        tarjetaDescartada = -1;
        Ejercicio e = ejercicios[ejercicioActual];
        if (imagenFondo != null && e.fondo != null) imagenFondo.sprite = e.fondo;
        if (imagenTitulo != null && e.titulo != null) imagenTitulo.sprite = e.titulo;
        for (int i = 0; i < 4; i++) indicesTarjetas[i] = i;
        if (mezclarTarjetas)
        {
            for (int i = 3; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int tmp = indicesTarjetas[i]; indicesTarjetas[i] = indicesTarjetas[j]; indicesTarjetas[j] = tmp;
            }
        }
        for (int i = 0; i < 4; i++)
        {
            tarjetas[i].gameObject.SetActive(true);
            tarjetas[i].interactable = true;
            tarjetas[i].image.sprite = e.pasos[indicesTarjetas[i]];
            tarjetas[i].image.color = colorNormal;
            tarjetas[i].image.preserveAspect = false;
        }
        zonaDescarte.color = new Color(.72f, .86f, 1f, 1f);
        if (textoDescarte != null) textoDescarte.text = "ARRASTRA AQUÍ EL PASO SOBRANTE";
        btnComprobar.gameObject.SetActive(true);
        btnComprobar.interactable = true;
        btnSiguiente.gameObject.SetActive(false);
        ActualizarIndicadorProgreso();
        rutinaPregunta = StartCoroutine(MostrarPreguntaTemporal(e.pregunta));
    }

    IEnumerator MostrarPreguntaTemporal(string pregunta)
    {
        txtPregunta.text = pregunta;
        yield return new WaitForSeconds(tiempoPregunta);
        txtPregunta.text = "¡Encuentra el paso que sobra y llévalo al descarte!";
        rutinaPregunta = null;
    }

    void DetenerPregunta()
    {
        if (rutinaPregunta != null) { StopCoroutine(rutinaPregunta); rutinaPregunta = null; }
    }

    public void SeleccionarTarjeta(int indice)
    {
        if (completado || indice < 0 || indice >= tarjetas.Length) return;
        DetenerPregunta();
        tarjetaSeleccionada = indice;
        // Permite recuperar la tarjeta que se habia descartado.
        tarjetaDescartada = -1;
        ActualizarColoresTarjetas();
        txtPregunta.text = "¡Ahora toca la zona de descarte o arrastra la tarjeta!";
    }

    public void DescartarSeleccionada()
    {
        if (completado || tarjetaSeleccionada < 0) return;
        DescartarTarjeta(tarjetaSeleccionada);
    }

    public void DescartarTarjeta(int indice)
    {
        if (completado || indice < 0 || indice >= tarjetas.Length) return;
        DetenerPregunta();
        tarjetaDescartada = indice;
        tarjetaSeleccionada = indice;
        ActualizarColoresTarjetas();
        if (textoDescarte != null) textoDescarte.text = "TARJETA ELEGIDA ✓";
        txtPregunta.text = "¡Bien! Presiona COMPROBAR para revisar tu respuesta.";
    }

    void ActualizarColoresTarjetas()
    {
        for (int i = 0; i < tarjetas.Length; i++)
            tarjetas[i].image.color = i == tarjetaSeleccionada ? colorSeleccionado : colorNormal;
    }

    void Comprobar()
    {
        if (completado) return;
        DetenerPregunta();
        if (tarjetaDescartada < 0)
        {
            txtPregunta.text = "¡Primero lleva una tarjeta a la zona de descarte!";
            return;
        }
        if (indicesTarjetas[tarjetaDescartada] == ejercicios[ejercicioActual].sobranteOriginal)
        {
            completado = true;
            ProgresoEstrellas.GuardarEjercicio("DetectarErrores", 3, ejercicioActual + 1);
            tarjetas[tarjetaDescartada].image.color = colorCorrecto;
            foreach (Button tarjeta in tarjetas) tarjeta.interactable = false;
            if (textoDescarte != null) textoDescarte.text = "¡PASO SOBRANTE ENCONTRADO!";
            txtPregunta.text = "¡Excelente! ¡Encontraste el paso sobrante!";
            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);
        }
        else
        {
            tarjetas[tarjetaDescartada].image.color = colorError;
            tarjetaDescartada = -1;
            tarjetaSeleccionada = -1;
            if (textoDescarte != null) textoDescarte.text = "ARRASTRA AQUÍ EL PASO SOBRANTE";
            txtPregunta.text = "¡Casi! Observa las tarjetas y vuelve a intentarlo.";
        }
    }

    void SiguienteEjercicio()
    {
        if (!completado) return;
        ejercicioActual++;
        if (ejercicioActual < ejercicios.Count) CargarEjercicio();
        else FinalizarMision();
    }

    void FinalizarMision()
    {
        DetenerPregunta();
        txtPregunta.text = "¡MISIÓN COMPLETADA!";
        foreach (Button tarjeta in tarjetas) tarjeta.gameObject.SetActive(false);
        zonaDescarte.gameObject.SetActive(false);
        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);
        if (circulos != null) foreach (Image c in circulos) if (c != null) c.transform.parent.gameObject.SetActive(false);
    }

    // =========================================================
    // 11. CREAR CIRCULOS CON BORDES CONFIGURABLES
    // =========================================================
    Sprite CrearSpriteCircular()
    {
        const int n = 128;
        Texture2D t = new Texture2D(n, n, TextureFormat.RGBA32, false);
        t.filterMode = FilterMode.Bilinear;
        float centro = (n - 1) / 2f;
        float radio = centro - 1f;
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(centro, centro));
                t.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(radio - d + .5f)));
            }
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, n, n), new Vector2(.5f, .5f), 100f);
    }

    void CrearIndicadorProgreso()
    {
        circuloSprite = CrearSpriteCircular();
        circulos = new Image[ejercicios.Count];
        for (int i = 0; i < ejercicios.Count; i++)
        {
            GameObject bordeObj = new GameObject("Circulo_Progreso_" + (i + 1), typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            bordeObj.transform.SetParent(panelProgreso, false);
            RectTransform r = bordeObj.GetComponent<RectTransform>();
            r.anchorMin = r.anchorMax = new Vector2(.5f, .5f);
            r.pivot = new Vector2(.5f, .5f);
            r.sizeDelta = Vector2.one * tamanoCirculo;
            r.anchoredPosition = new Vector2((i - (ejercicios.Count - 1) / 2f) * separacionCirculos, posicionVerticalCirculos);
            Image borde = bordeObj.GetComponent<Image>();
            borde.sprite = circuloSprite;
            borde.color = colorBorde;
            borde.raycastTarget = false;

            GameObject rellenoObj = new GameObject("Relleno", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            rellenoObj.transform.SetParent(bordeObj.transform, false);
            RectTransform rr = rellenoObj.GetComponent<RectTransform>();
            rr.anchorMin = Vector2.zero;
            rr.anchorMax = Vector2.one;
            rr.offsetMin = new Vector2(grosorBorde, grosorBorde);
            rr.offsetMax = new Vector2(-grosorBorde, -grosorBorde);
            Image relleno = rellenoObj.GetComponent<Image>();
            relleno.sprite = circuloSprite;
            relleno.raycastTarget = false;
            circulos[i] = relleno;

            GameObject numObj = new GameObject("Numero_" + (i + 1), typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            numObj.transform.SetParent(bordeObj.transform, false);
            RectTransform nr = numObj.GetComponent<RectTransform>();
            nr.anchorMin = Vector2.zero; nr.anchorMax = Vector2.one;
            nr.offsetMin = nr.offsetMax = Vector2.zero;
            TextMeshProUGUI num = numObj.GetComponent<TextMeshProUGUI>();
            num.text = (i + 1).ToString();
            num.fontSize = tamanoCirculo * .5f;
            num.fontStyle = FontStyles.Bold;
            num.alignment = TextAlignmentOptions.Center;
            num.color = colorNumero;
            num.raycastTarget = false;
        }
    }

    void ActualizarIndicadorProgreso()
    {
        if (circulos == null) return;
        for (int i = 0; i < circulos.Length; i++)
            if (circulos[i] != null) circulos[i].color = i <= ejercicioActual ? colorActivo : colorPendiente;
    }

    void OnDisable() { DetenerPregunta(); }
}

// Arrastre: el jugador puede llevar una tarjeta al area de descarte.
public class ArrastrarTarjetaSobrante : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private JuegoPasoSobrante juego;
    private int indice;
    private GameObject fantasma;
    private Canvas canvas;

    public void Configurar(JuegoPasoSobrante controlador, int indiceTarjeta)
    {
        juego = controlador;
        indice = indiceTarjeta;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (juego == null || !GetComponent<Button>().interactable) return;
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        fantasma = new GameObject("Tarjeta_Arrastrada", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fantasma.transform.SetParent(canvas.transform, false);
        fantasma.transform.SetAsLastSibling();
        Image imagen = fantasma.GetComponent<Image>();
        imagen.sprite = GetComponent<Image>().sprite;
        imagen.color = Color.white;
        imagen.raycastTarget = false;
        fantasma.GetComponent<RectTransform>().sizeDelta = ((RectTransform)transform).rect.size;
        Mover(e);
    }

    public void OnDrag(PointerEventData e) { Mover(e); }

    void Mover(PointerEventData e)
    {
        if (fantasma == null || canvas == null) return;
        RectTransform cr = canvas.transform as RectTransform;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : e.pressEventCamera;
        Vector2 local;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(cr, e.position, cam, out local))
            fantasma.GetComponent<RectTransform>().anchoredPosition = local;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (fantasma != null) Destroy(fantasma);
        fantasma = null;
    }

    public void Descartar() { if (juego != null) juego.DescartarTarjeta(indice); }
}

// Soltar una tarjeta encima de la zona de descarte.
public class SoltarPasoSobrante : MonoBehaviour, IDropHandler
{
    private JuegoPasoSobrante juego;
    public void Configurar(JuegoPasoSobrante controlador) { juego = controlador; }
    public void OnDrop(PointerEventData e)
    {
        if (juego == null || e.pointerDrag == null) return;
        ArrastrarTarjetaSobrante tarjeta = e.pointerDrag.GetComponent<ArrastrarTarjetaSobrante>();
        if (tarjeta != null) tarjeta.Descartar();
    }
}
