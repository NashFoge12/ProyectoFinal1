
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoCompletarAlgoritmo : MonoBehaviour
{
    // =====================================================
    // TEXTOS DE INTERFAZ
    // =====================================================

    [Header("TEXTOS")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;
    public TMP_Text txtResultado;

    // =====================================================
    // FONDOS
    // =====================================================

    [Header("FONDOS DE LOS 3 TEMAS")]
    public Image imagenFondo;

    public Sprite fondoSandwich;
    public Sprite fondoManos;
    public Sprite fondoPlanta;

    // =====================================================
    // TITULOS
    // =====================================================

    [Header("TITULOS DE LOS 3 TEMAS")]
    public Image imagenTitulo;

    public Sprite tituloSandwich;
    public Sprite tituloManos;
    public Sprite tituloPlanta;

    // =====================================================
    // TARJETA CON SIGNO DE PREGUNTA
    // =====================================================

    [Header("TARJETAS DE INTERROGACION")]

    public Sprite interrogacionSandwich;
    public Sprite interrogacionManos;
    public Sprite interrogacionPlanta;

    // =====================================================
    // PANEL ALGORITMO
    // =====================================================

    [Header("PANEL ALGORITMO - 5 PASOS EN ORDEN")]
    public Image[] imagenesPasos = new Image[5];

    // =====================================================
    // PANEL ALTERNATIVAS
    // =====================================================

    [Header("PANEL ALTERNATIVAS - 3 BOTONES")]
    public Button[] botonesOpciones = new Button[3];

    // =====================================================
    // BOTONES
    // =====================================================

    [Header("BOTONES")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // =====================================================
    // TEMA 1 - PREPARAR UN SANDWICH
    // =====================================================

    [Header("TEMA 1 - SANDWICH - PASOS CORRECTOS")]

    public Sprite sprSacarElPan;
    public Sprite sprUntarLaMantequilla;
    public Sprite sprColocarLasVerduras;
    public Sprite sprColocarElJamon;
    public Sprite sprCerrarElSandwich;

    [Header("TEMA 1 - RESPUESTAS INCORRECTAS")]

    public Sprite sprPintarElPan;
    public Sprite sprGolpearElPan;

    // =====================================================
    // TEMA 2 - LAVARSE LAS MANOS
    // =====================================================

    [Header("TEMA 2 - MANOS - PASOS CORRECTOS")]

    public Sprite sprAbrirElGrifo;
    public Sprite sprColocarJabonEnLasManos;
    public Sprite sprFrotarLasManos;
    public Sprite sprEnjuagarLasManos;
    public Sprite sprSecarLasManos;

    [Header("TEMA 2 - RESPUESTAS INCORRECTAS")]

    public Sprite sprCepillarseLosDientes;
    public Sprite sprGuardarJuguetes;

    // =====================================================
    // TEMA 3 - SEMBRAR UNA PLANTA
    // =====================================================

    [Header("TEMA 3 - PLANTA - PASOS CORRECTOS")]

    public Sprite sprConseguirLaMaceta;
    public Sprite sprColocarLaTierra;
    public Sprite sprHacerUnHueco;
    public Sprite sprPonerLaSemillaYCubrirla;
    public Sprite sprRegarConAgua;

    [Header("TEMA 3 - RESPUESTAS INCORRECTAS")]

    public Sprite sprRegarConJugo;
    public Sprite sprMeterAlCongelador;

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
    // COLORES DE SELECCION
    // =====================================================

    [Header("COLORES")]

    public Color colorNormal = Color.white;

    public Color colorSeleccionado =
        new Color(0.65f, 0.90f, 1f, 1f);

    // =====================================================
    // DATOS DEL EJERCICIO
    // =====================================================

    private class Ejercicio
    {
        public string nombre;
        public string pregunta;

        public Sprite fondo;
        public Sprite titulo;
        public Sprite interrogacion;

        public Sprite[] pasos;
        public Sprite[] incorrectas;
    }

    private List<Ejercicio> ejercicios =
        new List<Ejercicio>();

    private int ejercicioActual = 0;

    private int pasoFaltante = -1;
    private int respuestaCorrecta = -1;
    private int opcionSeleccionada = -1;

    private bool ejercicioCompletado = false;

    private Image[] circulosProgreso;
    private Sprite spriteCirculo;

    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        // Comprobar referencias importantes.

        if (imagenesPasos == null ||
            imagenesPasos.Length != 5 ||
            botonesOpciones == null ||
            botonesOpciones.Length != 3)
        {
            Debug.LogError(
                "Debes configurar 5 pasos y 3 alternativas."
            );

            enabled = false;
            return;
        }

        if (txtPregunta == null ||
            txtProgreso == null ||
            txtResultado == null ||
            btnComprobar == null ||
            btnSiguiente == null)
        {
            Debug.LogError(
                "Faltan textos o botones del juego."
            );

            enabled = false;
            return;
        }

        for (int i = 0; i < imagenesPasos.Length; i++)
        {
            if (imagenesPasos[i] == null)
            {
                Debug.LogError(
                    "Falta asignar Paso_" + (i + 1)
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
                    "Falta asignar Boton_Opcion_" + (i + 1)
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
    // CREAR LOS TRES TEMAS
    // =====================================================

    void CrearEjercicios()
    {
        ejercicios.Clear();

        // -------------------------------------------------
        // TEMA 1: PREPARAR UN SANDWICH
        // -------------------------------------------------

        ejercicios.Add(new Ejercicio
        {
            nombre = "Sandwich",

            pregunta =
                "¿Qué paso falta para preparar un sándwich?",

            fondo = fondoSandwich,
            titulo = tituloSandwich,
            interrogacion = interrogacionSandwich,

            // ORDEN CORRECTO - NUNCA SE DESORDENA

            pasos = new Sprite[]
            {
                sprSacarElPan,             // PASO 1
                sprUntarLaMantequilla,     // PASO 2
                sprColocarLasVerduras,     // PASO 3
                sprColocarElJamon,         // PASO 4
                sprCerrarElSandwich        // PASO 5
            },

            incorrectas = new Sprite[]
            {
                sprPintarElPan,
                sprGolpearElPan
            }
        });

        // -------------------------------------------------
        // TEMA 2: LAVARSE LAS MANOS
        // -------------------------------------------------

        ejercicios.Add(new Ejercicio
        {
            nombre = "Manos",

            pregunta =
                "¿Qué paso falta para lavarse las manos?",

            fondo = fondoManos,
            titulo = tituloManos,
            interrogacion = interrogacionManos,

            // ORDEN CORRECTO - NUNCA SE DESORDENA

            pasos = new Sprite[]
            {
                sprAbrirElGrifo,             // PASO 1
                sprColocarJabonEnLasManos,   // PASO 2
                sprFrotarLasManos,           // PASO 3
                sprEnjuagarLasManos,         // PASO 4
                sprSecarLasManos             // PASO 5
            },

            incorrectas = new Sprite[]
            {
                sprCepillarseLosDientes,
                sprGuardarJuguetes
            }
        });

        // -------------------------------------------------
        // TEMA 3: SEMBRAR UNA PLANTA
        // -------------------------------------------------

        ejercicios.Add(new Ejercicio
        {
            nombre = "Planta",

            pregunta =
                "¿Qué paso falta para sembrar una planta?",

            fondo = fondoPlanta,
            titulo = tituloPlanta,
            interrogacion = interrogacionPlanta,

            // ORDEN CORRECTO - NUNCA SE DESORDENA

            pasos = new Sprite[]
            {
                sprConseguirLaMaceta,         // PASO 1
                sprColocarLaTierra,           // PASO 2
                sprHacerUnHueco,              // PASO 3
                sprPonerLaSemillaYCubrirla,   // PASO 4
                sprRegarConAgua               // PASO 5
            },

            incorrectas = new Sprite[]
            {
                sprRegarConJugo,
                sprMeterAlCongelador
            }
        });
    }

    // =====================================================
    // ELEGIR UN PASO FALTANTE ALEATORIO
    // =====================================================

    int ElegirPasoAleatorio()
    {
        // Se guarda el ultimo paso faltante de cada
        // tema para evitar repetirlo en la siguiente
        // partida, incluso si se cierra el juego.

        string clave =
            "MundoPujllay_UltimoPaso_" + ejercicioActual;

        int anterior = PlayerPrefs.GetInt(clave, -1);

        int nuevo = Random.Range(0, 5);

        // No repetir el mismo paso inmediatamente.

        if (anterior >= 0 && anterior < 5)
        {
            nuevo = Random.Range(0, 4);

            if (nuevo >= anterior)
            {
                nuevo++;
            }
        }

        PlayerPrefs.SetInt(clave, nuevo);
        PlayerPrefs.Save();

        return nuevo;
    }

    // =====================================================
    // CARGAR EJERCICIO
    // =====================================================

    void CargarEjercicio()
    {
        ejercicioCompletado = false;
        opcionSeleccionada = -1;

        Ejercicio ejercicio = ejercicios[ejercicioActual];

        // Elegir un paso diferente aleatoriamente.

        pasoFaltante = ElegirPasoAleatorio();

        // -------------------------------------------------
        // PREGUNTA
        // -------------------------------------------------

        txtPregunta.text = ejercicio.pregunta;

        // -------------------------------------------------
        // PANEL MENSAJE
        // -------------------------------------------------

        txtResultado.text =
            "¡Observa los pasos y descubre cuál falta!";

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
        // FONDO Y TITULO
        // -------------------------------------------------

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

        // -------------------------------------------------
        // MOSTRAR LOS 5 PASOS EN ORDEN
        // -------------------------------------------------

        for (int i = 0; i < 5; i++)
        {
            Image imagen = imagenesPasos[i];

            imagen.gameObject.SetActive(true);
            imagen.color = Color.white;
            imagen.preserveAspect = false;

            // Mostrar la tarjeta del paso correspondiente.

            imagen.sprite = ejercicio.pasos[i];

            // Solo reemplazar el paso elegido por "?".

            if (i == pasoFaltante)
            {
                imagen.sprite = ejercicio.interrogacion;
            }
        }

        // -------------------------------------------------
        // PREPARAR LAS 3 ALTERNATIVAS
        // -------------------------------------------------

        CrearAlternativas(ejercicio);

        btnComprobar.gameObject.SetActive(true);
        btnComprobar.interactable = true;

        btnSiguiente.gameObject.SetActive(false);
    }

    // =====================================================
    // CREAR ALTERNATIVAS ALEATORIAS
    // =====================================================

    void CrearAlternativas(Ejercicio ejercicio)
    {
        // La alternativa correcta siempre es
        // el paso que ha desaparecido.

        Sprite correcta = ejercicio.pasos[pasoFaltante];

        // Dos opciones incorrectas de ese tema.

        Sprite incorrecta1 = ejercicio.incorrectas[0];
        Sprite incorrecta2 = ejercicio.incorrectas[1];

        Sprite[] opciones = new Sprite[]
        {
            correcta,
            incorrecta1,
            incorrecta2
        };

        // Mezclar las tres alternativas.

        for (int i = opciones.Length - 1; i > 0; i--)
        {
            int aleatorio = Random.Range(0, i + 1);

            Sprite temporal = opciones[i];

            opciones[i] = opciones[aleatorio];
            opciones[aleatorio] = temporal;
        }

        // Mostrar las alternativas y localizar
        // la posicion de la respuesta correcta.

        respuestaCorrecta = -1;

        for (int i = 0; i < 3; i++)
        {
            Button boton = botonesOpciones[i];

            boton.gameObject.SetActive(true);
            boton.interactable = true;

            boton.image.sprite = opciones[i];
            boton.image.color = colorNormal;
            boton.image.preserveAspect = false;

            if (opciones[i] == correcta)
            {
                respuestaCorrecta = i;
            }
        }
    }

    // =====================================================
    // SELECCIONAR UNA TARJETA
    // =====================================================

    void SeleccionarOpcion(int indice)
    {
        if (ejercicioCompletado)
            return;

        opcionSeleccionada = indice;

        // Restaurar colores.

        foreach (Button boton in botonesOpciones)
        {
            boton.image.color = colorNormal;
        }

        // Marcar la tarjeta seleccionada.

        botonesOpciones[indice].image.color =
            colorSeleccionado;

        txtResultado.text =
            "¡Muy bien! Ahora presiona COMPROBAR.";
    }

    // =====================================================
    // COMPROBAR
    // =====================================================

    void Comprobar()
    {
        if (ejercicioCompletado)
            return;

        if (opcionSeleccionada == -1)
        {
            txtResultado.text =
                "¡Primero selecciona una tarjeta!";

            return;
        }

        Ejercicio ejercicio = ejercicios[ejercicioActual];

        // -------------------------------------------------
        // RESPUESTA CORRECTA
        // -------------------------------------------------

        if (opcionSeleccionada == respuestaCorrecta)
        {
            ejercicioCompletado = true;

            txtResultado.text =
                "¡Excelente! ¡Completaste el algoritmo!";

            // Reemplazar la interrogacion
            // con el paso correcto.

            imagenesPasos[pasoFaltante].sprite =
                ejercicio.pasos[pasoFaltante];

            imagenesPasos[pasoFaltante].color =
                Color.white;

            // Mantener las tarjetas color crema/blanco;
            // solo la seleccion usa el tinte azul.

            foreach (Button boton in botonesOpciones)
            {
                boton.image.color = colorNormal;
                boton.interactable = false;
            }

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);
        }

        // -------------------------------------------------
        // RESPUESTA INCORRECTA
        // -------------------------------------------------

        else
        {
            txtResultado.text =
                "¡Casi! Observa el orden e inténtalo otra vez.";

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
            "¡Felicidades! Completaste los 3 algoritmos.";

        txtProgreso.text = "";

        foreach (Image imagen in imagenesPasos)
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
            return;

        spriteCirculo = CrearSpriteCircular();

        circulosProgreso = new Image[3];

        for (int i = 0; i < 3; i++)
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
}
