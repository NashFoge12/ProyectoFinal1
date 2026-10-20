
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JuegoProgramaCamino : MonoBehaviour
{
    [Header("TABLERO")]
    public RectTransform panelTablero;
    public RectTransform panelObstaculos;
    public RectTransform imagenRobot;
    public RectTransform imagenEstrella;
    public GridLayoutGroup gridTablero;

    [Header("SPRITES DE CASILLAS")]
    public Sprite casillaJardin;
    public Sprite casillaBosque;
    public Sprite casillaTecnologica;

    [Header("SPRITES DE OBSTACULOS")]
    public Sprite obstaculoRoca;
    public Sprite obstaculoTronco;
    public Sprite obstaculoArbusto;
    public Sprite obstaculoCaja;

    [Header("FONDOS")]
    public Image imagenFondo;
    public Sprite fondoJardin;
    public Sprite fondoBosque;
    public Sprite fondoTecnologico;

    [Header("ROBOT - TUS CUATRO IMAGENES")]
    [Tooltip("Imagen frontal: se ve la carita")]
    public Sprite robotArriba;

    [Tooltip("Imagen trasera: se ve la espalda")]
    public Sprite robotAbajo;

    public Sprite robotIzquierda;
    public Sprite robotDerecha;

    [Header("LISTA DE INSTRUCCIONES")]
    public TMP_Text txtTituloInstrucciones;
    public RectTransform contenedorComandos;
    public Sprite tarjetaComando;
    public Sprite spriteArriba;
    public Sprite spriteAbajo;
    public Sprite spriteIzquierda;
    public Sprite spriteDerecha;

    [Header("BOTONES DE CONTROL")]
    public Button btnArriba;
    public Button btnAbajo;
    public Button btnIzquierda;
    public Button btnDerecha;
    public Button btnGO;
    public Button btnBorrar;
    public Button btnPausa;

    [Header("PANEL DE PRESENTACION")]
    public GameObject panelPresentacion;
    public TMP_Text txtEjercicio;
    public TMP_Text txtObjetivo;
    public Button btnIr;

    [Header("PANEL DE VICTORIA")]
    public GameObject panelVictoria;
    public TMP_Text txtExcelente;
    public TMP_Text txtLlegasteEstrella;
    public Button btnSiguiente;

    [Header("PROGRESO")]
    public RectTransform panelProgreso;

    [Header("MENSAJES OPCIONALES")]
    public TMP_Text txtMensaje;

    [Header("CONFIGURACION")]
    [Range(0.1f, 1.5f)]
    public float duracionPorCasilla = 0.45f;

    [Range(0.1f, 1f)]
    public float duracionGiro = 0.25f;

    public float separacionCasillas = 8f;

    [Range(4, 60)]
    public int maximoComandos = 30;

    [Header("ANIMACION DE ESTRELLA")]
    [Range(0.2f, 2f)]
    public float duracionAnimacionEstrella = 0.8f;

    [Range(1f, 2.5f)]
    public float escalaMaximaEstrella = 1.45f;

    [Header("NAVEGACION FINAL")]
    public string escenaAlTerminar = "";

    private const int TOTAL_EJERCICIOS = 4;

    private const string CLAVE_ESTRELLAS =
        "MundoPujllay_Programacion_PrimerosPasos_Mision1";

    private enum Direccion
    {
        Arriba = 0,
        Derecha = 1,
        Abajo = 2,
        Izquierda = 3
    }

    private readonly List<Direccion> comandos =
        new List<Direccion>();

    private readonly HashSet<Vector2Int> obstaculos =
        new HashSet<Vector2Int>();

    private readonly List<GameObject> casillasCreadas =
        new List<GameObject>();

    private readonly List<GameObject> obstaculosCreados =
        new List<GameObject>();

    private readonly List<GameObject> comandosCreados =
        new List<GameObject>();

    private readonly List<Image> circulos =
        new List<Image>();

    private readonly List<TMP_Text> numeros =
        new List<TMP_Text>();

    private int ejercicioActual = 1;
    private int tamanoTablero = 4;
    private int numeroObstaculos = 0;

    private Vector2Int inicio;
    private Vector2Int meta;
    private Vector2Int posicionRobot;

    private Direccion orientacionInicial = Direccion.Arriba;
    private Direccion orientacionRobot = Direccion.Arriba;

    private float tamanoCasilla;

    private bool ejecutando;
    private bool pausado;
    private bool ganado;
    private bool iniciado;
    private bool animandoEstrella;

    private Coroutine movimiento;
    private Coroutine animacionEstrella;

    private Image imagenRobotUI;
    private Image imagenEstrellaUI;

    private Vector3 escalaOriginalEstrella;
    private Color colorOriginalEstrella;

    private Sprite spriteCirculo;

    // =====================================================
    // INICIO
    // =====================================================

    private void Awake()
    {
        if (panelTablero != null && gridTablero == null)
        {
            gridTablero =
                panelTablero.GetComponent<GridLayoutGroup>();
        }

        if (imagenRobot != null)
        {
            imagenRobotUI =
                imagenRobot.GetComponent<Image>();
        }

        if (imagenEstrella != null)
        {
            imagenEstrellaUI =
                imagenEstrella.GetComponent<Image>();

            escalaOriginalEstrella =
                imagenEstrella.localScale;

            if (imagenEstrellaUI != null)
            {
                colorOriginalEstrella =
                    imagenEstrellaUI.color;
            }
        }

        ConectarBotones();
    }

    private void Start()
    {
        if (!ValidarReferencias())
        {
            Debug.LogError(
                "JuegoProgramaCamino: faltan referencias " +
                "importantes en el Inspector."
            );

            enabled = false;
            return;
        }

        PrepararContenedorComandos();
        PrepararProgreso();

        IniciarEjercicio(1);
    }

    private bool ValidarReferencias()
    {
        return panelTablero != null &&
               panelObstaculos != null &&
               imagenRobot != null &&
               imagenEstrella != null &&
               imagenEstrellaUI != null &&
               gridTablero != null &&
               contenedorComandos != null &&
               panelPresentacion != null &&
               panelVictoria != null &&
               panelProgreso != null &&
               btnIr != null &&
               btnSiguiente != null;
    }

    // =====================================================
    // CONECTAR BOTONES
    // =====================================================

    private void ConectarBotones()
    {
        if (btnArriba != null)
            btnArriba.onClick.AddListener(
                () => AgregarComando(Direccion.Arriba)
            );

        if (btnAbajo != null)
            btnAbajo.onClick.AddListener(
                () => AgregarComando(Direccion.Abajo)
            );

        if (btnIzquierda != null)
            btnIzquierda.onClick.AddListener(
                () => AgregarComando(Direccion.Izquierda)
            );

        if (btnDerecha != null)
            btnDerecha.onClick.AddListener(
                () => AgregarComando(Direccion.Derecha)
            );

        if (btnGO != null)
            btnGO.onClick.AddListener(EjecutarPrograma);

        if (btnBorrar != null)
            btnBorrar.onClick.AddListener(BorrarComando);

        if (btnPausa != null)
            btnPausa.onClick.AddListener(AlternarPausa);

        if (btnIr != null)
            btnIr.onClick.AddListener(ComenzarJuego);

        if (btnSiguiente != null)
            btnSiguiente.onClick.AddListener(SiguienteEjercicio);
    }

    // =====================================================
    // EJERCICIOS
    // =====================================================

    private void IniciarEjercicio(int numero)
    {
        DetenerMovimiento();
        DetenerAnimacionEstrella();

        ejercicioActual = Mathf.Clamp(
            numero, 1, TOTAL_EJERCICIOS
        );

        iniciado = false;
        ganado = false;
        pausado = false;
        ejecutando = false;
        animandoEstrella = false;

        comandos.Clear();

        LimpiarLista(comandosCreados);

        RestaurarEstrella();

        ConfigurarDificultad();
        CambiarFondo();

        GenerarCasillas();
        GenerarDistribucion();
        GenerarObstaculos();

        posicionRobot = inicio;
        orientacionRobot = orientacionInicial;

        CambiarSpriteRobot(orientacionRobot);
        ActualizarRobotYMeta();

        ActualizarComandosVisuales();
        ActualizarProgreso();

        MostrarMensaje("");
        MostrarPresentacion();
        ActualizarBotones();
    }

    private void ConfigurarDificultad()
    {
        switch (ejercicioActual)
        {
            case 1:
                tamanoTablero = 4;
                numeroObstaculos = 0;
                break;

            case 2:
                tamanoTablero = 5;
                numeroObstaculos = 4;
                break;

            case 3:
                tamanoTablero = 5;
                numeroObstaculos = 6;
                break;

            case 4:
                tamanoTablero = 6;
                numeroObstaculos = 8;
                break;
        }
    }

    private void CambiarFondo()
    {
        if (imagenFondo == null)
            return;

        Sprite fondo = fondoJardin;

        if (ejercicioActual == 2)
            fondo = fondoBosque;

        if (ejercicioActual >= 3)
            fondo = fondoTecnologico;

        if (fondo != null)
            imagenFondo.sprite = fondo;
    }

    // =====================================================
    // GENERAR CASILLAS
    // =====================================================

    private void GenerarCasillas()
    {
        LimpiarLista(casillasCreadas);
        LimpiarLista(obstaculosCreados);

        for (int i = panelTablero.childCount - 1; i >= 0; i--)
        {
            GameObject hijo =
                panelTablero.GetChild(i).gameObject;

            hijo.SetActive(false);
            Destroy(hijo);
        }

        float lado = Mathf.Min(
            panelTablero.rect.width,
            panelTablero.rect.height
        );

        tamanoCasilla =
            (lado - separacionCasillas *
            (tamanoTablero - 1)) / tamanoTablero;

        gridTablero.padding =
            new RectOffset(0, 0, 0, 0);

        gridTablero.spacing = new Vector2(
            separacionCasillas,
            separacionCasillas
        );

        gridTablero.cellSize = new Vector2(
            tamanoCasilla,
            tamanoCasilla
        );

        gridTablero.startCorner =
            GridLayoutGroup.Corner.UpperLeft;

        gridTablero.startAxis =
            GridLayoutGroup.Axis.Horizontal;

        gridTablero.constraint =
            GridLayoutGroup.Constraint.FixedColumnCount;

        gridTablero.constraintCount = tamanoTablero;

        gridTablero.childAlignment =
            TextAnchor.MiddleCenter;

        Sprite sprite = ObtenerSpriteCasilla();

        for (int fila = 0; fila < tamanoTablero; fila++)
        {
            for (int columna = 0;
                 columna < tamanoTablero;
                 columna++)
            {
                GameObject objeto = new GameObject(
                    "Casilla_" + fila + "_" + columna,
                    typeof(RectTransform),
                    typeof(Image)
                );

                objeto.transform.SetParent(
                    panelTablero, false
                );

                Image imagen =
                    objeto.GetComponent<Image>();

                imagen.sprite = sprite;
                imagen.color = Color.white;
                imagen.preserveAspect = true;
                imagen.raycastTarget = false;

                casillasCreadas.Add(objeto);
            }
        }

        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            panelTablero
        );
    }

    private Sprite ObtenerSpriteCasilla()
    {
        if (ejercicioActual == 1)
            return casillaJardin;

        if (ejercicioActual == 2)
            return casillaBosque;

        return casillaTecnologica;
    }

    // =====================================================
    // GENERACION DE TABLEROS SOLUCIONABLES
    // =====================================================

    private void GenerarDistribucion()
    {
        string clave =
            "MundoPujllay_UltimaDistribucion_" +
            ejercicioActual;

        string anterior =
            PlayerPrefs.GetString(clave, "");

        bool encontrada = false;
        string firma = "";

        for (int intento = 0; intento < 1200; intento++)
        {
            obstaculos.Clear();

            inicio = PosicionInicialRobot();
            meta = PosicionAleatoria();

            orientacionInicial =
                ObtenerOrientacionInicial(inicio);

            if (inicio == meta)
                continue;

            if (Distancia(inicio, meta) < DistanciaMinima())
                continue;

            CrearObstaculosAleatorios(numeroObstaculos);

            int instrucciones =
                CalcularMinimoComandos(
                    inicio,
                    orientacionInicial,
                    meta
                );

            if (instrucciones < 0 ||
                instrucciones > maximoComandos)
                continue;

            firma = ObtenerFirma();

            if (firma == anterior)
                continue;

            encontrada = true;
            break;
        }

        if (!encontrada)
        {
            for (int cantidad = numeroObstaculos - 1;
                 cantidad >= 1 && !encontrada;
                 cantidad--)
            {
                for (int intento = 0; intento < 600; intento++)
                {
                    obstaculos.Clear();

                    inicio = PosicionInicialRobot();
                    meta = PosicionAleatoria();

                    orientacionInicial =
                        ObtenerOrientacionInicial(inicio);

                    if (inicio == meta)
                        continue;

                    if (Distancia(inicio, meta) <
                        DistanciaMinima())
                        continue;

                    CrearObstaculosAleatorios(cantidad);

                    int instrucciones =
                        CalcularMinimoComandos(
                            inicio,
                            orientacionInicial,
                            meta
                        );

                    if (instrucciones < 0 ||
                        instrucciones > maximoComandos)
                        continue;

                    firma = ObtenerFirma();

                    if (firma == anterior)
                        continue;

                    encontrada = true;
                    break;
                }
            }
        }

        if (!encontrada)
        {
            CrearDistribucionSegura();
            firma = ObtenerFirma();
        }

        PlayerPrefs.SetString(clave, firma);
        PlayerPrefs.Save();
    }

    private Vector2Int PosicionAleatoria()
    {
        return new Vector2Int(
            UnityEngine.Random.Range(0, tamanoTablero),
            UnityEngine.Random.Range(0, tamanoTablero)
        );
    }

    private Vector2Int PosicionInicialRobot()
    {
        int columna = UnityEngine.Random.Range(
            0,
            tamanoTablero
        );

        bool comenzarArriba =
            UnityEngine.Random.value < 0.5f;

        int fila = comenzarArriba
            ? 0
            : tamanoTablero - 1;

        return new Vector2Int(columna, fila);
    }

    private Direccion ObtenerOrientacionInicial(
        Vector2Int posicion
    )
    {
        // Parte superior: mira hacia abajo (carita).
        if (posicion.y == 0)
            return Direccion.Abajo;

        // Parte inferior: mira hacia arriba (espalda).
        return Direccion.Arriba;
    }

    private int Distancia(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) +
               Mathf.Abs(a.y - b.y);
    }

    private int DistanciaMinima()
    {
        switch (ejercicioActual)
        {
            case 1: return 3;
            case 2: return 4;
            case 3: return 5;
            case 4: return 7;
        }

        return 3;
    }

    private void CrearObstaculosAleatorios(int cantidad)
    {
        int intentos = 0;

        while (obstaculos.Count < cantidad &&
               intentos < 500)
        {
            intentos++;

            Vector2Int posicion = PosicionAleatoria();

            if (posicion == inicio || posicion == meta)
                continue;

            obstaculos.Add(posicion);
        }
    }

    // =====================================================
    // CALCULAR CAMINO CON GIROS
    // =====================================================

    private int CalcularMinimoComandos(
        Vector2Int origen,
        Direccion orientacion,
        Vector2Int destino
    )
    {
        Queue<int> cola = new Queue<int>();

        Dictionary<int, int> distancias =
            new Dictionary<int, int>();

        int inicial =
            CodificarEstado(origen, orientacion);

        cola.Enqueue(inicial);
        distancias[inicial] = 0;

        while (cola.Count > 0)
        {
            int estado = cola.Dequeue();

            DecodificarEstado(
                estado,
                out Vector2Int posicion,
                out Direccion direccion
            );

            int costo = distancias[estado];

            if (posicion == destino)
                return costo;

            RegistrarEstado(
                posicion,
                GirarIzquierda(direccion),
                costo + 1,
                cola,
                distancias
            );

            RegistrarEstado(
                posicion,
                GirarDerecha(direccion),
                costo + 1,
                cola,
                distancias
            );

            Vector2Int frente =
                ObtenerDireccionFrontal(direccion);

            Vector2Int adelante = posicion + frente;

            if (EsCasillaLibre(adelante))
            {
                RegistrarEstado(
                    adelante,
                    direccion,
                    costo + 1,
                    cola,
                    distancias
                );
            }

            Vector2Int atras = posicion - frente;

            if (EsCasillaLibre(atras))
            {
                RegistrarEstado(
                    atras,
                    direccion,
                    costo + 1,
                    cola,
                    distancias
                );
            }
        }

        return -1;
    }

    private int CodificarEstado(
        Vector2Int posicion,
        Direccion direccion
    )
    {
        int indice =
            posicion.y * tamanoTablero + posicion.x;

        return indice * 4 + (int)direccion;
    }

    private void DecodificarEstado(
        int estado,
        out Vector2Int posicion,
        out Direccion direccion
    )
    {
        direccion = (Direccion)(estado % 4);

        int indice = estado / 4;

        posicion = new Vector2Int(
            indice % tamanoTablero,
            indice / tamanoTablero
        );
    }

    private void RegistrarEstado(
        Vector2Int posicion,
        Direccion direccion,
        int costo,
        Queue<int> cola,
        Dictionary<int, int> distancias
    )
    {
        int estado =
            CodificarEstado(posicion, direccion);

        if (distancias.ContainsKey(estado))
            return;

        distancias[estado] = costo;
        cola.Enqueue(estado);
    }

    private bool EsCasillaLibre(Vector2Int posicion)
    {
        return DentroTablero(posicion) &&
               !obstaculos.Contains(posicion);
    }

    private void CrearDistribucionSegura()
    {
        obstaculos.Clear();

        inicio = new Vector2Int(
            0,
            tamanoTablero - 1
        );

        meta = new Vector2Int(
            tamanoTablero - 1,
            0
        );

        orientacionInicial =
            ObtenerOrientacionInicial(inicio);

        // Ruta libre por la columna izquierda
        // y la fila superior.
        for (int y = 1; y < tamanoTablero; y++)
        {
            for (int x = 1; x < tamanoTablero; x++)
            {
                if (obstaculos.Count >= numeroObstaculos)
                    return;

                obstaculos.Add(new Vector2Int(x, y));
            }
        }
    }

    private string ObtenerFirma()
    {
        List<string> lista = new List<string>();

        foreach (Vector2Int posicion in obstaculos)
        {
            lista.Add(
                posicion.x + "," + posicion.y
            );
        }

        lista.Sort(StringComparer.Ordinal);

        return inicio.x + "," + inicio.y + "|" +
               meta.x + "," + meta.y + "|" +
               (int)orientacionInicial + "|" +
               string.Join(";", lista);
    }

    private bool DentroTablero(Vector2Int posicion)
    {
        return posicion.x >= 0 &&
               posicion.y >= 0 &&
               posicion.x < tamanoTablero &&
               posicion.y < tamanoTablero;
    }

    // =====================================================
    // POSICIONES VISUALES
    // =====================================================

    private Vector3 CentroMundo(Vector2Int posicion)
    {
        int indice =
            posicion.y * tamanoTablero +
            posicion.x;

        if (indice < 0 ||
            indice >= casillasCreadas.Count)
        {
            return panelTablero.position;
        }

        RectTransform casilla =
            casillasCreadas[indice]
            .GetComponent<RectTransform>();

        return casilla.TransformPoint(
            casilla.rect.center
        );
    }

    private void ColocarObjeto(
        RectTransform objeto,
        Vector2Int posicion,
        float factor
    )
    {
        if (objeto == null)
            return;

        objeto.position = CentroMundo(posicion);

        objeto.sizeDelta = new Vector2(
            tamanoCasilla * factor,
            tamanoCasilla * factor
        );
    }

    private void ActualizarRobotYMeta()
    {
        ColocarObjeto(
            imagenRobot,
            posicionRobot,
            0.85f
        );

        ColocarObjeto(
            imagenEstrella,
            meta,
            0.75f
        );

        imagenRobot.gameObject.SetActive(true);
        imagenEstrella.gameObject.SetActive(true);
    }

    // =====================================================
    // OBSTACULOS
    // =====================================================

    private void GenerarObstaculos()
    {
        LimpiarLista(obstaculosCreados);

        List<Vector2Int> lista =
            new List<Vector2Int>(obstaculos);

        lista.Sort((a, b) =>
        {
            int comparacion = a.y.CompareTo(b.y);

            return comparacion != 0
                ? comparacion
                : a.x.CompareTo(b.x);
        });

        for (int i = 0; i < lista.Count; i++)
        {
            GameObject objeto = new GameObject(
                "Obstaculo_" + i,
                typeof(RectTransform),
                typeof(Image)
            );

            objeto.transform.SetParent(
                panelObstaculos, false
            );

            RectTransform rect =
                objeto.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            Image imagen =
                objeto.GetComponent<Image>();

            imagen.sprite = SpriteObstaculo(i);
            imagen.preserveAspect = true;
            imagen.raycastTarget = false;

            ColocarObjeto(
                rect,
                lista[i],
                0.8f
            );

            obstaculosCreados.Add(objeto);
        }
    }

    private Sprite SpriteObstaculo(int indice)
    {
        Sprite[] opciones;

        if (ejercicioActual == 2)
        {
            opciones = new Sprite[]
            {
                obstaculoRoca,
                obstaculoTronco,
                obstaculoArbusto
            };
        }
        else
        {
            opciones = new Sprite[]
            {
                obstaculoCaja,
                obstaculoRoca,
                obstaculoTronco,
                obstaculoArbusto
            };
        }

        return opciones[indice % opciones.Length];
    }

    // =====================================================
    // IMAGENES Y GIROS DEL ROBOT
    // =====================================================

    private void CambiarSpriteRobot(Direccion direccion)
    {
        if (imagenRobotUI == null)
            return;

        Sprite nuevo = null;

        switch (direccion)
        {
            case Direccion.Arriba:
                // Mira hacia arriba: se ve la espalda.
                nuevo = robotAbajo;
                break;

            case Direccion.Abajo:
                // Mira hacia abajo: se ve la carita.
                nuevo = robotArriba;
                break;

            case Direccion.Izquierda:
                nuevo = robotIzquierda;
                break;

            case Direccion.Derecha:
                nuevo = robotDerecha;
                break;
        }

        if (nuevo != null)
            imagenRobotUI.sprite = nuevo;

        imagenRobotUI.preserveAspect = true;
    }

    private Direccion GirarDerecha(Direccion actual)
    {
        switch (actual)
        {
            case Direccion.Arriba:
                return Direccion.Derecha;

            case Direccion.Derecha:
                return Direccion.Abajo;

            case Direccion.Abajo:
                return Direccion.Izquierda;

            case Direccion.Izquierda:
                return Direccion.Arriba;
        }

        return actual;
    }

    private Direccion GirarIzquierda(Direccion actual)
    {
        switch (actual)
        {
            case Direccion.Arriba:
                return Direccion.Izquierda;

            case Direccion.Izquierda:
                return Direccion.Abajo;

            case Direccion.Abajo:
                return Direccion.Derecha;

            case Direccion.Derecha:
                return Direccion.Arriba;
        }

        return actual;
    }

    private Vector2Int ObtenerDireccionFrontal(
        Direccion orientacion
    )
    {
        switch (orientacion)
        {
            case Direccion.Arriba:
                return new Vector2Int(0, -1);

            case Direccion.Abajo:
                return new Vector2Int(0, 1);

            case Direccion.Izquierda:
                return new Vector2Int(-1, 0);

            case Direccion.Derecha:
                return new Vector2Int(1, 0);
        }

        return Vector2Int.zero;
    }

    // =====================================================
    // PRESENTACION
    // =====================================================

    private void MostrarPresentacion()
    {
        if (txtEjercicio != null)
        {
            txtEjercicio.text =
                "EJERCICIO " + ejercicioActual;
        }

        if (txtObjetivo != null)
        {
            switch (ejercicioActual)
            {
                case 1:
                    txtObjetivo.text =
                        "¡Ayuda al robot a llegar a la estrella!";
                    break;

                case 2:
                    txtObjetivo.text =
                        "¡Encuentra el camino entre los obstáculos!";
                    break;

                case 3:
                    txtObjetivo.text =
                        "¡Esquiva los obstáculos y llega a la meta!";
                    break;

                case 4:
                    txtObjetivo.text =
                        "¡Supera el gran desafío de programación!";
                    break;
            }
        }

        panelVictoria.SetActive(false);
        panelPresentacion.SetActive(true);
    }

    public void ComenzarJuego()
    {
        panelPresentacion.SetActive(false);

        iniciado = true;

        ActualizarBotones();
    }

    // =====================================================
    // INSTRUCCIONES
    // =====================================================

    private void PrepararContenedorComandos()
    {
        HorizontalLayoutGroup layout =
            contenedorComandos
            .GetComponent<HorizontalLayoutGroup>();

        if (layout == null)
        {
            layout = contenedorComandos.gameObject
                .AddComponent<HorizontalLayoutGroup>();
        }

        layout.spacing = 5f;
        layout.childAlignment = TextAnchor.MiddleCenter;

        layout.childControlWidth = false;
        layout.childControlHeight = false;

        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
    }

    private bool PuedeProgramar()
    {
        return iniciado &&
               !ejecutando &&
               !animandoEstrella &&
               !ganado &&
               !panelPresentacion.activeSelf &&
               !panelVictoria.activeSelf;
    }

    private void AgregarComando(Direccion direccion)
    {
        if (!PuedeProgramar())
            return;

        if (comandos.Count >= maximoComandos)
        {
            MostrarMensaje(
                "¡Llegaste al máximo de instrucciones!"
            );
            return;
        }

        comandos.Add(direccion);

        ActualizarComandosVisuales();
        ActualizarBotones();

        MostrarMensaje("");
    }

    private void ActualizarComandosVisuales()
    {
        LimpiarLista(comandosCreados);

        float anchoContenedor =
            contenedorComandos.rect.width;

        float espacio = 5f;
        int cantidad = comandos.Count;

        float anchoTarjeta = 70f;

        if (cantidad > 0)
        {
            anchoTarjeta = Mathf.Min(
                70f,
                (anchoContenedor -
                 espacio * (cantidad - 1)) /
                cantidad
            );
        }

        anchoTarjeta = Mathf.Max(
            15f,
            anchoTarjeta
        );

        for (int i = 0; i < comandos.Count; i++)
        {
            GameObject tarjeta = new GameObject(
                "Comando_" + (i + 1),
                typeof(RectTransform),
                typeof(Image),
                typeof(LayoutElement)
            );

            tarjeta.transform.SetParent(
                contenedorComandos, false
            );

            RectTransform rect =
                tarjeta.GetComponent<RectTransform>();

            rect.sizeDelta = new Vector2(
                anchoTarjeta,
                Mathf.Min(76f, anchoTarjeta)
            );

            LayoutElement elemento =
                tarjeta.GetComponent<LayoutElement>();

            elemento.preferredWidth = anchoTarjeta;
            elemento.preferredHeight =
                Mathf.Min(76f, anchoTarjeta);

            Image fondo =
                tarjeta.GetComponent<Image>();

            fondo.sprite = tarjetaComando;
            fondo.color = Color.white;
            fondo.preserveAspect = true;
            fondo.raycastTarget = false;

            GameObject flecha = new GameObject(
                "Image_Flecha",
                typeof(RectTransform),
                typeof(Image)
            );

            flecha.transform.SetParent(
                tarjeta.transform, false
            );

            RectTransform rectFlecha =
                flecha.GetComponent<RectTransform>();

            rectFlecha.anchorMin = Vector2.zero;
            rectFlecha.anchorMax = Vector2.one;

            rectFlecha.offsetMin =
                new Vector2(2f, 2f);

            rectFlecha.offsetMax =
                new Vector2(-2f, -2f);

            Image imagen =
                flecha.GetComponent<Image>();

            imagen.sprite =
                SpriteDireccion(comandos[i]);

            imagen.preserveAspect = true;
            imagen.raycastTarget = false;

            comandosCreados.Add(tarjeta);
        }

        if (txtTituloInstrucciones != null)
        {
            txtTituloInstrucciones.text =
                "MIS INSTRUCCIONES (" +
                comandos.Count + "/" +
                maximoComandos + ")";
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            contenedorComandos
        );
    }

    private Sprite SpriteDireccion(Direccion direccion)
    {
        switch (direccion)
        {
            case Direccion.Arriba:
                return spriteArriba;

            case Direccion.Abajo:
                return spriteAbajo;

            case Direccion.Izquierda:
                return spriteIzquierda;

            case Direccion.Derecha:
                return spriteDerecha;
        }

        return null;
    }

    public void BorrarComando()
    {
        if (!PuedeProgramar())
            return;

        if (comandos.Count == 0)
            return;

        comandos.RemoveAt(comandos.Count - 1);

        ActualizarComandosVisuales();
        ActualizarBotones();
    }

    // =====================================================
    // EJECUTAR PROGRAMA
    // =====================================================

    public void EjecutarPrograma()
    {
        if (!PuedeProgramar())
            return;

        if (comandos.Count == 0)
        {
            MostrarMensaje(
                "¡Selecciona primero tus instrucciones!"
            );
            return;
        }

        DetenerMovimiento();

        posicionRobot = inicio;
        orientacionRobot = orientacionInicial;

        CambiarSpriteRobot(orientacionRobot);
        ActualizarRobotYMeta();

        ejecutando = true;
        pausado = false;

        ActualizarBotones();
        MostrarMensaje("");

        movimiento = StartCoroutine(
            RecorrerInstrucciones()
        );
    }

    private IEnumerator RecorrerInstrucciones()
    {
        for (int i = 0; i < comandos.Count; i++)
        {
            while (pausado)
                yield return null;

            Direccion comando = comandos[i];

            // IZQUIERDA = GIRAR 90°
            if (comando == Direccion.Izquierda)
            {
                orientacionRobot =
                    GirarIzquierda(orientacionRobot);

                CambiarSpriteRobot(orientacionRobot);

                yield return EsperarConPausa(duracionGiro);
                continue;
            }

            // DERECHA = GIRAR 90°
            if (comando == Direccion.Derecha)
            {
                orientacionRobot =
                    GirarDerecha(orientacionRobot);

                CambiarSpriteRobot(orientacionRobot);

                yield return EsperarConPausa(duracionGiro);
                continue;
            }

            // ARRIBA = AVANZAR
            // ABAJO = RETROCEDER
            Vector2Int desplazamiento =
                ObtenerDireccionFrontal(orientacionRobot);

            if (comando == Direccion.Abajo)
                desplazamiento = -desplazamiento;

            Vector2Int siguiente =
                posicionRobot + desplazamiento;

            if (!DentroTablero(siguiente))
            {
                FinalizarIntento(
                    "¡Cuidado! El robot no puede salir del tablero."
                );

                yield break;
            }

            if (obstaculos.Contains(siguiente))
            {
                FinalizarIntento(
                    "¡Cuidado! Hay un obstáculo en el camino."
                );

                yield break;
            }

            Vector3 origenMundo =
                CentroMundo(posicionRobot);

            Vector3 destinoMundo =
                CentroMundo(siguiente);

            float tiempo = 0f;

            while (tiempo < duracionPorCasilla)
            {
                if (pausado)
                {
                    yield return null;
                    continue;
                }

                tiempo += Time.deltaTime;

                float t = Mathf.Clamp01(
                    tiempo / duracionPorCasilla
                );

                t = Mathf.SmoothStep(0f, 1f, t);

                imagenRobot.position = Vector3.Lerp(
                    origenMundo,
                    destinoMundo,
                    t
                );

                yield return null;
            }

            posicionRobot = siguiente;

            imagenRobot.position =
                CentroMundo(posicionRobot);

            if (posicionRobot == meta)
            {
                movimiento = null;

                // NUEVO:
                // La estrella se anima antes
                // de mostrar el panel de victoria.
                animacionEstrella = StartCoroutine(
                    AnimarEstrellaVictoria()
                );

                yield break;
            }
        }

        movimiento = null;
        ejecutando = false;
        pausado = false;

        MostrarMensaje(
            "¡Todavía falta llegar a la estrella! " +
            "Corrige tus instrucciones e inténtalo otra vez."
        );

        ActualizarBotones();
    }

    private IEnumerator EsperarConPausa(float duracion)
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            if (!pausado)
                tiempo += Time.deltaTime;

            yield return null;
        }
    }

    private void FinalizarIntento(string mensaje)
    {
        movimiento = null;
        ejecutando = false;
        pausado = false;

        MostrarMensaje(mensaje);
        ActualizarBotones();
    }

    // =====================================================
    // ANIMACION DE LA ESTRELLA
    // =====================================================

    private IEnumerator AnimarEstrellaVictoria()
    {
        animandoEstrella = true;
        ejecutando = false;
        pausado = false;

        ActualizarBotones();

        // Asegurarnos de que la estrella
        // empieza visible y con su escala normal.
        RestaurarEstrella();

        float duracion = Mathf.Max(
            0.1f,
            duracionAnimacionEstrella
        );

        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            float t = Mathf.Clamp01(
                tiempo / duracion
            );

            float escala;

            // Primera parte:
            // la estrella crece suavemente.
            if (t < 0.45f)
            {
                float progreso = t / 0.45f;

                progreso = Mathf.SmoothStep(
                    0f, 1f, progreso
                );

                escala = Mathf.Lerp(
                    1f,
                    escalaMaximaEstrella,
                    progreso
                );
            }
            else
            {
                // Segunda parte:
                // la estrella se encoge
                // hasta desaparecer.
                float progreso =
                    (t - 0.45f) / 0.55f;

                progreso = Mathf.SmoothStep(
                    0f, 1f, progreso
                );

                escala = Mathf.Lerp(
                    escalaMaximaEstrella,
                    0f,
                    progreso
                );
            }

            imagenEstrella.localScale =
                escalaOriginalEstrella * escala;

            // Brillo suave durante el zoom.
            Color color = colorOriginalEstrella;

            if (t < 0.45f)
            {
                float brillo = Mathf.Sin(
                    (t / 0.45f) * Mathf.PI
                );

                color = Color.Lerp(
                    colorOriginalEstrella,
                    Color.white,
                    brillo * 0.45f
                );

                color.a = colorOriginalEstrella.a;
            }
            else
            {
                // Desaparición gradual.
                float progreso =
                    (t - 0.45f) / 0.55f;

                color.a = Mathf.Lerp(
                    colorOriginalEstrella.a,
                    0f,
                    progreso
                );
            }

            imagenEstrellaUI.color = color;

            yield return null;
        }

        imagenEstrella.gameObject.SetActive(false);

        // Dejamos preparados sus valores originales
        // para el siguiente ejercicio.
        imagenEstrella.localScale =
            escalaOriginalEstrella;

        imagenEstrellaUI.color =
            colorOriginalEstrella;

        animandoEstrella = false;
        animacionEstrella = null;

        GanarEjercicio();
    }

    private void RestaurarEstrella()
    {
        if (imagenEstrella == null)
            return;

        imagenEstrella.localScale =
            escalaOriginalEstrella;

        if (imagenEstrellaUI != null)
        {
            imagenEstrellaUI.color =
                colorOriginalEstrella;
        }

        imagenEstrella.gameObject.SetActive(true);
    }

    private void DetenerAnimacionEstrella()
    {
        if (animacionEstrella != null)
        {
            StopCoroutine(animacionEstrella);
            animacionEstrella = null;
        }

        animandoEstrella = false;

        RestaurarEstrella();
    }

    // =====================================================
    // PAUSA
    // =====================================================

    public void AlternarPausa()
    {
        if (!ejecutando || ganado || animandoEstrella)
        {
            MostrarMensaje(
                "La pausa funciona mientras el robot está ejecutando."
            );
            return;
        }

        pausado = !pausado;

        MostrarMensaje(
            pausado ? "PAUSA" : ""
        );

        ActualizarBotones();
    }

    private void DetenerMovimiento()
    {
        if (movimiento != null)
        {
            StopCoroutine(movimiento);
            movimiento = null;
        }

        ejecutando = false;
        pausado = false;
    }

    // =====================================================
    // VICTORIA
    // =====================================================

    private void GanarEjercicio()
    {
        DetenerMovimiento();

        ganado = true;

        int anterior = PlayerPrefs.GetInt(
            CLAVE_ESTRELLAS, 0
        );

        PlayerPrefs.SetInt(
            CLAVE_ESTRELLAS,
            Mathf.Max(anterior, ejercicioActual)
        );

        PlayerPrefs.Save();

        if (txtExcelente != null)
            txtExcelente.text = "¡Excelente!";

        if (txtLlegasteEstrella != null)
            txtLlegasteEstrella.text =
                "¡Llegaste a la estrella!";

        ActualizarProgreso();

        panelVictoria.SetActive(true);

        ActualizarBotones();
    }

    public void SiguienteEjercicio()
    {
        if (!ganado)
            return;

        if (ejercicioActual < TOTAL_EJERCICIOS)
        {
            IniciarEjercicio(ejercicioActual + 1);
            return;
        }

        if (!string.IsNullOrWhiteSpace(escenaAlTerminar))
        {
            SceneManager.LoadScene(escenaAlTerminar);
        }
        else
        {
            IniciarEjercicio(1);
        }
    }

    // =====================================================
    // PROGRESO AUTOMATICO
    // =====================================================

    private void PrepararProgreso()
    {
        circulos.Clear();
        numeros.Clear();

        for (int i = panelProgreso.childCount - 1; i >= 0; i--)
        {
            GameObject hijo =
                panelProgreso.GetChild(i).gameObject;

            hijo.SetActive(false);
            Destroy(hijo);
        }

        HorizontalLayoutGroup layout =
            panelProgreso.GetComponent<HorizontalLayoutGroup>();

        if (layout == null)
        {
            layout = panelProgreso.gameObject
                .AddComponent<HorizontalLayoutGroup>();
        }

        layout.spacing = 18f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        for (int i = 0; i < TOTAL_EJERCICIOS; i++)
        {
            GameObject objeto = new GameObject(
                "Circulo_" + (i + 1),
                typeof(RectTransform),
                typeof(Image),
                typeof(LayoutElement)
            );

            objeto.transform.SetParent(
                panelProgreso, false
            );

            LayoutElement elemento =
                objeto.GetComponent<LayoutElement>();

            elemento.preferredWidth = 65f;
            elemento.preferredHeight = 65f;

            Image imagen =
                objeto.GetComponent<Image>();

            imagen.sprite = ObtenerSpriteCircular();
            imagen.raycastTarget = false;

            GameObject texto = new GameObject(
                "Txt_Numero",
                typeof(RectTransform),
                typeof(TextMeshProUGUI)
            );

            texto.transform.SetParent(
                objeto.transform, false
            );

            RectTransform rect =
                texto.GetComponent<RectTransform>();

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            TextMeshProUGUI numero =
                texto.GetComponent<TextMeshProUGUI>();

            numero.text = (i + 1).ToString();
            numero.fontSize = 36f;
            numero.fontStyle = FontStyles.Bold;
            numero.alignment =
                TextAlignmentOptions.Center;

            numero.raycastTarget = false;

            circulos.Add(imagen);
            numeros.Add(numero);
        }
    }

    private Sprite ObtenerSpriteCircular()
    {
        if (spriteCirculo != null)
            return spriteCirculo;

        const int resolucion = 64;

        Texture2D textura = new Texture2D(
            resolucion,
            resolucion,
            TextureFormat.RGBA32,
            false
        );

        textura.filterMode = FilterMode.Bilinear;

        Vector2 centro = new Vector2(
            (resolucion - 1) / 2f,
            (resolucion - 1) / 2f
        );

        float radio = resolucion * 0.47f;

        for (int y = 0; y < resolucion; y++)
        {
            for (int x = 0; x < resolucion; x++)
            {
                float distancia = Vector2.Distance(
                    new Vector2(x, y), centro
                );

                float alpha = Mathf.Clamp01(
                    radio - distancia + 0.5f
                );

                textura.SetPixel(
                    x, y,
                    new Color(1f, 1f, 1f, alpha)
                );
            }
        }

        textura.Apply();

        spriteCirculo = Sprite.Create(
            textura,
            new Rect(0, 0, resolucion, resolucion),
            new Vector2(0.5f, 0.5f),
            100f
        );

        return spriteCirculo;
    }


    private void ActualizarProgreso()
    {
        for (int i = 0; i < circulos.Count; i++)
        {
            int numero = i + 1;

            if (numero <= ejercicioActual)
            {
                // Amarillo acumulativo:
                // ejercicio actual y anteriores.
                circulos[i].color =
                    new Color(1f, 0.78f, 0.2f);

                numeros[i].color =
                    new Color(0.3f, 0.2f, 0.1f);
            }
            else
            {
                // Gris: ejercicios que faltan.
                circulos[i].color =
                    new Color(0.7f, 0.75f, 0.82f);

                numeros[i].color = Color.white;
            }
        }
    }


    // =====================================================
    // BOTONES Y MENSAJES
    // =====================================================

    private void ActualizarBotones()
    {
        bool puede = PuedeProgramar();

        if (btnArriba != null)
            btnArriba.interactable = puede;

        if (btnAbajo != null)
            btnAbajo.interactable = puede;

        if (btnIzquierda != null)
            btnIzquierda.interactable = puede;

        if (btnDerecha != null)
            btnDerecha.interactable = puede;

        if (btnGO != null)
        {
            btnGO.interactable =
                puede && comandos.Count > 0;
        }

        if (btnBorrar != null)
        {
            btnBorrar.interactable =
                puede && comandos.Count > 0;
        }

        // El botón Pausa siempre permanece visible
        // y no se desactiva visualmente.
        if (btnPausa != null)
            btnPausa.interactable = true;
    }

    private void MostrarMensaje(string mensaje)
    {
        if (txtMensaje != null)
            txtMensaje.text = mensaje;

        if (!string.IsNullOrEmpty(mensaje))
        {
            Debug.Log(
                "[Programa el camino] " + mensaje
            );
        }
    }

    private void LimpiarLista(List<GameObject> lista)
    {
        foreach (GameObject objeto in lista)
        {
            if (objeto != null)
            {
                objeto.SetActive(false);
                Destroy(objeto);
            }
        }

        lista.Clear();
    }

    // =====================================================
    // FUNCIONES PUBLICAS
    // =====================================================

    public void VolverAJugarEjercicio()
    {
        IniciarEjercicio(ejercicioActual);
    }

    public void VolverAlInicioDeMision()
    {
        IniciarEjercicio(1);
    }

    public int ObtenerEjercicioActual()
    {
        return ejercicioActual;
    }

    public int ObtenerEstrellasGuardadas()
    {
        return Mathf.Clamp(
            PlayerPrefs.GetInt(CLAVE_ESTRELLAS, 0),
            0,
            TOTAL_EJERCICIOS
        );
    }

    private void OnDisable()
    {
        DetenerMovimiento();

        if (animacionEstrella != null)
        {
            StopCoroutine(animacionEstrella);
            animacionEstrella = null;
        }
    }
}
