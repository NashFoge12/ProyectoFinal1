using System.Collections;

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JuegoOrdenarSecuencia : MonoBehaviour
{
    // =====================================================
    // TEXTOS
    // =====================================================

    [Header("TEXTOS DE INTERFAZ")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;

    // =====================================================
    // FONDOS Y TITULOS
    // =====================================================

    [Header("IMAGENES DE INTERFAZ")]
    public Image imagenFondo;
    public Image imagenTitulo;

    [Header("FONDOS DE LOS 3 EJERCICIOS")]
    public Sprite fondoMariposa;
    public Sprite fondoPlanta;
    public Sprite fondoAgua;

    [Header("TITULOS DE LOS 3 EJERCICIOS")]
    public Sprite tituloMariposa;
    public Sprite tituloPlanta;
    public Sprite tituloAgua;

    // =====================================================
    // CUATRO ESPACIOS NUMERADOS
    // =====================================================

    [Header("ESPACIOS - TUS 4 IMAGENES NUMERADAS")]
    public Image[] espacios = new Image[4];

    // =====================================================
    // CUATRO FICHAS
    // =====================================================

    [Header("FICHAS PARA ARRASTRAR")]
    public Button[] fichas = new Button[4];

    // =====================================================
    // EJERCICIO 1 - MARIPOSA
    // =====================================================

    [Header("EJERCICIO 1 - CICLO DE LA MARIPOSA")]
    public Sprite sprHuevo;
    public Sprite sprOruga;
    public Sprite sprCrisalida;
    public Sprite sprMariposaAdulta;

    // =====================================================
    // EJERCICIO 2 - PLANTA
    // =====================================================

    [Header("EJERCICIO 2 - CRECIMIENTO DE PLANTA")]
    public Sprite sprSemilla;
    public Sprite sprBrote;
    public Sprite sprPlantaGrande;
    public Sprite sprFlor;

    // =====================================================
    // EJERCICIO 3 - CICLO DEL AGUA
    // =====================================================

    [Header("EJERCICIO 3 - CICLO DEL AGUA")]
    public Sprite sprEvaporacion;
    public Sprite sprCondensacion;
    public Sprite sprPrecipitacion;
    public Sprite sprAcumulacion;

    // =====================================================
    // BOTONES
    // =====================================================

    [Header("BOTONES")]
    public Button btnComprobar;
    public Button btnSiguiente;

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

    public Color colorBorde =
        new Color(0.43f, 0.29f, 0.17f, 1f);

    // =====================================================
    // VARIABLES INTERNAS
    // =====================================================

    private int ejercicioActual = 0;
    private bool ejercicioCompletado = false;

    // Conserva la pregunta original mientras muestra los mensajes.
    private string preguntaBase = "";
    private bool misionFinalizada = false;

    [Header("DURACION DE LA PREGUNTA INICIAL")]
    [Range(1f, 5f)] public float tiempoPregunta = 2f;
    private Coroutine rutinaPregunta;

    // Cada ficha guarda el paso que representa.
    private int[] ordenFichas = new int[4];

    // Cada espacio guarda el indice de su ficha.
    // -1 significa espacio vacio.
    private int[] contenidoEspacios =
        { -1, -1, -1, -1 };

    private Image[] imagenesColocadas = new Image[4];

    private Image[] circulosProgreso;
    private Sprite spriteCirculo;

    private Canvas canvasPrincipal;
    private Image imagenArrastrada;

    private const int totalEjercicios = 3;

    // =====================================================
    // MENSAJES EN RESULTADO Y PREGUNTA
    // =====================================================

    void MostrarMensaje(string mensaje)
    {
        DetenerPreguntaTemporal();
        if (txtPregunta != null)
            txtPregunta.text = mensaje;
    }

    void DetenerPreguntaTemporal()
    {
        if (rutinaPregunta != null)
        {
            StopCoroutine(rutinaPregunta);
            rutinaPregunta = null;
        }
    }

    IEnumerator MostrarPreguntaInicial()
    {
        if (txtPregunta != null)
            txtPregunta.text = preguntaBase;

        yield return new WaitForSeconds(tiempoPregunta);
        rutinaPregunta = null;
        MostrarMensaje("¡Arrastra las fichas a los espacios numerados!");
    }

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

        canvasPrincipal = GetComponentInParent<Canvas>();

        if (canvasPrincipal == null)
            canvasPrincipal = FindFirstObjectByType<Canvas>();

        if (canvasPrincipal == null)
        {
            Debug.LogError("No se encontro Canvas.");
            enabled = false;
            return;
        }

        // Crear una imagen dentro de cada espacio.
        // La imagen numerada original se conserva.

        for (int i = 0; i < 4; i++)
        {
            CrearImagenParaEspacio(i);
        }

        // Preparar las cuatro fichas arrastrables.

        for (int i = 0; i < 4; i++)
        {
            ArrastrarFichaSecuencia arrastre =
                fichas[i].GetComponent<ArrastrarFichaSecuencia>();

            if (arrastre == null)
            {
                arrastre =
                    fichas[i].gameObject.AddComponent<
                        ArrastrarFichaSecuencia>();
            }

            arrastre.Configurar(this, i);
        }

        // Permitir retirar una ficha con un toque y moverla con arrastre.
        // La retirada se gestiona en ArrastrarEspacioSecuencia mediante
        // IPointerClickHandler para evitar que el Button y el toque
        // intenten retirar la misma ficha dos veces.
        for (int i = 0; i < 4; i++)
        {
            Button boton = espacios[i].GetComponent<Button>();

            if (boton == null)
                boton = espacios[i].gameObject.AddComponent<Button>();

            boton.transition = Selectable.Transition.None;
            boton.targetGraphic = espacios[i];
            boton.interactable = true;
            boton.onClick.RemoveAllListeners();
            espacios[i].raycastTarget = true;

            ArrastrarEspacioSecuencia arrastreEspacio =
                espacios[i].GetComponent<ArrastrarEspacioSecuencia>();

            if (arrastreEspacio == null)
                arrastreEspacio =
                    espacios[i].gameObject.AddComponent<ArrastrarEspacioSecuencia>();

            arrastreEspacio.Configurar(this, i);
        }

        btnComprobar.onClick.AddListener(Comprobar);
        btnSiguiente.onClick.AddListener(SiguienteEjercicio);

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
            Debug.LogError("Faltan textos o botones.");
            return false;
        }

        if (espacios == null || espacios.Length != 4 ||
            fichas == null || fichas.Length != 4)
        {
            Debug.LogError("Asigna 4 espacios y 4 fichas.");
            return false;
        }

        for (int i = 0; i < 4; i++)
        {
            if (espacios[i] == null || fichas[i] == null)
            {
                Debug.LogError(
                    "Falta el espacio o ficha " + (i + 1)
                );
                return false;
            }
        }

        return true;
    }

    // =====================================================
    // OBTENER LAS IMAGENES CORRECTAS
    // =====================================================

    Sprite[] ObtenerPasos()
    {
        // EJERCICIO 1 - MARIPOSA

        if (ejercicioActual == 0)
        {
            return new Sprite[]
            {
                sprHuevo,
                sprOruga,
                sprCrisalida,
                sprMariposaAdulta
            };
        }

        // EJERCICIO 2 - PLANTA

        if (ejercicioActual == 1)
        {
            return new Sprite[]
            {
                sprSemilla,
                sprBrote,
                sprPlantaGrande,
                sprFlor
            };
        }

        // EJERCICIO 3 - AGUA

        return new Sprite[]
        {
            sprEvaporacion,
            sprCondensacion,
            sprPrecipitacion,
            sprAcumulacion
        };
    }

    // =====================================================
    // CARGAR EJERCICIO
    // =====================================================

    void CargarEjercicio()
    {
        DetenerPreguntaTemporal();
        ejercicioCompletado = false;
        misionFinalizada = false;

        if (ejercicioActual == 0)
        {
            preguntaBase =
                "Ordena las etapas del ciclo de la mariposa.";

            CambiarImagen(imagenFondo, fondoMariposa);
            CambiarImagen(imagenTitulo, tituloMariposa);
        }
        else if (ejercicioActual == 1)
        {
            preguntaBase =
                "Ordena las etapas del crecimiento de una planta.";

            CambiarImagen(imagenFondo, fondoPlanta);
            CambiarImagen(imagenTitulo, tituloPlanta);
        }
        else
        {
            preguntaBase =
                "Ordena las etapas del ciclo del agua.";

            CambiarImagen(imagenFondo, fondoAgua);
            CambiarImagen(imagenTitulo, tituloAgua);
        }

        rutinaPregunta = StartCoroutine(MostrarPreguntaInicial());

        txtProgreso.text =
            "Ejercicio " + (ejercicioActual + 1) +
            " de " + totalEjercicios;

        txtProgreso.textWrappingMode = TextWrappingModes.NoWrap;
        txtProgreso.overflowMode = TextOverflowModes.Overflow;

        ActualizarIndicadorProgreso();

        // Vaciar los cuatro espacios.

        for (int i = 0; i < 4; i++)
        {
            contenidoEspacios[i] = -1;

            espacios[i].gameObject.SetActive(true);

            // NO reemplazamos la imagen numerada.

            imagenesColocadas[i].sprite = null;
            imagenesColocadas[i].gameObject.SetActive(false);

            Button botonEspacio =
                espacios[i].GetComponent<Button>();

            botonEspacio.interactable = true;
        }

        // Mezclar las fichas disponibles.

        MezclarFichas();

        Sprite[] pasos = ObtenerPasos();

        for (int i = 0; i < 4; i++)
        {
            fichas[i].gameObject.SetActive(true);
            fichas[i].interactable = true;

            fichas[i].image.sprite =
                pasos[ordenFichas[i]];

            fichas[i].image.color = Color.white;
            fichas[i].image.preserveAspect = false;

            // Evita tintes por la transicion del boton.
            fichas[i].transition =
                Selectable.Transition.None;
        }

        btnComprobar.gameObject.SetActive(true);
        btnComprobar.interactable = true;

        btnSiguiente.gameObject.SetActive(false);
    }

    // =====================================================
    // CAMBIAR IMAGEN
    // =====================================================

    void CambiarImagen(Image destino, Sprite nueva)
    {
        if (destino != null && nueva != null)
        {
            destino.sprite = nueva;
            destino.preserveAspect = false;
        }
    }

    // =====================================================
    // MEZCLAR FICHAS
    // =====================================================

    void MezclarFichas()
    {
        for (int i = 0; i < 4; i++)
            ordenFichas[i] = i;

        // Mezcla Fisher-Yates.

        for (int i = 3; i > 0; i--)
        {
            int aleatorio = Random.Range(0, i + 1);

            int temporal = ordenFichas[i];

            ordenFichas[i] = ordenFichas[aleatorio];
            ordenFichas[aleatorio] = temporal;
        }

        // Evitar que empiecen completamente ordenadas.

        bool ordenadas = true;

        for (int i = 0; i < 4; i++)
        {
            if (ordenFichas[i] != i)
            {
                ordenadas = false;
                break;
            }
        }

        if (ordenadas)
        {
            int temporal = ordenFichas[0];
            ordenFichas[0] = ordenFichas[1];
            ordenFichas[1] = temporal;
        }
    }

    // =====================================================
    // CREAR IMAGEN DENTRO DE CADA ESPACIO
    // =====================================================

    void CrearImagenParaEspacio(int indice)
    {
        GameObject objeto = new GameObject(
            "Ficha_Colocada",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        objeto.transform.SetParent(
            espacios[indice].transform,
            false
        );

        RectTransform rect =
            objeto.GetComponent<RectTransform>();

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;

        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image imagen = objeto.GetComponent<Image>();

        imagen.raycastTarget = false;
        imagen.preserveAspect = false;

        imagenesColocadas[indice] = imagen;

        objeto.SetActive(false);
    }

    // =====================================================
    // COMENZAR A ARRASTRAR
    // =====================================================

    public void ComenzarArrastre(
        int indiceFicha,
        PointerEventData evento)
    {
        if (ejercicioCompletado || indiceFicha < 0 || indiceFicha >= fichas.Length)
            return;

        // La ficha original puede estar oculta porque ya fue colocada
        // en una casilla. También debe poder arrastrarse desde allí.

        if (imagenArrastrada != null)
            Destroy(imagenArrastrada.gameObject);

        GameObject objeto = new GameObject(
            "Ficha_Arrastrando",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        objeto.transform.SetParent(
            canvasPrincipal.transform,
            false
        );

        objeto.transform.SetAsLastSibling();

        imagenArrastrada = objeto.GetComponent<Image>();

        imagenArrastrada.sprite =
            fichas[indiceFicha].image.sprite;

        imagenArrastrada.color =
            new Color(1f, 1f, 1f, 0.85f);

        imagenArrastrada.raycastTarget = false;

        RectTransform rectFicha =
            fichas[indiceFicha].GetComponent<RectTransform>();

        imagenArrastrada.rectTransform.sizeDelta =
            rectFicha.rect.size;

        MoverArrastre(evento);
    }

    // =====================================================
    // MOVER FICHA
    // =====================================================

    public void MoverArrastre(PointerEventData evento)
    {
        if (imagenArrastrada == null)
            return;

        RectTransform canvasRect =
            canvasPrincipal.GetComponent<RectTransform>();

        Vector2 posicion;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            evento.position,
            ObtenerCamaraCanvas(),
            out posicion))
        {
            imagenArrastrada.rectTransform.anchoredPosition =
                posicion;
        }
    }

    // =====================================================
    // SOLTAR FICHA
    // =====================================================

    public void TerminarArrastre(
        int indiceFicha,
        PointerEventData evento)
    {
        if (imagenArrastrada != null)
        {
            Destroy(imagenArrastrada.gameObject);
            imagenArrastrada = null;
        }

        if (ejercicioCompletado)
            return;

        for (int i = 0; i < 4; i++)
        {
            bool dentro =
                RectTransformUtility.RectangleContainsScreenPoint(
                    espacios[i].rectTransform,
                    evento.position,
                    ObtenerCamaraCanvas()
                );

            if (dentro)
            {
                ColocarFicha(indiceFicha, i);
                return;
            }
        }

        MostrarMensaje("¡Arrastra la ficha dentro de un espacio!");
    }

    Camera ObtenerCamaraCanvas()
    {
        if (canvasPrincipal.renderMode ==
            RenderMode.ScreenSpaceOverlay)
            return null;

        return canvasPrincipal.worldCamera;
    }

    // =====================================================
    // COLOCAR FICHA EN UN ESPACIO
    // =====================================================

    void ColocarFicha(int indiceFicha, int indiceEspacio)
    {
        if (ejercicioCompletado)
            return;

        // Si hay una ficha anterior, devolverla.

        int fichaAnterior =
            contenidoEspacios[indiceEspacio];

        if (fichaAnterior != -1)
        {
            fichas[fichaAnterior].gameObject.SetActive(true);
        }

        contenidoEspacios[indiceEspacio] = indiceFicha;

        // Mostrar la ficha sobre el espacio numerado.

        imagenesColocadas[indiceEspacio].sprite =
            fichas[indiceFicha].image.sprite;

        imagenesColocadas[indiceEspacio]
            .gameObject.SetActive(true);

        fichas[indiceFicha].gameObject.SetActive(false);

        MostrarMensaje("¡Bien! Sigue colocando las demás fichas.");
    }

    // =====================================================
    // QUITAR UNA FICHA DEL ESPACIO
    // =====================================================

    void QuitarFicha(int indiceEspacio)
    {
        if (ejercicioCompletado)
            return;

        if (indiceEspacio < 0 || indiceEspacio >= contenidoEspacios.Length)
            return;

        int ficha = contenidoEspacios[indiceEspacio];

        if (ficha == -1)
            return;

        fichas[ficha].gameObject.SetActive(true);
        fichas[ficha].interactable = true;

        contenidoEspacios[indiceEspacio] = -1;

        imagenesColocadas[indiceEspacio].sprite = null;
        imagenesColocadas[indiceEspacio].gameObject.SetActive(false);

        MostrarMensaje("¡Puedes colocar nuevamente esa ficha!");
    }

    // Método público para retirar una ficha mediante un toque en Android
    // o un clic con el ratón en la computadora.
    public void TocarEspacioParaQuitar(int indiceEspacio)
    {
        QuitarFicha(indiceEspacio);
    }

    // =====================================================
    // ARRASTRAR DESDE UN ESPACIO YA OCUPADO
    // =====================================================

    public void ComenzarArrastreDesdeEspacio(int indiceEspacio, PointerEventData evento)
    {
        if (ejercicioCompletado || contenidoEspacios[indiceEspacio] == -1)
            return;

        int indiceFicha = contenidoEspacios[indiceEspacio];
        ComenzarArrastre(indiceFicha, evento);
    }

    public void TerminarArrastreDesdeEspacio(int origen, PointerEventData evento)
    {
        if (imagenArrastrada != null)
        {
            Destroy(imagenArrastrada.gameObject);
            imagenArrastrada = null;
        }

        if (ejercicioCompletado || contenidoEspacios[origen] == -1)
            return;

        // Soltar sobre otra casilla: mover o intercambiar las dos fichas.
        for (int destino = 0; destino < 4; destino++)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                espacios[destino].rectTransform, evento.position, ObtenerCamaraCanvas()))
                continue;

            if (destino == origen)
                return;

            int fichaOrigen = contenidoEspacios[origen];
            int fichaDestino = contenidoEspacios[destino];
            contenidoEspacios[destino] = fichaOrigen;
            contenidoEspacios[origen] = fichaDestino;
            ActualizarImagenEspacio(origen);
            ActualizarImagenEspacio(destino);
            MostrarMensaje("¡Bien! Puedes seguir cambiando el orden de las fichas.");
            return;
        }

        // Si se suelta fuera de las casillas, vuelve a las fichas disponibles.
        QuitarFicha(origen);
    }

    void ActualizarImagenEspacio(int indiceEspacio)
    {
        int indiceFicha = contenidoEspacios[indiceEspacio];
        Image imagen = imagenesColocadas[indiceEspacio];
        if (indiceFicha == -1)
        {
            imagen.sprite = null;
            imagen.gameObject.SetActive(false);
        }
        else
        {
            imagen.sprite = fichas[indiceFicha].image.sprite;
            imagen.gameObject.SetActive(true);
            fichas[indiceFicha].gameObject.SetActive(false);
        }
    }

    // =====================================================
    // COMPROBAR ORDEN
    // =====================================================

    void Comprobar()
    {
        if (ejercicioCompletado)
            return;

        // Revisar que no falten fichas.

        for (int i = 0; i < 4; i++)
        {
            if (contenidoEspacios[i] == -1)
            {
                MostrarMensaje("¡Primero llena los cuatro espacios!");
                return;
            }
        }

        // Revisar el orden de los cuatro pasos.

        for (int i = 0; i < 4; i++)
        {
            int ficha = contenidoEspacios[i];

            int pasoReal = ordenFichas[ficha];

            if (pasoReal != i)
            {
                MostrarMensaje("¡Casi! Revisa el orden e inténtalo otra vez.");
                return;
            }
        }

        // Todos los pasos estan correctamente ordenados.

        ejercicioCompletado = true;

        // =================================================
        // GUARDAR ESTRELLAS - SECUENCIAS MISION 1
        // =================================================

        ProgresoEstrellas.GuardarEjercicio(
            "Secuencias",
            1,
            ejercicioActual + 1
        );

        Debug.Log("Mision 1 - Secuencias - Estrella guardada: " +
                  (ejercicioActual + 1));

        MostrarMensaje("¡Excelente! Ordenaste correctamente la secuencia.");

        for (int i = 0; i < 4; i++)
        {
            espacios[i].GetComponent<Button>()
                .interactable = false;
        }

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(true);
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
            CargarEjercicio();
        else
            FinalizarMision();
    }

    // =====================================================
    // FINALIZAR MISION
    // =====================================================

    void FinalizarMision()
    {
        DetenerPreguntaTemporal();
        misionFinalizada = true;
        preguntaBase = "";

        MostrarMensaje("¡MISIÓN COMPLETADA!\n¡Felicidades! Ordenaste las 3 secuencias.");

        txtProgreso.text = "";

        foreach (Image espacio in espacios)
            espacio.gameObject.SetActive(false);

        foreach (Button ficha in fichas)
            ficha.gameObject.SetActive(false);

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);

        if (circulosProgreso != null)
        {
            foreach (Image circulo in circulosProgreso)
            {
                if (circulo != null)
                    circulo.transform.parent.gameObject
                        .SetActive(false);
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
    // CREAR CIRCULITOS DE PROGRESO
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

            objeto.transform.SetParent(panelProgreso, false);

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

            Image borde = objeto.GetComponent<Image>();

            borde.sprite = spriteCirculo;
            borde.color = colorBorde;
            borde.raycastTarget = false;

            GameObject interior = new GameObject(
                "Interior",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );

            interior.transform.SetParent(
                objeto.transform,
                false
            );

            RectTransform rectInterior =
                interior.GetComponent<RectTransform>();

            rectInterior.anchorMin = Vector2.zero;
            rectInterior.anchorMax = Vector2.one;

            rectInterior.offsetMin = new Vector2(2f, 2f);
            rectInterior.offsetMax = new Vector2(-2f, -2f);

            Image imagenInterior =
                interior.GetComponent<Image>();

            imagenInterior.sprite = spriteCirculo;
            imagenInterior.raycastTarget = false;

            circulosProgreso[i] = imagenInterior;

            GameObject numero = new GameObject(
                "Numero_" + (i + 1),
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI)
            );

            numero.transform.SetParent(
                objeto.transform,
                false
            );

            RectTransform rectNumero =
                numero.GetComponent<RectTransform>();

            rectNumero.anchorMin = Vector2.zero;
            rectNumero.anchorMax = Vector2.one;
            rectNumero.offsetMin = Vector2.zero;
            rectNumero.offsetMax = Vector2.zero;

            TextMeshProUGUI texto =
                numero.GetComponent<TextMeshProUGUI>();

            texto.text = (i + 1).ToString();
            texto.fontSize = tamanoCirculo * 0.5f;
            texto.fontStyle = FontStyles.Bold;
            texto.alignment = TextAlignmentOptions.Center;
            texto.color = Color.white;
            texto.raycastTarget = false;
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
            circulosProgreso[i].color =
                i <= ejercicioActual
                ? colorActivo
                : colorPendiente;
        }
    }
}

// =========================================================
// ARRASTRE DE FICHAS
// =========================================================

// ESTA CLASE VA EN EL MISMO ARCHIVO.
// NO CREES OTRO SCRIPT.

public class ArrastrarFichaSecuencia :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    private JuegoOrdenarSecuencia juego;
    private int indiceFicha;

    public void Configurar(
        JuegoOrdenarSecuencia controlador,
        int indice)
    {
        juego = controlador;
        indiceFicha = indice;
    }

    public void OnBeginDrag(PointerEventData evento)
    {
        if (juego != null)
            juego.ComenzarArrastre(indiceFicha, evento);
    }

    public void OnDrag(PointerEventData evento)
    {
        if (juego != null)
            juego.MoverArrastre(evento);
    }

    public void OnEndDrag(PointerEventData evento)
    {
        if (juego != null)
            juego.TerminarArrastre(indiceFicha, evento);
    }
}

// Permite corregir arrastrando desde las casillas de destino.
// Esta clase permanece en el mismo archivo JuegoOrdenarSecuencia.cs.
public class ArrastrarEspacioSecuencia : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerClickHandler
{
    private JuegoOrdenarSecuencia juego;
    private int indiceEspacio;
    private bool seInicioArrastre;

    public void Configurar(JuegoOrdenarSecuencia controlador, int indice)
    {
        juego = controlador;
        indiceEspacio = indice;
        seInicioArrastre = false;
    }

    public void OnBeginDrag(PointerEventData evento)
    {
        if (juego == null)
            return;

        seInicioArrastre = true;
        juego.ComenzarArrastreDesdeEspacio(indiceEspacio, evento);
    }

    public void OnDrag(PointerEventData evento)
    {
        if (juego != null)
            juego.MoverArrastre(evento);
    }

    public void OnEndDrag(PointerEventData evento)
    {
        if (juego != null && seInicioArrastre)
            juego.TerminarArrastreDesdeEspacio(indiceEspacio, evento);

        // Evita conservar el estado de arrastre para el siguiente toque.
        seInicioArrastre = false;
    }

    public void OnPointerClick(PointerEventData evento)
    {
        // Si fue un toque/clic sin arrastre, retirar la ficha.
        // El EventSystem no llama normalmente a OnPointerClick tras un arrastre.
        if (juego != null && !seInicioArrastre)
            juego.TocarEspacioParaQuitar(indiceEspacio);

        seInicioArrastre = false;
    }
}
