
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoCompletarAlgoritmo : MonoBehaviour
{
    [Header("TEXTOS")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;

    [Header("TIEMPO DE LA PREGUNTA")]
    [Range(1f, 5f)]
    public float tiempoPregunta = 2f;

    private Coroutine rutinaPregunta;

    [Header("FONDOS DE LOS 3 TEMAS")]
    public Image imagenFondo;
    public Sprite fondoSandwich;
    public Sprite fondoManos;
    public Sprite fondoPlanta;

    [Header("TITULOS DE LOS 3 TEMAS")]
    public Image imagenTitulo;
    public Sprite tituloSandwich;
    public Sprite tituloManos;
    public Sprite tituloPlanta;

    [Header("TARJETAS DE INTERROGACION")]
    public Sprite interrogacionSandwich;
    public Sprite interrogacionManos;
    public Sprite interrogacionPlanta;

    [Header("PANEL ALGORITMO - 5 PASOS EN ORDEN")]
    public Image[] imagenesPasos = new Image[5];

    [Header("PANEL ALTERNATIVAS - 3 BOTONES")]
    public Button[] botonesOpciones = new Button[3];

    [Header("BOTONES")]
    public Button btnComprobar;
    public Button btnSiguiente;

    [Header("TEMA 1 - SANDWICH - PASOS CORRECTOS")]
    public Sprite sprSacarElPan;
    public Sprite sprUntarLaMantequilla;
    public Sprite sprColocarLasVerduras;
    public Sprite sprColocarElJamon;
    public Sprite sprCerrarElSandwich;

    [Header("TEMA 1 - RESPUESTAS INCORRECTAS")]
    public Sprite sprPintarElPan;
    public Sprite sprGolpearElPan;

    [Header("TEMA 2 - MANOS - PASOS CORRECTOS")]
    public Sprite sprAbrirElGrifo;
    public Sprite sprColocarJabonEnLasManos;
    public Sprite sprFrotarLasManos;
    public Sprite sprEnjuagarLasManos;
    public Sprite sprSecarLasManos;

    [Header("TEMA 2 - RESPUESTAS INCORRECTAS")]
    public Sprite sprCepillarseLosDientes;
    public Sprite sprGuardarJuguetes;

    [Header("TEMA 3 - PLANTA - PASOS CORRECTOS")]
    public Sprite sprConseguirLaMaceta;
    public Sprite sprColocarLaTierra;
    public Sprite sprHacerUnHueco;
    public Sprite sprPonerLaSemillaYCubrirla;
    public Sprite sprRegarConAgua;

    [Header("TEMA 3 - RESPUESTAS INCORRECTAS")]
    public Sprite sprRegarConJugo;
    public Sprite sprMeterAlCongelador;

    [Header("INDICADOR DE PROGRESO")]
    public RectTransform panelProgreso;
    public float tamanoCirculo = 28f;
    public float separacionCirculos = 38f;
    public float posicionVerticalCirculos = -20f;

    public Color colorActivo =
        new Color(1f, 0.78f, 0.24f, 1f);

    public Color colorPendiente =
        new Color(0.78f, 0.76f, 0.72f, 1f);

    [Header("COLORES")]
    public Color colorNormal = Color.white;

    public Color colorSeleccionado =
        new Color(0.65f, 0.90f, 1f, 1f);

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
    // INICIO
    // =====================================================

    void Start()
    {
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
    // MENSAJES
    // =====================================================

    void MostrarMensaje(string mensaje)
    {
        if (txtPregunta != null)
            txtPregunta.text = mensaje;
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
            "¡Observa los pasos y descubre cuál falta!"
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

        // EJERCICIO 1: SANDWICH

        ejercicios.Add(new Ejercicio
        {
            nombre = "Sandwich",
            pregunta =
                "¿Qué paso falta para preparar un sándwich?",
            fondo = fondoSandwich,
            titulo = tituloSandwich,
            interrogacion = interrogacionSandwich,

            pasos = new Sprite[]
            {
                sprSacarElPan,
                sprUntarLaMantequilla,
                sprColocarLasVerduras,
                sprColocarElJamon,
                sprCerrarElSandwich
            },

            incorrectas = new Sprite[]
            {
                sprPintarElPan,
                sprGolpearElPan
            }
        });

        // EJERCICIO 2: LAVARSE LAS MANOS

        ejercicios.Add(new Ejercicio
        {
            nombre = "Manos",
            pregunta =
                "¿Qué paso falta para lavarse las manos?",
            fondo = fondoManos,
            titulo = tituloManos,
            interrogacion = interrogacionManos,

            pasos = new Sprite[]
            {
                sprAbrirElGrifo,
                sprColocarJabonEnLasManos,
                sprFrotarLasManos,
                sprEnjuagarLasManos,
                sprSecarLasManos
            },

            incorrectas = new Sprite[]
            {
                sprCepillarseLosDientes,
                sprGuardarJuguetes
            }
        });

        // EJERCICIO 3: SEMBRAR UNA PLANTA

        ejercicios.Add(new Ejercicio
        {
            nombre = "Planta",
            pregunta =
                "¿Qué paso falta para sembrar una planta?",
            fondo = fondoPlanta,
            titulo = tituloPlanta,
            interrogacion = interrogacionPlanta,

            pasos = new Sprite[]
            {
                sprConseguirLaMaceta,
                sprColocarLaTierra,
                sprHacerUnHueco,
                sprPonerLaSemillaYCubrirla,
                sprRegarConAgua
            },

            incorrectas = new Sprite[]
            {
                sprRegarConJugo,
                sprMeterAlCongelador
            }
        });
    }

    // =====================================================
    // PASO FALTANTE ALEATORIO
    // =====================================================

    int ElegirPasoAleatorio()
    {
        string clave =
            "MundoPujllay_UltimoPaso_" + ejercicioActual;

        int anterior = PlayerPrefs.GetInt(clave, -1);
        int nuevo = Random.Range(0, 5);

        if (anterior >= 0 && anterior < 5)
        {
            nuevo = Random.Range(0, 4);

            if (nuevo >= anterior)
                nuevo++;
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
        DetenerPreguntaTemporal();

        ejercicioCompletado = false;
        opcionSeleccionada = -1;

        Ejercicio ejercicio = ejercicios[ejercicioActual];

        pasoFaltante = ElegirPasoAleatorio();

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

        // Mostrar 5 pasos ordenados.

        for (int i = 0; i < 5; i++)
        {
            Image imagen = imagenesPasos[i];

            imagen.gameObject.SetActive(true);
            imagen.color = Color.white;
            imagen.preserveAspect = false;
            imagen.sprite = ejercicio.pasos[i];

            if (i == pasoFaltante)
            {
                imagen.sprite = ejercicio.interrogacion;
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
        Sprite correcta =
            ejercicio.pasos[pasoFaltante];

        Sprite[] opciones = new Sprite[]
        {
            correcta,
            ejercicio.incorrectas[0],
            ejercicio.incorrectas[1]
        };

        // Mezclar las respuestas.

        for (int i = opciones.Length - 1; i > 0; i--)
        {
            int aleatorio = Random.Range(0, i + 1);

            Sprite temporal = opciones[i];
            opciones[i] = opciones[aleatorio];
            opciones[aleatorio] = temporal;
        }

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
    // SELECCIONAR OPCION
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

        Ejercicio ejercicio = ejercicios[ejercicioActual];

        if (opcionSeleccionada == respuestaCorrecta)
        {
            ejercicioCompletado = true;

            // =============================================
            // NUEVO: GUARDAR ESTRELLA DE LA MISION 2
            // =============================================

            ProgresoEstrellas.GuardarEjercicio(
                "Algoritmos",
                2,
                ejercicioActual + 1
            );

            Debug.Log(
                "Mision 2 - Estrella guardada: " +
                (ejercicioActual + 1)
            );

            // =============================================
            // RESPUESTA CORRECTA
            // =============================================

            MostrarMensaje(
                "¡Excelente! ¡Completaste el algoritmo!"
            );

            imagenesPasos[pasoFaltante].sprite =
                ejercicio.pasos[pasoFaltante];

            imagenesPasos[pasoFaltante].color =
                Color.white;

            foreach (Button boton in botonesOpciones)
            {
                boton.image.color = colorNormal;
                boton.interactable = false;
            }

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);
        }
        else
        {
            MostrarMensaje(
                "¡Casi! Observa el orden e inténtalo otra vez."
            );

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
        DetenerPreguntaTemporal();

        MostrarMensaje(
            "¡MISIÓN COMPLETADA!\n" +
            "¡Felicidades! Completaste los 3 algoritmos."
        );

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
