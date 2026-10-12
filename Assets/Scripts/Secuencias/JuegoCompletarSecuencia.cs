
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoCompletarSecuencia : MonoBehaviour
{
    // =====================================================
    // TEXTOS
    // =====================================================

    [Header("Textos de interfaz")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;
    public TMP_Text txtResultado;

    // =====================================================
    // FONDOS
    // =====================================================

    [Header("Fondos")]
    public Image imagenFondo;

    public Sprite fondoColores;
    public Sprite fondoFlechas;
    public Sprite fondoNumeros;

    // =====================================================
    // TITULOS
    // =====================================================

    [Header("Titulos en imagen")]
    public Image imagenTitulo;

    public Sprite tituloColores;
    public Sprite tituloFlechas;
    public Sprite tituloNumeros;

    // =====================================================
    // SECUENCIAS EN IMAGEN
    // =====================================================

    [Header("Secuencia principal")]
    public Image imagenSecuencia;

    public Sprite secuenciaColores;
    public Sprite secuenciaFlechas;
    public Sprite secuenciaNumeros;

    // =====================================================
    // BOTONES DE OPCIONES
    // =====================================================

    [Header("Botones de opciones")]
    public Button[] botonesOpciones = new Button[3];

    // =====================================================
    // EJERCICIO 1 - COLORES
    // =====================================================

    [Header("Opciones - Colores")]

    // ROJO - AZUL
    public Sprite coloresCorrecta;

    // AZUL - ROJO
    public Sprite coloresIncorrecta1;

    // ROJO - ROJO
    public Sprite coloresIncorrecta2;

    // =====================================================
    // EJERCICIO 2 - FLECHAS
    // =====================================================

    [Header("Opciones - Flechas")]

    // ARRIBA - ABAJO
    public Sprite flechasCorrecta;

    // ABAJO - ARRIBA
    public Sprite flechasIncorrecta1;

    // ARRIBA - ARRIBA
    public Sprite flechasIncorrecta2;

    // =====================================================
    // EJERCICIO 3 - NUMEROS
    // =====================================================

    [Header("Opciones - Numeros")]

    // 2 - 3
    public Sprite numerosCorrecta;

    // 3 - 2
    public Sprite numerosIncorrecta1;

    // 1 - 2
    public Sprite numerosIncorrecta2;

    // =====================================================
    // BOTONES DE CONTROL
    // =====================================================

    [Header("Botones")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // =====================================================
    // INDICADOR DE PROGRESO
    // =====================================================

    [Header("Indicador de progreso")]

    [Tooltip("Panel que contiene Txt_Progreso")]
    public RectTransform panelProgreso;

    [Tooltip("Tamano de cada circulo")]
    public float tamanoCirculo = 28f;

    [Tooltip("Separacion entre circulos")]
    public float separacionCirculos = 38f;

    [Tooltip("Posicion vertical dentro del panel")]
    public float posicionVerticalCirculos = -20f;

    [Tooltip("Color del ejercicio alcanzado")]
    public Color colorActivo =
        new Color(1f, 0.78f, 0.24f, 1f);

    [Tooltip("Color de los ejercicios pendientes")]
    public Color colorPendiente =
        new Color(0.78f, 0.76f, 0.72f, 1f);

    [Tooltip("Color del borde de los circulos")]
    public Color colorBorde =
        new Color(0.43f, 0.29f, 0.17f, 1f);

    // =====================================================
    // COLORES DE OPCIONES
    // =====================================================

    [Header("Colores de seleccion")]

    public Color colorNormal = Color.white;

    public Color colorSeleccionado =
        new Color(0.65f, 0.90f, 1f, 1f);

    // =====================================================
    // VARIABLES INTERNAS
    // =====================================================

    private const int totalEjercicios = 3;

    private int ejercicioActual = 0;

    private int opcionSeleccionada = -1;

    private int indiceCorrectoActual = -1;

    private bool ejercicioCompletado = false;

    private List<int> posicionesCorrectas =
        new List<int>();

    private Image[] circulosProgreso;

    private Sprite spriteCirculoGenerado;

    // =====================================================
    // PREGUNTAS
    // =====================================================

    private string[] preguntas =
    {
        "Descubre el patrón y completa lo que falta.",
        "Observa la repetición y completa la secuencia.",
        "Encuentra los números que faltan."
    };

    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        // Verificar referencias.

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
                    "Falta asignar el boton de opcion " + (i + 1)
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
    // PREPARAR POSICIONES DE RESPUESTAS
    // =====================================================

    void PrepararPosicionesCorrectas()
    {
        posicionesCorrectas.Clear();

        posicionesCorrectas.Add(0);
        posicionesCorrectas.Add(1);
        posicionesCorrectas.Add(2);

        // Mezclar posiciones.
        // Cada ejercicio tendra su respuesta correcta
        // en una posicion diferente.

        for (int i = posicionesCorrectas.Count - 1; i > 0; i--)
        {
            int posicionAleatoria =
                Random.Range(0, i + 1);

            int temporal =
                posicionesCorrectas[i];

            posicionesCorrectas[i] =
                posicionesCorrectas[posicionAleatoria];

            posicionesCorrectas[posicionAleatoria] =
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

        // ---------------------------------------------
        // MENSAJE
        // ---------------------------------------------

        txtResultado.text =
            "Selecciona la opción que completa la secuencia.";

        // ---------------------------------------------
        // PREGUNTA
        // ---------------------------------------------

        txtPregunta.text =
            preguntas[ejercicioActual];

        // ---------------------------------------------
        // PROGRESO EN UNA SOLA LINEA
        // ---------------------------------------------

        txtProgreso.text =
            "Ejercicio " +
            (ejercicioActual + 1) +
            " de " +
            totalEjercicios;

        txtProgreso.enableWordWrapping = false;

        txtProgreso.overflowMode =
            TextOverflowModes.Overflow;

        ActualizarIndicadorProgreso();

        // ---------------------------------------------
        // MOSTRAR SECUENCIA
        // ---------------------------------------------

        imagenSecuencia.gameObject.SetActive(true);

        // ---------------------------------------------
        // EJERCICIO 1 - COLORES
        // ---------------------------------------------

        if (ejercicioActual == 0)
        {
            CambiarImagen(
                imagenFondo,
                fondoColores,
                false
            );

            CambiarImagen(
                imagenTitulo,
                tituloColores,
                false
            );

            CambiarImagen(
                imagenSecuencia,
                secuenciaColores,
                false
            );
        }

        // ---------------------------------------------
        // EJERCICIO 2 - FLECHAS
        // ---------------------------------------------

        else if (ejercicioActual == 1)
        {
            CambiarImagen(
                imagenFondo,
                fondoFlechas,
                false
            );

            CambiarImagen(
                imagenTitulo,
                tituloFlechas,
                false
            );

            CambiarImagen(
                imagenSecuencia,
                secuenciaFlechas,
                false
            );
        }

        // ---------------------------------------------
        // EJERCICIO 3 - NUMEROS
        // ---------------------------------------------

        else if (ejercicioActual == 2)
        {
            CambiarImagen(
                imagenFondo,
                fondoNumeros,
                false
            );

            CambiarImagen(
                imagenTitulo,
                tituloNumeros,
                false
            );

            CambiarImagen(
                imagenSecuencia,
                secuenciaNumeros,
                false
            );
        }

        // ---------------------------------------------
        // CARGAR ALTERNATIVAS
        // ---------------------------------------------

        CargarOpciones();

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

        // EJERCICIO 1

        if (ejercicioActual == 0)
        {
            correcta = coloresCorrecta;
            incorrecta1 = coloresIncorrecta1;
            incorrecta2 = coloresIncorrecta2;
        }

        // EJERCICIO 2

        else if (ejercicioActual == 1)
        {
            correcta = flechasCorrecta;
            incorrecta1 = flechasIncorrecta1;
            incorrecta2 = flechasIncorrecta2;
        }

        // EJERCICIO 3

        else if (ejercicioActual == 2)
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
            {
                posicionesIncorrectas.Add(i);
            }
        }

        // Variar posiciones de las incorrectas.

        if (Random.value > 0.5f)
        {
            int temporal = posicionesIncorrectas[0];

            posicionesIncorrectas[0] =
                posicionesIncorrectas[1];

            posicionesIncorrectas[1] = temporal;
        }

        // Reiniciar botones.

        for (int i = 0; i < 3; i++)
        {
            botonesOpciones[i].gameObject.SetActive(true);

            botonesOpciones[i].interactable = true;

            botonesOpciones[i].image.color = colorNormal;
        }

        // RESPUESTA CORRECTA

        botonesOpciones[posicionCorrecta].image.sprite =
            correcta;

        botonesOpciones[posicionCorrecta].image.preserveAspect =
            false;

        // RESPUESTA INCORRECTA 1

        botonesOpciones[posicionesIncorrectas[0]].image.sprite =
            incorrecta1;

        botonesOpciones[posicionesIncorrectas[0]].image.preserveAspect =
            false;

        // RESPUESTA INCORRECTA 2

        botonesOpciones[posicionesIncorrectas[1]].image.sprite =
            incorrecta2;

        botonesOpciones[posicionesIncorrectas[1]].image.preserveAspect =
            false;
    }

    // =====================================================
    // SELECCIONAR OPCION
    // =====================================================

    void SeleccionarOpcion(int indice)
    {
        if (ejercicioCompletado)
            return;

        opcionSeleccionada = indice;

        // Restaurar colores.

        for (int i = 0; i < 3; i++)
        {
            botonesOpciones[i].image.color =
                colorNormal;
        }

        // Resaltar opcion seleccionada.

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

        // ---------------------------------------------
        // NINGUNA OPCION SELECCIONADA
        // ---------------------------------------------

        if (opcionSeleccionada == -1)
        {
            txtResultado.text =
                "¡Selecciona una opción primero!";

            return;
        }

        // ---------------------------------------------
        // RESPUESTA CORRECTA
        // ---------------------------------------------

        if (opcionSeleccionada == indiceCorrectoActual)
        {
            ejercicioCompletado = true;

            txtResultado.text =
                "¡Correcto! Completaste la secuencia.";

            btnComprobar.gameObject.SetActive(false);

            btnSiguiente.gameObject.SetActive(true);

            // Bloquear las tres opciones.

            foreach (Button boton in botonesOpciones)
            {
                boton.interactable = false;
            }
        }

        // ---------------------------------------------
        // RESPUESTA INCORRECTA
        // ---------------------------------------------

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
        txtPregunta.text =
            "¡MISIÓN COMPLETADA!";

        txtProgreso.text = "";

        txtResultado.text =
            "¡Excelente! Completaste correctamente las 3 secuencias.";

        // Ocultar secuencia principal.

        imagenSecuencia.gameObject.SetActive(false);

        // Ocultar opciones.

        foreach (Button boton in botonesOpciones)
        {
            boton.gameObject.SetActive(false);
        }

        // Ocultar botones.

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);

        // Ocultar los circulos de progreso.

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
    // CREAR LOS TRES CIRCULOS DE PROGRESO
    // =====================================================

    void CrearIndicadorProgreso()
    {
        // Buscar automaticamente Panel_Progreso
        // si no fue asignado desde el Inspector.

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

        // Evitar duplicar circulos si se vuelve
        // a ejecutar la inicializacion.

        circulosProgreso = new Image[totalEjercicios];

        spriteCirculoGenerado = CrearSpriteCircular();

        for (int i = 0; i < totalEjercicios; i++)
        {
            // ---------------------------------------------
            // CIRCULO EXTERIOR - BORDE
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

            rect.anchoredPosition = new Vector2(
                (i - 1) * separacionCirculos,
                posicionVerticalCirculos
            );

            Image imagenBorde =
                objetoCirculo.GetComponent<Image>();

            imagenBorde.sprite = spriteCirculoGenerado;
            imagenBorde.color = colorBorde;
            imagenBorde.raycastTarget = false;

            // ---------------------------------------------
            // CIRCULO INTERIOR - COLOR DE PROGRESO
            // ---------------------------------------------

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

            numero.alignment =
                TextAlignmentOptions.Center;

            numero.color = Color.white;

            numero.enableAutoSizing = false;

            numero.raycastTarget = false;
        }

        ActualizarIndicadorProgreso();
    }

    // =====================================================
    // ACTUALIZAR LOS COLORES DEL PROGRESO
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
    // OCULTAR PROGRESO AL FINALIZAR
    // =====================================================

    void OcultarIndicadorProgreso()
    {
        if (circulosProgreso == null)
            return;

        foreach (Image circulo in circulosProgreso)
        {
            if (circulo != null)
            {
                // Desactivar todo el circulo,
                // incluyendo borde y numero.

                circulo.transform.parent
                    .gameObject.SetActive(false);
            }
        }
    }
}
