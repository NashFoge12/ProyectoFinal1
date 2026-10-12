
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoOrdenarAlgoritmo : MonoBehaviour
{
    [System.Serializable]
    public class Tarjeta
    {
        public string id;
        public Sprite imagen;
    }

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

    public Sprite fondoDientes;
    public Sprite fondoMochila;
    public Sprite fondoPlanta;

    // =====================================================
    // TITULOS
    // =====================================================

    [Header("Titulos en imagen")]
    public Image imagenTitulo;

    public Sprite tituloDientes;
    public Sprite tituloMochila;
    public Sprite tituloPlanta;

    // =====================================================
    // TARJETAS
    // =====================================================

    [Header("Tarjetas")]
    public Button[] botonesPasos;

    // =====================================================
    // BOTONES
    // =====================================================

    [Header("Botones")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // =====================================================
    // SPRITES - DIENTES
    // =====================================================

    [Header("Sprites - Lavarse los dientes")]
    public Sprite sprTomarCepillo;
    public Sprite sprPonerPasta;
    public Sprite sprCepillarDientes;
    public Sprite sprEnjuagarBoca;
    public Sprite sprGuardarCepillo;

    // =====================================================
    // SPRITES - MOCHILA
    // =====================================================

    [Header("Sprites - Preparar mochila")]
    public Sprite sprRevisarHorario;
    public Sprite sprAbrirMochila;
    public Sprite sprColocarCuadernos;
    public Sprite sprColocarLapices;
    public Sprite sprCerrarMochila;

    // =====================================================
    // SPRITES - PLANTA
    // =====================================================

    [Header("Sprites - Regar planta")]
    public Sprite sprBuscarRegadera;
    public Sprite sprLlenarRegadera;
    public Sprite sprAcercarsePlanta;
    public Sprite sprRegarPlanta;
    public Sprite sprGuardarRegadera;

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

    [Tooltip("Posicion vertical de los circulos dentro del panel")]
    public float posicionVerticalCirculos = -20f;

    [Tooltip("Color de los ejercicios alcanzados")]
    public Color colorActivo =
        new Color(1f, 0.78f, 0.24f, 1f);

    [Tooltip("Color de los ejercicios pendientes")]
    public Color colorPendiente =
        new Color(0.78f, 0.76f, 0.72f, 1f);

    [Tooltip("Color del borde de los circulos")]
    public Color colorBorde =
        new Color(0.43f, 0.29f, 0.17f, 1f);

    private Image[] circulosProgreso;
    private Sprite spriteCirculoGenerado;

    // =====================================================
    // VARIABLES DEL JUEGO
    // =====================================================

    private int ejercicioActual = 0;
    private int tarjetaSeleccionada = -1;

    private List<Tarjeta[]> ejercicios =
        new List<Tarjeta[]>();

    private List<string[]> respuestasCorrectas =
        new List<string[]>();

    // =====================================================
    // START
    // =====================================================

    void Start()
    {
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
    // GENERAR SPRITE CIRCULAR POR CODIGO
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
    // CREAR LOS 3 CIRCULOS DENTRO DEL PANEL
    // =====================================================

    void CrearIndicadorProgreso()
    {
        if (txtProgreso == null)
        {
            Debug.LogError(
                "No se asigno Txt_Progreso."
            );
            return;
        }

        // Si no asignas el panel en el Inspector,
        // usamos automaticamente el padre de Txt_Progreso.
        if (panelProgreso == null)
        {
            panelProgreso =
                txtProgreso.transform.parent
                as RectTransform;
        }

        if (panelProgreso == null)
        {
            Debug.LogError(
                "No se encontro Panel_Progreso."
            );
            return;
        }

        spriteCirculoGenerado = CrearSpriteCircular();

        circulosProgreso = new Image[3];

        for (int i = 0; i < 3; i++)
        {
            // -----------------------------
            // OBJETO PRINCIPAL
            // -----------------------------

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

            float posicionX =
                (i - 1) * separacionCirculos;

            rect.anchoredPosition = new Vector2(
                posicionX,
                posicionVerticalCirculos
            );

            // -----------------------------
            // IMAGEN CIRCULAR
            // -----------------------------

            Image imagen =
                objetoCirculo.GetComponent<Image>();

            imagen.sprite = spriteCirculoGenerado;
            imagen.type = Image.Type.Simple;
            imagen.preserveAspect = true;
            imagen.raycastTarget = false;

            circulosProgreso[i] = imagen;

            // -----------------------------
            // BORDE INTERIOR
            // -----------------------------

            GameObject objetoBorde =
                new GameObject(
                    "Borde",
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(Image)
                );

            objetoBorde.transform.SetParent(
                objetoCirculo.transform,
                false
            );

            RectTransform rectBorde =
                objetoBorde.GetComponent<RectTransform>();

            rectBorde.anchorMin = Vector2.zero;
            rectBorde.anchorMax = Vector2.one;
            rectBorde.offsetMin = new Vector2(3f, 3f);
            rectBorde.offsetMax = new Vector2(-3f, -3f);

            Image imagenBorde =
                objetoBorde.GetComponent<Image>();

            imagenBorde.sprite = spriteCirculoGenerado;
            imagenBorde.color = colorBorde;
            imagenBorde.raycastTarget = false;

            // -----------------------------
            // NUMERO DEL EJERCICIO
            // -----------------------------

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
            numero.fontSize = tamanoCirculo * 0.5f;
            numero.fontStyle = FontStyles.Bold;
            numero.alignment =
                TextAlignmentOptions.Center;

            numero.color = Color.white;
            numero.raycastTarget = false;
            numero.enableAutoSizing = false;
        }

        ActualizarIndicadorProgreso();
    }

    // =====================================================
    // ACTUALIZAR COLORES DE LOS CIRCULOS
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
    // OCULTAR CIRCULOS AL TERMINAR
    // =====================================================

    void OcultarIndicadorProgreso()
    {
        if (circulosProgreso == null)
            return;

        foreach (Image circulo in circulosProgreso)
        {
            if (circulo != null)
            {
                circulo.gameObject.SetActive(false);
            }
        }
    }

    // =====================================================
    // CREAR EJERCICIOS
    // =====================================================

    void CrearEjercicios()
    {
        // EJERCICIO 1 - DIENTES

        ejercicios.Add(new Tarjeta[]
        {
            new Tarjeta {
                id = "enjuagar",
                imagen = sprEnjuagarBoca
            },
            new Tarjeta {
                id = "tomar",
                imagen = sprTomarCepillo
            },
            new Tarjeta {
                id = "guardar",
                imagen = sprGuardarCepillo
            },
            new Tarjeta {
                id = "pasta",
                imagen = sprPonerPasta
            },
            new Tarjeta {
                id = "cepillar",
                imagen = sprCepillarDientes
            }
        });

        respuestasCorrectas.Add(new string[]
        {
            "tomar",
            "pasta",
            "cepillar",
            "enjuagar",
            "guardar"
        });

        // EJERCICIO 2 - MOCHILA

        ejercicios.Add(new Tarjeta[]
        {
            new Tarjeta {
                id = "lapices",
                imagen = sprColocarLapices
            },
            new Tarjeta {
                id = "cerrar",
                imagen = sprCerrarMochila
            },
            new Tarjeta {
                id = "horario",
                imagen = sprRevisarHorario
            },
            new Tarjeta {
                id = "abrir",
                imagen = sprAbrirMochila
            },
            new Tarjeta {
                id = "cuadernos",
                imagen = sprColocarCuadernos
            }
        });

        respuestasCorrectas.Add(new string[]
        {
            "horario",
            "abrir",
            "cuadernos",
            "lapices",
            "cerrar"
        });

        // EJERCICIO 3 - PLANTA

        ejercicios.Add(new Tarjeta[]
        {
            new Tarjeta {
                id = "regar",
                imagen = sprRegarPlanta
            },
            new Tarjeta {
                id = "guardarRegadera",
                imagen = sprGuardarRegadera
            },
            new Tarjeta {
                id = "llenar",
                imagen = sprLlenarRegadera
            },
            new Tarjeta {
                id = "buscar",
                imagen = sprBuscarRegadera
            },
            new Tarjeta {
                id = "acercarse",
                imagen = sprAcercarsePlanta
            }
        });

        respuestasCorrectas.Add(new string[]
        {
            "buscar",
            "llenar",
            "acercarse",
            "regar",
            "guardarRegadera"
        });
    }

    // =====================================================
    // CARGAR EJERCICIO
    // =====================================================

    void CargarEjercicio()
    {
        tarjetaSeleccionada = -1;

        txtResultado.text =
            "Toca dos tarjetas para intercambiarlas.";

        btnComprobar.gameObject.SetActive(true);
        btnSiguiente.gameObject.SetActive(false);

        // Texto en una sola linea.
        txtProgreso.text =
            "Ejercicio " +
            (ejercicioActual + 1) +
            " de " +
            ejercicios.Count;

        txtProgreso.enableWordWrapping = false;
        txtProgreso.overflowMode =
            TextOverflowModes.Overflow;

        ActualizarIndicadorProgreso();

        // EJERCICIO 1

        if (ejercicioActual == 0)
        {
            txtPregunta.text =
                "Ordena los pasos para\nlavarse los dientes.";

            if (imagenFondo != null &&
                fondoDientes != null)
            {
                imagenFondo.sprite = fondoDientes;
            }

            if (imagenTitulo != null &&
                tituloDientes != null)
            {
                imagenTitulo.sprite = tituloDientes;
            }
        }

        // EJERCICIO 2

        else if (ejercicioActual == 1)
        {
            txtPregunta.text =
                "Ordena los pasos para\npreparar la mochila.";

            if (imagenFondo != null &&
                fondoMochila != null)
            {
                imagenFondo.sprite = fondoMochila;
            }

            if (imagenTitulo != null &&
                tituloMochila != null)
            {
                imagenTitulo.sprite = tituloMochila;
            }
        }

        // EJERCICIO 3

        else
        {
            txtPregunta.text =
                "Ordena los pasos para\nregar una planta.";

            if (imagenFondo != null &&
                fondoPlanta != null)
            {
                imagenFondo.sprite = fondoPlanta;
            }

            if (imagenTitulo != null &&
                tituloPlanta != null)
            {
                imagenTitulo.sprite = tituloPlanta;
            }
        }

        // CARGAR TARJETAS

        for (int i = 0; i < botonesPasos.Length; i++)
        {
            Tarjeta tarjeta =
                ejercicios[ejercicioActual][i];

            botonesPasos[i].image.sprite =
                tarjeta.imagen;

            botonesPasos[i].image.preserveAspect =
                false;

            botonesPasos[i].name =
                tarjeta.id;

            botonesPasos[i].gameObject.SetActive(true);
            botonesPasos[i].interactable = true;
        }
    }

    // =====================================================
    // SELECCIONAR TARJETA
    // =====================================================

    void SeleccionarTarjeta(int indice)
    {
        if (tarjetaSeleccionada == -1)
        {
            tarjetaSeleccionada = indice;

            txtResultado.text =
                "Ahora selecciona otra tarjeta para intercambiarlas.";

            return;
        }

        if (tarjetaSeleccionada == indice)
        {
            tarjetaSeleccionada = -1;

            txtResultado.text =
                "Toca dos tarjetas para intercambiarlas.";

            return;
        }

        IntercambiarTarjetas(
            tarjetaSeleccionada,
            indice
        );

        tarjetaSeleccionada = -1;

        txtResultado.text =
            "Puedes seguir ordenando o comprobar tu respuesta.";
    }

    // =====================================================
    // INTERCAMBIAR TARJETAS
    // =====================================================

    void IntercambiarTarjetas(
        int indice1,
        int indice2
    )
    {
        if (indice1 < 0 ||
            indice2 < 0 ||
            indice1 >= botonesPasos.Length ||
            indice2 >= botonesPasos.Length)
        {
            Debug.LogError(
                "Indice de tarjeta fuera de rango."
            );
            return;
        }

        Sprite spriteTemporal =
            botonesPasos[indice1].image.sprite;

        botonesPasos[indice1].image.sprite =
            botonesPasos[indice2].image.sprite;

        botonesPasos[indice2].image.sprite =
            spriteTemporal;

        string idTemporal =
            botonesPasos[indice1].name;

        botonesPasos[indice1].name =
            botonesPasos[indice2].name;

        botonesPasos[indice2].name =
            idTemporal;
    }

    // =====================================================
    // COMPROBAR
    // =====================================================

    void Comprobar()
    {
        bool correcto = true;

        for (int i = 0; i < botonesPasos.Length; i++)
        {
            string idActual =
                botonesPasos[i].name;

            if (idActual !=
                respuestasCorrectas[ejercicioActual][i])
            {
                correcto = false;
                break;
            }
        }

        if (correcto)
        {
            txtResultado.text =
                "¡Excelente! Ordenaste correctamente el algoritmo.";

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);

            foreach (Button boton in botonesPasos)
            {
                boton.interactable = false;
            }
        }
        else
        {
            txtResultado.text =
                "Aún no está correcto. ¡Inténtalo otra vez!";
        }
    }

    // =====================================================
    // SIGUIENTE EJERCICIO
    // =====================================================

    void SiguienteEjercicio()
    {
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

        txtProgreso.text = "";

        txtResultado.text =
            "¡Excelente! Completaste los 3 algoritmos.";

        OcultarIndicadorProgreso();

        foreach (Button boton in botonesPasos)
        {
            boton.gameObject.SetActive(false);
        }

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);
    }
}
