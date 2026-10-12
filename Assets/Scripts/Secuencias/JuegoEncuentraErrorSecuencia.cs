
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoEncuentraErrorSecuencia : MonoBehaviour
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

    [Header("FONDOS")]
    public Image imagenFondo;

    public Sprite fondoLeche;
    public Sprite fondoLluvia;
    public Sprite fondoColegio;

    // =====================================================
    // TITULOS
    // =====================================================

    [Header("TITULOS EN IMAGEN")]
    public Image imagenTitulo;

    public Sprite tituloLeche;
    public Sprite tituloLluvia;
    public Sprite tituloColegio;

    // =====================================================
    // LAS CUATRO TARJETAS
    // =====================================================

    [Header("4 TARJETAS SELECCIONABLES")]
    public Button[] botonesPasos = new Button[4];

    // =====================================================
    // BOTONES
    // =====================================================

    [Header("BOTONES")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // Btn_Volver sigue utilizando Navegacion.

    // =====================================================
    // EJERCICIO 1 - PREPARAR LECHE
    // =====================================================

    [Header("EJERCICIO 1 - PREPARAR LA LECHE")]

    // PASO 1
    public Sprite sprServirLaLeche;

    // PASO 2
    public Sprite sprAgregarCocoaAzucar;

    // PASO 3
    public Sprite sprMezclar;

    // PASO 4
    public Sprite sprTomarLaLeche;

    // =====================================================
    // EJERCICIO 2 - SALIR CUANDO LLUEVE
    // =====================================================

    [Header("EJERCICIO 2 - CUANDO LLUEVE")]

    // PASO 1
    public Sprite sprMirarPorLaVentana;

    // PASO 2
    public Sprite sprPonerseLaCasaca;

    // PASO 3
    public Sprite sprAbrirElParaguas;

    // PASO 4
    public Sprite sprCaminarSobreLosCharcos;

    // =====================================================
    // EJERCICIO 3 - IR AL COLEGIO
    // =====================================================

    [Header("EJERCICIO 3 - PREPARAR LA MOCHILA")]

    // PASO 1
    public Sprite sprRevisarLaLista;

    // PASO 2
    public Sprite sprGuardarLosUtiles;

    // PASO 3
    public Sprite sprCerrarLaMochila;

    // PASO 4
    public Sprite sprIrAlColegio;

    // =====================================================
    // COLORES
    // =====================================================

    [Header("COLORES DE LAS TARJETAS")]

    public Color colorNormal = Color.white;

    public Color colorSeleccionado =
        new Color(0.65f, 0.90f, 1f, 1f);

    public Color colorCorrecto =
        new Color(0.70f, 1f, 0.75f, 1f);

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

    public Color colorBorde =
        new Color(0.43f, 0.29f, 0.17f, 1f);

    // =====================================================
    // DATOS DE LOS EJERCICIOS
    // =====================================================

    private class Ejercicio
    {
        public string pregunta;

        public Sprite fondo;
        public Sprite titulo;

        // Las 4 tarjetas en su orden verdadero.
        public Sprite[] pasos;
    }

    private List<Ejercicio> ejercicios =
        new List<Ejercicio>();

    // =====================================================
    // VARIABLES DEL JUEGO
    // =====================================================

    private int ejercicioActual = 0;

    private int tarjetaSeleccionada = -1;

    // Posicion donde aparece la tarjeta desplazada.
    private int posicionError = -1;

    private bool ejercicioCompletado = false;

    private int[] ordenTarjetas = new int[4];

    private Image[] circulosProgreso;

    private Sprite spriteCirculo;

    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        if (!ValidarReferencias())
        {
            enabled = false;
            return;
        }

        CrearEjercicios();

        // Conectar las 4 tarjetas.

        for (int i = 0; i < 4; i++)
        {
            int indice = i;

            botonesPasos[i].onClick.AddListener(
                () => SeleccionarTarjeta(indice)
            );

            botonesPasos[i].transition =
                Selectable.Transition.None;
        }

        btnComprobar.onClick.AddListener(Comprobar);

        btnSiguiente.onClick.AddListener(SiguienteEjercicio);

        CrearIndicadorProgreso();

        CargarEjercicio();
    }

    // =====================================================
    // VALIDAR REFERENCIAS
    // =====================================================

    bool ValidarReferencias()
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

            return false;
        }

        if (botonesPasos == null ||
            botonesPasos.Length != 4)
        {
            Debug.LogError(
                "Debes asignar exactamente 4 tarjetas."
            );

            return false;
        }

        for (int i = 0; i < 4; i++)
        {
            if (botonesPasos[i] == null)
            {
                Debug.LogError(
                    "Falta asignar Boton_Paso_" + (i + 1)
                );

                return false;
            }

            if (botonesPasos[i].image == null)
            {
                Debug.LogError(
                    "La tarjeta " + (i + 1) +
                    " necesita una imagen."
                );

                return false;
            }
        }

        return true;
    }

    // =====================================================
    // CREAR LOS 3 EJERCICIOS
    // =====================================================

    void CrearEjercicios()
    {
        ejercicios.Clear();

        // -------------------------------------------------
        // EJERCICIO 1 - PREPARAR LA LECHE
        // -------------------------------------------------

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué tarjeta está fuera de orden al preparar la leche?",

            fondo = fondoLeche,
            titulo = tituloLeche,

            pasos = new Sprite[]
            {
                sprServirLaLeche,       // PASO 1
                sprAgregarCocoaAzucar,  // PASO 2
                sprMezclar,            // PASO 3
                sprTomarLaLeche        // PASO 4
            }
        });

        // -------------------------------------------------
        // EJERCICIO 2 - CUANDO LLUEVE
        // -------------------------------------------------

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué tarjeta está fuera de orden al salir cuando llueve?",

            fondo = fondoLluvia,
            titulo = tituloLluvia,

            pasos = new Sprite[]
            {
                sprMirarPorLaVentana,      // PASO 1
                sprPonerseLaCasaca,        // PASO 2
                sprAbrirElParaguas,        // PASO 3
                sprCaminarSobreLosCharcos  // PASO 4
            }
        });

        // -------------------------------------------------
        // EJERCICIO 3 - PREPARAR LA MOCHILA
        // -------------------------------------------------

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué tarjeta está fuera de orden al preparar la mochila?",

            fondo = fondoColegio,
            titulo = tituloColegio,

            pasos = new Sprite[]
            {
                sprRevisarLaLista,      // PASO 1
                sprGuardarLosUtiles,    // PASO 2
                sprCerrarLaMochila,     // PASO 3
                sprIrAlColegio         // PASO 4
            }
        });
    }

    // =====================================================
    // CARGAR EJERCICIO
    // =====================================================

    void CargarEjercicio()
    {
        ejercicioCompletado = false;

        tarjetaSeleccionada = -1;

        Ejercicio ejercicio =
            ejercicios[ejercicioActual];

        // ---------------------------------------------
        // PREGUNTA
        // ---------------------------------------------

        txtPregunta.text = ejercicio.pregunta;

        // ---------------------------------------------
        // PANEL MENSAJE
        // ---------------------------------------------

        txtResultado.text =
            "¡Observa la secuencia y encuentra la tarjeta fuera de orden!";

        // ---------------------------------------------
        // PROGRESO
        // ---------------------------------------------

        txtProgreso.text =
            "Ejercicio " +
            (ejercicioActual + 1) +
            " de " +
            ejercicios.Count;

        txtProgreso.enableWordWrapping = false;

        txtProgreso.overflowMode =
            TextOverflowModes.Overflow;

        ActualizarIndicadorProgreso();

        // ---------------------------------------------
        // FONDO
        // ---------------------------------------------

        CambiarImagen(
            imagenFondo,
            ejercicio.fondo
        );

        // ---------------------------------------------
        // TITULO
        // ---------------------------------------------

        CambiarImagen(
            imagenTitulo,
            ejercicio.titulo
        );

        // ---------------------------------------------
        // CREAR ERROR ALEATORIO
        // ---------------------------------------------

        CrearSecuenciaConError();

        // ---------------------------------------------
        // MOSTRAR TARJETAS
        // ---------------------------------------------

        for (int i = 0; i < 4; i++)
        {
            Button boton = botonesPasos[i];

            boton.gameObject.SetActive(true);

            boton.interactable = true;

            int pasoOriginal = ordenTarjetas[i];

            boton.image.sprite =
                ejercicio.pasos[pasoOriginal];

            boton.image.color = colorNormal;

            boton.image.preserveAspect = false;
        }

        // ---------------------------------------------
        // BOTONES
        // ---------------------------------------------

        btnComprobar.gameObject.SetActive(true);
        btnComprobar.interactable = true;

        btnSiguiente.gameObject.SetActive(false);
    }

    // =====================================================
    // CAMBIAR IMAGEN
    // =====================================================

    void CambiarImagen(
        Image imagen,
        Sprite spriteNuevo)
    {
        if (imagen == null || spriteNuevo == null)
            return;

        imagen.sprite = spriteNuevo;

        imagen.preserveAspect = false;
    }

    // =====================================================
    // CREAR SECUENCIA CON UN PASO DESPLAZADO
    // =====================================================

    void CrearSecuenciaConError()
    {
        // Orden correcto:
        // 0 - 1 - 2 - 3

        List<int> orden = new List<int>()
        {
            0, 1, 2, 3
        };

        // Recordar el movimiento de la ultima
        // partida para no repetirlo inmediatamente.

        string clave =
            "MundoPujllay_ErrorSecuencia_" +
            ejercicioActual;

        int movimientoAnterior =
            PlayerPrefs.GetInt(clave, -1);

        int origen;
        int destino;
        int movimientoActual;

        // Generar un movimiento diferente.
        do
        {
            origen = Random.Range(0, 4);

            destino = Random.Range(0, 4);

            movimientoActual = origen * 4 + destino;
        }
        while (origen == destino ||
               movimientoActual == movimientoAnterior);

        // Sacar una tarjeta de su posicion original.

        int tarjetaMovida = orden[origen];

        orden.RemoveAt(origen);

        // Insertarla en otra posicion.

        orden.Insert(destino, tarjetaMovida);

        // Guardar el orden resultante.

        for (int i = 0; i < 4; i++)
        {
            ordenTarjetas[i] = orden[i];
        }

        // Esta es la tarjeta que el juego
        // considera como el error.

        posicionError = destino;

        // Guardar para evitar repetir el movimiento
        // en la siguiente partida.

        PlayerPrefs.SetInt(
            clave,
            movimientoActual
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

        // Restaurar los colores.

        for (int i = 0; i < 4; i++)
        {
            botonesPasos[i].image.color =
                colorNormal;
        }

        // Resaltar tarjeta seleccionada.

        botonesPasos[indice].image.color =
            colorSeleccionado;

        // Panel_Mensaje

        txtResultado.text =
            "¡Tarjeta seleccionada! Presiona COMPROBAR.";
    }

    // =====================================================
    // COMPROBAR RESPUESTA
    // =====================================================

    void Comprobar()
    {
        if (ejercicioCompletado)
            return;

        // ---------------------------------------------
        // NO SELECCIONO UNA TARJETA
        // ---------------------------------------------

        if (tarjetaSeleccionada == -1)
        {
            txtResultado.text =
                "¡Primero selecciona una tarjeta!";

            return;
        }

        // ---------------------------------------------
        // RESPUESTA CORRECTA
        // ---------------------------------------------

        if (tarjetaSeleccionada == posicionError)
        {
            ejercicioCompletado = true;

            txtResultado.text =
                "¡Excelente! Encontraste la tarjeta fuera de orden.";

            // Colorear tarjeta seleccionada.

            botonesPasos[tarjetaSeleccionada]
                .image.color = colorCorrecto;

            // Restaurar la secuencia correcta
            // para mostrar como deberia quedar.

            MostrarSecuenciaCorrecta();

            // Bloquear las tarjetas.

            foreach (Button boton in botonesPasos)
            {
                boton.interactable = false;
            }

            btnComprobar.gameObject.SetActive(false);

            btnSiguiente.gameObject.SetActive(true);
        }

        // ---------------------------------------------
        // RESPUESTA INCORRECTA
        // ---------------------------------------------

        else
        {
            txtResultado.text =
                "¡Casi! Observa el orden e inténtalo otra vez.";

            botonesPasos[tarjetaSeleccionada]
                .image.color = colorNormal;

            tarjetaSeleccionada = -1;
        }
    }

    // =====================================================
    // MOSTRAR LA SECUENCIA CORRECTA
    // =====================================================

    void MostrarSecuenciaCorrecta()
    {
        Ejercicio ejercicio =
            ejercicios[ejercicioActual];

        // Colocar las cuatro imagenes en orden.

        for (int i = 0; i < 4; i++)
        {
            botonesPasos[i].image.sprite =
                ejercicio.pasos[i];

            botonesPasos[i].image.color =
                colorNormal;
        }

        txtResultado.text =
            "¡Muy bien! Así es el orden correcto de la secuencia.";
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
            "¡Felicidades! Encontraste los errores en las 3 secuencias.";

        txtProgreso.text = "";

        // Ocultar tarjetas.

        foreach (Button boton in botonesPasos)
        {
            boton.gameObject.SetActive(false);
        }

        // Ocultar botones.

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);

        // Ocultar circulos.

        OcultarIndicadorProgreso();
    }

    // =====================================================
    // CREAR SPRITE CIRCULAR
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
        if (panelProgreso == null &&
            txtProgreso != null)
        {
            panelProgreso =
                txtProgreso.transform.parent
                as RectTransform;
        }

        if (panelProgreso == null)
        {
            Debug.LogWarning(
                "No se encontro Panel_Progreso."
            );

            return;
        }

        spriteCirculo = CrearSpriteCircular();

        circulosProgreso =
            new Image[ejercicios.Count];

        for (int i = 0; i < ejercicios.Count; i++)
        {
            // -----------------------------------------
            // CIRCULO EXTERIOR - BORDE
            // -----------------------------------------

            GameObject objetoCirculo =
                new GameObject(
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
            // CIRCULO INTERIOR
            // -----------------------------------------

            GameObject objetoInterior =
                new GameObject(
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

            rectInterior.offsetMin =
                new Vector2(2f, 2f);

            rectInterior.offsetMax =
                new Vector2(-2f, -2f);

            Image imagenInterior =
                objetoInterior.GetComponent<Image>();

            imagenInterior.sprite = spriteCirculo;
            imagenInterior.raycastTarget = false;

            circulosProgreso[i] = imagenInterior;

            // -----------------------------------------
            // NUMERO
            // -----------------------------------------

            GameObject objetoNumero =
                new GameObject(
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

            numero.fontSize =
                tamanoCirculo * 0.5f;

            numero.fontStyle = FontStyles.Bold;

            numero.alignment =
                TextAlignmentOptions.Center;

            numero.color = Color.white;
            numero.raycastTarget = false;
        }

        ActualizarIndicadorProgreso();
    }

    // =====================================================
    // ACTUALIZAR PROGRESO
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

    // =====================================================
    // OCULTAR PROGRESO
    // =====================================================

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
}
