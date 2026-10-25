
using System.Collections;
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

    [Header("TIEMPO DE LA PREGUNTA")]
    [Range(1f, 5f)]
    public float tiempoPregunta = 2f;

    private Coroutine rutinaPregunta;

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

    // Fondo del recuadro donde aparece Txt_Pregunta.
    // Es independiente del fondo general de la escena.
    [Header("FONDOS DE PREGUNTA - POR EJERCICIO")]
    public Image imagenFondoPregunta;
    public Sprite fondoPreguntaLeche;
    public Sprite fondoPreguntaLluvia;
    public Sprite fondoPreguntaColegio;

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

    // =====================================================
    // EJERCICIO 1 - PREPARAR LECHE
    // =====================================================

    [Header("EJERCICIO 1 - PREPARAR LA LECHE")]
    public Sprite sprServirLaLeche;
    public Sprite sprAgregarCocoaAzucar;
    public Sprite sprMezclar;
    public Sprite sprTomarLaLeche;

    // =====================================================
    // EJERCICIO 2 - CUANDO LLUEVE
    // =====================================================

    [Header("EJERCICIO 2 - CUANDO LLUEVE")]
    public Sprite sprMirarPorLaVentana;
    public Sprite sprPonerseLaCasaca;
    public Sprite sprAbrirElParaguas;
    public Sprite sprCaminarSobreLosCharcos;

    // =====================================================
    // EJERCICIO 3 - PREPARAR MOCHILA
    // =====================================================

    [Header("EJERCICIO 3 - PREPARAR LA MOCHILA")]
    public Sprite sprRevisarLaLista;
    public Sprite sprGuardarLosUtiles;
    public Sprite sprCerrarLaMochila;
    public Sprite sprIrAlColegio;

    // =====================================================
    // COLORES DE TARJETAS
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
        public Sprite[] pasos;
    }

    private List<Ejercicio> ejercicios =
        new List<Ejercicio>();

    // =====================================================
    // VARIABLES DEL JUEGO
    // =====================================================

    private int ejercicioActual = 0;
    private int tarjetaSeleccionada = -1;
    private int posicionError = -1;

    private bool ejercicioCompletado = false;

    private int[] ordenTarjetas = new int[4];

    [Header("NUMEROS FIJOS DE LAS 4 TARJETAS")]
    [Tooltip("Tamano de los circulitos numerados en las tarjetas")]
    public float tamanoNumeroTarjeta = 32f;

    [Tooltip("Separacion respecto a la esquina superior izquierda")]
    public Vector2 margenNumeroTarjeta = new Vector2(5f, -5f);

    [Tooltip("Color de los circulitos; no indica respuestas correctas")]
    public Color colorNumeroTarjeta = new Color(1f, 0.78f, 0.24f, 1f);

    private Image[] circulosProgreso;
    private Sprite spriteCirculo;

    // =====================================================
    // INICIO
    // =====================================================

    void Start()
    {
        if (!ValidarReferencias())
        {
            enabled = false;
            return;
        }

        CrearEjercicios();

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

        btnSiguiente.onClick.AddListener(
            SiguienteEjercicio
        );

        CrearIndicadorProgreso();
        CrearNumerosFijosTarjetas();
        CargarEjercicio();
    }

    // =====================================================
    // NUMEROS FIJOS: POSICIONES 1, 2, 3 Y 4
    // No se mezclan con las imagenes ni revelan errores.
    // =====================================================

    void CrearNumerosFijosTarjetas()
    {
        if (spriteCirculo == null)
            spriteCirculo = CrearSpriteCircular();

        for (int i = 0; i < botonesPasos.Length; i++)
        {
            Button boton = botonesPasos[i];
            if (boton == null) continue;

            Transform anterior = boton.transform.Find("NumeroFijo_Posicion");
            GameObject circulo;

            if (anterior != null)
            {
                circulo = anterior.gameObject;
            }
            else
            {
                circulo = new GameObject(
                    "NumeroFijo_Posicion",
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(Image)
                );
                circulo.transform.SetParent(boton.transform, false);
            }

            circulo.transform.SetAsLastSibling();
            RectTransform rect = circulo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.sizeDelta = Vector2.one * tamanoNumeroTarjeta;
            rect.anchoredPosition = margenNumeroTarjeta;

            Image imagen = circulo.GetComponent<Image>();
            imagen.sprite = spriteCirculo;
            imagen.color = colorBorde;
            imagen.raycastTarget = false;

            Transform interiorAnterior = circulo.transform.Find("InteriorNumero");
            GameObject interior;
            if (interiorAnterior != null)
                interior = interiorAnterior.gameObject;
            else
            {
                interior = new GameObject("InteriorNumero",
                    typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                interior.transform.SetParent(circulo.transform, false);
            }

            RectTransform rectInterior = interior.GetComponent<RectTransform>();
            rectInterior.anchorMin = Vector2.zero;
            rectInterior.anchorMax = Vector2.one;
            rectInterior.offsetMin = new Vector2(2f, 2f);
            rectInterior.offsetMax = new Vector2(-2f, -2f);

            Image imagenInterior = interior.GetComponent<Image>();
            imagenInterior.sprite = spriteCirculo;
            imagenInterior.color = colorNumeroTarjeta;
            imagenInterior.raycastTarget = false;

            Transform textoAnterior = circulo.transform.Find("Numero");
            GameObject texto;
            if (textoAnterior != null)
                texto = textoAnterior.gameObject;
            else
            {
                texto = new GameObject("Numero",
                    typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                texto.transform.SetParent(circulo.transform, false);
            }

            RectTransform rectTexto = texto.GetComponent<RectTransform>();
            rectTexto.anchorMin = Vector2.zero;
            rectTexto.anchorMax = Vector2.one;
            rectTexto.offsetMin = Vector2.zero;
            rectTexto.offsetMax = Vector2.zero;

            TextMeshProUGUI numero = texto.GetComponent<TextMeshProUGUI>();
            numero.text = (i + 1).ToString();
            numero.fontSize = tamanoNumeroTarjeta * 0.53f;
            numero.fontStyle = FontStyles.Bold;
            numero.alignment = TextAlignmentOptions.Center;
            numero.color = Color.white;
            numero.raycastTarget = false;
            numero.enableAutoSizing = false;
        }
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
    // MOSTRAR MENSAJE
    // =====================================================

    void MostrarMensaje(string mensaje)
    {
        if (txtPregunta == null)
            return;

        txtPregunta.text = mensaje;
    }

    // =====================================================
    // PREGUNTA TEMPORAL
    // =====================================================

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

        yield return new WaitForSeconds(
            tiempoPregunta
        );

        MostrarMensaje(
            "¡Observa la secuencia y encuentra " +
            "la tarjeta fuera de orden!"
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
    // CREAR LOS TRES EJERCICIOS
    // =====================================================

    void CrearEjercicios()
    {
        ejercicios.Clear();

        // EJERCICIO 1 - LECHE

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué tarjeta está fuera de orden " +
                "al preparar la leche?",

            fondo = fondoLeche,
            titulo = tituloLeche,

            pasos = new Sprite[]
            {
                sprServirLaLeche,
                sprAgregarCocoaAzucar,
                sprMezclar,
                sprTomarLaLeche
            }
        });

        // EJERCICIO 2 - LLUVIA

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué tarjeta está fuera de orden " +
                "al salir cuando llueve?",

            fondo = fondoLluvia,
            titulo = tituloLluvia,

            pasos = new Sprite[]
            {
                sprMirarPorLaVentana,
                sprPonerseLaCasaca,
                sprAbrirElParaguas,
                sprCaminarSobreLosCharcos
            }
        });

        // EJERCICIO 3 - COLEGIO

        ejercicios.Add(new Ejercicio
        {
            pregunta =
                "¿Qué tarjeta está fuera de orden " +
                "al preparar la mochila?",

            fondo = fondoColegio,
            titulo = tituloColegio,

            pasos = new Sprite[]
            {
                sprRevisarLaLista,
                sprGuardarLosUtiles,
                sprCerrarLaMochila,
                sprIrAlColegio
            }
        });
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

        IniciarPregunta(ejercicio.pregunta);

        txtProgreso.text =
            "Ejercicio " +
            (ejercicioActual + 1) +
            " de " +
            ejercicios.Count;

        txtProgreso.textWrappingMode = TextWrappingModes.NoWrap;

        txtProgreso.overflowMode =
            TextOverflowModes.Overflow;

        ActualizarIndicadorProgreso();

        CambiarImagen(
            imagenFondo,
            ejercicio.fondo
        );

        CambiarImagen(
            imagenTitulo,
            ejercicio.titulo
        );

        // Cambiar solo el fondo del panel de pregunta.
        // El texto Txt_Pregunta sigue funcionando como antes.
        Sprite fondoPreguntaActual = null;
        if (ejercicioActual == 0)
            fondoPreguntaActual = fondoPreguntaLeche;
        else if (ejercicioActual == 1)
            fondoPreguntaActual = fondoPreguntaLluvia;
        else
            fondoPreguntaActual = fondoPreguntaColegio;

        // Buscar el Image del Panel_Pregunta si no fue asignado.
        if (imagenFondoPregunta == null && txtPregunta != null)
        {
            Transform padre = txtPregunta.transform.parent;
            while (padre != null)
            {
                if (padre.name == "Panel_Pregunta")
                {
                    imagenFondoPregunta = padre.GetComponent<Image>();
                    break;
                }
                padre = padre.parent;
            }
        }

        if (imagenFondoPregunta == null)
        {
            Debug.LogWarning("FONDO PREGUNTA: Asigna el componente Image de Panel_Pregunta en GameManager.");
        }
        else if (fondoPreguntaActual == null)
        {
            Debug.LogWarning("FONDO PREGUNTA: Falta asignar el sprite del ejercicio " + (ejercicioActual + 1) + " en GameManager.");
        }
        else
        {
            imagenFondoPregunta.sprite = fondoPreguntaActual;
            imagenFondoPregunta.type = Image.Type.Simple;
            imagenFondoPregunta.preserveAspect = false;
            imagenFondoPregunta.enabled = true;
            Debug.Log("FONDO PREGUNTA: Ejercicio " + (ejercicioActual + 1) +
                      " -> " + fondoPreguntaActual.name +
                      " en " + imagenFondoPregunta.gameObject.name);
        }

        CrearSecuenciaConError();

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
    // CREAR ERROR ALEATORIO
    // =====================================================

    void CrearSecuenciaConError()
    {
        List<int> orden = new List<int>()
        {
            0, 1, 2, 3
        };

        string clave =
            "MundoPujllay_ErrorSecuencia_" +
            ejercicioActual;

        int movimientoAnterior =
            PlayerPrefs.GetInt(clave, -1);

        int origen;
        int destino;
        int movimientoActual;

        do
        {
            origen = Random.Range(0, 4);
            destino = Random.Range(0, 4);

            movimientoActual = origen * 4 + destino;
        }
        while (
            origen == destino ||
            movimientoActual == movimientoAnterior
        );

        int tarjetaMovida = orden[origen];

        orden.RemoveAt(origen);
        orden.Insert(destino, tarjetaMovida);

        for (int i = 0; i < 4; i++)
        {
            ordenTarjetas[i] = orden[i];
        }

        posicionError = destino;

        PlayerPrefs.SetInt(
            clave,
            movimientoActual
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

        for (int i = 0; i < 4; i++)
        {
            botonesPasos[i].image.color =
                colorNormal;
        }

        botonesPasos[indice].image.color =
            colorSeleccionado;

        MostrarMensaje(
            "¡Tarjeta seleccionada! " +
            "Presiona COMPROBAR."
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
                "¡Primero selecciona una tarjeta!"
            );

            return;
        }

        // =================================================
        // RESPUESTA CORRECTA
        // =================================================

        if (tarjetaSeleccionada == posicionError)
        {
            ejercicioCompletado = true;

            // =============================================
            // GUARDAR ESTRELLAS - SECUENCIAS MISION 3
            // =============================================

            ProgresoEstrellas.GuardarEjercicio(
                "Secuencias",
                3,
                ejercicioActual + 1
            );

            botonesPasos[tarjetaSeleccionada]
                .image.color = colorCorrecto;

            MostrarSecuenciaCorrecta();

            foreach (Button boton in botonesPasos)
            {
                boton.interactable = false;
            }

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);
        }
        else
        {
            // RESPUESTA INCORRECTA

            MostrarMensaje(
                "¡Casi! Observa el orden " +
                "e inténtalo otra vez."
            );

            botonesPasos[tarjetaSeleccionada]
                .image.color = colorNormal;

            tarjetaSeleccionada = -1;
        }
    }

    // =====================================================
    // MOSTRAR SECUENCIA CORRECTA
    // =====================================================

    void MostrarSecuenciaCorrecta()
    {
        Ejercicio ejercicio =
            ejercicios[ejercicioActual];

        for (int i = 0; i < 4; i++)
        {
            botonesPasos[i].image.sprite =
                ejercicio.pasos[i];

            botonesPasos[i].image.color =
                colorNormal;
        }

        MostrarMensaje(
            "¡Excelente! Encontraste la tarjeta " +
            "fuera de orden. ¡Así es la secuencia correcta!"
        );
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
            "¡Felicidades! Encontraste los errores " +
            "en las 3 secuencias."
        );

        txtProgreso.text = "";

        foreach (Button boton in botonesPasos)
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
            // CIRCULO EXTERIOR

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

            rect.anchorMin =
                new Vector2(0.5f, 0.5f);

            rect.anchorMax =
                new Vector2(0.5f, 0.5f);

            rect.pivot =
                new Vector2(0.5f, 0.5f);

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

            // CIRCULO INTERIOR

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
            imagenInterior.color = colorPendiente;
            imagenInterior.raycastTarget = false;

            circulosProgreso[i] = imagenInterior;

            // NUMERO DEL CIRCULO

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
                circulo.transform.parent.gameObject
                    .SetActive(false);
            }
        }
    }

    // =====================================================
    // SEGURIDAD
    // =====================================================

    void OnDisable()
    {
        DetenerPreguntaTemporal();
    }
}
