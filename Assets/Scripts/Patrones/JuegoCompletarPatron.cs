using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JuegoCompletarPatron : MonoBehaviour
{
    // =====================================================
    // TEXTOS
    // =====================================================

    [Header("TEXTOS")]
    public TMP_Text txtPregunta;

    [Header("TIEMPO DE INSTRUCCION")]
    [Range(1f, 5f)]
    public float tiempoPregunta = 2f;

    private Coroutine rutinaPregunta;

    // =====================================================
    // FONDOS
    // =====================================================

    [Header("FONDOS")]
    public Image imagenFondo;

    public Sprite fondoDulceria;
    public Sprite fondoCielo;
    public Sprite fondoFiguras;

    // =====================================================
    // FONDO DEL PANEL PREGUNTA
    // =====================================================

    [Header("FONDO DEL PANEL PREGUNTA")]
    public Image imagenPanelPregunta;

    public Sprite fondoPanelDulceria;
    public Sprite fondoPanelCielo;
    public Sprite fondoPanelFiguras;

    // =====================================================
    // TITULOS
    // =====================================================

    [Header("TITULOS")]
    public Image imagenTitulo;

    public Sprite tituloDulceria;
    public Sprite tituloCielo;
    public Sprite tituloFiguras;

    // =====================================================
    // INTERROGACIONES
    // =====================================================

    [Header("SPRITES DE INTERROGACION")]
    public Sprite interrogacionDulceria;
    public Sprite interrogacionCielo;
    public Sprite interrogacionFiguras;

    // =====================================================
    // TARJETAS DE SECUENCIA
    // =====================================================

    [Header("6 IMAGES DE SECUENCIA")]
    public Image[] imagenesSecuencia = new Image[6];

    // =====================================================
    // TARJETAS ARRASTRABLES
    // =====================================================

    [Header("3 IMAGES DE ALTERNATIVAS")]
    public Image[] imagenesOpciones = new Image[3];

    // =====================================================
    // BOTONES
    // =====================================================

    [Header("BOTONES")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // =====================================================
    // DULCERIA
    // =====================================================

    [Header("EJERCICIO 1 - DULCERIA")]
    public Sprite dulceCaramelo;
    public Sprite dulceChocolate;
    public Sprite dulcePaleta;

    // =====================================================
    // CIELO
    // =====================================================

    [Header("EJERCICIO 2 - CIELO")]
    public Sprite cieloSol;
    public Sprite cieloLuna;
    public Sprite cieloEstrella;

    // =====================================================
    // FIGURAS
    // =====================================================

    [Header("EJERCICIO 3 - FIGURAS")]
    public Sprite figuraCirculo;
    public Sprite figuraCuadrado;
    public Sprite figuraTriangulo;

    // =====================================================
    // PROGRESO
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

    [Header("BORDE DE LOS CIRCULOS")]
    public Color colorBorde = new Color(0.43f, 0.29f, 0.17f, 1f);
    [Range(1f, 10f)] public float grosorBorde = 2f;

    // =====================================================
    // COLORES
    // =====================================================

    [Header("COLORES DE CASILLAS")]
    public Color colorNormal = Color.white;

    public Color colorCorrecto =
        new Color(0.68f, 1f, 0.73f, 1f);

    public Color colorError =
        new Color(1f, 0.78f, 0.78f, 1f);

    // =====================================================
    // ALEATORIEDAD
    // =====================================================

    [Header("PATRONES ALEATORIOS")]
    public bool evitarRepeticion = true;
    public bool mezclarAlternativas = true;

    // =====================================================
    // DATOS
    // =====================================================

    private class Ejercicio
    {
        public string pregunta;
        public Sprite fondo;
        public Sprite fondoPanelPregunta;
        public Sprite titulo;
        public Sprite interrogacion;
        public Sprite[] elementos;
        public int cantidadCasillas;
    }

    private List<Ejercicio> ejercicios =
        new List<Ejercicio>();

    private int ejercicioActual = 0;

    private Sprite[] secuenciaCorrecta;
    private Sprite[] secuenciaVisible;

    private List<int> posicionesVacias =
        new List<int>();

    private bool ejercicioCompletado = false;

    private Image[] circulosProgreso;
    private Sprite spriteCirculo;

    private Canvas canvasPrincipal;

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

        canvasPrincipal =
            GetComponentInParent<Canvas>();

        if (canvasPrincipal == null)
        {
            canvasPrincipal =
                FindFirstObjectByType<Canvas>();
        }

        if (canvasPrincipal == null)
        {
            Debug.LogError(
                "No se encontro un Canvas."
            );

            enabled = false;
            return;
        }

        PrepararArrastrarYSoltar();

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
            btnComprobar == null ||
            btnSiguiente == null)
        {
            Debug.LogError(
                "Faltan textos o botones en GameManager."
            );

            return false;
        }

        if (imagenesSecuencia == null ||
            imagenesSecuencia.Length != 6)
        {
            Debug.LogError(
                "Debes asignar 6 Images de secuencia."
            );

            return false;
        }

        if (imagenesOpciones == null ||
            imagenesOpciones.Length != 3)
        {
            Debug.LogError(
                "Debes asignar 3 Images de alternativas."
            );

            return false;
        }

        for (int i = 0; i < 6; i++)
        {
            if (imagenesSecuencia[i] == null)
            {
                Debug.LogError(
                    "Falta Tarjeta_" + (i + 1)
                );

                return false;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (imagenesOpciones[i] == null)
            {
                Debug.LogError(
                    "Falta Opcion_" + (i + 1)
                );

                return false;
            }
        }

        return true;
    }

    // =====================================================
    // PREPARAR DRAG AND DROP
    // =====================================================

    void PrepararArrastrarYSoltar()
    {
        for (int i = 0; i < imagenesOpciones.Length; i++)
        {
            Image imagen = imagenesOpciones[i];

            imagen.raycastTarget = true;

            ArrastrarOpcionPatron componente =
                imagen.GetComponent<ArrastrarOpcionPatron>();

            if (componente == null)
            {
                componente =
                    imagen.gameObject.AddComponent<
                        ArrastrarOpcionPatron>();
            }

            componente.Configurar(
                this,
                canvasPrincipal
            );
        }

        for (int i = 0; i < imagenesSecuencia.Length; i++)
        {
            Image imagen = imagenesSecuencia[i];

            imagen.raycastTarget = true;

            SoltarCasillaPatron componente =
                imagen.GetComponent<SoltarCasillaPatron>();

            if (componente == null)
            {
                componente =
                    imagen.gameObject.AddComponent<
                        SoltarCasillaPatron>();
            }

            componente.Configurar(this, i);
        }
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
            "¡Arrastra la figura correcta hasta la tarjeta con ?!"
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
                "¿Qué dulce falta en el patrón?",

            fondo = fondoDulceria,
            fondoPanelPregunta = fondoPanelDulceria,
            titulo = tituloDulceria,
            interrogacion = interrogacionDulceria,

            elementos = new Sprite[]
            {
                dulceCaramelo,
                dulceChocolate,
                dulcePaleta
            },

            cantidadCasillas = 1
        });

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué figura falta en el cielo?",

            fondo = fondoCielo,
            fondoPanelPregunta = fondoPanelCielo,
            titulo = tituloCielo,
            interrogacion = interrogacionCielo,

            elementos = new Sprite[]
            {
                cieloSol,
                cieloLuna,
                cieloEstrella
            },

            cantidadCasillas = 1
        });

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¡Completa los dos espacios del patrón!",

            fondo = fondoFiguras,
            fondoPanelPregunta = fondoPanelFiguras,
            titulo = tituloFiguras,
            interrogacion = interrogacionFiguras,

            elementos = new Sprite[]
            {
                figuraCirculo,
                figuraCuadrado,
                figuraTriangulo
            },

            cantidadCasillas = 2
        });
    }

    // =====================================================
    // GENERAR PATRON
    // =====================================================

    void GenerarPatron(Ejercicio ejercicio)
    {
        int permutacion = Random.Range(0, 6);

        string clave =
            "MundoPujllay_CompletarPatron_Ultima_" +
            ejercicioActual;

        if (evitarRepeticion)
        {
            int anterior = PlayerPrefs.GetInt(clave, -1);

            if (anterior >= 0 && anterior < 6)
            {
                permutacion = Random.Range(0, 5);

                if (permutacion >= anterior)
                {
                    permutacion++;
                }
            }
        }

        PlayerPrefs.SetInt(clave, permutacion);
        PlayerPrefs.Save();

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
                A, B, A, B, A
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
                A, B, A, B, A, B
            };
        }

        secuenciaVisible =
            (Sprite[])secuenciaCorrecta.Clone();

        ElegirCasillasVacias(ejercicio.cantidadCasillas);
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
    // ELEGIR CASILLAS VACIAS
    // =====================================================

    void ElegirCasillasVacias(int cantidad)
    {
        posicionesVacias.Clear();

        List<int> candidatas = new List<int>();

        // Mantener las primeras dos figuras visibles.

        for (int i = 2; i < secuenciaCorrecta.Length; i++)
        {
            candidatas.Add(i);
        }

        for (int i = candidatas.Count - 1; i > 0; i--)
        {
            int aleatorio = Random.Range(0, i + 1);

            int temporal = candidatas[i];
            candidatas[i] = candidatas[aleatorio];
            candidatas[aleatorio] = temporal;
        }

        for (int i = 0; i < cantidad; i++)
        {
            posicionesVacias.Add(candidatas[i]);
        }

        foreach (int posicion in posicionesVacias)
        {
            secuenciaVisible[posicion] = null;
        }
    }

    // =====================================================
    // CARGAR EJERCICIO
    // =====================================================

    void CargarEjercicio()
    {
        DetenerPreguntaTemporal();

        ejercicioCompletado = false;

        Ejercicio ejercicio =
            ejercicios[ejercicioActual];

        GenerarPatron(ejercicio);

        IniciarPregunta(ejercicio.pregunta);

        ActualizarIndicadorProgreso();

        if (imagenFondo != null &&
            ejercicio.fondo != null)
        {
            imagenFondo.sprite = ejercicio.fondo;
        }

        // ACTUALIZAR FONDO DEL PANEL PREGUNTA

        if (imagenPanelPregunta != null &&
            ejercicio.fondoPanelPregunta != null)
        {
            imagenPanelPregunta.sprite =
                ejercicio.fondoPanelPregunta;
        }

        if (imagenTitulo != null &&
            ejercicio.titulo != null)
        {
            imagenTitulo.sprite = ejercicio.titulo;
        }

        ActualizarSecuencia();
        CrearAlternativas(ejercicio);

        btnComprobar.gameObject.SetActive(true);
        btnComprobar.interactable = true;

        btnSiguiente.gameObject.SetActive(false);
    }

    // =====================================================
    // ACTUALIZAR SECUENCIA
    // =====================================================

    void ActualizarSecuencia()
    {
        Ejercicio ejercicio =
            ejercicios[ejercicioActual];

        for (int i = 0; i < imagenesSecuencia.Length; i++)
        {
            Image imagen = imagenesSecuencia[i];

            if (i >= secuenciaCorrecta.Length)
            {
                imagen.gameObject.SetActive(false);
                continue;
            }

            imagen.gameObject.SetActive(true);
            imagen.color = colorNormal;

            if (secuenciaVisible[i] == null)
            {
                imagen.sprite =
                    ejercicio.interrogacion;
            }
            else
            {
                imagen.sprite =
                    secuenciaVisible[i];
            }

            imagen.preserveAspect = false;

            SoltarCasillaPatron componente =
                imagen.GetComponent<SoltarCasillaPatron>();

            if (componente != null)
            {
                componente.Habilitar(
                    posicionesVacias.Contains(i) &&
                    !ejercicioCompletado
                );
            }
        }
    }

    // =====================================================
    // CREAR ALTERNATIVAS
    // =====================================================

    void CrearAlternativas(Ejercicio ejercicio)
    {
        List<Sprite> opciones =
            new List<Sprite>(ejercicio.elementos);

        if (mezclarAlternativas)
        {
            for (int i = opciones.Count - 1; i > 0; i--)
            {
                int aleatorio = Random.Range(0, i + 1);

                Sprite temporal = opciones[i];
                opciones[i] = opciones[aleatorio];
                opciones[aleatorio] = temporal;
            }
        }

        for (int i = 0; i < imagenesOpciones.Length; i++)
        {
            Image imagen = imagenesOpciones[i];

            imagen.gameObject.SetActive(true);
            imagen.sprite = opciones[i];
            imagen.color = colorNormal;
            imagen.preserveAspect = false;

            ArrastrarOpcionPatron arrastrar =
                imagen.GetComponent<ArrastrarOpcionPatron>();

            if (arrastrar != null)
            {
                arrastrar.Habilitar(true);
            }
        }
    }

    // =====================================================
    // RECIBIR FIGURA ARRASTRADA
    // =====================================================

    public void ColocarFigura(int posicion, Sprite figura)
    {
        if (ejercicioCompletado)
            return;

        if (!posicionesVacias.Contains(posicion))
            return;

        if (figura == null)
            return;

        DetenerPreguntaTemporal();

        secuenciaVisible[posicion] = figura;

        ActualizarSecuencia();

        MostrarMensaje(
            "¡Muy bien! Completa los espacios y presiona COMPROBAR."
        );
    }

    // =====================================================
    // COMPROBAR
    // =====================================================

    void Comprobar()
    {
        if (ejercicioCompletado)
            return;

        DetenerPreguntaTemporal();

        foreach (int posicion in posicionesVacias)
        {
            if (secuenciaVisible[posicion] == null)
            {
                MostrarMensaje(
                    "¡Todavía falta completar una casilla!"
                );
                return;
            }
        }

        bool todoCorrecto = true;

        foreach (int posicion in posicionesVacias)
        {
            if (secuenciaVisible[posicion] !=
                secuenciaCorrecta[posicion])
            {
                todoCorrecto = false;
            }
        }

        if (todoCorrecto)
        {
            ejercicioCompletado = true;

            // =============================================
            // GUARDAR ESTRELLA - PATRONES MISION 2
            // =============================================

            ProgresoEstrellas.GuardarEjercicio(
                "Patrones",
                2,
                ejercicioActual + 1
            );

            MostrarMensaje(
                "¡Excelente! ¡Completaste el patrón!"
            );

            foreach (int posicion in posicionesVacias)
            {
                imagenesSecuencia[posicion].color =
                    colorCorrecto;
            }

            foreach (Image imagen in imagenesOpciones)
            {
                ArrastrarOpcionPatron arrastrar =
                    imagen.GetComponent<ArrastrarOpcionPatron>();

                if (arrastrar != null)
                {
                    arrastrar.Habilitar(false);
                }
            }

            foreach (Image imagen in imagenesSecuencia)
            {
                SoltarCasillaPatron soltar =
                    imagen.GetComponent<SoltarCasillaPatron>();

                if (soltar != null)
                {
                    soltar.Habilitar(false);
                }
            }

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);
        }
        else
        {
            MostrarMensaje(
                "¡Casi! Revisa las figuras e inténtalo otra vez."
            );

            foreach (int posicion in posicionesVacias)
            {
                imagenesSecuencia[posicion].color =
                    secuenciaVisible[posicion] ==
                    secuenciaCorrecta[posicion]
                    ? colorCorrecto
                    : colorError;
            }
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
        MostrarMensaje("¡MISIÓN COMPLETADA!");

        foreach (Image imagen in imagenesSecuencia)
        {
            imagen.gameObject.SetActive(false);
        }

        foreach (Image imagen in imagenesOpciones)
        {
            imagen.gameObject.SetActive(false);
        }

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);

        if (circulosProgreso != null)
        {
            foreach (Image circulo in circulosProgreso)
            {
                if (circulo != null)
                {
                    circulo.transform.parent.gameObject.SetActive(false);
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
    // CREAR CIRCULOS DE PROGRESO
    // =====================================================

    void CrearIndicadorProgreso()
    {
        if (panelProgreso == null)
        {
            GameObject encontrado = GameObject.Find("Panel_Progreso");
            if (encontrado != null)
                panelProgreso = encontrado.GetComponent<RectTransform>();
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
            imagen.color = colorBorde;
            imagen.raycastTarget = false;

            // Circulo interior: deja visible el borde exterior.
            GameObject interior = new GameObject(
                "Interior", typeof(RectTransform),
                typeof(CanvasRenderer), typeof(Image));
            interior.transform.SetParent(objeto.transform, false);
            RectTransform rectInterior = interior.GetComponent<RectTransform>();
            rectInterior.anchorMin = Vector2.zero;
            rectInterior.anchorMax = Vector2.one;
            float borde = Mathf.Clamp(grosorBorde, 0f, tamanoCirculo * 0.45f);
            rectInterior.offsetMin = new Vector2(borde, borde);
            rectInterior.offsetMax = new Vector2(-borde, -borde);
            Image imagenInterior = interior.GetComponent<Image>();
            imagenInterior.sprite = spriteCirculo;
            imagenInterior.raycastTarget = false;
            circulosProgreso[i] = imagenInterior;

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

// =========================================================
// COMPONENTE PARA ARRASTRAR LAS ALTERNATIVAS
// =========================================================

public class ArrastrarOpcionPatron :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    private JuegoCompletarPatron gameManager;
    private Canvas canvas;

    private Image imagen;
    private bool habilitado = true;

    private GameObject objetoFantasma;
    private RectTransform rectFantasma;

    public void Configurar(
        JuegoCompletarPatron manager,
        Canvas canvasPrincipal)
    {
        gameManager = manager;
        canvas = canvasPrincipal;

        imagen = GetComponent<Image>();
    }

    public void Habilitar(bool valor)
    {
        habilitado = valor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!habilitado || imagen == null || canvas == null)
            return;

        objetoFantasma = new GameObject(
            "Figura_Arrastrada",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        objetoFantasma.transform.SetParent(
            canvas.transform,
            false
        );

        objetoFantasma.transform.SetAsLastSibling();

        rectFantasma =
            objetoFantasma.GetComponent<RectTransform>();

        RectTransform original =
            imagen.rectTransform;

        rectFantasma.sizeDelta = original.rect.size;

        Image copia =
            objetoFantasma.GetComponent<Image>();

        copia.sprite = imagen.sprite;
        copia.color = imagen.color;
        copia.preserveAspect = true;
        copia.raycastTarget = false;

        MoverFantasma(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (objetoFantasma == null)
            return;

        MoverFantasma(eventData);
    }

    void MoverFantasma(PointerEventData eventData)
    {
        RectTransform canvasRect =
            canvas.transform as RectTransform;

        Camera camara =
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : eventData.pressEventCamera;

        Vector2 posicionLocal;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            camara,
            out posicionLocal))
        {
            rectFantasma.anchoredPosition =
                posicionLocal;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (objetoFantasma != null)
        {
            Destroy(objetoFantasma);
        }

        objetoFantasma = null;
        rectFantasma = null;
    }
}

// =========================================================
// COMPONENTE PARA RECIBIR FIGURAS
// =========================================================

public class SoltarCasillaPatron :
    MonoBehaviour,
    IDropHandler
{
    private JuegoCompletarPatron gameManager;
    private int posicion;
    private bool habilitado;

    public void Configurar(
        JuegoCompletarPatron manager,
        int indice)
    {
        gameManager = manager;
        posicion = indice;
    }

    public void Habilitar(bool valor)
    {
        habilitado = valor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!habilitado || gameManager == null)
            return;

        if (eventData.pointerDrag == null)
            return;

        ArrastrarOpcionPatron opcion =
            eventData.pointerDrag.GetComponent<
                ArrastrarOpcionPatron>();

        if (opcion == null)
            return;

        Image imagen =
            eventData.pointerDrag.GetComponent<Image>();

        if (imagen == null)
            return;

        gameManager.ColocarFigura(
            posicion,
            imagen.sprite
        );
    }
}