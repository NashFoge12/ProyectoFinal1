
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogrosManager : MonoBehaviour
{
    [System.Serializable]
    public class CategoriaLogros
    {
        public string nombre;
        public RectTransform fila;

        [HideInInspector]
        public Image[,] estrellas = new Image[3, 3];
    }

    // ============================================
    // FILAS DE CATEGORIAS
    // ============================================

    [Header("FILAS DE CATEGORIAS")]
    [SerializeField] private RectTransform filaAlgoritmos;
    [SerializeField] private RectTransform filaSecuencias;
    [SerializeField] private RectTransform filaPatrones;
    [SerializeField] private RectTransform filaDescomposicion;
    [SerializeField] private RectTransform filaDetectarErrores;

    // ============================================
    // RESUMEN DE PROGRESO
    // ============================================

    [Header("RESUMEN DE PROGRESO")]
    [SerializeField] private TMP_Text txtEstrellasObtenidas;
    [SerializeField] private Slider barraProgreso;
    [SerializeField] private TMP_Text txtProgreso;

    // ============================================
    // COLORES DE ESTRELLAS
    // ============================================

    [Header("COLORES DE ESTRELLAS")]
    [SerializeField]
    private Color colorEstrellaGanada =
        new Color32(255, 201, 65, 255);

    [SerializeField]
    private Color colorEstrellaVacia =
        new Color32(185, 187, 202, 255);

    // ============================================
    // DISTRIBUCION
    // ============================================

    [Header("DISTRIBUCION")]
    [SerializeField] private float anchoEstrella = 48f;
    [SerializeField] private float separacionEstrellas = 54f;
    [SerializeField] private float separacionMisiones = 205f;

    // NUEVO: Mover todas las estrellas horizontalmente
    [SerializeField] private float posicionXEstrellas = 0f;

    [SerializeField] private float posicionYEstrellas = -35f;

    // ============================================
    // CATEGORIAS DISPONIBLES
    // ============================================

    [Header("CATEGORIAS DISPONIBLES")]
    [SerializeField] private bool incluirDescomposicion = false;

    private CategoriaLogros[] categorias;

    private int totalEstrellasGanadas;
    private int totalEstrellasPosibles;

    private Sprite spriteEstrella;

    // ============================================
    // INICIALIZACION
    // ============================================

    private void Awake()
    {
        categorias = new CategoriaLogros[]
        {
            new CategoriaLogros
            {
                nombre = "Algoritmos",
                fila = filaAlgoritmos
            },

            new CategoriaLogros
            {
                nombre = "Secuencias",
                fila = filaSecuencias
            },

            new CategoriaLogros
            {
                nombre = "Patrones",
                fila = filaPatrones
            },

            new CategoriaLogros
            {
                nombre = "Descomposicion",
                fila = filaDescomposicion
            },

            new CategoriaLogros
            {
                nombre = "DetectarErrores",
                fila = filaDetectarErrores
            }
        };
    }

    private void Start()
    {
        GenerarTodasLasEstrellas();
        ActualizarLogros();
    }

    private void OnEnable()
    {
        if (categorias != null)
        {
            ActualizarLogros();
        }
    }

    // ============================================
    // GENERAR TODAS LAS ESTRELLAS
    // ============================================

    private void GenerarTodasLasEstrellas()
    {
        foreach (CategoriaLogros categoria in categorias)
        {
            if (categoria.fila == null)
                continue;

            for (int mision = 0; mision < 3; mision++)
            {
                GameObject grupo = new GameObject(
                    "Grupo_Mision" + (mision + 1),
                    typeof(RectTransform)
                );

                grupo.transform.SetParent(
                    categoria.fila,
                    false
                );

                RectTransform rectGrupo =
                    grupo.GetComponent<RectTransform>();

                rectGrupo.anchorMin =
                    new Vector2(0.5f, 0.5f);

                rectGrupo.anchorMax =
                    new Vector2(0.5f, 0.5f);

                rectGrupo.pivot =
                    new Vector2(0.5f, 0.5f);

                rectGrupo.sizeDelta =
                    new Vector2(180f, 65f);

                // POSICION X E Y DE LOS GRUPOS
                rectGrupo.anchoredPosition =
                    new Vector2(
                        posicionXEstrellas +
                        (mision - 1) * separacionMisiones,
                        posicionYEstrellas
                    );

                for (int ejercicio = 0; ejercicio < 3; ejercicio++)
                {
                    CrearEstrella(
                        categoria,
                        rectGrupo,
                        mision,
                        ejercicio
                    );
                }
            }
        }
    }

    // ============================================
    // CREAR CADA ESTRELLA
    // ============================================

    private void CrearEstrella(
        CategoriaLogros categoria,
        RectTransform padre,
        int mision,
        int ejercicio
    )
    {
        GameObject objeto = new GameObject(
            "Estrella_" + (ejercicio + 1),
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        objeto.transform.SetParent(padre, false);

        RectTransform rect =
            objeto.GetComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(0.5f, 0.5f);

        rect.anchorMax =
            new Vector2(0.5f, 0.5f);

        rect.pivot =
            new Vector2(0.5f, 0.5f);

        rect.sizeDelta = new Vector2(
            anchoEstrella,
            anchoEstrella
        );

        rect.anchoredPosition = new Vector2(
            (ejercicio - 1) * separacionEstrellas,
            0f
        );

        Image imagen = objeto.GetComponent<Image>();

        imagen.sprite = CrearSpriteEstrella();
        imagen.color = colorEstrellaVacia;
        imagen.raycastTarget = false;

        categoria.estrellas[mision, ejercicio] = imagen;
    }

    // ============================================
    // CREAR SPRITE DE ESTRELLA
    // ============================================

    private Sprite CrearSpriteEstrella()
    {
        if (spriteEstrella != null)
            return spriteEstrella;

        const int tamano = 128;

        Texture2D textura = new Texture2D(
            tamano,
            tamano,
            TextureFormat.RGBA32,
            false
        );

        textura.filterMode = FilterMode.Bilinear;
        textura.wrapMode = TextureWrapMode.Clamp;

        Color[] pixeles = new Color[tamano * tamano];

        Vector2 centro = new Vector2(64f, 64f);

        Vector2[] vertices = new Vector2[10];

        for (int i = 0; i < 10; i++)
        {
            float angulo =
                Mathf.PI / 2f + i * Mathf.PI / 5f;

            float radio =
                (i % 2 == 0) ? 56f : 24f;

            vertices[i] = centro + new Vector2(
                Mathf.Cos(angulo) * radio,
                Mathf.Sin(angulo) * radio
            );
        }

        for (int y = 0; y < tamano; y++)
        {
            for (int x = 0; x < tamano; x++)
            {
                Vector2 punto = new Vector2(x, y);

                bool dentro = PuntoEnPoligono(
                    punto,
                    vertices
                );

                pixeles[y * tamano + x] =
                    dentro ? Color.white : Color.clear;
            }
        }

        textura.SetPixels(pixeles);
        textura.Apply();

        spriteEstrella = Sprite.Create(
            textura,
            new Rect(0, 0, tamano, tamano),
            new Vector2(0.5f, 0.5f),
            100f
        );

        return spriteEstrella;
    }

    // ============================================
    // DETECTAR PUNTO DENTRO DEL POLIGONO
    // ============================================

    private bool PuntoEnPoligono(
        Vector2 punto,
        Vector2[] vertices
    )
    {
        bool dentro = false;
        int j = vertices.Length - 1;

        for (int i = 0; i < vertices.Length; i++)
        {
            if (
                (vertices[i].y > punto.y) !=
                (vertices[j].y > punto.y)
            )
            {
                float interseccion =
                    (vertices[j].x - vertices[i].x) *
                    (punto.y - vertices[i].y) /
                    (vertices[j].y - vertices[i].y) +
                    vertices[i].x;

                if (punto.x < interseccion)
                    dentro = !dentro;
            }

            j = i;
        }

        return dentro;
    }

    // ============================================
    // ACTUALIZAR LOGROS
    // ============================================

    public void ActualizarLogros()
    {
        if (categorias == null)
            return;

        totalEstrellasGanadas = 0;
        totalEstrellasPosibles = 0;

        foreach (CategoriaLogros categoria in categorias)
        {
            bool disponible =
                categoria.nombre != "Descomposicion" ||
                incluirDescomposicion;

            if (disponible)
                totalEstrellasPosibles += 9;

            for (int mision = 0; mision < 3; mision++)
            {
                for (int ejercicio = 0; ejercicio < 3; ejercicio++)
                {
                    bool ganada = disponible &&
                        ConsultarEstrella(
                            categoria.nombre,
                            mision + 1,
                            ejercicio + 1
                        );

                    Image imagen =
                        categoria.estrellas[mision, ejercicio];

                    if (imagen != null)
                    {
                        imagen.color = ganada
                            ? colorEstrellaGanada
                            : colorEstrellaVacia;
                    }

                    if (ganada)
                        totalEstrellasGanadas++;
                }
            }
        }

        ActualizarResumen();
    }

    // ============================================
    // CONSULTAR PROGRESO REAL
    // ============================================

    private bool ConsultarEstrella(
        string categoria,
        int mision,
        int ejercicio
    )
    {
        int estrellasGuardadas =
            ProgresoEstrellas.ObtenerEstrellas(
                categoria,
                mision
            );

        return estrellasGuardadas >= ejercicio;
    }

    // ============================================
    // ACTUALIZAR RESUMEN
    // ============================================

    private void ActualizarResumen()
    {
        if (txtEstrellasObtenidas != null)
        {
            txtEstrellasObtenidas.text =
                totalEstrellasGanadas +
                " / " +
                totalEstrellasPosibles;
        }

        if (barraProgreso != null)
        {
            barraProgreso.minValue = 0;

            barraProgreso.maxValue =
                Mathf.Max(1, totalEstrellasPosibles);

            barraProgreso.wholeNumbers = true;
            barraProgreso.interactable = false;

            barraProgreso.SetValueWithoutNotify(
                totalEstrellasGanadas
            );
        }

        if (txtProgreso != null)
        {
            if (totalEstrellasGanadas == 0)
            {
                txtProgreso.text =
                    "¡Comienza tu aventura y consigue estrellas!";
            }
            else if (
                totalEstrellasGanadas >=
                totalEstrellasPosibles
            )
            {
                txtProgreso.text =
                    "¡Increíble! ¡Completaste tu colección!";
            }
            else
            {
                txtProgreso.text =
                    "¡Sigue jugando para llenar tu colección!";
            }
        }
    }

    // ============================================
    // LIBERAR RECURSOS
    // ============================================

    private void OnDestroy()
    {
        if (spriteEstrella != null)
        {
            Texture2D textura = spriteEstrella.texture;

            Destroy(spriteEstrella);

            if (textura != null)
                Destroy(textura);
        }
    }
}
