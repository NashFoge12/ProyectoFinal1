
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoDescubrirPatron : MonoBehaviour
{
    // =====================================================
    // TEXTOS
    // =====================================================

    [Header("TEXTOS")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;

    [Header("TIEMPO DE LA PREGUNTA")]
    [Range(1f, 5f)]
    public float tiempoPregunta = 2f;

    private Coroutine rutinaPregunta;

    // =====================================================
    // FONDOS
    // =====================================================

    [Header("FONDOS")]
    public Image imagenFondo;

    public Sprite fondoJardin;
    public Sprite fondoBosque;
    public Sprite fondoEspacio;

    // =====================================================
    // TITULOS
    // =====================================================

    [Header("TITULOS")]
    public Image imagenTitulo;

    public Sprite tituloJardin;
    public Sprite tituloBosque;
    public Sprite tituloEspacio;

    // =====================================================
    // INTERROGACION
    // =====================================================

    [Header("TARJETA CON INTERROGACION")]
    public Sprite spriteInterrogacion;

    // =====================================================
    // TARJETAS DE LA SECUENCIA
    // =====================================================

    [Header("TARJETAS DE SECUENCIA")]
    public Image[] imagenesSecuencia = new Image[6];

    // =====================================================
    // ALTERNATIVAS
    // =====================================================

    [Header("3 BOTONES DE RESPUESTA")]
    public Button[] botonesOpciones = new Button[3];

    // =====================================================
    // BOTONES
    // =====================================================

    [Header("BOTONES")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // =====================================================
    // JARDIN
    // =====================================================

    [Header("EJERCICIO 1 - JARDIN")]
    public Sprite florRosa;
    public Sprite florAmarilla;
    public Sprite florTulipan;

    // =====================================================
    // BOSQUE
    // =====================================================

    [Header("EJERCICIO 2 - BOSQUE")]
    public Sprite animalConejo;
    public Sprite animalOso;
    public Sprite animalZorro;

    // =====================================================
    // ESPACIO
    // =====================================================

    [Header("EJERCICIO 3 - ROBOTS")]
    public Sprite robotAzul;
    public Sprite robotNaranja;
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
    // COLORES
    // =====================================================

    [Header("COLORES DE RESPUESTA")]
    public Color colorNormal = Color.white;

    public Color colorSeleccionado =
        new Color(0.65f, 0.90f, 1f, 1f);

    public Color colorCorrecto =
        new Color(0.68f, 1f, 0.73f, 1f);

    // =====================================================
    // CONFIGURACION DE PATRONES
    // =====================================================

    [Header("VARIACION DE PATRONES")]
    [Tooltip("Evita repetir la misma combinacion al volver a entrar.")]
    public bool evitarRepeticion = true;

    [Tooltip("Mezcla las tres alternativas de respuesta.")]
    public bool mezclarAlternativas = true;

    // =====================================================
    // DATOS DEL EJERCICIO
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

    private int ejercicioActual = 0;
    private int opcionSeleccionada = -1;
    private int respuestaCorrecta = -1;

    private bool ejercicioCompletado = false;

    private Image[] circulosProgreso;
    private Sprite spriteCirculo;

    // Secuencia generada para el ejercicio actual.
    private Sprite[] secuenciaActual;
    private Sprite respuestaActual;

    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        if (txtPregunta == null ||
            txtProgreso == null ||
            btnComprobar == null ||
            btnSiguiente == null)
        {
            Debug.LogError(
                "Faltan textos o botones en GameManager."
            );

            enabled = false;
            return;
        }

        if (imagenesSecuencia == null ||
            imagenesSecuencia.Length != 6)
        {
            Debug.LogError(
                "Debes asignar 6 Images de secuencia."
            );

            enabled = false;
            return;
        }

        if (botonesOpciones == null ||
            botonesOpciones.Length != 3)
        {
            Debug.LogError(
                "Debes asignar 3 botones de respuesta."
            );

            enabled = false;
            return;
        }

        for (int i = 0; i < imagenesSecuencia.Length; i++)
        {
            if (imagenesSecuencia[i] == null)
            {
                Debug.LogError(
                    "Falta Imagen_Secuencia_" + (i + 1)
                );

                enabled = false;
                return;
            }
        }

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            if (botonesOpciones[i] == null)
            {
                Debug.LogError(
                    "Falta Boton_Opcion_" + (i + 1)
                );

                enabled = false;
                return;
            }

            int indice = i;

            botonesOpciones[i].onClick.AddListener(
                () => SeleccionarOpcion(indice)
            );
        }

        btnComprobar.onClick.AddListener(Comprobar);
        btnSiguiente.onClick.AddListener(SiguienteEjercicio);

        CrearEjercicios();
        CrearIndicadorProgreso();
        CargarEjercicio();
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
            "¡Observa el patrón y descubre qué figura sigue!"
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
    // CREAR LOS TRES TEMAS
    // =====================================================

    void CrearEjercicios()
    {
        ejercicios.Clear();

        ejercicios.Add(new Ejercicio
        {
            pregunta = "¿Qué flor sigue en el patrón?",
            fondo = fondoJardin,
            titulo = tituloJardin,

            elementos = new Sprite[]
            {
                florRosa,
                florAmarilla,
                florTulipan
            }
        });

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué animal continúa la secuencia?",
            fondo = fondoBosque,
            titulo = tituloBosque,

            elementos = new Sprite[]
            {
                animalConejo,
                animalOso,
                animalZorro
            }
        });

        ejercicios.Add(new Ejercicio
        {
            pregunta = "¡Descubre qué robot sigue!",
            fondo = fondoEspacio,
            titulo = tituloEspacio,

            elementos = new Sprite[]
            {
                robotAzul,
                robotNaranja,
                robotMorado
            }
        });
    }

    // =====================================================
    // GENERAR PATRON DIFERENTE
    // =====================================================

    void GenerarPatron(Ejercicio ejercicio)
    {
        // Cada tema tiene 3 elementos.
        // Se elige una permutacion y un tipo de patron.

        int tipoPatron = Random.Range(0, 3);
        int permutacion = Random.Range(0, 6);

        // Hay 18 combinaciones posibles por tema.
        int combinacion = tipoPatron * 6 + permutacion;

        string clave =
            "MundoPujllay_Patrones_UltimaCombinacion_" +
            ejercicioActual;

        if (evitarRepeticion)
        {
            int anterior = PlayerPrefs.GetInt(clave, -1);

            if (anterior >= 0 && anterior < 18)
            {
                // Elegir cualquiera de las otras 17 combinaciones.
                int nueva = Random.Range(0, 17);

                if (nueva >= anterior)
                {
                    nueva++;
                }

                combinacion = nueva;
            }
        }

        PlayerPrefs.SetInt(clave, combinacion);
        PlayerPrefs.Save();

        tipoPatron = combinacion / 6;
        permutacion = combinacion % 6;

        Sprite[] elementosMezclados =
            ObtenerPermutacion(
                ejercicio.elementos,
                permutacion
            );

        Sprite A = elementosMezclados[0];
        Sprite B = elementosMezclados[1];
        Sprite C = elementosMezclados[2];

        // =================================================
        // TIPO 0: ABAB
        //
        // A - B - A - B - ?
        // Respuesta: A
        // =================================================

        if (tipoPatron == 0)
        {
            secuenciaActual = new Sprite[]
            {
                A, B, A, B
            };

            respuestaActual = A;
        }

        // =================================================
        // TIPO 1: ABCAB
        //
        // A - B - C - A - B - ?
        // Respuesta: C
        // =================================================

        else if (tipoPatron == 1)
        {
            secuenciaActual = new Sprite[]
            {
                A, B, C, A, B
            };

            respuestaActual = C;
        }

        // =================================================
        // TIPO 2: AABA A
        //
        // A - A - B - A - A - ?
        // Respuesta: B
        // =================================================

        else
        {
            secuenciaActual = new Sprite[]
            {
                A, A, B, A, A
            };

            respuestaActual = B;
        }
    }

    // =====================================================
    // PERMUTAR LOS TRES ELEMENTOS
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
        opcionSeleccionada = -1;
        respuestaCorrecta = -1;

        Ejercicio ejercicio =
            ejercicios[ejercicioActual];

        // Crear nueva combinacion.
        GenerarPatron(ejercicio);

        IniciarPregunta(ejercicio.pregunta);

        txtProgreso.text =
            "Ejercicio " +
            (ejercicioActual + 1) +
            " de " +
            ejercicios.Count;

        txtProgreso.enableWordWrapping = false;

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

        // =================================================
        // MOSTRAR SECUENCIA GENERADA
        // =================================================

        for (int i = 0; i < imagenesSecuencia.Length; i++)
        {
            Image imagen = imagenesSecuencia[i];

            imagen.color = Color.white;
            imagen.preserveAspect = false;

            if (i < secuenciaActual.Length)
            {
                imagen.gameObject.SetActive(true);
                imagen.sprite = secuenciaActual[i];
            }
            else if (i == secuenciaActual.Length)
            {
                imagen.gameObject.SetActive(true);
                imagen.sprite = spriteInterrogacion;
            }
            else
            {
                imagen.gameObject.SetActive(false);
            }
        }

        CrearAlternativas(ejercicio);

        btnComprobar.gameObject.SetActive(true);
        btnComprobar.interactable = true;

        btnSiguiente.gameObject.SetActive(false);
    }

    // =====================================================
    // CREAR ALTERNATIVAS
    // =====================================================

    void CrearAlternativas(Ejercicio ejercicio)
    {
        // Los tres elementos del tema se muestran
        // como alternativas, pero mezclados.

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

        respuestaCorrecta = -1;

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            Button boton = botonesOpciones[i];

            boton.gameObject.SetActive(true);
            boton.interactable = true;

            boton.image.sprite = opciones[i];
            boton.image.color = colorNormal;
            boton.image.preserveAspect = false;

            if (opciones[i] == respuestaActual)
            {
                respuestaCorrecta = i;
            }
        }
    }

    // =====================================================
    // SELECCIONAR RESPUESTA
    // =====================================================

    void SeleccionarOpcion(int indice)
    {
        if (ejercicioCompletado)
            return;

        DetenerPreguntaTemporal();

        opcionSeleccionada = indice;

        foreach (Button boton in botonesOpciones)
        {
            boton.image.color = colorNormal;
        }

        botonesOpciones[indice].image.color =
            colorSeleccionado;

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

        if (opcionSeleccionada == -1)
        {
            MostrarMensaje(
                "¡Primero selecciona una tarjeta!"
            );

            return;
        }

        if (opcionSeleccionada == respuestaCorrecta)
        {
            ejercicioCompletado = true;

            // =============================================
            // GUARDAR ESTRELLA DE PATRONES - MISION 1
            // =============================================

            ProgresoEstrellas.GuardarEjercicio(
                "Patrones",
                1,
                ejercicioActual + 1
            );

            // Mostrar la respuesta en lugar de ?.

            imagenesSecuencia[secuenciaActual.Length].sprite =
                respuestaActual;

            botonesOpciones[opcionSeleccionada]
                .image.color = colorCorrecto;

            foreach (Button boton in botonesOpciones)
            {
                boton.interactable = false;
            }

            MostrarMensaje(
                "¡Excelente! ¡Descubriste el patrón!"
            );

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);
        }
        else
        {
            botonesOpciones[opcionSeleccionada]
                .image.color = colorNormal;

            opcionSeleccionada = -1;

            MostrarMensaje(
                "¡Casi! Observa la secuencia e inténtalo otra vez."
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
            "¡Felicidades! Descubriste los 3 patrones."
        );

        txtProgreso.text = "";

        foreach (Image imagen in imagenesSecuencia)
        {
            imagen.gameObject.SetActive(false);
        }

        foreach (Button boton in botonesOpciones)
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

    void OnDisable()
    {
        DetenerPreguntaTemporal();
    }
}
