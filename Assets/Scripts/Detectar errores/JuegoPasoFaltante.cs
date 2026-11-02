
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// MISION 2 - DETECTAR ERRORES: PASO FALTANTE
// Guarda este archivo como JuegoPasoFaltante.cs
public class JuegoPasoFaltante : MonoBehaviour
{
    // ==================== 01. PREGUNTAS ====================
    [Header("01 - PREGUNTAS E INSTRUCCIONES")]
    public TMP_Text txtPregunta;
    [Range(1f, 5f)] public float tiempoPregunta = 2f;

    // ==================== 02. FONDOS ====================
    [Header("02 - FONDO Y TITULO (IMAGES)")]
    public Image imagenFondo;
    public Image imagenTitulo;

    [Header("02A - FONDOS DE LOS 3 EJERCICIOS")]
    public Sprite fondoCastillo;
    public Sprite fondoPocion;
    public Sprite fondoNave;

    [Header("02B - TITULOS DE LOS 3 EJERCICIOS")]
    public Sprite tituloCastillo;
    public Sprite tituloPocion;
    public Sprite tituloNave;

    // ==================== 02C - PANEL PREGUNTA ====================
    [Header("02C - FONDO DEL PANEL PREGUNTA")]
    public Image imagenPanelPregunta;
    public Sprite fondoPanelCastillo;
    public Sprite fondoPanelPocion;
    public Sprite fondoPanelNave;

    // ==================== 03. TARJETAS ====================
    [Header("03 - CUATRO TARJETAS DE SECUENCIA (IMAGE)")]
    [Tooltip("En orden: arriba izquierda, arriba derecha, abajo izquierda, abajo derecha")]
    public Image[] tarjetasSecuencia = new Image[4];

    [Header("03A - INTERROGACION")]
    public Sprite spriteInterrogacion;

    [Header("03B - TRES ALTERNATIVAS (IMAGE)")]
    public Image[] tarjetasAlternativas = new Image[3];

    // ==================== 04. CASTILLO ====================
    [Header("04 - EJERCICIO 1: CONSTRUIR UN CASTILLO")]
    public Sprite castilloPrepararBloques;
    public Sprite castilloLevantarMuros;
    public Sprite castilloColocarPuerta;
    public Sprite castilloPonerBandera;

    [Tooltip("Alternativa incorrecta: por ejemplo, regar una maceta")]
    public Sprite castilloDistractor1;

    [Tooltip("Alternativa incorrecta: por ejemplo, afinar una guitarra")]
    public Sprite castilloDistractor2;

    // ==================== 05. POCION ====================
    [Header("05 - EJERCICIO 2: PREPARAR UNA POCION")]
    public Sprite pocionReunirIngredientes;
    public Sprite pocionLlenarCaldero;
    public Sprite pocionMezclarIngredientes;
    public Sprite pocionServirPocion;
    public Sprite pocionDistractor1;
    public Sprite pocionDistractor2;

    // ==================== 06. NAVE ====================
    [Header("06 - EJERCICIO 3: ACTIVAR UNA NAVE")]
    public Sprite naveRevisarEnergia;
    public Sprite naveEncenderPanel;
    public Sprite naveActivarMotores;
    public Sprite naveDespegar;

    [Tooltip("Una opcion incorrecta; las otras dos alternativas son los dos pasos faltantes")]
    public Sprite naveDistractor;

    // ==================== 07. BOTONES ====================
    [Header("07 - BOTONES")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // ==================== 08. PROGRESO ====================
    [Header("08 - PANEL Y POSICION DE PROGRESO")]
    public RectTransform panelProgreso;
    [Min(12f)] public float tamanoCirculo = 28f;
    [Min(16f)] public float separacionCirculos = 38f;
    public float posicionVerticalCirculos = -20f;

    [Header("08A - COLORES DE PROGRESO")]
    public Color colorActivo = new Color(1f, .78f, .24f, 1f);
    public Color colorPendiente = new Color(.78f, .76f, .72f, 1f);
    public Color colorNumero = Color.white;

    [Header("08B - BORDES DE PROGRESO")]
    public Color colorBorde = new Color(.46f, .29f, .65f, 1f);
    [Range(0f, 12f)] public float grosorBorde = 3f;

    // ==================== 09. COLORES ====================
    [Header("09 - COLORES DE TARJETAS")]
    public Color colorNormal = Color.white;
    public Color colorSeleccionado = new Color(.72f, .9f, 1f, 1f);
    public Color colorCorrecto = new Color(.7f, 1f, .75f, 1f);
    public Color colorError = new Color(1f, .75f, .75f, 1f);

    // ==================== 10. VARIACION ====================
    [Header("10 - VARIACIONES")]
    public bool mezclarAlternativas = true;
    public bool variarPosicionVacia = true;

    private class Ejercicio
    {
        public string pregunta;
        public Sprite fondo;
        public Sprite titulo;
        public Sprite fondoPanelPregunta;
        public Sprite[] pasos;
        public Sprite[] distractores;
        public int cantidadVacios;
    }

    private readonly List<Ejercicio> ejercicios = new List<Ejercicio>();
    private readonly List<int> posicionesVacias = new List<int>();

    private Sprite[] pasosCorrectos;
    private Sprite[] pasosVisibles;
    private Sprite[] opcionesActuales;

    private int ejercicioActual;
    private int alternativaSeleccionada = -1;

    private bool completado;
    private Coroutine rutinaPregunta;

    private Image[] circulosProgreso;
    private Sprite spriteCirculo;
    private Canvas canvasPrincipal;

    // ==================== INICIO ====================
    private void Start()
    {
        if (!ValidarReferencias())
        {
            enabled = false;
            return;
        }

        canvasPrincipal = GetComponentInParent<Canvas>();

        if (canvasPrincipal == null)
            canvasPrincipal = FindFirstObjectByType<Canvas>();

        if (canvasPrincipal == null)
        {
            Debug.LogError("JuegoPasoFaltante: falta Canvas.");
            enabled = false;
            return;
        }

        for (int i = 0; i < tarjetasAlternativas.Length; i++)
        {
            int indice = i;
            Image img = tarjetasAlternativas[i];

            img.raycastTarget = true;

            OpcionArrastrablePaso componente =
                img.GetComponent<OpcionArrastrablePaso>();

            if (componente == null)
                componente = img.gameObject.AddComponent<OpcionArrastrablePaso>();

            componente.Configurar(this, indice, canvasPrincipal);
        }

        for (int i = 0; i < tarjetasSecuencia.Length; i++)
        {
            int indice = i;
            Image img = tarjetasSecuencia[i];

            img.raycastTarget = true;

            CasillaPasoFaltante componente =
                img.GetComponent<CasillaPasoFaltante>();

            if (componente == null)
                componente = img.gameObject.AddComponent<CasillaPasoFaltante>();

            componente.Configurar(this, indice);
        }

        btnComprobar.onClick.AddListener(Comprobar);
        btnSiguiente.onClick.AddListener(SiguienteEjercicio);

        CrearEjercicios();
        CrearIndicadorProgreso();
        CargarEjercicio();
    }

    private bool ValidarReferencias()
    {
        if (txtPregunta == null || panelProgreso == null ||
            btnComprobar == null || btnSiguiente == null)
        {
            Debug.LogError(
                "JuegoPasoFaltante: asigna Txt_Pregunta, Panel_Progreso y ambos botones."
            );
            return false;
        }

        if (tarjetasSecuencia == null || tarjetasSecuencia.Length != 4 ||
            tarjetasAlternativas == null || tarjetasAlternativas.Length != 3)
        {
            Debug.LogError(
                "JuegoPasoFaltante: necesitas 4 tarjetas de secuencia y 3 alternativas."
            );
            return false;
        }

        foreach (Image img in tarjetasSecuencia)
        {
            if (img == null)
            {
                Debug.LogError("Falta una tarjeta de secuencia.");
                return false;
            }
        }

        foreach (Image img in tarjetasAlternativas)
        {
            if (img == null)
            {
                Debug.LogError("Falta una alternativa.");
                return false;
            }
        }

        return true;
    }

    // ==================== EJERCICIOS ====================
    private void CrearEjercicios()
    {
        ejercicios.Clear();

        ejercicios.Add(new Ejercicio
        {
            pregunta = "¿Qué paso falta para construir el castillo?",
            fondo = fondoCastillo,
            titulo = tituloCastillo,
            fondoPanelPregunta = fondoPanelCastillo,
            pasos = new[]
            {
                castilloPrepararBloques,
                castilloLevantarMuros,
                castilloColocarPuerta,
                castilloPonerBandera
            },
            distractores = new[]
            {
                castilloDistractor1,
                castilloDistractor2
            },
            cantidadVacios = 1
        });

        ejercicios.Add(new Ejercicio
        {
            pregunta = "¿Qué paso falta para preparar la poción?",
            fondo = fondoPocion,
            titulo = tituloPocion,
            fondoPanelPregunta = fondoPanelPocion,
            pasos = new[]
            {
                pocionReunirIngredientes,
                pocionLlenarCaldero,
                pocionMezclarIngredientes,
                pocionServirPocion
            },
            distractores = new[]
            {
                pocionDistractor1,
                pocionDistractor2
            },
            cantidadVacios = 1
        });

        ejercicios.Add(new Ejercicio
        {
            pregunta = "¡Completa los dos pasos para activar la nave!",
            fondo = fondoNave,
            titulo = tituloNave,
            fondoPanelPregunta = fondoPanelNave,
            pasos = new[]
            {
                naveRevisarEnergia,
                naveEncenderPanel,
                naveActivarMotores,
                naveDespegar
            },
            distractores = new[]
            {
                naveDistractor
            },
            cantidadVacios = 2
        });
    }

    private void CargarEjercicio()
    {
        DetenerPregunta();

        completado = false;
        alternativaSeleccionada = -1;

        Ejercicio ejercicio = ejercicios[ejercicioActual];

        pasosCorrectos = (Sprite[])ejercicio.pasos.Clone();
        pasosVisibles = (Sprite[])pasosCorrectos.Clone();

        posicionesVacias.Clear();

        // Se mantienen visibles los dos primeros pasos para que el patron sea entendible.
        List<int> candidatas = new List<int> { 2, 3 };

        if (variarPosicionVacia && Random.Range(0, 2) == 1)
        {
            int aux = candidatas[0];
            candidatas[0] = candidatas[1];
            candidatas[1] = aux;
        }

        for (int i = 0; i < ejercicio.cantidadVacios; i++)
        {
            posicionesVacias.Add(candidatas[i]);
            pasosVisibles[candidatas[i]] = null;
        }

        List<Sprite> opciones = new List<Sprite>();

        foreach (int posicion in posicionesVacias)
            opciones.Add(pasosCorrectos[posicion]);

        opciones.AddRange(ejercicio.distractores);

        if (mezclarAlternativas)
        {
            for (int i = opciones.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);

                Sprite temp = opciones[i];
                opciones[i] = opciones[j];
                opciones[j] = temp;
            }
        }

        opcionesActuales = opciones.ToArray();

        for (int i = 0; i < tarjetasAlternativas.Length; i++)
        {
            tarjetasAlternativas[i].gameObject.SetActive(true);
            tarjetasAlternativas[i].sprite = opcionesActuales[i];
            tarjetasAlternativas[i].color = colorNormal;

            // Se conserva el comportamiento original de las tarjetas.
            tarjetasAlternativas[i].preserveAspect = false;
        }

        if (imagenFondo != null && ejercicio.fondo != null)
            imagenFondo.sprite = ejercicio.fondo;

        if (imagenTitulo != null && ejercicio.titulo != null)
            imagenTitulo.sprite = ejercicio.titulo;

        // Cambia el fondo del Panel_Pregunta según el ejercicio actual.
        if (imagenPanelPregunta != null &&
            ejercicio.fondoPanelPregunta != null)
        {
            imagenPanelPregunta.sprite = ejercicio.fondoPanelPregunta;
        }

        ActualizarTarjetas();
        ActualizarIndicadorProgreso();

        btnComprobar.gameObject.SetActive(true);
        btnComprobar.interactable = true;

        btnSiguiente.gameObject.SetActive(false);

        rutinaPregunta = StartCoroutine(
            MostrarPreguntaTemporal(ejercicio.pregunta)
        );
    }

    private void ActualizarTarjetas()
    {
        for (int i = 0; i < tarjetasSecuencia.Length; i++)
        {
            tarjetasSecuencia[i].gameObject.SetActive(true);

            tarjetasSecuencia[i].sprite =
                pasosVisibles[i] != null
                ? pasosVisibles[i]
                : spriteInterrogacion;

            tarjetasSecuencia[i].color = colorNormal;

            // Se conserva el comportamiento original de las tarjetas.
            tarjetasSecuencia[i].preserveAspect = false;
        }
    }

    // ==================== TOCAR O ARRASTRAR ====================
    public void SeleccionarAlternativa(int indice)
    {
        if (completado || indice < 0 ||
            indice >= opcionesActuales.Length)
            return;

        DetenerPregunta();

        alternativaSeleccionada = indice;

        for (int i = 0; i < tarjetasAlternativas.Length; i++)
        {
            tarjetasAlternativas[i].color =
                i == indice ? colorSeleccionado : colorNormal;
        }

        MostrarMensaje(
            "¡Ahora toca el espacio vacío donde irá esa figura!"
        );
    }

    public void TocarCasilla(int posicion)
    {
        if (completado || !posicionesVacias.Contains(posicion))
            return;

        if (alternativaSeleccionada >= 0)
        {
            ColocarOpcion(posicion, alternativaSeleccionada);
        }
        else if (pasosVisibles[posicion] != null)
        {
            pasosVisibles[posicion] = null;

            ActualizarTarjetas();

            MostrarMensaje(
                "¡Casilla libre! Elige otra alternativa."
            );
        }
    }

    public void ColocarOpcion(int posicion, int indiceOpcion)
    {
        if (completado || !posicionesVacias.Contains(posicion))
            return;

        if (indiceOpcion < 0 ||
            indiceOpcion >= opcionesActuales.Length)
            return;

        DetenerPregunta();

        pasosVisibles[posicion] = opcionesActuales[indiceOpcion];

        alternativaSeleccionada = -1;

        ActualizarTarjetas();

        foreach (Image img in tarjetasAlternativas)
            img.color = colorNormal;

        MostrarMensaje(
            "¡Muy bien! Completa los espacios y presiona COMPROBAR."
        );
    }

    // ==================== COMPROBAR Y ESTRELLAS ====================
    private void Comprobar()
    {
        if (completado)
            return;

        DetenerPregunta();

        foreach (int posicion in posicionesVacias)
        {
            if (pasosVisibles[posicion] == null)
            {
                MostrarMensaje(
                    "¡Todavía falta completar una casilla!"
                );
                return;
            }
        }

        bool correcto = true;

        foreach (int posicion in posicionesVacias)
        {
            if (pasosVisibles[posicion] != pasosCorrectos[posicion])
            {
                correcto = false;
                tarjetasSecuencia[posicion].color = colorError;
            }
            else
            {
                tarjetasSecuencia[posicion].color = colorCorrecto;
            }
        }

        if (!correcto)
        {
            MostrarMensaje(
                "¡Casi! Revisa los pasos y vuelve a intentarlo."
            );
            return;
        }

        completado = true;

        ProgresoEstrellas.GuardarEjercicio(
            "DetectarErrores",
            2,
            ejercicioActual + 1
        );

        MostrarMensaje(
            "¡Excelente! ¡Encontraste el paso faltante!"
        );

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(true);
    }

    private void SiguienteEjercicio()
    {
        if (!completado)
            return;

        ejercicioActual++;

        if (ejercicioActual < ejercicios.Count)
            CargarEjercicio();
        else
            FinalizarMision();
    }

    private void FinalizarMision()
    {
        DetenerPregunta();

        MostrarMensaje("¡MISIÓN COMPLETADA!");

        foreach (Image img in tarjetasSecuencia)
            img.gameObject.SetActive(false);

        foreach (Image img in tarjetasAlternativas)
            img.gameObject.SetActive(false);

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);

        if (circulosProgreso != null)
        {
            foreach (Image img in circulosProgreso)
            {
                if (img != null)
                    img.gameObject.SetActive(false);
            }
        }
    }

    // ==================== PREGUNTA TEMPORAL ====================
    private IEnumerator MostrarPreguntaTemporal(string pregunta)
    {
        MostrarMensaje(pregunta);

        yield return new WaitForSeconds(tiempoPregunta);

        MostrarMensaje(
            "¡Descubre qué paso falta y completa la secuencia!"
        );

        rutinaPregunta = null;
    }

    private void MostrarMensaje(string mensaje)
    {
        if (txtPregunta != null)
            txtPregunta.text = mensaje;
    }

    private void DetenerPregunta()
    {
        if (rutinaPregunta != null)
        {
            StopCoroutine(rutinaPregunta);
            rutinaPregunta = null;
        }
    }

    private void OnDisable()
    {
        DetenerPregunta();
    }

    // ==================== CIRCULOS CON BORDES ====================
    private Sprite CrearSpriteCircular()
    {
        const int resolucion = 128;

        Texture2D textura = new Texture2D(
            resolucion,
            resolucion,
            TextureFormat.RGBA32,
            false
        );

        textura.filterMode = FilterMode.Bilinear;

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
                    radio - distancia + .5f
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
            new Vector2(.5f, .5f),
            100f
        );
    }

    private void CrearIndicadorProgreso()
    {
        spriteCirculo = CrearSpriteCircular();

        circulosProgreso = new Image[ejercicios.Count];

        for (int i = 0; i < ejercicios.Count; i++)
        {
            GameObject raiz = new GameObject(
                "Circulo_Progreso_" + (i + 1),
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );

            raiz.transform.SetParent(panelProgreso, false);

            RectTransform rect = raiz.GetComponent<RectTransform>();

            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.pivot = new Vector2(.5f, .5f);
            rect.sizeDelta = Vector2.one * tamanoCirculo;

            rect.anchoredPosition = new Vector2(
                (i - (ejercicios.Count - 1) / 2f) * separacionCirculos,
                posicionVerticalCirculos
            );

            Image borde = raiz.GetComponent<Image>();

            borde.sprite = spriteCirculo;
            borde.color = colorBorde;
            borde.raycastTarget = false;

            GameObject rellenoObj = new GameObject(
                "Relleno",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );

            rellenoObj.transform.SetParent(raiz.transform, false);

            RectTransform rellenoRect =
                rellenoObj.GetComponent<RectTransform>();

            rellenoRect.anchorMin = rellenoRect.anchorMax =
                new Vector2(.5f, .5f);

            rellenoRect.pivot = new Vector2(.5f, .5f);

            rellenoRect.sizeDelta = Vector2.one *
                Mathf.Max(1f, tamanoCirculo - grosorBorde * 2f);

            Image relleno = rellenoObj.GetComponent<Image>();

            relleno.sprite = spriteCirculo;
            relleno.raycastTarget = false;

            circulosProgreso[i] = relleno;

            GameObject numeroObj = new GameObject(
                "Numero_" + (i + 1),
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI)
            );

            numeroObj.transform.SetParent(raiz.transform, false);

            RectTransform numeroRect =
                numeroObj.GetComponent<RectTransform>();

            numeroRect.anchorMin = Vector2.zero;
            numeroRect.anchorMax = Vector2.one;
            numeroRect.offsetMin = numeroRect.offsetMax = Vector2.zero;

            TextMeshProUGUI numero =
                numeroObj.GetComponent<TextMeshProUGUI>();

            numero.text = (i + 1).ToString();
            numero.fontSize = tamanoCirculo * .5f;
            numero.fontStyle = FontStyles.Bold;
            numero.alignment = TextAlignmentOptions.Center;
            numero.color = colorNumero;
            numero.raycastTarget = false;
        }

        ActualizarIndicadorProgreso();
    }

    private void ActualizarIndicadorProgreso()
    {
        if (circulosProgreso == null)
            return;

        for (int i = 0; i < circulosProgreso.Length; i++)
        {
            if (circulosProgreso[i] == null)
                continue;

            circulosProgreso[i].color =
                i <= ejercicioActual ? colorActivo : colorPendiente;
        }
    }
}

// Detecta toques y arrastres desde una alternativa.
public class OpcionArrastrablePaso : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerClickHandler
{
    private JuegoPasoFaltante juego;
    private int indice;
    private Canvas canvas;

    private GameObject fantasma;
    private RectTransform rectFantasma;
    private bool arrastrando;

    public void Configurar(
        JuegoPasoFaltante manager,
        int numero,
        Canvas canvasRaiz
    )
    {
        juego = manager;
        indice = numero;
        canvas = canvasRaiz;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!arrastrando && juego != null)
            juego.SeleccionarAlternativa(indice);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (juego == null || canvas == null)
            return;

        arrastrando = true;

        fantasma = new GameObject(
            "Alternativa_Arrastrada",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        fantasma.transform.SetParent(canvas.transform, false);
        fantasma.transform.SetAsLastSibling();

        rectFantasma = fantasma.GetComponent<RectTransform>();

        Image origen = GetComponent<Image>();

        rectFantasma.sizeDelta = origen.rectTransform.rect.size;

        Image imagen = fantasma.GetComponent<Image>();

        imagen.sprite = origen.sprite;
        imagen.color = origen.color;
        imagen.raycastTarget = false;

        // SOLO el fantasma conserva la proporción de su imagen.
        // No modifica el preserveAspect de la tarjeta original.
        imagen.preserveAspect = true;

        Mover(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Mover(eventData);
    }

    private void Mover(PointerEventData eventData)
    {
        if (rectFantasma == null)
            return;

        RectTransform canvasRect =
            canvas.transform as RectTransform;

        Camera camara =
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : eventData.pressEventCamera;

        Vector2 local;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            camara,
            out local
        ))
        {
            rectFantasma.anchoredPosition = local;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (fantasma != null)
            Destroy(fantasma);

        fantasma = null;
        rectFantasma = null;
        arrastrando = false;
    }

    public int Indice => indice;
}

// Recibe el arrastre y permite tocar una casilla vacia.
public class CasillaPasoFaltante : MonoBehaviour,
    IDropHandler,
    IPointerClickHandler
{
    private JuegoPasoFaltante juego;
    private int posicion;

    public void Configurar(JuegoPasoFaltante manager, int indice)
    {
        juego = manager;
        posicion = indice;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (juego != null)
            juego.TocarCasilla(posicion);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (juego == null || eventData.pointerDrag == null)
            return;

        OpcionArrastrablePaso opcion =
            eventData.pointerDrag.GetComponent<OpcionArrastrablePaso>();

        if (opcion != null)
            juego.ColocarOpcion(posicion, opcion.Indice);
    }
}
