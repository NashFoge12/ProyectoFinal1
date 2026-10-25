using System.Collections;

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoEncuentraAlgoritmo : MonoBehaviour
{
    // =====================================================
    // TEXTOS
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
    // TARJETAS
    // =====================================================

    [Header("PANEL PASOS - 4 BOTONES")]
    public Button[] botonesPasos = new Button[4];

    // =====================================================
    // BOTONES
    // =====================================================

    [Header("BOTONES")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // =====================================================
    // EJERCICIO 1 - PASTEL
    // =====================================================

    [Header("TEMA 1 - HORNEAR UN PASTEL")]
    public Sprite sprPesarIngredientes;
    public Sprite sprMezclarIngredientes;
    public Sprite sprComersePastelAntes;
    public Sprite sprHornear180Grados;

    // =====================================================
    // EJERCICIO 2 - CALLE
    // =====================================================

    [Header("TEMA 2 - CRUZAR LA CALLE")]
    public Sprite sprDetenerseEnVereda;
    public Sprite sprMirarAmbosLados;
    public Sprite sprCorrerSinMirar;
    public Sprite sprCruzarPasoPeatonal;

    // =====================================================
    // EJERCICIO 3 - COLEGIO
    // =====================================================

    [Header("TEMA 3 - PREPARARSE PARA EL COLEGIO")]
    public Sprite sprAbrirLaPuerta;
    public Sprite sprPonerseZapatos;
    public Sprite sprGuardarUtilesMochila;
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
    // COLORES
    // =====================================================

    [Header("COLORES DE SELECCION")]
    public Color colorNormal = Color.white;

    public Color colorSeleccionado =
        new Color(0.65f, 0.90f, 1f, 1f);

    public Color colorCorrecto =
        new Color(0.68f, 1f, 0.73f, 1f);

    // =====================================================
    // DATOS DE LOS EJERCICIOS
    // =====================================================

    private class Ejercicio
    {
        public string pregunta;
        public string mensajeCorrecto;
        public Sprite fondo;
        public Sprite titulo;
        public Sprite[] tarjetas;
        public int pasoIncorrecto;
    }

    private List<Ejercicio> ejercicios =
        new List<Ejercicio>();

    // =====================================================
    // VARIABLES
    // =====================================================

    private int ejercicioActual = 0;
    private int tarjetaSeleccionada = -1;
    private int posicionIncorrecta = -1;

    private bool ejercicioCompletado = false;

    private Image[] circulosProgreso;
    private Sprite spriteCirculo;

    private int[] indicesTarjetas = new int[4];

    [Header("TIEMPO DE PREGUNTA INICIAL")]
    [Range(0.5f, 5f)]
    public float tiempoPregunta = 2f;

    private Coroutine rutinaPregunta;

    private void MostrarMensaje(string mensaje)
    {
        if (txtPregunta != null) txtPregunta.text = mensaje;
        if (txtResultado != null) txtResultado.text = mensaje;
    }

    private void DetenerPreguntaTemporal()
    {
        if (rutinaPregunta != null)
        {
            StopCoroutine(rutinaPregunta);
            rutinaPregunta = null;
        }
    }

    private IEnumerator MostrarPreguntaTemporal(string pregunta)
    {
        // Primero aparece únicamente la pregunta, durante dos segundos.
        if (txtPregunta != null) txtPregunta.text = pregunta;
        if (txtResultado != null) txtResultado.text = "";

        yield return new WaitForSeconds(tiempoPregunta);

        MostrarMensaje("¡Observa las tarjetas y encuentra el paso sobrante!");
        rutinaPregunta = null;
    }


    // =====================================================
    // INICIO
    // =====================================================

    void Start()
    {
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
                sprPesarIngredientes,
                sprMezclarIngredientes,
                sprComersePastelAntes,
                sprHornear180Grados
            },

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
                sprDetenerseEnVereda,
                sprMirarAmbosLados,
                sprCorrerSinMirar,
                sprCruzarPasoPeatonal
            },

            pasoIncorrecto = 2
        });

        // =================================================
        // EJERCICIO 3 - PREPARARSE PARA EL COLEGIO
        // =================================================

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué paso no corresponde al prepararse para ir al colegio?",

            mensajeCorrecto =
                "¡Excelente! Desayunar después de volver no ayuda a prepararse para salir.",

            fondo = fondoColegio,
            titulo = tituloColegio,

            tarjetas = new Sprite[]
            {
                sprAbrirLaPuerta,
                sprPonerseZapatos,
                sprGuardarUtilesMochila,
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
        DetenerPreguntaTemporal();
        tarjetaSeleccionada = -1;
        ejercicioCompletado = false;

        Ejercicio ejercicio = ejercicios[ejercicioActual];

        rutinaPregunta = StartCoroutine(MostrarPreguntaTemporal(ejercicio.pregunta));

        txtProgreso.text =
            "Ejercicio " +
            (ejercicioActual + 1) +
            " de " +
            ejercicios.Count;

        txtProgreso.textWrappingMode = TextWrappingModes.NoWrap;

        ActualizarIndicadorProgreso();

        if (imagenFondo != null &&
            ejercicio.fondo != null)
        {
            imagenFondo.sprite = ejercicio.fondo;
        }

        if (imagenTitulo != null &&
            ejercicio.titulo != null)
        {
            imagenTitulo.sprite = ejercicio.titulo;
        }

        MezclarTarjetas(ejercicio);

        btnComprobar.gameObject.SetActive(true);
        btnComprobar.interactable = true;

        btnSiguiente.gameObject.SetActive(false);
    }

    // =====================================================
    // MEZCLAR LAS CUATRO TARJETAS
    // =====================================================

    void MezclarTarjetas(Ejercicio ejercicio)
    {
        for (int i = 0; i < 4; i++)
        {
            indicesTarjetas[i] = i;
        }

        // Mezcla Fisher-Yates.

        for (int i = 3; i > 0; i--)
        {
            int aleatorio = Random.Range(0, i + 1);

            int temporal = indicesTarjetas[i];

            indicesTarjetas[i] =
                indicesTarjetas[aleatorio];

            indicesTarjetas[aleatorio] = temporal;
        }

        // Evitar repetir la posicion incorrecta.

        string clave =
            "MundoPujllay_PasoSobrante_" + ejercicioActual;

        int ultimaPosicion =
            PlayerPrefs.GetInt(clave, -1);

        int posicionActual =
            System.Array.IndexOf(
                indicesTarjetas,
                ejercicio.pasoIncorrecto
            );

        if (posicionActual == ultimaPosicion)
        {
            int otraPosicion =
                (posicionActual + Random.Range(1, 4)) % 4;

            int temporal =
                indicesTarjetas[posicionActual];

            indicesTarjetas[posicionActual] =
                indicesTarjetas[otraPosicion];

            indicesTarjetas[otraPosicion] = temporal;
        }

        // Mostrar tarjetas mezcladas.

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
    // SELECCIONAR TARJETA
    // =====================================================

    void SeleccionarTarjeta(int indice)
    {
        if (ejercicioCompletado)
            return;

        DetenerPreguntaTemporal();
        tarjetaSeleccionada = indice;

        // Restaurar colores.

        for (int i = 0; i < botonesPasos.Length; i++)
        {
            botonesPasos[i].image.color = colorNormal;
        }

        // Resaltar la seleccionada.

        botonesPasos[indice].image.color =
            colorSeleccionado;

        MostrarMensaje("¡Tarjeta seleccionada! Presiona COMPROBAR.");
    }

    // =====================================================
    // COMPROBAR RESPUESTA
    // =====================================================

    void Comprobar()
    {
        DetenerPreguntaTemporal();
        if (ejercicioCompletado)
            return;

        if (tarjetaSeleccionada == -1)
        {
            MostrarMensaje("¡Primero selecciona una tarjeta!");

            return;
        }

        // =================================================
        // RESPUESTA CORRECTA
        // =================================================

        if (tarjetaSeleccionada == posicionIncorrecta)
        {
            ejercicioCompletado = true;

            // =============================================
            // NUEVO: GUARDAR ESTRELLA DE LA MISION 3
            // =============================================

            ProgresoEstrellas.GuardarEjercicio(
                "Algoritmos",
                3,
                ejercicioActual + 1
            );

            Debug.Log(
                "Mision 3 - Estrella guardada: " +
                (ejercicioActual + 1)
            );

            // =============================================
            // RESPUESTA CORRECTA
            // =============================================

            MostrarMensaje(ejercicios[ejercicioActual].mensajeCorrecto);

            botonesPasos[tarjetaSeleccionada]
                .image.color = colorCorrecto;

            foreach (Button boton in botonesPasos)
            {
                boton.interactable = false;
            }

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);
        }

        // =================================================
        // RESPUESTA INCORRECTA
        // =================================================

        else
        {
            MostrarMensaje("¡Casi! Observa los pasos e inténtalo otra vez.");

            botonesPasos[tarjetaSeleccionada]
                .image.color = colorNormal;

            tarjetaSeleccionada = -1;
        }
    }

    // =====================================================
    // SIGUIENTE EJERCICIO
    // =====================================================

    void SiguienteEjercicio()
    {
        DetenerPreguntaTemporal();
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
        MostrarMensaje("¡MISIÓN COMPLETADA!\n¡Felicidades! Encontraste los 3 pasos incorrectos.");

        txtProgreso.text = "";

        foreach (Button boton in botonesPasos)
        {
            boton.gameObject.SetActive(false);
        }

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);

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

            Image imagen =
                objetoCirculo.GetComponent<Image>();

            imagen.sprite = spriteCirculo;
            imagen.raycastTarget = false;

            circulosProgreso[i] = imagen;

            // NUMERO DEL CIRCULO

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
    // ACTUALIZAR CIRCULOS
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

    private void OnDisable()
    {
        DetenerPreguntaTemporal();
    }

}
