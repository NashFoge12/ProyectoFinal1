
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoDivideProblema : MonoBehaviour
{
    // =========================================================
    // TEXTOS
    // =========================================================

    [Header("TEXTOS DE INTERFAZ")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;
    public TMP_Text txtResultado;

    // =========================================================
    // FONDO Y TITULO (OPCIONALES)
    // =========================================================

    [Header("FONDO Y TITULO")]
    public Image imagenFondo;
    public Image imagenTitulo;

    [Header("FONDOS DE LOS EJERCICIOS")]
    public Sprite fondoFiesta;
    public Sprite fondoExposicion;
    public Sprite fondoJardin;

    [Header("TITULOS DE LOS EJERCICIOS")]
    public Sprite tituloFiesta;
    public Sprite tituloExposicion;
    public Sprite tituloJardin;

    // =========================================================
    // TARJETAS
    // =========================================================

    [Header("CINCO BOTONES DE TARJETAS")]
    public Button[] botonesTarjetas = new Button[5];

    [Header("TEXTOS DE LAS CINCO TARJETAS")]
    public TMP_Text[] textosTarjetas = new TMP_Text[5];

    // =========================================================
    // BOTONES
    // =========================================================

    [Header("BOTONES DE CONTROL")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // Btn_Volver continúa usando NavegacionEscenas.

    // =========================================================
    // COLORES
    // =========================================================

    [Header("COLORES DE TARJETAS")]

    public Color colorNormal =
        new Color(1f, 1f, 1f, 1f);

    public Color colorSeleccionado =
        new Color(0.56f, 0.83f, 1f, 1f);

    public Color colorCorrecto =
        new Color(0.56f, 0.90f, 0.66f, 1f);

    [Header("COLORES DEL TEXTO")]

    public Color colorTexto =
        new Color(0.17f, 0.22f, 0.34f, 1f);

    // =========================================================
    // PROGRESO
    // =========================================================

    [Header("INDICADOR DE PROGRESO")]
    public RectTransform panelProgreso;

    public float tamanoCirculo = 28f;
    public float separacionCirculos = 38f;
    public float posicionVerticalCirculos = -20f;

    public Color colorActivo =
        new Color(1f, 0.78f, 0.24f, 1f);

    public Color colorPendiente =
        new Color(0.78f, 0.76f, 0.72f, 1f);

    public Color colorBorde =
        new Color(0.43f, 0.29f, 0.17f, 1f);

    // =========================================================
    // CONFIGURACION DE LOS EJERCICIOS
    // =========================================================

    private class Tarea
    {
        public string texto;
        public bool esCorrecta;

        public Tarea(string texto, bool esCorrecta)
        {
            this.texto = texto;
            this.esCorrecta = esCorrecta;
        }
    }

    private class Ejercicio
    {
        public string pregunta;
        public string explicacion;
        public List<Tarea> tareas;
        public Sprite fondo;
        public Sprite titulo;
    }

    private List<Ejercicio> ejercicios =
        new List<Ejercicio>();

    // =========================================================
    // VARIABLES INTERNAS
    // =========================================================

    private int ejercicioActual = 0;

    private int cantidadSeleccionada = 0;

    private bool ejercicioCompletado = false;

    // Las tareas del ejercicio actual, ya mezcladas.
    private List<Tarea> tareasActuales =
        new List<Tarea>();

    // Estado de selección de los cinco botones.
    private bool[] seleccionadas = new bool[5];

    private Image[] circulosProgreso;

    private Sprite spriteCirculo;

    private const int tareasNecesarias = 3;

    // =========================================================
    // INICIO
    // =========================================================

    void Start()
    {
        if (!ValidarReferencias())
        {
            enabled = false;
            return;
        }

        CrearEjercicios();

        // Conectar los cinco botones.
        for (int i = 0; i < botonesTarjetas.Length; i++)
        {
            int indice = i;

            botonesTarjetas[i].onClick.AddListener(
                () => SeleccionarTarjeta(indice)
            );

            // El código controla los colores.
            botonesTarjetas[i].transition =
                Selectable.Transition.None;
        }

        btnComprobar.onClick.AddListener(Comprobar);

        btnSiguiente.onClick.AddListener(SiguienteEjercicio);

        CrearIndicadorProgreso();

        CargarEjercicio();
    }

    // =========================================================
    // VALIDAR OBJETOS DEL INSPECTOR
    // =========================================================

    bool ValidarReferencias()
    {
        if (txtPregunta == null ||
            txtProgreso == null ||
            txtResultado == null ||
            btnComprobar == null ||
            btnSiguiente == null)
        {
            Debug.LogError(
                "JuegoDivideProblema: faltan textos o botones."
            );

            return false;
        }

        if (botonesTarjetas == null ||
            botonesTarjetas.Length != 5)
        {
            Debug.LogError(
                "JuegoDivideProblema: asigna cinco botones."
            );

            return false;
        }

        if (textosTarjetas == null ||
            textosTarjetas.Length != 5)
        {
            Debug.LogError(
                "JuegoDivideProblema: asigna cinco textos TMP."
            );

            return false;
        }

        for (int i = 0; i < 5; i++)
        {
            if (botonesTarjetas[i] == null)
            {
                Debug.LogError(
                    "Falta Boton_Tarjeta_" + (i + 1)
                );

                return false;
            }

            if (textosTarjetas[i] == null)
            {
                // Intentar buscarlo automáticamente.
                textosTarjetas[i] =
                    botonesTarjetas[i]
                    .GetComponentInChildren<TMP_Text>(true);

                if (textosTarjetas[i] == null)
                {
                    Debug.LogError(
                        "Boton_Tarjeta_" + (i + 1) +
                        " necesita un texto TextMeshPro."
                    );

                    return false;
                }
            }

            if (botonesTarjetas[i].image == null)
            {
                Debug.LogError(
                    "Boton_Tarjeta_" + (i + 1) +
                    " necesita un componente Image."
                );

                return false;
            }
        }

        return true;
    }

    // =========================================================
    // CREAR LOS TRES EJERCICIOS
    // =========================================================

    void CrearEjercicios()
    {
        ejercicios.Clear();

        // -----------------------------------------------------
        // EJERCICIO 1: FIESTA DE CUMPLEAÑOS
        // -----------------------------------------------------

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¡Tenemos un gran reto! Organizar una fiesta de cumpleaños. " +
                "¿Qué tres tareas pequeñas nos ayudarán?",

            explicacion =
                "¡Muy bien! Una fiesta se puede organizar " +
                "invitando amigos, preparando decoraciones " +
                "y consiguiendo la torta.",

            fondo = fondoFiesta,
            titulo = tituloFiesta,

            tareas = new List<Tarea>
            {
                new Tarea("Invitar a los amigos", true),
                new Tarea("Preparar las decoraciones", true),
                new Tarea("Conseguir la torta", true),

                new Tarea("Lavar un automóvil", false),
                new Tarea("Barrer otro salón", false)
            }
        });

        // -----------------------------------------------------
        // EJERCICIO 2: EXPOSICION ESCOLAR
        // -----------------------------------------------------

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¡Otro gran reto! Preparar una exposición escolar. " +
                "¿Qué tres tareas pequeñas necesitamos?",

            explicacion =
                "¡Excelente! Para preparar una exposición " +
                "podemos investigar, preparar materiales " +
                "y practicar lo que diremos.",

            fondo = fondoExposicion,
            titulo = tituloExposicion,

            tareas = new List<Tarea>
            {
                new Tarea("Investigar el tema", true),
                new Tarea("Preparar los materiales", true),
                new Tarea("Practicar la exposición", true),

                new Tarea("Jugar videojuegos", false),
                new Tarea("Esconder los cuadernos", false)
            }
        });

        // -----------------------------------------------------
        // EJERCICIO 3: CREAR UN JARDIN
        // -----------------------------------------------------

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¡Nuestro último reto! Crear un pequeño jardín. " +
                "¿Qué tres tareas pequeñas debemos realizar?",

            explicacion =
                "¡Genial! Para crear un jardín podemos " +
                "preparar la tierra, sembrar semillas " +
                "y regar las plantas.",

            fondo = fondoJardin,
            titulo = tituloJardin,

            tareas = new List<Tarea>
            {
                new Tarea("Preparar la tierra", true),
                new Tarea("Sembrar las semillas", true),
                new Tarea("Regar las plantas", true),

                new Tarea("Pintar las hojas", false),
                new Tarea(
                    "Guardar las semillas en el congelador",
                    false
                )
            }
        });
    }

    // =========================================================
    // CARGAR EJERCICIO
    // =========================================================

    void CargarEjercicio()
    {
        ejercicioCompletado = false;
        cantidadSeleccionada = 0;

        Ejercicio ejercicio = ejercicios[ejercicioActual];

        // -------------------------------------------
        // TEXTOS
        // -------------------------------------------

        txtPregunta.text = ejercicio.pregunta;

        txtResultado.text =
            "Selecciona las 3 tareas que ayudan a resolver el reto.";

        txtProgreso.text =
            "Ejercicio " + (ejercicioActual + 1) +
            " de " + ejercicios.Count;

        txtProgreso.enableWordWrapping = false;
        txtProgreso.overflowMode =
            TextOverflowModes.Overflow;

        // -------------------------------------------
        // FONDO Y TITULO
        // -------------------------------------------

        CambiarSprite(
            imagenFondo,
            ejercicio.fondo
        );

        CambiarSprite(
            imagenTitulo,
            ejercicio.titulo
        );

        // -------------------------------------------
        // PREPARAR LAS TAREAS MEZCLADAS
        // -------------------------------------------

        tareasActuales =
            new List<Tarea>(ejercicio.tareas);

        MezclarTareas();

        // -------------------------------------------
        // MOSTRAR LAS CINCO TARJETAS
        // -------------------------------------------

        for (int i = 0; i < 5; i++)
        {
            seleccionadas[i] = false;

            Button boton = botonesTarjetas[i];
            TMP_Text texto = textosTarjetas[i];

            boton.gameObject.SetActive(true);
            boton.interactable = true;

            boton.image.color = colorNormal;

            texto.text = tareasActuales[i].texto;
            texto.color = colorTexto;

            texto.alignment =
                TextAlignmentOptions.Center;

            texto.enableWordWrapping = true;

            texto.overflowMode =
                TextOverflowModes.Ellipsis;
        }

        btnComprobar.gameObject.SetActive(true);

        // Se puede comprobar incluso sin tres elecciones,
        // pero aparecerá un mensaje indicando qué falta.
        btnComprobar.interactable = true;

        btnSiguiente.gameObject.SetActive(false);

        ActualizarIndicadorProgreso();
    }

    // =========================================================
    // CAMBIAR SPRITE (SI FUE ASIGNADO)
    // =========================================================

    void CambiarSprite(Image imagen, Sprite sprite)
    {
        if (imagen != null && sprite != null)
        {
            imagen.sprite = sprite;
        }
    }

    // =========================================================
    // MEZCLAR TARJETAS
    // =========================================================

    void MezclarTareas()
    {
        // Fisher-Yates: mezcla uniforme.
        for (int i = tareasActuales.Count - 1; i > 0; i--)
        {
            int aleatorio = Random.Range(0, i + 1);

            Tarea temporal = tareasActuales[i];

            tareasActuales[i] = tareasActuales[aleatorio];
            tareasActuales[aleatorio] = temporal;
        }
    }

    // =========================================================
    // SELECCIONAR O DESELECCIONAR TARJETA
    // =========================================================

    void SeleccionarTarjeta(int indice)
    {
        if (ejercicioCompletado)
            return;

        if (indice < 0 || indice >= 5)
            return;

        // Si ya estaba seleccionada, quitar selección.
        if (seleccionadas[indice])
        {
            seleccionadas[indice] = false;
            cantidadSeleccionada--;
        }
        else
        {
            // No permitir más de tres.
            if (cantidadSeleccionada >= tareasNecesarias)
            {
                txtResultado.text =
                    "Ya elegiste 3 tareas. " +
                    "Toca una para quitarla y elegir otra.";

                return;
            }

            seleccionadas[indice] = true;
            cantidadSeleccionada++;
        }

        ActualizarColoresTarjetas();

        txtResultado.text =
            "Tareas seleccionadas: " +
            cantidadSeleccionada +
            " de " + tareasNecesarias + ".";
    }

    // =========================================================
    // ACTUALIZAR COLORES
    // =========================================================

    void ActualizarColoresTarjetas()
    {
        for (int i = 0; i < 5; i++)
        {
            botonesTarjetas[i].image.color =
                seleccionadas[i]
                ? colorSeleccionado
                : colorNormal;
        }
    }

    // =========================================================
    // COMPROBAR RESPUESTAS
    // =========================================================

    void Comprobar()
    {
        if (ejercicioCompletado)
            return;

        // -------------------------------------------
        // TODAVIA NO HA SELECCIONADO TRES
        // -------------------------------------------

        if (cantidadSeleccionada != tareasNecesarias)
        {
            txtResultado.text =
                "¡Selecciona exactamente 3 tareas antes de comprobar!";

            return;
        }

        // -------------------------------------------
        // REVISAR LAS TRES RESPUESTAS
        // -------------------------------------------

        bool todasCorrectas = true;

        for (int i = 0; i < 5; i++)
        {
            if (seleccionadas[i] &&
                !tareasActuales[i].esCorrecta)
            {
                todasCorrectas = false;
                break;
            }
        }

        // -------------------------------------------
        // ACIERTO
        // -------------------------------------------

        if (todasCorrectas)
        {
            ejercicioCompletado = true;

            txtResultado.text =
                ejercicios[ejercicioActual].explicacion;

            // Pintar las tres tarjetas de verde.
            for (int i = 0; i < 5; i++)
            {
                if (seleccionadas[i])
                {
                    botonesTarjetas[i].image.color =
                        colorCorrecto;
                }

                botonesTarjetas[i].interactable = false;
            }

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);
        }

        // -------------------------------------------
        // ERROR
        // -------------------------------------------

        else
        {
            txtResultado.text =
                "¡Casi! Algunas tareas no ayudan a resolver " +
                "el problema. Revisa tus elecciones.";

            // No borra las respuestas.
            // Puede tocar las tarjetas y corregir.
        }
    }

    // =========================================================
    // SIGUIENTE EJERCICIO
    // =========================================================

    void SiguienteEjercicio()
    {
        if (!ejercicioCompletado)
            return;

        ejercicioActual++;

        if (ejercicioActual < ejercicios.Count)
        {
            CargarEjercicio();
        }
        else
        {
            FinalizarMision();
        }
    }

    // =========================================================
    // FINALIZAR LA MISION
    // =========================================================

    void FinalizarMision()
    {
        txtPregunta.text =
            "¡MISIÓN COMPLETADA!";

        txtResultado.text =
            "¡Felicidades! Aprendiste a dividir " +
            "tres grandes retos en tareas pequeñas.";

        txtProgreso.text = "";

        // Ocultar las cinco tarjetas.
        for (int i = 0; i < 5; i++)
        {
            botonesTarjetas[i].gameObject.SetActive(false);
        }

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);

        OcultarIndicadorProgreso();
    }

    // =========================================================
    // CREAR SPRITE CIRCULAR
    // =========================================================

    Sprite CrearSpriteCircular()
    {
        int resolucion = 128;

        Texture2D textura = new Texture2D(
            resolucion,
            resolucion,
            TextureFormat.RGBA32,
            false
        );

        textura.filterMode = FilterMode.Bilinear;
        textura.wrapMode = TextureWrapMode.Clamp;

        float centro = (resolucion - 1) / 2f;
        float radio = centro - 1f;

        for (int y = 0; y < resolucion; y++)
        {
            for (int x = 0; x < resolucion; x++)
            {
                float distancia = Vector2.Distance(
                    new Vector2(x, y),
                    new Vector2(centro, centro)
                );

                float alpha = Mathf.Clamp01(
                    radio - distancia + 0.5f
                );

                textura.SetPixel(
                    x,
                    y,
                    new Color(1f, 1f, 1f, alpha)
                );
            }
        }

        textura.Apply();

        return Sprite.Create(
            textura,
            new Rect(0, 0, resolucion, resolucion),
            new Vector2(0.5f, 0.5f),
            100f
        );
    }

    // =========================================================
    // CREAR TRES CIRCULOS DE PROGRESO
    // =========================================================

    void CrearIndicadorProgreso()
    {
        if (panelProgreso == null && txtProgreso != null)
        {
            panelProgreso =
                txtProgreso.transform.parent as RectTransform;
        }

        if (panelProgreso == null)
        {
            Debug.LogWarning(
                "JuegoDivideProblema: falta Panel_Progreso."
            );
            return;
        }

        spriteCirculo = CrearSpriteCircular();

        circulosProgreso = new Image[ejercicios.Count];

        for (int i = 0; i < ejercicios.Count; i++)
        {
            // -----------------------------------------
            // BORDE EXTERIOR
            // -----------------------------------------

            GameObject objetoCirculo = new GameObject(
                "Circulo_Progreso_" + (i + 1),
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );

            objetoCirculo.transform.SetParent(
                panelProgreso,
                false
            );

            RectTransform rect =
                objetoCirculo.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            rect.sizeDelta = new Vector2(
                tamanoCirculo,
                tamanoCirculo
            );

            rect.anchoredPosition = new Vector2(
                (i - 1) * separacionCirculos,
                posicionVerticalCirculos
            );

            Image imagenBorde =
                objetoCirculo.GetComponent<Image>();

            imagenBorde.sprite = spriteCirculo;
            imagenBorde.color = colorBorde;
            imagenBorde.raycastTarget = false;

            // -----------------------------------------
            // INTERIOR DEL CIRCULO
            // -----------------------------------------

            GameObject objetoInterior = new GameObject(
                "Interior",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );

            objetoInterior.transform.SetParent(
                objetoCirculo.transform,
                false
            );

            RectTransform rectInterior =
                objetoInterior.GetComponent<RectTransform>();

            rectInterior.anchorMin = Vector2.zero;
            rectInterior.anchorMax = Vector2.one;

            rectInterior.offsetMin = new Vector2(2f, 2f);
            rectInterior.offsetMax = new Vector2(-2f, -2f);

            Image imagenInterior =
                objetoInterior.GetComponent<Image>();

            imagenInterior.sprite = spriteCirculo;
            imagenInterior.color = colorPendiente;
            imagenInterior.raycastTarget = false;

            circulosProgreso[i] = imagenInterior;

            // -----------------------------------------
            // NUMERO DEL CIRCULO
            // -----------------------------------------

            GameObject objetoNumero = new GameObject(
                "Numero_" + (i + 1),
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI)
            );

            objetoNumero.transform.SetParent(
                objetoCirculo.transform,
                false
            );

            RectTransform rectNumero =
                objetoNumero.GetComponent<RectTransform>();

            rectNumero.anchorMin = Vector2.zero;
            rectNumero.anchorMax = Vector2.one;

            rectNumero.offsetMin = Vector2.zero;
            rectNumero.offsetMax = Vector2.zero;

            TextMeshProUGUI numero =
                objetoNumero.GetComponent<TextMeshProUGUI>();

            numero.text = (i + 1).ToString();
            numero.fontSize = tamanoCirculo * 0.5f;
            numero.fontStyle = FontStyles.Bold;
            numero.alignment = TextAlignmentOptions.Center;
            numero.color = Color.white;
            numero.raycastTarget = false;
        }

        ActualizarIndicadorProgreso();
    }

    // =========================================================
    // ACTUALIZAR CIRCULOS
    // =========================================================

    void ActualizarIndicadorProgreso()
    {
        if (circulosProgreso == null)
            return;

        for (int i = 0; i < circulosProgreso.Length; i++)
        {
            if (circulosProgreso[i] == null)
                continue;

            circulosProgreso[i].color =
                i <= ejercicioActual
                ? colorActivo
                : colorPendiente;
        }
    }

    // =========================================================
    // OCULTAR CIRCULOS AL FINALIZAR
    // =========================================================

    void OcultarIndicadorProgreso()
    {
        if (circulosProgreso == null)
            return;

        foreach (Image circulo in circulosProgreso)
        {
            if (circulo != null)
            {
                circulo.transform.parent.gameObject
                    .SetActive(false);
            }
        }
    }

    // =========================================================
    // LIMPIAR TEXTURA GENERADA
    // =========================================================

    void OnDestroy()
    {
        if (spriteCirculo != null)
        {
            Texture2D textura = spriteCirculo.texture;

            Destroy(spriteCirculo);

            if (textura != null)
                Destroy(textura);
        }
    }
}
