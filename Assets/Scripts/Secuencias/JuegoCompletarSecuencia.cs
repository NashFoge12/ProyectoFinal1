
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoCompletarSecuencia : MonoBehaviour
{
    [Header("Textos de interfaz")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;
    public TMP_Text txtResultado;

    [Header("Fondos")]
    public Image imagenFondo;
    public Sprite fondoColores;
    public Sprite fondoFlechas;
    public Sprite fondoNumeros;

    [Header("Titulos en imagen")]
    public Image imagenTitulo;
    public Sprite tituloColores;
    public Sprite tituloFlechas;
    public Sprite tituloNumeros;

    [Header("Secuencia principal")]
    public Image imagenSecuencia;
    public Sprite secuenciaColores;
    public Sprite secuenciaFlechas;
    public Sprite secuenciaNumeros;

    [Header("Botones de opciones")]
    public Button[] botonesOpciones = new Button[3];

    [Header("Opciones - Colores")]
    public Sprite coloresCorrecta;
    public Sprite coloresIncorrecta1;
    public Sprite coloresIncorrecta2;

    [Header("Opciones - Flechas")]
    public Sprite flechasCorrecta;
    public Sprite flechasIncorrecta1;
    public Sprite flechasIncorrecta2;

    [Header("Opciones - Numeros")]
    public Sprite numerosCorrecta;
    public Sprite numerosIncorrecta1;
    public Sprite numerosIncorrecta2;

    [Header("Botones")]
    public Button btnComprobar;
    public Button btnSiguiente;

    [Header("Indicador de progreso")]
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

    [Header("Colores de seleccion")]
    public Color colorNormal = Color.white;

    public Color colorSeleccionado =
        new Color(0.65f, 0.90f, 1f, 1f);

    private const int totalEjercicios = 3;

    private int ejercicioActual = 0;
    private int opcionSeleccionada = -1;
    private int indiceCorrectoActual = -1;

    private bool ejercicioCompletado = false;

    private List<int> posicionesCorrectas =
        new List<int>();

    private Image[] circulosProgreso;
    private Sprite spriteCirculoGenerado;

    private string[] preguntas =
    {
        "Descubre el patrón y completa lo que falta.",
        "Observa la repetición y completa la secuencia.",
        "Encuentra los números que faltan."
    };

    // =====================================================
    // INICIO
    // =====================================================

    void Start()
    {
        if (txtPregunta == null ||
            txtProgreso == null ||
            txtResultado == null ||
            btnComprobar == null ||
            btnSiguiente == null ||
            imagenSecuencia == null)
        {
            Debug.LogError(
                "Faltan referencias en JuegoCompletarSecuencia."
            );

            enabled = false;
            return;
        }

        if (botonesOpciones == null ||
            botonesOpciones.Length != 3)
        {
            Debug.LogError(
                "Debes asignar exactamente 3 botones."
            );

            enabled = false;
            return;
        }

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            if (botonesOpciones[i] == null)
            {
                Debug.LogError(
                    "Falta asignar el boton " + (i + 1)
                );

                enabled = false;
                return;
            }

            int indice = i;

            botonesOpciones[i].onClick.AddListener(
                () => SeleccionarOpcion(indice)
            );
        }

        btnComprobar.onClick.AddListener(
            ComprobarRespuesta
        );

        btnSiguiente.onClick.AddListener(
            SiguienteEjercicio
        );

        PrepararPosicionesCorrectas();
        CrearIndicadorProgreso();
        CargarEjercicio();
    }

    // =====================================================
    // MEZCLAR POSICIONES CORRECTAS
    // =====================================================

    void PrepararPosicionesCorrectas()
    {
        posicionesCorrectas.Clear();

        posicionesCorrectas.Add(0);
        posicionesCorrectas.Add(1);
        posicionesCorrectas.Add(2);

        for (int i = posicionesCorrectas.Count - 1;
             i > 0; i--)
        {
            int aleatorio = Random.Range(0, i + 1);

            int temporal = posicionesCorrectas[i];

            posicionesCorrectas[i] =
                posicionesCorrectas[aleatorio];

            posicionesCorrectas[aleatorio] =
                temporal;
        }
    }

    // =====================================================
    // CARGAR EJERCICIO
    // =====================================================

    void CargarEjercicio()
    {
        opcionSeleccionada = -1;
        indiceCorrectoActual = -1;
        ejercicioCompletado = false;

        txtResultado.text =
            "Selecciona la opción que completa la secuencia.";

        txtPregunta.text =
            preguntas[ejercicioActual];

        txtProgreso.text =
            "Ejercicio " + (ejercicioActual + 1) +
            " de " + totalEjercicios;

        txtProgreso.textWrappingMode = TextWrappingModes.NoWrap;
        txtProgreso.overflowMode =
            TextOverflowModes.Overflow;

        ActualizarIndicadorProgreso();

        imagenSecuencia.gameObject.SetActive(true);

        if (ejercicioActual == 0)
        {
            CambiarImagen(
                imagenFondo, fondoColores, false
            );

            CambiarImagen(
                imagenTitulo, tituloColores, false
            );

            CambiarImagen(
                imagenSecuencia, secuenciaColores, false
            );
        }
        else if (ejercicioActual == 1)
        {
            CambiarImagen(
                imagenFondo, fondoFlechas, false
            );

            CambiarImagen(
                imagenTitulo, tituloFlechas, false
            );

            CambiarImagen(
                imagenSecuencia, secuenciaFlechas, false
            );
        }
        else
        {
            CambiarImagen(
                imagenFondo, fondoNumeros, false
            );

            CambiarImagen(
                imagenTitulo, tituloNumeros, false
            );

            CambiarImagen(
                imagenSecuencia, secuenciaNumeros, false
            );
        }

        CargarOpciones();

        btnComprobar.gameObject.SetActive(true);
        btnComprobar.interactable = true;

        btnSiguiente.gameObject.SetActive(false);
    }

    // =====================================================
    // CAMBIAR IMAGEN
    // =====================================================

    void CambiarImagen(
        Image imagen,
        Sprite spriteNuevo,
        bool mantenerProporcion)
    {
        if (imagen == null || spriteNuevo == null)
            return;

        imagen.sprite = spriteNuevo;
        imagen.preserveAspect = mantenerProporcion;
    }

    // =====================================================
    // CARGAR OPCIONES
    // =====================================================

    void CargarOpciones()
    {
        Sprite correcta = null;
        Sprite incorrecta1 = null;
        Sprite incorrecta2 = null;

        if (ejercicioActual == 0)
        {
            correcta = coloresCorrecta;
            incorrecta1 = coloresIncorrecta1;
            incorrecta2 = coloresIncorrecta2;
        }
        else if (ejercicioActual == 1)
        {
            correcta = flechasCorrecta;
            incorrecta1 = flechasIncorrecta1;
            incorrecta2 = flechasIncorrecta2;
        }
        else
        {
            correcta = numerosCorrecta;
            incorrecta1 = numerosIncorrecta1;
            incorrecta2 = numerosIncorrecta2;
        }

        int posicionCorrecta =
            posicionesCorrectas[ejercicioActual];

        indiceCorrectoActual = posicionCorrecta;

        List<int> posicionesIncorrectas =
            new List<int>();

        for (int i = 0; i < 3; i++)
        {
            if (i != posicionCorrecta)
                posicionesIncorrectas.Add(i);
        }

        if (Random.value > 0.5f)
        {
            int temporal = posicionesIncorrectas[0];

            posicionesIncorrectas[0] =
                posicionesIncorrectas[1];

            posicionesIncorrectas[1] = temporal;
        }

        for (int i = 0; i < 3; i++)
        {
            botonesOpciones[i].gameObject.SetActive(true);
            botonesOpciones[i].interactable = true;
            botonesOpciones[i].image.color = colorNormal;
        }

        botonesOpciones[posicionCorrecta].image.sprite =
            correcta;

        botonesOpciones[posicionesIncorrectas[0]]
            .image.sprite = incorrecta1;

        botonesOpciones[posicionesIncorrectas[1]]
            .image.sprite = incorrecta2;

        for (int i = 0; i < 3; i++)
        {
            botonesOpciones[i].image.preserveAspect = false;
        }
    }

    // =====================================================
    // SELECCIONAR OPCION
    // =====================================================

    void SeleccionarOpcion(int indice)
    {
        if (ejercicioCompletado)
            return;

        opcionSeleccionada = indice;

        for (int i = 0; i < 3; i++)
        {
            botonesOpciones[i].image.color = colorNormal;
        }

        botonesOpciones[indice].image.color =
            colorSeleccionado;

        txtResultado.text =
            "Opción seleccionada. Pulsa COMPROBAR.";
    }

    // =====================================================
    // COMPROBAR RESPUESTA
    // =====================================================

    void ComprobarRespuesta()
    {
        if (ejercicioCompletado)
            return;

        if (opcionSeleccionada == -1)
        {
            txtResultado.text =
                "¡Selecciona una opción primero!";

            return;
        }

        if (opcionSeleccionada == indiceCorrectoActual)
        {
            ejercicioCompletado = true;

            // =============================================
            // GUARDAR ESTRELLAS - SECUENCIAS MISION 2
            // =============================================

            ProgresoEstrellas.GuardarEjercicio(
                "Secuencias",
                2,
                ejercicioActual + 1
            );

            txtResultado.text =
                "¡Correcto! Completaste la secuencia.";

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);

            foreach (Button boton in botonesOpciones)
            {
                boton.interactable = false;
            }
        }
        else
        {
            txtResultado.text =
                "¡Casi! Observa la secuencia e inténtalo otra vez.";

            botonesOpciones[opcionSeleccionada]
                .image.color = colorNormal;

            opcionSeleccionada = -1;
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

        if (ejercicioActual < totalEjercicios)
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
        txtPregunta.text = "¡MISIÓN COMPLETADA!";

        txtProgreso.text = "";

        txtResultado.text =
            "¡Excelente! Completaste correctamente las 3 secuencias.";

        imagenSecuencia.gameObject.SetActive(false);

        foreach (Button boton in botonesOpciones)
        {
            boton.gameObject.SetActive(false);
        }

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);

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
        if (panelProgreso == null && txtProgreso != null)
        {
            panelProgreso =
                txtProgreso.transform.parent as RectTransform;
        }

        if (panelProgreso == null)
        {
            Debug.LogError(
                "No se encontro Panel_Progreso."
            );
            return;
        }

        circulosProgreso = new Image[totalEjercicios];
        spriteCirculoGenerado = CrearSpriteCircular();

        for (int i = 0; i < totalEjercicios; i++)
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

            Image imagenBorde =
                objetoCirculo.GetComponent<Image>();

            imagenBorde.sprite = spriteCirculoGenerado;
            imagenBorde.color = colorBorde;
            imagenBorde.raycastTarget = false;

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

            imagenInterior.sprite = spriteCirculoGenerado;
            imagenInterior.color = colorPendiente;
            imagenInterior.raycastTarget = false;

            circulosProgreso[i] = imagenInterior;

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
            numero.enableAutoSizing = false;
            numero.raycastTarget = false;
        }

        ActualizarIndicadorProgreso();
    }

    // =====================================================
    // ACTUALIZAR INDICADOR
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
    // OCULTAR INDICADOR
    // =====================================================

    void OcultarIndicadorProgreso()
    {
        if (circulosProgreso == null)
            return;

        foreach (Image circulo in circulosProgreso)
        {
            if (circulo != null)
            {
                circulo.transform.parent
                    .gameObject.SetActive(false);
            }
        }
    }
}
