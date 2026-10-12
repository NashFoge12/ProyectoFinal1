
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoPasoSobrante : MonoBehaviour
{
    // =====================================================
    // TEXTOS DE INTERFAZ
    // =====================================================

    [Header("TEXTOS DE INTERFAZ")]

    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;
    public TMP_Text txtResultado;

    // =====================================================
    // FONDOS
    // =====================================================

    [Header("FONDOS DE LOS 3 TEMAS")]

    public Image imagenFondo;

    public Sprite fondoPastel;
    public Sprite fondoCalle;
    public Sprite fondoColegio;

    // =====================================================
    // TITULOS
    // =====================================================

    [Header("TITULOS EN IMAGEN")]

    public Image imagenTitulo;

    public Sprite tituloPastel;
    public Sprite tituloCalle;
    public Sprite tituloColegio;

    // =====================================================
    // CUATRO TARJETAS SELECCIONABLES
    // =====================================================

    [Header("PANEL PASOS - 4 BOTONES")]

    public Button[] botonesPasos = new Button[4];

    // =====================================================
    // BOTONES
    // =====================================================

    [Header("BOTONES")]

    public Button btnComprobar;
    public Button btnSiguiente;

    // Btn_Volver funciona con Navegacion.
    // No necesita modificarse desde este script.

    // =====================================================
    // TEMA 1 - HORNEAR UN PASTEL
    // =====================================================

    [Header("TEMA 1 - HORNEAR UN PASTEL")]

    // PASO CORRECTO 1
    public Sprite sprPesarIngredientes;

    // PASO CORRECTO 2
    public Sprite sprMezclarIngredientes;

    // PASO INCORRECTO
    public Sprite sprComersePastelAntes;

    // PASO CORRECTO 3
    public Sprite sprHornear180Grados;

    // =====================================================
    // TEMA 2 - CRUZAR LA CALLE
    // =====================================================

    [Header("TEMA 2 - CRUZAR LA CALLE")]

    // PASO CORRECTO 1
    public Sprite sprDetenerseEnVereda;

    // PASO CORRECTO 2
    public Sprite sprMirarAmbosLados;

    // PASO INCORRECTO
    public Sprite sprCorrerSinMirar;

    // PASO CORRECTO 3
    public Sprite sprCruzarPasoPeatonal;

    // =====================================================
    // TEMA 3 - IR AL COLEGIO
    // =====================================================

    [Header("TEMA 3 - PREPARARSE PARA EL COLEGIO")]

    // PASO CORRECTO 1
    public Sprite sprAbrirLaPuerta;

    // PASO CORRECTO 2
    public Sprite sprPonerseZapatos;

    // PASO CORRECTO 3
    public Sprite sprGuardarUtilesMochila;

    // PASO INCORRECTO
    public Sprite sprDesayunarDespuesVolver;

    // =====================================================
    // INDICADOR DE PROGRESO
    // =====================================================

    [Header("INDICADOR DE PROGRESO")]

    public RectTransform panelProgreso;

    public float tamanoCirculo = 28f;
    public float separacionCirculos = 38f;
    public float posicionVerticalCirculos = -20f;

    public Color colorActivo =
        new Color(1f, 0.78f, 0.24f, 1f);

    public Color colorPendiente =
        new Color(0.78f, 0.76f, 0.72f, 1f);

    // =====================================================
    // COLORES DE LAS TARJETAS
    // =====================================================

    [Header("COLORES DE SELECCION")]

    public Color colorNormal = Color.white;

    public Color colorSeleccionado =
        new Color(0.65f, 0.90f, 1f, 1f);

    public Color colorCorrecto =
        new Color(0.68f, 1f, 0.73f, 1f);

    // =====================================================
    // CLASE PARA LOS DATOS DEL EJERCICIO
    // =====================================================

    private class Ejercicio
    {
        public string pregunta;

        public string mensajeCorrecto;

        public Sprite fondo;
        public Sprite titulo;

        public Sprite[] tarjetas;

        // Indice original del paso incorrecto.
        public int pasoIncorrecto;
    }

    private List<Ejercicio> ejercicios =
        new List<Ejercicio>();

    // =====================================================
    // VARIABLES DEL JUEGO
    // =====================================================

    private int ejercicioActual = 0;

    private int tarjetaSeleccionada = -1;

    private int posicionIncorrecta = -1;

    private bool ejercicioCompletado = false;

    private Image[] circulosProgreso;

    private Sprite spriteCirculo;

    // Para identificar cada tarjeta despues de mezclar.
    private int[] indicesTarjetas = new int[4];

    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        // Comprobar referencias principales.

        if (txtPregunta == null ||
            txtProgreso == null ||
            txtResultado == null ||
            btnComprobar == null ||
            btnSiguiente == null)
        {
            Debug.LogError(
                "Faltan textos o botones en GameManager."
            );

            enabled = false;
            return;
        }

        if (botonesPasos == null ||
            botonesPasos.Length != 4)
        {
            Debug.LogError(
                "Debes asignar los 4 botones de pasos."
            );

            enabled = false;
            return;
        }

        for (int i = 0; i < botonesPasos.Length; i++)
        {
            if (botonesPasos[i] == null)
            {
                Debug.LogError(
                    "Falta Boton_Paso_" + (i + 1)
                );

                enabled = false;
                return;
            }
        }

        CrearEjercicios();

        // Asignar los eventos de cada tarjeta.

        for (int i = 0; i < botonesPasos.Length; i++)
        {
            int indice = i;

            botonesPasos[i].onClick.AddListener(
                () => SeleccionarTarjeta(indice)
            );
        }

        btnComprobar.onClick.AddListener(Comprobar);

        btnSiguiente.onClick.AddListener(SiguienteEjercicio);

        CrearIndicadorProgreso();

        CargarEjercicio();
    }

    // =====================================================
    // CREAR LOS TRES EJERCICIOS
    // =====================================================

    void CrearEjercicios()
    {
        ejercicios.Clear();

        // =================================================
        // EJERCICIO 1 - HORNEAR UN PASTEL
        // =================================================

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué paso no corresponde al hornear un pastel?",

            mensajeCorrecto =
                "¡Excelente! El pastel debe hornearse antes de comerlo.",

            fondo = fondoPastel,
            titulo = tituloPastel,

            tarjetas = new Sprite[]
            {
                // TARJETA 1
                // Pesar los ingredientes.
                sprPesarIngredientes,

                // TARJETA 2
                // Mezclar la harina, el huevo y el azucar.
                sprMezclarIngredientes,

                // TARJETA 3 - INCORRECTA
                // Comerse el pastel antes de hornearlo.
                sprComersePastelAntes,

                // TARJETA 4
                // Hornear a 180 grados.
                sprHornear180Grados
            },

            // La tarjeta incorrecta es la tercera.
            // En C# contamos desde cero.

            pasoIncorrecto = 2
        });

        // =================================================
        // EJERCICIO 2 - CRUZAR LA CALLE
        // =================================================

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué paso es incorrecto al cruzar la calle?",

            mensajeCorrecto =
                "¡Muy bien! Correr sin mirar es peligroso.",

            fondo = fondoCalle,
            titulo = tituloCalle,

            tarjetas = new Sprite[]
            {
                // TARJETA 1
                // Detenerse en la vereda.
                sprDetenerseEnVereda,

                // TARJETA 2
                // Mirar a ambos lados.
                sprMirarAmbosLados,

                // TARJETA 3 - INCORRECTA
                // Correr sin mirar entre los autos.
                sprCorrerSinMirar,

                // TARJETA 4
                // Cruzar por el paso peatonal.
                sprCruzarPasoPeatonal
            },

            pasoIncorrecto = 2
        });

        // =================================================
        // EJERCICIO 3 - PREPARARSE PARA IR AL COLEGIO
        // =================================================

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué paso no corresponde al prepararse para ir al colegio?",

            mensajeCorrecto =
                "¡Excelente! Desayunar despues de volver no ayuda a prepararse para salir.",

            fondo = fondoColegio,
            titulo = tituloColegio,

            tarjetas = new Sprite[]
            {
                // TARJETA 1
                // Abrir la puerta.
                sprAbrirLaPuerta,

                // TARJETA 2
                // Ponerse los zapatos.
                sprPonerseZapatos,

                // TARJETA 3
                // Guardar los utiles en la mochila.
                sprGuardarUtilesMochila,

                // TARJETA 4 - INCORRECTA
                // Desayunar despues de volver.
                sprDesayunarDespuesVolver
            },

            pasoIncorrecto = 3
        });
    }

    // =====================================================
    // CARGAR EJERCICIO
    // =====================================================

    void CargarEjercicio()
    {
        tarjetaSeleccionada = -1;

        ejercicioCompletado = false;

        Ejercicio ejercicio = ejercicios[ejercicioActual];

        // -------------------------------------------------
        // PREGUNTA
        // -------------------------------------------------

        txtPregunta.text = ejercicio.pregunta;

        // -------------------------------------------------
        // MENSAJE INICIAL
        // -------------------------------------------------

        txtResultado.text =
            "¡Observa las tarjetas y encuentra el paso sobrante!";

        // -------------------------------------------------
        // PROGRESO
        // -------------------------------------------------

        txtProgreso.text =
            "Ejercicio " +
            (ejercicioActual + 1) +
            " de " +
            ejercicios.Count;

        txtProgreso.enableWordWrapping = false;

        ActualizarIndicadorProgreso();

        // -------------------------------------------------
        // CAMBIAR FONDO
        // -------------------------------------------------

        if (imagenFondo != null &&
            ejercicio.fondo != null)
        {
            imagenFondo.sprite = ejercicio.fondo;
        }

        // -------------------------------------------------
        // CAMBIAR TITULO
        // -------------------------------------------------

        if (imagenTitulo != null &&
            ejercicio.titulo != null)
        {
            imagenTitulo.sprite = ejercicio.titulo;
        }

        // -------------------------------------------------
        // MEZCLAR TARJETAS
        // -------------------------------------------------

        MezclarTarjetas(ejercicio);

        // -------------------------------------------------
        // MOSTRAR BOTONES
        // -------------------------------------------------

        btnComprobar.gameObject.SetActive(true);
        btnComprobar.interactable = true;

        btnSiguiente.gameObject.SetActive(false);
    }

    // =====================================================
    // MEZCLAR LAS CUATRO TARJETAS
    // =====================================================

    void MezclarTarjetas(Ejercicio ejercicio)
    {
        // Cada numero representa la posicion original
        // de la tarjeta dentro del ejercicio.

        for (int i = 0; i < 4; i++)
        {
            indicesTarjetas[i] = i;
        }

        // Barajar usando Fisher-Yates.

        for (int i = 3; i > 0; i--)
        {
            int aleatorio = Random.Range(0, i + 1);

            int temporal = indicesTarjetas[i];

            indicesTarjetas[i] =
                indicesTarjetas[aleatorio];

            indicesTarjetas[aleatorio] = temporal;
        }

        // Evitar que el paso incorrecto aparezca
        // inmediatamente en la misma posicion que
        // en la partida anterior de este tema.

        string clave =
            "MundoPujllay_PasoSobrante_" + ejercicioActual;

        int ultimaPosicion = PlayerPrefs.GetInt(clave, -1);

        int posicionActual =
            System.Array.IndexOf(
                indicesTarjetas,
                ejercicio.pasoIncorrecto
            );

        if (posicionActual == ultimaPosicion)
        {
            int otraPosicion =
                (posicionActual + Random.Range(1, 4)) % 4;

            int temporal = indicesTarjetas[posicionActual];

            indicesTarjetas[posicionActual] =
                indicesTarjetas[otraPosicion];

            indicesTarjetas[otraPosicion] = temporal;
        }

        // Cargar las imagenes mezcladas.

        for (int i = 0; i < 4; i++)
        {
            int indiceOriginal = indicesTarjetas[i];

            Button boton = botonesPasos[i];

            boton.gameObject.SetActive(true);
            boton.interactable = true;

            boton.image.sprite =
                ejercicio.tarjetas[indiceOriginal];

            boton.image.color = colorNormal;

            boton.image.preserveAspect = false;
        }

        // Encontrar la posicion actual de la
        // tarjeta incorrecta despues de mezclar.

        posicionIncorrecta =
            System.Array.IndexOf(
                indicesTarjetas,
                ejercicio.pasoIncorrecto
            );

        PlayerPrefs.SetInt(
            clave,
            posicionIncorrecta
        );

        PlayerPrefs.Save();
    }

    // =====================================================
    // SELECCIONAR UNA TARJETA
    // =====================================================

    void SeleccionarTarjeta(int indice)
    {
        if (ejercicioCompletado)
            return;

        tarjetaSeleccionada = indice;

        // Restaurar el color de todas.

        for (int i = 0; i < botonesPasos.Length; i++)
        {
            botonesPasos[i].image.color = colorNormal;
        }

        // Resaltar la tarjeta elegida.

        botonesPasos[indice].image.color =
            colorSeleccionado;

        txtResultado.text =
            "¡Tarjeta seleccionada! Presiona COMPROBAR.";
    }

    // =====================================================
    // COMPROBAR
    // =====================================================

    void Comprobar()
    {
        if (ejercicioCompletado)
            return;

        // -------------------------------------------------
        // NO SELECCIONO NINGUNA TARJETA
        // -------------------------------------------------

        if (tarjetaSeleccionada == -1)
        {
            txtResultado.text =
                "¡Primero selecciona una tarjeta!";

            return;
        }

        // -------------------------------------------------
        // RESPUESTA CORRECTA
        // -------------------------------------------------

        if (tarjetaSeleccionada == posicionIncorrecta)
        {
            ejercicioCompletado = true;

            txtResultado.text =
                ejercicios[ejercicioActual].mensajeCorrecto;

            // Resaltar la tarjeta identificada.

            botonesPasos[tarjetaSeleccionada]
                .image.color = colorCorrecto;

            // Desactivar las cuatro tarjetas.

            foreach (Button boton in botonesPasos)
            {
                boton.interactable = false;
            }

            // Ocultar Comprobar.

            btnComprobar.gameObject.SetActive(false);

            // Mostrar Siguiente.

            btnSiguiente.gameObject.SetActive(true);
        }

        // -------------------------------------------------
        // RESPUESTA INCORRECTA
        // -------------------------------------------------

        else
        {
            txtResultado.text =
                "¡Casi! Observa los pasos e inténtalo otra vez.";

            // Restaurar el color de la tarjeta.

            botonesPasos[tarjetaSeleccionada]
                .image.color = colorNormal;

            // Permitir un nuevo intento.

            tarjetaSeleccionada = -1;
        }
    }

    // =====================================================
    // SIGUIENTE EJERCICIO
    // =====================================================

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

    // =====================================================
    // FINALIZAR MISION
    // =====================================================

    void FinalizarMision()
    {
        txtPregunta.text =
            "¡MISIÓN COMPLETADA!";

        txtResultado.text =
            "¡Felicidades! Encontraste los 3 pasos incorrectos.";

        txtProgreso.text = "";

        // Ocultar las cuatro tarjetas.

        foreach (Button boton in botonesPasos)
        {
            boton.gameObject.SetActive(false);
        }

        // Ocultar botones.

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);

        // Ocultar indicador de progreso.

        if (circulosProgreso != null)
        {
            foreach (Image circulo in circulosProgreso)
            {
                if (circulo != null)
                {
                    circulo.gameObject.SetActive(false);
                }
            }
        }

        // Btn_Volver permanece disponible.
    }

    // =====================================================
    // GENERAR SPRITE CIRCULAR
    // =====================================================

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

    // =====================================================
    // CREAR INDICADOR DE PROGRESO
    // =====================================================

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
                "No se encontro Panel_Progreso."
            );
            return;
        }

        spriteCirculo = CrearSpriteCircular();

        circulosProgreso = new Image[ejercicios.Count];

        for (int i = 0; i < ejercicios.Count; i++)
        {
            // ---------------------------------------------
            // OBJETO CIRCULO
            // ---------------------------------------------

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

            float posicionX =
                (i - 1) * separacionCirculos;

            rect.anchoredPosition = new Vector2(
                posicionX,
                posicionVerticalCirculos
            );

            Image imagen =
                objetoCirculo.GetComponent<Image>();

            imagen.sprite = spriteCirculo;
            imagen.raycastTarget = false;

            circulosProgreso[i] = imagen;

            // ---------------------------------------------
            // NUMERO DEL CIRCULO
            // ---------------------------------------------

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

    // =====================================================
    // ACTUALIZAR INDICADOR DE PROGRESO
    // =====================================================

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
}
