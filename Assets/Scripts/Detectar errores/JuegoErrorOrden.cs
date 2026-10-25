using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JuegoErrorOrden : MonoBehaviour
{
    // ============================================================
    // GRUPO 1 - PREGUNTAS E INSTRUCCIONES
    // ============================================================
    [Header("01 - PREGUNTAS E INSTRUCCIONES")]
    public TMP_Text txtPregunta;

    [Range(1f, 5f)]
    public float tiempoPregunta = 2f;

    // ============================================================
    // GRUPO 2 - FONDOS Y TITULOS
    // ============================================================
    [Header("02 - FONDOS Y TITULOS (OBJETOS)")]
    public Image imagenFondo;
    public Image imagenTitulo;

    [Header("02A - FONDOS DE CADA EJERCICIO")]
    public Sprite fondoComputadora;
    public Sprite fondoPizza;
    public Sprite fondoDron;

    [Header("02B - TITULOS DE CADA EJERCICIO")]
    public Sprite tituloComputadora;
    public Sprite tituloPizza;
    public Sprite tituloDron;

    // ============================================================
    // GRUPO 3 - TARJETAS DE LA CUADRICULA 2 X 2
    // ============================================================
    [Header("03 - CUATRO TARJETAS (BUTTON)")]
    public Button[] tarjetas = new Button[4];

    // ============================================================
    // GRUPO 4 - IMAGENES DE LOS EJERCICIOS (ORDEN CORRECTO)
    // ============================================================
    [Header("04A - EJERCICIO 1: COMPUTADORA")]
    public Sprite pcConectar;
    public Sprite pcEncender;
    public Sprite pcIniciarSesion;
    public Sprite pcAbrirPrograma;

    [Header("04B - EJERCICIO 2: PIZZA")]
    public Sprite pizzaPrepararMasa;
    public Sprite pizzaAgregarSalsa;
    public Sprite pizzaAgregarIngredientes;
    public Sprite pizzaHornear;

    [Header("04C - EJERCICIO 3: DRON")]
    public Sprite dronRevisar;
    public Sprite dronDespegar;
    public Sprite dronExplorar;
    public Sprite dronAterrizar;

    // ============================================================
    // GRUPO 5 - BOTONES DEL JUEGO
    // ============================================================
    [Header("05 - BOTONES")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // ============================================================
    // GRUPO 6 - CIRCULOS DE PROGRESO
    // ============================================================
    [Header("06A - PANEL Y POSICION DEL PROGRESO")]
    public RectTransform panelProgreso;
    public float tamanoCirculo = 28f;
    public float separacionCirculos = 38f;
    public float posicionVerticalCirculos = -20f;

    [Header("06B - COLORES DE PROGRESO")]
    public Color colorActivo = new Color(1f, 0.78f, 0.24f, 1f);
    public Color colorPendiente = new Color(0.78f, 0.76f, 0.72f, 1f);
    public Color colorNumero = Color.white;

    [Header("06C - BORDE DE LOS CIRCULOS")]
    public Color colorBorde = new Color(0.37f, 0.24f, 0.65f, 1f);

    [Range(1f, 10f)]
    public float grosorBorde = 3f;

    // ============================================================
    // GRUPO 7 - COLORES DE LAS TARJETAS
    // ============================================================
    [Header("07 - COLORES DE LAS TARJETAS")]
    public Color colorNormal = Color.white;
    public Color colorSeleccionado = new Color(0.75f, 0.88f, 1f, 1f);
    public Color colorCorrecto = new Color(0.68f, 1f, 0.73f, 1f);
    public Color colorError = new Color(1f, 0.78f, 0.78f, 1f);

    // ============================================================
    // VARIABLES INTERNAS - NO SE CONFIGURAN EN EL INSPECTOR
    // ============================================================
    private readonly int[] orden = new int[4];
    private Sprite[] pasos;
    private int ejercicioActual;
    private bool completado;
    private bool misionFinalizada;
    private Coroutine rutinaPregunta;
    private Canvas canvasPrincipal;
    private Image[] circulos;
    private Sprite spriteCirculo;
    private Sprite spriteBorde;
    private GameObject fantasma;
    private RectTransform rectFantasma;
    private int origenArrastre = -1;
    private int tarjetaElegida = -1;
    private int ultimoFrameArrastre = -100;

    // ============================================================
    // INICIO Y CONFIGURACION
    // ============================================================
    void Start()
    {
        if (txtPregunta == null || btnComprobar == null || btnSiguiente == null ||
            panelProgreso == null || tarjetas == null || tarjetas.Length != 4)
        {
            Debug.LogError("JuegoErrorOrden: asigna Txt_Pregunta, botones, Panel_Progreso y las 4 tarjetas.");
            enabled = false;
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            if (tarjetas[i] == null)
            {
                Debug.LogError("JuegoErrorOrden: falta Tarjeta_" + (i + 1));
                enabled = false;
                return;
            }
        }
        canvasPrincipal = GetComponentInParent<Canvas>();
        if (canvasPrincipal == null) canvasPrincipal = FindFirstObjectByType<Canvas>();
        if (canvasPrincipal == null)
        {
            Debug.LogError("JuegoErrorOrden: no se encontró Canvas.");
            enabled = false;
            return;
        }
        for (int i = 0; i < tarjetas.Length; i++)
        {
            tarjetas[i].image.raycastTarget = true;
            tarjetas[i].transition = Selectable.Transition.None;
            int indice = i;
            tarjetas[i].onClick.AddListener(() => SeleccionarTarjeta(indice));
            ArrastrarTarjetaErrorOrden arrastrar = tarjetas[i].GetComponent<ArrastrarTarjetaErrorOrden>();
            if (arrastrar == null) arrastrar = tarjetas[i].gameObject.AddComponent<ArrastrarTarjetaErrorOrden>();
            arrastrar.Configurar(this, i);
        }
        btnComprobar.onClick.AddListener(Comprobar);
        btnSiguiente.onClick.AddListener(SiguienteEjercicio);
        CrearIndicadorProgreso();
        CargarEjercicio();
    }

    // ============================================================
    // MENSAJES Y PREGUNTA TEMPORAL
    // ============================================================
    private void MostrarMensaje(string mensaje) { if (txtPregunta != null) txtPregunta.text = mensaje; }
    private void DetenerPregunta()
    {
        if (rutinaPregunta != null) { StopCoroutine(rutinaPregunta); rutinaPregunta = null; }
    }
    private IEnumerator PreguntaTemporal(string pregunta)
    {
        MostrarMensaje(pregunta);
        yield return new WaitForSeconds(tiempoPregunta);
        MostrarMensaje("¡Encuentra los dos pasos desordenados y cámbialos de lugar!");
        rutinaPregunta = null;
    }

    // ============================================================
    // CARGAR CADA EJERCICIO
    // ============================================================
    private void CargarEjercicio()
    {
        DetenerPregunta();
        completado = false;
        misionFinalizada = false;
        tarjetaElegida = -1;
        string pregunta;
        if (ejercicioActual == 0)
        {
            pasos = new[] { pcConectar, pcEncender, pcIniciarSesion, pcAbrirPrograma };
            pregunta = "¿Qué pasos están desordenados para usar la computadora?";
            CambiarImagen(imagenFondo, fondoComputadora);
            CambiarImagen(imagenTitulo, tituloComputadora);
        }
        else if (ejercicioActual == 1)
        {
            pasos = new[] { pizzaPrepararMasa, pizzaAgregarSalsa, pizzaAgregarIngredientes, pizzaHornear };
            pregunta = "¡La receta de pizza tiene dos pasos cambiados!";
            CambiarImagen(imagenFondo, fondoPizza);
            CambiarImagen(imagenTitulo, tituloPizza);
        }
        else
        {
            pasos = new[] { dronRevisar, dronDespegar, dronExplorar, dronAterrizar };
            pregunta = "¡Corrige las instrucciones del dron explorador!";
            CambiarImagen(imagenFondo, fondoDron);
            CambiarImagen(imagenTitulo, tituloDron);
        }
        for (int i = 0; i < 4; i++) orden[i] = i;
        // Cada desafío comienza con exactamente dos pasos intercambiados.
        int a = Random.Range(0, 4);
        int b = Random.Range(0, 3);
        if (b >= a) b++;
        Intercambiar(a, b);
        for (int i = 0; i < 4; i++) tarjetas[i].gameObject.SetActive(true);
        ActualizarTarjetas();
        ActualizarIndicadorProgreso();
        btnComprobar.gameObject.SetActive(true);
        btnComprobar.interactable = true;
        btnSiguiente.gameObject.SetActive(false);
        rutinaPregunta = StartCoroutine(PreguntaTemporal(pregunta));
    }

    // ============================================================
    // IMAGENES E INTERCAMBIO DE TARJETAS
    // ============================================================
    private void CambiarImagen(Image imagen, Sprite sprite)
    {
        if (imagen != null && sprite != null) imagen.sprite = sprite;
    }
    private void Intercambiar(int a, int b)
    {
        int temporal = orden[a]; orden[a] = orden[b]; orden[b] = temporal;
    }
    private void ActualizarTarjetas()
    {
        for (int i = 0; i < 4; i++)
        {
            tarjetas[i].image.sprite = pasos[orden[i]];
            tarjetas[i].image.color = i == tarjetaElegida ? colorSeleccionado : colorNormal;
            tarjetas[i].image.preserveAspect = false;
            tarjetas[i].interactable = !completado;
        }
    }
    // ============================================================
    // ARRASTRAR TARJETAS
    // ============================================================
    public void ComenzarArrastre(int indice, PointerEventData evento)
    {
        if (completado || misionFinalizada || fantasma != null) return;
        tarjetaElegida = -1;
        ActualizarTarjetas();
        origenArrastre = indice;
        ultimoFrameArrastre = Time.frameCount;
        DetenerPregunta();
        fantasma = new GameObject("Tarjeta_Arrastrada", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fantasma.transform.SetParent(canvasPrincipal.transform, false);
        fantasma.transform.SetAsLastSibling();
        rectFantasma = fantasma.GetComponent<RectTransform>();
        rectFantasma.sizeDelta = (tarjetas[indice].transform as RectTransform).rect.size;
        Image imagen = fantasma.GetComponent<Image>();
        imagen.sprite = tarjetas[indice].image.sprite;
        imagen.color = Color.white;
        imagen.raycastTarget = false;
        imagen.preserveAspect = false;
        MoverArrastre(evento);
    }
    public void MoverArrastre(PointerEventData evento)
    {
        if (rectFantasma == null) return;
        RectTransform canvasRect = canvasPrincipal.transform as RectTransform;
        Camera camara = canvasPrincipal.renderMode == RenderMode.ScreenSpaceOverlay ? null : evento.pressEventCamera;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, evento.position, camara, out Vector2 local))
            rectFantasma.anchoredPosition = local;
    }
    public void TerminarArrastre(int indice, PointerEventData evento)
    {
        if (fantasma != null) Destroy(fantasma);
        fantasma = null;
        rectFantasma = null;
        ultimoFrameArrastre = Time.frameCount;
        if (origenArrastre != indice || completado || misionFinalizada) { origenArrastre = -1; return; }
        Camera camara = canvasPrincipal.renderMode == RenderMode.ScreenSpaceOverlay ? null : evento.pressEventCamera;
        for (int i = 0; i < 4; i++)
        {
            if (i != indice && RectTransformUtility.RectangleContainsScreenPoint(tarjetas[i].transform as RectTransform, evento.position, camara))
            {
                Intercambiar(indice, i);
                tarjetaElegida = -1;
                ActualizarTarjetas();
                MostrarMensaje("¡Bien! Presiona COMPROBAR para revisar el orden.");
                break;
            }
        }
        origenArrastre = -1;
    }
    // ============================================================
    // SELECCIONAR TARJETAS CON DOS TOQUES
    // ============================================================
    private void SeleccionarTarjeta(int indice)
    {
        if (completado || misionFinalizada || Time.frameCount <= ultimoFrameArrastre + 1) return;
        DetenerPregunta();
        if (tarjetaElegida == -1)
        {
            tarjetaElegida = indice;
            MostrarMensaje("¡Ahora toca otra tarjeta para intercambiarlas!");
        }
        else if (tarjetaElegida == indice)
        {
            tarjetaElegida = -1;
            MostrarMensaje("Selecciona dos tarjetas para cambiar su orden.");
        }
        else
        {
            Intercambiar(tarjetaElegida, indice);
            tarjetaElegida = -1;
            MostrarMensaje("¡Bien! Puedes seguir corrigiendo el orden.");
        }
        ActualizarTarjetas();
    }

    // ============================================================
    // COMPROBAR Y GUARDAR ESTRELLAS
    // ============================================================
    private void Comprobar()
    {
        if (completado || misionFinalizada) return;
        DetenerPregunta();
        bool correcto = true;
        for (int i = 0; i < 4; i++) if (orden[i] != i) correcto = false;
        if (!correcto)
        {
            for (int i = 0; i < 4; i++) tarjetas[i].image.color = orden[i] == i ? colorNormal : colorError;
            MostrarMensaje("¡Casi! Revisa las tarjetas marcadas e inténtalo otra vez.");
            return;
        }
        completado = true;
        ProgresoEstrellas.GuardarEjercicio("DetectarErrores", 1, ejercicioActual + 1);
        for (int i = 0; i < 4; i++)
        {
            tarjetas[i].image.color = colorCorrecto;
            tarjetas[i].interactable = false;
        }
        MostrarMensaje("¡Excelente! ¡Corregiste el orden!");
        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(true);
    }
    // ============================================================
    // AVANZAR Y FINALIZAR MISION
    // ============================================================
    private void SiguienteEjercicio()
    {
        if (!completado) return;
        ejercicioActual++;
        if (ejercicioActual < 3) CargarEjercicio();
        else FinalizarMision();
    }
    private void FinalizarMision()
    {
        misionFinalizada = true;
        DetenerPregunta();
        MostrarMensaje("¡MISIÓN COMPLETADA!");
        foreach (Button tarjeta in tarjetas) tarjeta.gameObject.SetActive(false);
        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);
        if (circulos != null) foreach (Image circulo in circulos) if (circulo != null) circulo.transform.parent.gameObject.SetActive(false);
    }

    // ============================================================
    // DIBUJAR CIRCULOS CON BORDES
    // ============================================================
    private Sprite CrearSpriteCircular(float radioRelativo)
    {
        const int resolucion = 128;
        Texture2D textura = new Texture2D(resolucion, resolucion, TextureFormat.RGBA32, false);
        textura.filterMode = FilterMode.Bilinear;
        textura.wrapMode = TextureWrapMode.Clamp;
        float centro = (resolucion - 1) / 2f;
        float radio = centro * radioRelativo;
        for (int y = 0; y < resolucion; y++)
            for (int x = 0; x < resolucion; x++)
            {
                float distancia = Vector2.Distance(new Vector2(x, y), new Vector2(centro, centro));
                float alfa = Mathf.Clamp01(radio - distancia + .5f);
                textura.SetPixel(x, y, new Color(1f, 1f, 1f, alfa));
            }
        textura.Apply();
        return Sprite.Create(textura, new Rect(0, 0, resolucion, resolucion), new Vector2(.5f, .5f), 100f);
    }
    // ============================================================
    // CREAR LOS TRES CIRCULOS DE PROGRESO
    // ============================================================
    private void CrearIndicadorProgreso()
    {
        spriteCirculo = CrearSpriteCircular(.97f);
        spriteBorde = CrearSpriteCircular(.97f);
        circulos = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject contenedor = new GameObject("Progreso_" + (i + 1), typeof(RectTransform));
            contenedor.transform.SetParent(panelProgreso, false);
            RectTransform rect = contenedor.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.pivot = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2(tamanoCirculo, tamanoCirculo);
            rect.anchoredPosition = new Vector2((i - 1) * separacionCirculos, posicionVerticalCirculos);
            GameObject borde = new GameObject("Borde", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            borde.transform.SetParent(contenedor.transform, false);
            RectTransform rb = borde.GetComponent<RectTransform>();
            rb.anchorMin = Vector2.zero; rb.anchorMax = Vector2.one;
            rb.offsetMin = Vector2.zero; rb.offsetMax = Vector2.zero;
            Image ib = borde.GetComponent<Image>();
            ib.sprite = spriteBorde; ib.color = colorBorde; ib.raycastTarget = false;
            GameObject relleno = new GameObject("Relleno", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            relleno.transform.SetParent(contenedor.transform, false);
            RectTransform rr = relleno.GetComponent<RectTransform>();
            rr.anchorMin = Vector2.zero; rr.anchorMax = Vector2.one;
            float grosor = Mathf.Clamp(grosorBorde, 0f, tamanoCirculo * .45f);
            rr.offsetMin = new Vector2(grosor, grosor);
            rr.offsetMax = new Vector2(-grosor, -grosor);
            Image ir = relleno.GetComponent<Image>();
            ir.sprite = spriteCirculo; ir.raycastTarget = false;
            circulos[i] = ir;
            GameObject texto = new GameObject("Numero_" + (i + 1), typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            texto.transform.SetParent(contenedor.transform, false);
            RectTransform rt = texto.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            TextMeshProUGUI numero = texto.GetComponent<TextMeshProUGUI>();
            numero.text = (i + 1).ToString(); numero.fontSize = tamanoCirculo * .5f;
            numero.fontStyle = FontStyles.Bold;
            numero.alignment = TextAlignmentOptions.Center;
            numero.color = colorNumero; numero.raycastTarget = false;
        }
        ActualizarIndicadorProgreso();
    }
    // ============================================================
    // ACTUALIZAR COLORES DEL PROGRESO
    // ============================================================
    private void ActualizarIndicadorProgreso()
    {
        if (circulos == null) return;
        for (int i = 0; i < circulos.Length; i++)
            circulos[i].color = i <= ejercicioActual ? colorActivo : colorPendiente;
    }
    // ============================================================
    // LIMPIEZA AL SALIR
    // ============================================================
    private void OnDisable()
    {
        DetenerPregunta();
        if (fantasma != null) Destroy(fantasma);
    }
}

// ============================================================
// COMPONENTE AUXILIAR: ARRASTRAR TARJETAS
// Se agrega automaticamente a cada Button.
// ============================================================
public class ArrastrarTarjetaErrorOrden : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private JuegoErrorOrden juego;
    private int indice;
    public void Configurar(JuegoErrorOrden controlador, int indiceTarjeta)
    {
        juego = controlador; indice = indiceTarjeta;
    }
    public void OnBeginDrag(PointerEventData evento) { if (juego != null) juego.ComenzarArrastre(indice, evento); }
    public void OnDrag(PointerEventData evento) { if (juego != null) juego.MoverArrastre(evento); }
    public void OnEndDrag(PointerEventData evento) { if (juego != null) juego.TerminarArrastre(indice, evento); }
}
