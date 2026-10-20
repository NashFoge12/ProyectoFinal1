
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoErrorPatron : MonoBehaviour
{
    // =====================================================
    // TEXTOS
    // =====================================================

    [Header("TEXTOS")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;

    [Header("TIEMPO DE INSTRUCCION")]
    [Range(1f, 5f)]
    public float tiempoPregunta = 2f;

    private Coroutine rutinaPregunta;

    // =====================================================
    // FONDOS
    // =====================================================

    [Header("FONDOS")]
    public Image imagenFondo;

    public Sprite fondoTren;
    public Sprite fondoGlobos;
    public Sprite fondoRobots;

    // =====================================================
    // TITULOS
    // =====================================================

    [Header("TITULOS")]
    public Image imagenTitulo;

    public Sprite tituloTren;
    public Sprite tituloGlobos;
    public Sprite tituloRobots;

    // =====================================================
    // TARJETAS
    // =====================================================

    [Header("6 TARJETAS DE SECUENCIA")]
    public Button[] botonesSecuencia = new Button[6];

    // =====================================================
    // BOTONES
    // =====================================================

    [Header("BOTONES")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // =====================================================
    // EJERCICIO 1
    // =====================================================

    [Header("EJERCICIO 1 - TREN DE COLORES")]
    public Sprite trenRojo;
    public Sprite trenAzul;
    public Sprite trenVerde;

    // =====================================================
    // EJERCICIO 2
    // =====================================================

    [Header("EJERCICIO 2 - FIESTA DE GLOBOS")]
    public Sprite globoCorazon;
    public Sprite globoEstrella;
    public Sprite globoLuna;

    // =====================================================
    // EJERCICIO 3
    // =====================================================

    [Header("EJERCICIO 3 - FABRICA DE ROBOTS")]
    public Sprite robotAzul;
    public Sprite robotAmarillo;
    public Sprite robotMorado;

    // =====================================================
    // PROGRESO
    // =====================================================

    [Header("PANEL DE PROGRESO")]
    public RectTransform panelProgreso;

    public float tamanoCirculo = 28f;
    public float separacionCirculos = 38f;
    public float posicionVerticalCirculos = -20f;

    public Color colorActivo =
        new Color(1f, 0.78f, 0.24f, 1f);

    public Color colorPendiente =
        new Color(0.78f, 0.76f, 0.72f, 1f);

    // =====================================================
    // COLORES DE TARJETAS
    // =====================================================

    [Header("COLORES DE SELECCION")]
    public Color colorNormal = Color.white;

    public Color colorSeleccionado =
        new Color(0.65f, 0.90f, 1f, 1f);

    public Color colorCorrecto =
        new Color(0.68f, 1f, 0.73f, 1f);

    public Color colorError =
        new Color(1f, 0.78f, 0.78f, 1f);

    // =====================================================
    // PATRONES ALEATORIOS
    // =====================================================

    [Header("VARIACIONES")]
    public bool evitarRepeticion = true;

    // =====================================================
    // DATOS
    // =====================================================

    private class Ejercicio
    {
        public string pregunta;
        public Sprite fondo;
        public Sprite titulo;
        public Sprite[] elementos;
    }

    private List<Ejercicio> ejercicios =
        new List<Ejercicio>();

    private Sprite[] secuenciaCorrecta;
    private Sprite[] secuenciaConError;

    private int ejercicioActual = 0;
    private int tarjetaSeleccionada = -1;
    private int posicionIncorrecta = -1;

    private bool ejercicioCompletado = false;

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

        for (int i = 0; i < botonesSecuencia.Length; i++)
        {
            int indice = i;

            botonesSecuencia[i].onClick.AddListener(
                () => SeleccionarTarjeta(indice)
            );
        }

        btnComprobar.onClick.AddListener(Comprobar);
        btnSiguiente.onClick.AddListener(SiguienteEjercicio);

        CrearEjercicios();
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
            btnComprobar == null ||
            btnSiguiente == null)
        {
            Debug.LogError(
                "Faltan textos o botones en GameManager."
            );
            return false;
        }

        if (botonesSecuencia == null ||
            botonesSecuencia.Length != 6)
        {
            Debug.LogError(
                "Debes asignar 6 botones de secuencia."
            );
            return false;
        }

        for (int i = 0; i < 6; i++)
        {
            if (botonesSecuencia[i] == null)
            {
                Debug.LogError(
                    "Falta Tarjeta_" + (i + 1)
                );
                return false;
            }
        }

        return true;
    }

    // =====================================================
    // MENSAJES
    // =====================================================

    void MostrarMensaje(string mensaje)
    {
        if (txtPregunta != null)
        {
            txtPregunta.text = mensaje;
        }
    }

    void IniciarPregunta(string pregunta)
    {
        DetenerPreguntaTemporal();

        rutinaPregunta = StartCoroutine(
            MostrarPreguntaTemporal(pregunta)
        );
    }

    IEnumerator MostrarPreguntaTemporal(string pregunta)
    {
        MostrarMensaje(pregunta);

        yield return new WaitForSeconds(tiempoPregunta);

        MostrarMensaje(
            "¡Observa el patrón y toca la tarjeta equivocada!"
        );

        rutinaPregunta = null;
    }

    void DetenerPreguntaTemporal()
    {
        if (rutinaPregunta != null)
        {
            StopCoroutine(rutinaPregunta);
            rutinaPregunta = null;
        }
    }

    // =====================================================
    // CREAR EJERCICIOS
    // =====================================================

    void CrearEjercicios()
    {
        ejercicios.Clear();

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué vagón rompe el patrón?",

            fondo = fondoTren,
            titulo = tituloTren,

            elementos = new Sprite[]
            {
                trenRojo,
                trenAzul,
                trenVerde
            }
        });

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué globo está fuera del patrón?",

            fondo = fondoGlobos,
            titulo = tituloGlobos,

            elementos = new Sprite[]
            {
                globoCorazon,
                globoEstrella,
                globoLuna
            }
        });

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¡Encuentra el robot equivocado!",

            fondo = fondoRobots,
            titulo = tituloRobots,

            elementos = new Sprite[]
            {
                robotAzul,
                robotAmarillo,
                robotMorado
            }
        });
    }

    // =====================================================
    // GENERAR PATRON ALEATORIO
    // =====================================================

    void GenerarPatron(Ejercicio ejercicio)
    {
        // Se crean combinaciones diferentes de:
        //
        // Ejercicio 1: ABABAB
        // Ejercicio 2: ABCABC
        // Ejercicio 3: AABAAB
        //
        // Luego se sustituye una figura por otra
        // para generar un unico error.

        int permutacion = Random.Range(0, 6);

        // El error aparece desde la tercera tarjeta
        // para que se pueda observar el patron.

        int cantidadPosiciones = 4;

        // 6 permutaciones x 4 posiciones x 2 sustitutos.
        // Total: 48 combinaciones posibles.

        int combinacion = Random.Range(
            0,
            6 * cantidadPosiciones * 2
        );

        string clave =
            "MundoPujllay_ErrorPatron_Ultima_" +
            ejercicioActual;

        if (evitarRepeticion)
        {
            int anterior = PlayerPrefs.GetInt(clave, -1);

            if (anterior >= 0 && anterior < 48)
            {
                combinacion = Random.Range(0, 47);

                if (combinacion >= anterior)
                {
                    combinacion++;
                }
            }
        }

        PlayerPrefs.SetInt(clave, combinacion);
        PlayerPrefs.Save();

        permutacion = combinacion / 8;

        int resto = combinacion % 8;

        posicionIncorrecta = 2 + resto / 2;

        int sustituto = resto % 2;

        Sprite[] elementos =
            ObtenerPermutacion(
                ejercicio.elementos,
                permutacion
            );

        Sprite A = elementos[0];
        Sprite B = elementos[1];
        Sprite C = elementos[2];

        if (ejercicioActual == 0)
        {
            secuenciaCorrecta = new Sprite[]
            {
                A, B, A, B, A, B
            };
        }
        else if (ejercicioActual == 1)
        {
            secuenciaCorrecta = new Sprite[]
            {
                A, B, C, A, B, C
            };
        }
        else
        {
            secuenciaCorrecta = new Sprite[]
            {
                A, A, B, A, A, B
            };
        }

        secuenciaConError =
            (Sprite[])secuenciaCorrecta.Clone();

        // Elegir una figura que sea distinta
        // de la respuesta correcta.

        List<Sprite> alternativasIncorrectas =
            new List<Sprite>();

        foreach (Sprite sprite in ejercicio.elementos)
        {
            if (sprite !=
                secuenciaCorrecta[posicionIncorrecta])
            {
                alternativasIncorrectas.Add(sprite);
            }
        }

        secuenciaConError[posicionIncorrecta] =
            alternativasIncorrectas[sustituto];
    }

    // =====================================================
    // PERMUTACIONES
    // =====================================================

    Sprite[] ObtenerPermutacion(
        Sprite[] elementos,
        int indice)
    {
        int[,] permutaciones = new int[,]
        {
            { 0, 1, 2 },
            { 0, 2, 1 },
            { 1, 0, 2 },
            { 1, 2, 0 },
            { 2, 0, 1 },
            { 2, 1, 0 }
        };

        Sprite[] resultado = new Sprite[3];

        for (int i = 0; i < 3; i++)
        {
            resultado[i] =
                elementos[permutaciones[indice, i]];
        }

        return resultado;
    }

    // =====================================================
    // CARGAR EJERCICIO
    // =====================================================

    void CargarEjercicio()
    {
        DetenerPreguntaTemporal();

        ejercicioCompletado = false;
        tarjetaSeleccionada = -1;

        Ejercicio ejercicio =
            ejercicios[ejercicioActual];

        GenerarPatron(ejercicio);

        IniciarPregunta(ejercicio.pregunta);

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

        // Mostrar las 6 tarjetas.

        for (int i = 0; i < botonesSecuencia.Length; i++)
        {
            Button boton = botonesSecuencia[i];

            boton.gameObject.SetActive(true);
            boton.interactable = true;

            boton.image.sprite = secuenciaConError[i];
            boton.image.color = colorNormal;
            boton.image.preserveAspect = false;
        }

        btnComprobar.gameObject.SetActive(true);
        btnComprobar.interactable = true;

        btnSiguiente.gameObject.SetActive(false);
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

        for (int i = 0; i < botonesSecuencia.Length; i++)
        {
            botonesSecuencia[i].image.color =
                i == indice
                ? colorSeleccionado
                : colorNormal;
        }

        MostrarMensaje(
            "¡Muy bien! Ahora presiona COMPROBAR."
        );
    }

    // =====================================================
    // COMPROBAR RESPUESTA
    // =====================================================

    void Comprobar()
    {
        if (ejercicioCompletado)
            return;

        DetenerPreguntaTemporal();

        if (tarjetaSeleccionada == -1)
        {
            MostrarMensaje(
                "¡Primero selecciona la tarjeta equivocada!"
            );
            return;
        }

        if (tarjetaSeleccionada == posicionIncorrecta)
        {
            ejercicioCompletado = true;

            // =============================================
            // GUARDAR ESTRELLA - PATRONES MISION 3
            // =============================================

            ProgresoEstrellas.GuardarEjercicio(
                "Patrones",
                3,
                ejercicioActual + 1
            );

            // Reemplazar la figura equivocada.

            botonesSecuencia[posicionIncorrecta]
                .image.sprite =
                    secuenciaCorrecta[posicionIncorrecta];

            botonesSecuencia[posicionIncorrecta]
                .image.color = colorCorrecto;

            foreach (Button boton in botonesSecuencia)
            {
                boton.interactable = false;
            }

            MostrarMensaje(
                "¡Excelente! ¡Encontraste y corregiste el error!"
            );

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);
        }
        else
        {
            botonesSecuencia[tarjetaSeleccionada]
                .image.color = colorError;

            tarjetaSeleccionada = -1;

            MostrarMensaje(
                "¡Casi! Observa el orden e inténtalo otra vez."
            );
        }
    }

    // =====================================================
    // SIGUIENTE EJERCICIO
    // =====================================================

    void SiguienteEjercicio()
    {
        if (!ejercicioCompletado)
            return;

        DetenerPreguntaTemporal();

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
        MostrarMensaje(
            "¡MISIÓN COMPLETADA!\n" +
            "¡Felicidades! Encontraste los 3 errores."
        );

        txtProgreso.text = "";

        foreach (Button boton in botonesSecuencia)
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

        circulosProgreso =
            new Image[ejercicios.Count];

        for (int i = 0; i < ejercicios.Count; i++)
        {
            GameObject objeto = new GameObject(
                "Circulo_Progreso_" + (i + 1),
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );

            objeto.transform.SetParent(
                panelProgreso,
                false
            );

            RectTransform rect =
                objeto.GetComponent<RectTransform>();

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

            Image imagen = objeto.GetComponent<Image>();

            imagen.sprite = spriteCirculo;
            imagen.raycastTarget = false;

            circulosProgreso[i] = imagen;

            GameObject objetoNumero = new GameObject(
                "Numero_" + (i + 1),
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI)
            );

            objetoNumero.transform.SetParent(
                objeto.transform,
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

    void OnDisable()
    {
        DetenerPreguntaTemporal();
    }
}
