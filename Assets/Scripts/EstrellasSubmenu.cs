
using UnityEngine;
using UnityEngine.UI;

public class EstrellasSubmenu : MonoBehaviour
{
    // =====================================================
    // CATEGORIA
    // =====================================================

    [Header("CATEGORIA")]
    [Tooltip("Ejemplo: Algoritmos, Secuencias o Patrones")]
    public string categoria = "Algoritmos";

    // =====================================================
    // PANELES
    // =====================================================

    [Header("PANELES DE ESTRELLAS")]
    public RectTransform panelEstrellasMision1;
    public RectTransform panelEstrellasMision2;
    public RectTransform panelEstrellasMision3;

    // =====================================================
    // CONFIGURACION VISUAL
    // =====================================================

    [Header("CONFIGURACION VISUAL")]
    public float tamanoEstrella = 30f;
    public float separacion = 38f;

    [Header("COLORES")]
    public Color colorGanada =
        new Color(1f, 0.78f, 0.12f, 1f);

    public Color colorPendiente =
        new Color(0.72f, 0.72f, 0.72f, 1f);

    // =====================================================
    // DEPURACION
    // =====================================================

    [Header("DEPURACION")]
    [Tooltip("Muestra en Console las estrellas encontradas")]
    public bool mostrarMensajes = true;

    // =====================================================
    // VARIABLES
    // =====================================================

    private Sprite spriteEstrella;
    private bool inicializado = false;

    // =====================================================
    // INICIO
    // =====================================================

    void Awake()
    {
        Inicializar();
    }

    void OnEnable()
    {
        if (inicializado)
        {
            ActualizarEstrellas();
        }
    }

    void Start()
    {
        ActualizarEstrellas();
    }

    // =====================================================
    // INICIALIZAR
    // =====================================================

    void Inicializar()
    {
        if (inicializado)
            return;

        spriteEstrella = CrearSpriteEstrella();

        inicializado = true;
    }

    // =====================================================
    // ACTUALIZAR ESTRELLAS
    // =====================================================

    public void ActualizarEstrellas()
    {
        if (!inicializado)
        {
            Inicializar();
        }

        if (string.IsNullOrWhiteSpace(categoria))
        {
            Debug.LogError(
                "EstrellasSubmenu: Debes configurar " +
                "la categoria en el Inspector."
            );

            return;
        }

        // Evitar espacios accidentales.
        categoria = categoria.Trim();

        if (mostrarMensajes)
        {
            Debug.Log(
                "ACTUALIZANDO ESTRELLAS DE: " + categoria
            );
        }

        ActualizarPanel(
            panelEstrellasMision1,
            1
        );

        ActualizarPanel(
            panelEstrellasMision2,
            2
        );

        ActualizarPanel(
            panelEstrellasMision3,
            3
        );
    }

    // =====================================================
    // ACTUALIZAR PANEL DE UNA MISION
    // =====================================================

    void ActualizarPanel(
        RectTransform panel,
        int numeroMision)
    {
        if (panel == null)
        {
            Debug.LogWarning(
                "EstrellasSubmenu: Falta asignar " +
                "Panel_Estrellas de la Mision " +
                numeroMision
            );

            return;
        }

        // Leer las estrellas guardadas.

        int estrellasGanadas =
            ProgresoEstrellas.ObtenerEstrellas(
                categoria,
                numeroMision
            );

        // Mostrar resultado en Console.

        if (mostrarMensajes)
        {
            Debug.Log(
                "Categoria: " + categoria +
                " | Mision: " + numeroMision +
                " | Estrellas guardadas: " +
                estrellasGanadas
            );
        }

        // Crear o actualizar las tres estrellas.

        for (int i = 0; i < 3; i++)
        {
            string nombre =
                "Estrella_" + (i + 1);

            Transform existente =
                panel.Find(nombre);

            Image imagenEstrella;

            // ---------------------------------------------
            // SI NO EXISTE, CREARLA
            // ---------------------------------------------

            if (existente == null)
            {
                GameObject objeto =
                    new GameObject(
                        nombre,
                        typeof(RectTransform),
                        typeof(CanvasRenderer),
                        typeof(Image)
                    );

                objeto.transform.SetParent(
                    panel,
                    false
                );

                imagenEstrella =
                    objeto.GetComponent<Image>();
            }

            // ---------------------------------------------
            // SI EXISTE, REUTILIZARLA
            // ---------------------------------------------

            else
            {
                existente.gameObject.SetActive(true);

                TMPro.TextMeshProUGUI textoAnterior =
                    existente.GetComponent<
                        TMPro.TextMeshProUGUI
                    >();

                if (textoAnterior != null)
                {
                    textoAnterior.enabled = false;
                }

                imagenEstrella =
                    existente.GetComponent<Image>();

                if (imagenEstrella == null)
                {
                    imagenEstrella =
                        existente.gameObject
                        .AddComponent<Image>();
                }
            }

            // ---------------------------------------------
            // CONFIGURAR RECT TRANSFORM
            // ---------------------------------------------

            RectTransform rect =
                imagenEstrella.GetComponent<
                    RectTransform>();

            rect.anchorMin =
                new Vector2(0.5f, 0.5f);

            rect.anchorMax =
                new Vector2(0.5f, 0.5f);

            rect.pivot =
                new Vector2(0.5f, 0.5f);

            rect.sizeDelta =
                new Vector2(
                    tamanoEstrella,
                    tamanoEstrella
                );

            rect.anchoredPosition =
                new Vector2(
                    (i - 1) * separacion,
                    0f
                );

            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;

            // ---------------------------------------------
            // CONFIGURAR IMAGEN
            // ---------------------------------------------

            imagenEstrella.enabled = true;

            imagenEstrella.sprite =
                spriteEstrella;

            imagenEstrella.type =
                Image.Type.Simple;

            imagenEstrella.preserveAspect = true;

            imagenEstrella.raycastTarget = false;

            // ---------------------------------------------
            // PINTAR ESTRELLA
            // ---------------------------------------------

            bool estaGanada =
                i < estrellasGanadas;

            imagenEstrella.color =
                estaGanada
                ? colorGanada
                : colorPendiente;

            // ---------------------------------------------
            // ASEGURAR QUE SEA VISIBLE
            // ---------------------------------------------

            imagenEstrella.transform.SetAsLastSibling();
        }
    }

    // =====================================================
    // CREAR SPRITE DE ESTRELLA DE 5 PUNTAS
    // =====================================================

    Sprite CrearSpriteEstrella()
    {
        int resolucion = 256;

        Texture2D textura = new Texture2D(
            resolucion,
            resolucion,
            TextureFormat.RGBA32,
            false
        );

        textura.filterMode =
            FilterMode.Bilinear;

        textura.wrapMode =
            TextureWrapMode.Clamp;

        // Limpiar textura.

        Color[] pixeles =
            new Color[resolucion * resolucion];

        for (int i = 0; i < pixeles.Length; i++)
        {
            pixeles[i] = Color.clear;
        }

        textura.SetPixels(pixeles);

        // ---------------------------------------------
        // CALCULAR LOS 10 VERTICES
        // ---------------------------------------------

        Vector2[] vertices =
            new Vector2[10];

        Vector2 centro =
            new Vector2(
                resolucion / 2f,
                resolucion / 2f
            );

        float radioExterior =
            resolucion * 0.46f;

        float radioInterior =
            resolucion * 0.20f;

        for (int i = 0; i < 10; i++)
        {
            float angulo =
                Mathf.PI / 2f +
                i * Mathf.PI / 5f;

            float radio =
                i % 2 == 0
                ? radioExterior
                : radioInterior;

            vertices[i] =
                centro + new Vector2(
                    Mathf.Cos(angulo) * radio,
                    Mathf.Sin(angulo) * radio
                );
        }

        // ---------------------------------------------
        // DIBUJAR ESTRELLA
        // ---------------------------------------------

        for (int y = 0; y < resolucion; y++)
        {
            for (int x = 0; x < resolucion; x++)
            {
                Vector2 punto =
                    new Vector2(
                        x + 0.5f,
                        y + 0.5f
                    );

                if (
                    PuntoDentroPoligono(
                        punto,
                        vertices
                    )
                )
                {
                    textura.SetPixel(
                        x,
                        y,
                        Color.white
                    );
                }
            }
        }

        textura.Apply();

        return Sprite.Create(
            textura,
            new Rect(
                0,
                0,
                resolucion,
                resolucion
            ),
            new Vector2(0.5f, 0.5f),
            100f
        );
    }

    // =====================================================
    // DETECTAR PUNTO DENTRO DEL POLIGONO
    // =====================================================

    bool PuntoDentroPoligono(
        Vector2 punto,
        Vector2[] vertices)
    {
        bool dentro = false;

        int j = vertices.Length - 1;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 vi = vertices[i];
            Vector2 vj = vertices[j];

            if (
                (vi.y > punto.y) !=
                (vj.y > punto.y)
            )
            {
                float interseccionX =
                    (vj.x - vi.x) *
                    (punto.y - vi.y) /
                    (vj.y - vi.y) +
                    vi.x;

                if (punto.x < interseccionX)
                {
                    dentro = !dentro;
                }
            }

            j = i;
        }

        return dentro;
    }

    // =====================================================
    // LIBERAR TEXTURA AL DESTRUIR
    // =====================================================

    void OnDestroy()
    {
        if (spriteEstrella != null)
        {
            Texture2D textura =
                spriteEstrella.texture;

            Destroy(spriteEstrella);

            if (textura != null)
            {
                Destroy(textura);
            }
        }
    }
}
