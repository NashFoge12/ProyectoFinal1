
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JuegoTransportaEstrellas : MonoBehaviour
{
    [Serializable]
    public class DisenoEjercicio
    {
        public Sprite fondo;
        public Sprite casilla;
        public Sprite[] obstaculos;
        public Sprite estrella;
        public Sprite puntoEntrega;
    }

    [Header("DISEÑO EJERCICIO 1")]
    public DisenoEjercicio disenoEjercicio1 = new DisenoEjercicio();

    [Header("DISEÑO EJERCICIO 2")]
    public DisenoEjercicio disenoEjercicio2 = new DisenoEjercicio();

    [Header("DISEÑO EJERCICIO 3")]
    public DisenoEjercicio disenoEjercicio3 = new DisenoEjercicio();

    [Header("DISEÑO EJERCICIO 4")]
    public DisenoEjercicio disenoEjercicio4 = new DisenoEjercicio();

    [Header("TABLERO")]
    public RectTransform panelTablero;
    public RectTransform panelObstaculos;
    public GridLayoutGroup gridTablero;
    public Image imagenFondo;

    [Header("ROBOT")]
    public RectTransform imagenRobot;

    public Sprite robotArriba;
    public Sprite robotAbajo;
    public Sprite robotIzquierda;
    public Sprite robotDerecha;

    [Header("ESTRELLAS")]
    public RectTransform panelEstrellas;
    public RectTransform imagenPuntoEntrega;
    public RectTransform imagenEstrellaCargada;

    [Header("PROGRAMACION")]
    public TMP_Text txtTituloInstrucciones;
    public RectTransform contenedorComandos;

    public Sprite tarjetaComando;
    public Sprite spriteArriba;
    public Sprite spriteAbajo;
    public Sprite spriteIzquierda;
    public Sprite spriteDerecha;

    [Header("BOTONES")]
    public Button btnArriba;
    public Button btnAbajo;
    public Button btnIzquierda;
    public Button btnDerecha;
    public Button btnGO;
    public Button btnBorrar;
    public Button btnPausa;

    [Header("PRESENTACION")]
    public GameObject panelPresentacion;
    public TMP_Text txtEjercicio;
    public TMP_Text txtObjetivo;
    public Button btnIr;

    [Header("VICTORIA")]
    public GameObject panelVictoria;
    public TMP_Text txtExcelente;
    public TMP_Text txtLlegasteEstrella;
    public Button btnSiguiente;

    [Header("PROGRESO")]
    public RectTransform panelProgreso;

    [Header("MENSAJES")]
    public TMP_Text txtMensaje;

    [Header("CONFIGURACION")]
    public float duracionPorCasilla = 0.45f;
    public float duracionGiro = 0.25f;
    public float separacionCasillas = 8f;
    public int maximoComandos = 30;

    [Header("TARJETAS DE PROGRAMACION")]
    public float anchoTarjeta = 45f;
    public float altoTarjeta = 45f;
    public float separacionTarjetas = 5f;

    [Header("BORRADO DE INSTRUCCIONES")]
    [Range(0.15f, 0.6f)]
    public float intervaloDobleClick = 0.32f;

    private float ultimoClickBorrar = -100f;

    [Header("ANIMACION ESTRELLAS")]
    public float duracionAnimacionEstrella = 0.7f;
    public float escalaMaximaEstrella = 1.45f;

    [Header("NAVEGACION")]
    public string escenaAlTerminar = "";

    private const int TOTAL_EJERCICIOS = 4;

    private enum Direccion
    {
        Arriba = 0,
        Derecha = 1,
        Abajo = 2,
        Izquierda = 3
    }

    private readonly List<Direccion> comandos =
        new List<Direccion>();

    private readonly List<GameObject> casillas =
        new List<GameObject>();

    private readonly List<GameObject> objetosObstaculos =
        new List<GameObject>();

    private readonly List<GameObject> objetosEstrellas =
        new List<GameObject>();

    private readonly List<GameObject> objetosComandos =
        new List<GameObject>();

    private readonly List<RectTransform> estrellasSuelo =
        new List<RectTransform>();

    private readonly List<RectTransform> estrellasEntregadas =
        new List<RectTransform>();

    private readonly List<Vector2Int> posicionesEstrellas =
        new List<Vector2Int>();

    private readonly HashSet<Vector2Int> obstaculos =
        new HashSet<Vector2Int>();

    private readonly List<Image> circulosProgreso =
        new List<Image>();

    private readonly List<TMP_Text> numerosProgreso =
        new List<TMP_Text>();

    private int ejercicioActual = 1;
    private int tamanoTablero = 4;
    private int cantidadObstaculos;
    private int cantidadEstrellas = 1;

    private int estrellasDepositadas;
    private int estrellaCargada = -1;

    private Vector2Int posicionInicial;
    private Vector2Int posicionRobot;
    private Vector2Int puntoEntrega;

    private Direccion orientacionInicial;
    private Direccion orientacionRobot;

    private float tamanoCasilla;

    private bool juegoIniciado;
    private bool ejecutando;
    private bool pausado;
    private bool victoria;

    private Coroutine rutinaPrograma;

    private Image componenteRobot;
    private Image componenteEntrega;
    private Image componenteEstrellaCargada;

    private DisenoEjercicio disenoActual;

    private Sprite spriteCirculo;

    // =====================================================
    // INICIALIZACION
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
            componenteRobot =
                imagenRobot.GetComponent<Image>();
        }

        if (imagenPuntoEntrega != null)
        {
            componenteEntrega =
                imagenPuntoEntrega.GetComponent<Image>();
        }

        if (imagenEstrellaCargada != null)
        {
            componenteEstrellaCargada =
                imagenEstrellaCargada.GetComponent<Image>();
        }

        ConectarBotones();
    }

    private void Start()
    {
        if (!ValidarReferencias())
        {
            Debug.LogError(
                "MISION 2: Faltan referencias en el Inspector."
            );

            enabled = false;
            return;
        }

        ConfigurarListaComandos();
        CrearProgreso();

        CargarEjercicio(1);
    }

    private bool ValidarReferencias()
    {
        return panelTablero != null &&
               panelObstaculos != null &&
               gridTablero != null &&
               imagenRobot != null &&
               componenteRobot != null &&
               panelEstrellas != null &&
               imagenPuntoEntrega != null &&
               componenteEntrega != null &&
               imagenEstrellaCargada != null &&
               componenteEstrellaCargada != null &&
               contenedorComandos != null &&
               panelPresentacion != null &&
               panelVictoria != null &&
               panelProgreso != null;
    }

    private void ConectarBotones()
    {
        Registrar(btnArriba, () =>
            AgregarComando(Direccion.Arriba));

        Registrar(btnAbajo, () =>
            AgregarComando(Direccion.Abajo));

        Registrar(btnIzquierda, () =>
            AgregarComando(Direccion.Izquierda));

        Registrar(btnDerecha, () =>
            AgregarComando(Direccion.Derecha));

        Registrar(btnGO, EjecutarPrograma);
        Registrar(btnBorrar, BorrarComando);
        Registrar(btnPausa, AlternarPausa);
        Registrar(btnIr, ComenzarEjercicio);
        Registrar(btnSiguiente, SiguienteEjercicio);
    }

    private void Registrar(
        Button boton,
        UnityEngine.Events.UnityAction accion)
    {
        if (boton == null)
            return;

        boton.onClick.RemoveAllListeners();
        boton.onClick.AddListener(accion);
    }

    // =====================================================
    // EJERCICIOS
    // =====================================================

    private void CargarEjercicio(int numero)
    {
        DetenerPrograma();

        ejercicioActual = Mathf.Clamp(
            numero, 1, TOTAL_EJERCICIOS
        );

        juegoIniciado = false;
        ejecutando = false;
        pausado = false;
        victoria = false;

        comandos.Clear();
        ultimoClickBorrar = -100f;
        LimpiarObjetos(objetosComandos);

        switch (ejercicioActual)
        {
            case 1:
                tamanoTablero = 4;
                cantidadObstaculos = 0;
                cantidadEstrellas = 1;
                disenoActual = disenoEjercicio1;
                break;

            case 2:
                tamanoTablero = 5;
                cantidadObstaculos = 4;
                cantidadEstrellas = 1;
                disenoActual = disenoEjercicio2;
                break;

            case 3:
                tamanoTablero = 5;
                cantidadObstaculos = 6;
                cantidadEstrellas = 2;
                disenoActual = disenoEjercicio3;
                break;

            default:
                tamanoTablero = 6;
                cantidadObstaculos = 8;
                cantidadEstrellas = 3;
                disenoActual = disenoEjercicio4;
                break;
        }

        AplicarDiseno();
        CrearTablero();
        GenerarDistribucion();
        CrearObstaculos();
        CrearEstrellas();

        ReiniciarPosiciones();

        ActualizarListaComandos();
        ActualizarProgreso();

        MostrarPresentacion();
        MostrarMensaje("");

        ActualizarBotones();
    }

    private void AplicarDiseno()
    {
        if (imagenFondo != null &&
            disenoActual.fondo != null)
        {
            imagenFondo.sprite = disenoActual.fondo;
        }

        if (disenoActual.puntoEntrega != null)
        {
            componenteEntrega.sprite =
                disenoActual.puntoEntrega;
        }

        componenteEntrega.preserveAspect = true;

        if (disenoActual.estrella != null)
        {
            componenteEstrellaCargada.sprite =
                disenoActual.estrella;
        }

        componenteEstrellaCargada.preserveAspect = true;
    }

    // =====================================================
    // TABLERO
    // =====================================================

    private void CrearTablero()
    {
        LimpiarObjetos(casillas);
        LimpiarObjetos(objetosObstaculos);
        LimpiarObjetos(objetosEstrellas);

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
            (lado - separacionCasillas * (tamanoTablero - 1))
            / tamanoTablero;

        gridTablero.padding =
            new RectOffset(0, 0, 0, 0);

        gridTablero.spacing =
            new Vector2(
                separacionCasillas,
                separacionCasillas
            );

        gridTablero.cellSize =
            Vector2.one * tamanoCasilla;

        gridTablero.startCorner =
            GridLayoutGroup.Corner.UpperLeft;

        gridTablero.startAxis =
            GridLayoutGroup.Axis.Horizontal;

        gridTablero.constraint =
            GridLayoutGroup.Constraint.FixedColumnCount;

        gridTablero.constraintCount = tamanoTablero;

        gridTablero.childAlignment =
            TextAnchor.MiddleCenter;

        for (int y = 0; y < tamanoTablero; y++)
        {
            for (int x = 0; x < tamanoTablero; x++)
            {
                GameObject objeto = CrearImagen(
                    "Casilla_" + y + "_" + x,
                    panelTablero,
                    disenoActual.casilla
                );

                casillas.Add(objeto);
            }
        }

        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            panelTablero
        );
    }

    private Vector3 CentroMundo(Vector2Int posicion)
    {
        int indice =
            posicion.y * tamanoTablero + posicion.x;

        if (indice < 0 || indice >= casillas.Count)
            return panelTablero.position;

        RectTransform rect =
            casillas[indice].GetComponent<RectTransform>();

        return rect.TransformPoint(rect.rect.center);
    }

    private void ColocarEnCasilla(
        RectTransform objeto,
        Vector2Int posicion,
        float factor)
    {
        if (objeto == null)
            return;

        objeto.position = CentroMundo(posicion);

        objeto.sizeDelta =
            Vector2.one * tamanoCasilla * factor;
    }

    private GameObject CrearImagen(
        string nombre,
        Transform padre,
        Sprite sprite)
    {
        GameObject objeto = new GameObject(
            nombre,
            typeof(RectTransform),
            typeof(Image)
        );

        objeto.transform.SetParent(padre, false);

        RectTransform rect =
            objeto.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        Image imagen = objeto.GetComponent<Image>();

        imagen.sprite = sprite;
        imagen.color = Color.white;
        imagen.preserveAspect = true;
        imagen.raycastTarget = false;

        return objeto;
    }

    // =====================================================
    // GENERACION DE POSICIONES
    // =====================================================

    private Vector2Int Aleatorio()
    {
        return new Vector2Int(
            UnityEngine.Random.Range(0, tamanoTablero),
            UnityEngine.Random.Range(0, tamanoTablero)
        );
    }

    private bool DentroDelTablero(Vector2Int posicion)
    {
        return posicion.x >= 0 &&
               posicion.y >= 0 &&
               posicion.x < tamanoTablero &&
               posicion.y < tamanoTablero;
    }

    private bool Libre(Vector2Int posicion)
    {
        return DentroDelTablero(posicion) &&
               !obstaculos.Contains(posicion);
    }

    private void GenerarDistribucion()
    {
        bool encontrada = false;

        for (int intento = 0; intento < 1000; intento++)
        {
            obstaculos.Clear();
            posicionesEstrellas.Clear();

            int columna =
                UnityEngine.Random.Range(0, tamanoTablero);

            bool desdeArriba =
                UnityEngine.Random.value < 0.5f;

            posicionInicial = new Vector2Int(
                columna,
                desdeArriba ? 0 : tamanoTablero - 1
            );

            orientacionInicial =
                desdeArriba
                    ? Direccion.Abajo
                    : Direccion.Arriba;

            puntoEntrega = Aleatorio();

            if (puntoEntrega == posicionInicial)
                continue;

            int seguridad = 0;

            while (obstaculos.Count < cantidadObstaculos &&
                   seguridad < 500)
            {
                seguridad++;

                Vector2Int posicion = Aleatorio();

                if (posicion == posicionInicial ||
                    posicion == puntoEntrega)
                    continue;

                obstaculos.Add(posicion);
            }

            if (obstaculos.Count != cantidadObstaculos)
                continue;

            seguridad = 0;

            while (posicionesEstrellas.Count < cantidadEstrellas &&
                   seguridad < 500)
            {
                seguridad++;

                Vector2Int posicion = Aleatorio();

                if (posicion == posicionInicial ||
                    posicion == puntoEntrega ||
                    obstaculos.Contains(posicion) ||
                    posicionesEstrellas.Contains(posicion))
                {
                    continue;
                }

                posicionesEstrellas.Add(posicion);
            }

            if (posicionesEstrellas.Count != cantidadEstrellas)
                continue;

            int minimo = CalcularComandosMinimos();

            if (minimo >= 0 && minimo <= maximoComandos)
            {
                encontrada = true;
                break;
            }
        }

        if (!encontrada)
        {
            GenerarDistribucionSegura();
        }
    }

    private struct EstadoRuta
    {
        public Vector2Int posicion;
        public Direccion orientacion;
        public int entregadas;
        public int cargando;
        public int pasos;
    }

    private int CalcularComandosMinimos()
    {
        Queue<EstadoRuta> cola =
            new Queue<EstadoRuta>();

        HashSet<string> visitados =
            new HashSet<string>();

        cola.Enqueue(new EstadoRuta
        {
            posicion = posicionInicial,
            orientacion = orientacionInicial,
            entregadas = 0,
            cargando = -1,
            pasos = 0
        });

        int mascaraFinal =
            (1 << cantidadEstrellas) - 1;

        while (cola.Count > 0)
        {
            EstadoRuta actual = cola.Dequeue();

            if (actual.cargando == -1)
            {
                for (int i = 0; i < posicionesEstrellas.Count; i++)
                {
                    bool entregada =
                        (actual.entregadas & (1 << i)) != 0;

                    if (!entregada &&
                        actual.posicion == posicionesEstrellas[i])
                    {
                        actual.cargando = i;
                        break;
                    }
                }
            }

            if (actual.cargando >= 0 &&
                actual.posicion == puntoEntrega)
            {
                actual.entregadas |=
                    1 << actual.cargando;

                actual.cargando = -1;
            }

            if (actual.entregadas == mascaraFinal &&
                actual.cargando == -1)
            {
                return actual.pasos;
            }

            if (actual.pasos >= maximoComandos)
                continue;

            string clave =
                actual.posicion.x + "_" +
                actual.posicion.y + "_" +
                (int)actual.orientacion + "_" +
                actual.entregadas + "_" +
                actual.cargando;

            if (!visitados.Add(clave))
                continue;

            EstadoRuta izquierda = actual;

            izquierda.orientacion =
                GirarIzquierda(actual.orientacion);

            izquierda.pasos++;

            cola.Enqueue(izquierda);

            EstadoRuta derecha = actual;

            derecha.orientacion =
                GirarDerecha(actual.orientacion);

            derecha.pasos++;

            cola.Enqueue(derecha);

            Vector2Int frente =
                DireccionFrontal(actual.orientacion);

            Vector2Int siguiente =
                actual.posicion + frente;

            if (Libre(siguiente))
            {
                EstadoRuta avanzar = actual;

                avanzar.posicion = siguiente;
                avanzar.pasos++;

                cola.Enqueue(avanzar);
            }

            siguiente = actual.posicion - frente;

            if (Libre(siguiente))
            {
                EstadoRuta retroceder = actual;

                retroceder.posicion = siguiente;
                retroceder.pasos++;

                cola.Enqueue(retroceder);
            }
        }

        return -1;
    }

    private void GenerarDistribucionSegura()
    {
        obstaculos.Clear();
        posicionesEstrellas.Clear();

        posicionInicial =
            new Vector2Int(0, tamanoTablero - 1);

        orientacionInicial = Direccion.Arriba;

        puntoEntrega =
            new Vector2Int(tamanoTablero - 1, 0);

        for (int i = 0; i < cantidadEstrellas; i++)
        {
            posicionesEstrellas.Add(
                new Vector2Int(i + 1, tamanoTablero - 1)
            );
        }

        List<Vector2Int> candidatos =
            new List<Vector2Int>();

        for (int y = 1; y < tamanoTablero - 1; y++)
        {
            for (int x = 1; x < tamanoTablero - 1; x++)
            {
                candidatos.Add(new Vector2Int(x, y));
            }
        }

        foreach (Vector2Int candidato in candidatos)
        {
            if (obstaculos.Count >= cantidadObstaculos)
                break;

            obstaculos.Add(candidato);

            int minimo = CalcularComandosMinimos();

            if (minimo < 0 || minimo > maximoComandos)
                obstaculos.Remove(candidato);
        }
    }

    // =====================================================
    // OBSTACULOS Y ESTRELLAS
    // =====================================================

    private void CrearObstaculos()
    {
        LimpiarObjetos(objetosObstaculos);

        foreach (Vector2Int posicion in obstaculos)
        {
            GameObject objeto = CrearImagen(
                "Obstaculo",
                panelObstaculos,
                ObtenerObstaculoAleatorio()
            );

            ColocarEnCasilla(
                objeto.GetComponent<RectTransform>(),
                posicion,
                0.80f
            );

            objetosObstaculos.Add(objeto);
        }
    }

    private Sprite ObtenerObstaculoAleatorio()
    {
        if (disenoActual.obstaculos == null ||
            disenoActual.obstaculos.Length == 0)
        {
            return null;
        }

        List<Sprite> disponibles =
            new List<Sprite>();

        foreach (Sprite sprite in disenoActual.obstaculos)
        {
            if (sprite != null)
                disponibles.Add(sprite);
        }

        if (disponibles.Count == 0)
            return null;

        return disponibles[
            UnityEngine.Random.Range(0, disponibles.Count)
        ];
    }

    private void CrearEstrellas()
    {
        LimpiarObjetos(objetosEstrellas);

        estrellasSuelo.Clear();
        estrellasEntregadas.Clear();

        for (int i = 0; i < posicionesEstrellas.Count; i++)
        {
            GameObject objeto = CrearImagen(
                "Estrella_" + (i + 1),
                panelEstrellas,
                disenoActual.estrella
            );

            RectTransform rect =
                objeto.GetComponent<RectTransform>();

            ColocarEnCasilla(
                rect,
                posicionesEstrellas[i],
                0.70f
            );

            estrellasSuelo.Add(rect);
            objetosEstrellas.Add(objeto);
        }
    }

    private void ReiniciarPosiciones()
    {
        posicionRobot = posicionInicial;
        orientacionRobot = orientacionInicial;

        estrellaCargada = -1;
        estrellasDepositadas = 0;

        foreach (RectTransform rect in estrellasEntregadas)
        {
            if (rect != null)
            {
                rect.gameObject.SetActive(false);
                Destroy(rect.gameObject);
            }
        }

        estrellasEntregadas.Clear();

        for (int i = 0; i < estrellasSuelo.Count; i++)
        {
            RectTransform rect = estrellasSuelo[i];

            if (rect == null)
                continue;

            rect.gameObject.SetActive(true);

            ColocarEnCasilla(
                rect,
                posicionesEstrellas[i],
                0.70f
            );
        }

        imagenEstrellaCargada.gameObject.SetActive(false);

        ColocarEnCasilla(
            imagenRobot,
            posicionRobot,
            0.85f
        );

        imagenRobot.gameObject.SetActive(true);

        CambiarSpriteRobot(orientacionRobot);

        ColocarEnCasilla(
            imagenPuntoEntrega,
            puntoEntrega,
            0.90f
        );

        imagenPuntoEntrega.gameObject.SetActive(true);
    }

    // =====================================================
    // GIROS INVERTIDOS SEGUN TU OBSERVACION
    // =====================================================

    private Direccion GirarDerecha(Direccion direccion)
    {
        // Misma lógica de orientación que la Misión 1.
        // Arriba -> Derecha -> Abajo -> Izquierda.
        return (Direccion)(((int)direccion + 1) % 4);
    }

    private Direccion GirarIzquierda(Direccion direccion)
    {
        // Arriba -> Izquierda -> Abajo -> Derecha.
        return (Direccion)(((int)direccion + 3) % 4);
    }

    private Vector2Int DireccionFrontal(Direccion direccion)
    {
        switch (direccion)
        {
            case Direccion.Arriba:
                return new Vector2Int(0, -1);

            case Direccion.Derecha:
                return new Vector2Int(1, 0);

            case Direccion.Abajo:
                return new Vector2Int(0, 1);

            case Direccion.Izquierda:
                return new Vector2Int(-1, 0);
        }

        return Vector2Int.zero;
    }

    private void CambiarSpriteRobot(Direccion direccion)
    {
        Sprite sprite = null;

        switch (direccion)
        {
            case Direccion.Arriba:
                sprite = robotAbajo;
                break;

            case Direccion.Abajo:
                sprite = robotArriba;
                break;

            case Direccion.Izquierda:
                // Conserva el aspecto visual de los giros que aprobaste.
                // En tu configuración actual, los sprites laterales están
                // asignados en sentido opuesto a la orientación lógica.
                sprite = robotDerecha;
                break;

            case Direccion.Derecha:
                sprite = robotIzquierda;
                break;
        }

        if (sprite != null)
            componenteRobot.sprite = sprite;

        componenteRobot.preserveAspect = true;
    }

    // =====================================================
    // TARJETAS DE PROGRAMACION PEQUEÑAS
    // =====================================================

    private void ConfigurarListaComandos()
    {
        // Unity no permite colocar un HorizontalLayoutGroup y un
        // GridLayoutGroup en el mismo objeto durante el mismo fotograma:
        // Destroy() elimina el componente al final del fotograma.
        // Por eso usamos un hijo exclusivo para la cuadrícula.
        HorizontalLayoutGroup horizontal =
            contenedorComandos.GetComponent<HorizontalLayoutGroup>();

        if (horizontal != null)
            horizontal.enabled = false;

        GridLayoutGroup cuadricula =
            contenedorComandos.GetComponent<GridLayoutGroup>();

        if (cuadricula == null)
        {
            Transform existente =
                contenedorComandos.Find("Cuadricula_10_Instrucciones");

            RectTransform rectCuadricula;
            if (existente != null)
            {
                rectCuadricula = existente as RectTransform;
            }
            else
            {
                GameObject hijo = new GameObject(
                    "Cuadricula_10_Instrucciones",
                    typeof(RectTransform));
                hijo.transform.SetParent(contenedorComandos, false);
                rectCuadricula = hijo.GetComponent<RectTransform>();
                rectCuadricula.anchorMin = Vector2.zero;
                rectCuadricula.anchorMax = Vector2.one;
                rectCuadricula.offsetMin = Vector2.zero;
                rectCuadricula.offsetMax = Vector2.zero;
                rectCuadricula.pivot = new Vector2(0.5f, 0.5f);
            }

            cuadricula = rectCuadricula.GetComponent<GridLayoutGroup>();
            if (cuadricula == null)
                cuadricula = rectCuadricula.gameObject.AddComponent<GridLayoutGroup>();

            // Desde ahora las tarjetas se crean en este hijo.
            contenedorComandos = rectCuadricula;
        }

        if (cuadricula == null)
        {
            Debug.LogError("No se pudo configurar la cuadrícula de instrucciones.");
            return;
        }

        cuadricula.cellSize = new Vector2(
            Mathf.Max(20f, anchoTarjeta), Mathf.Max(20f, altoTarjeta));
        cuadricula.spacing = new Vector2(separacionTarjetas, separacionTarjetas);
        cuadricula.padding = new RectOffset(0, 0, 0, 0);
        cuadricula.startCorner = GridLayoutGroup.Corner.UpperLeft;
        cuadricula.startAxis = GridLayoutGroup.Axis.Horizontal;
        cuadricula.childAlignment = TextAnchor.UpperCenter;
        cuadricula.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        cuadricula.constraintCount = 10;
    }

    private bool PuedeProgramar()
    {
        return juegoIniciado &&
               !ejecutando &&
               !victoria &&
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
                "¡Alcanzaste el máximo de instrucciones!"
            );
            return;
        }

        comandos.Add(direccion);

        ActualizarListaComandos();
        ActualizarBotones();
    }

    private void ActualizarListaComandos()
    {
        LimpiarObjetos(objetosComandos);

        // Tamaño fijo: 10 tarjetas por fila, sin agrandarlas.
        float ancho = Mathf.Max(20f, anchoTarjeta);
        float alto = Mathf.Max(20f, altoTarjeta);
        GridLayoutGroup cuadricula = contenedorComandos.GetComponent<GridLayoutGroup>();
        if (cuadricula != null)
        {
            cuadricula.cellSize = new Vector2(ancho, alto);
            cuadricula.spacing = new Vector2(separacionTarjetas, separacionTarjetas);
        }

        for (int i = 0; i < comandos.Count; i++)
        {
            GameObject tarjeta = CrearImagen(
                "Comando_" + (i + 1),
                contenedorComandos,
                tarjetaComando
            );

            RectTransform rectTarjeta =
                tarjeta.GetComponent<RectTransform>();

            rectTarjeta.sizeDelta =
                new Vector2(ancho, alto);

            LayoutElement elemento =
                tarjeta.AddComponent<LayoutElement>();

            elemento.minWidth = ancho;
            elemento.minHeight = alto;
            elemento.preferredWidth = ancho;
            elemento.preferredHeight = alto;
            elemento.flexibleWidth = 0f;
            elemento.flexibleHeight = 0f;

            GameObject flecha = CrearImagen(
                "Flecha",
                tarjeta.transform,
                SpriteComando(comandos[i])
            );

            RectTransform rectFlecha =
                flecha.GetComponent<RectTransform>();

            rectFlecha.anchorMin = Vector2.zero;
            rectFlecha.anchorMax = Vector2.one;

            rectFlecha.offsetMin = new Vector2(4f, 4f);
            rectFlecha.offsetMax = new Vector2(-4f, -4f);

            objetosComandos.Add(tarjeta);
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

    private Sprite SpriteComando(Direccion direccion)
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

        float ahora = Time.unscaledTime;
        bool dobleClick = ahora - ultimoClickBorrar <= intervaloDobleClick;
        ultimoClickBorrar = dobleClick ? -100f : ahora;

        if (dobleClick)
        {
            // Dos clics rápidos: borrar todas las instrucciones.
            comandos.Clear();
            DetenerPrograma();
            ReiniciarPosiciones();
            MostrarMensaje("¡Programación borrada! Robot en el inicio.");
        }
        else if (comandos.Count > 0)
        {
            // Un clic: borrar únicamente la última instrucción.
            comandos.RemoveAt(comandos.Count - 1);

            if (comandos.Count == 0)
            {
                DetenerPrograma();
                ReiniciarPosiciones();
                MostrarMensaje("¡Robot de vuelta al inicio!");
            }
        }

        ActualizarListaComandos();
        ActualizarBotones();
    }

    // =====================================================
    // EJECUCION COMPLETA DE INSTRUCCIONES
    // =====================================================

    public void EjecutarPrograma()
    {
        if (!PuedeProgramar())
            return;

        if (comandos.Count == 0)
        {
            MostrarMensaje(
                "¡Primero programa los movimientos del robot!"
            );
            return;
        }

        DetenerPrograma();
        ReiniciarPosiciones();
        ultimoClickBorrar = -100f;

        ejecutando = true;
        pausado = false;

        MostrarMensaje("");
        ActualizarBotones();

        rutinaPrograma =
            StartCoroutine(RecorrerInstrucciones());
    }

    private IEnumerator RecorrerInstrucciones()
    {
        for (int i = 0; i < comandos.Count; i++)
        {
            while (pausado)
                yield return null;

            Direccion comando = comandos[i];

            // Los giros cambian la orientación en la MISMA casilla.
            // El siguiente comando Arriba mueve una casilla hacia el frente.
            if (comando == Direccion.Izquierda)
            {
                orientacionRobot =
                    GirarIzquierda(orientacionRobot);

                CambiarSpriteRobot(orientacionRobot);

                yield return EsperarConPausa(duracionGiro);
                continue;
            }

            if (comando == Direccion.Derecha)
            {
                orientacionRobot =
                    GirarDerecha(orientacionRobot);

                CambiarSpriteRobot(orientacionRobot);

                yield return EsperarConPausa(duracionGiro);
                continue;
            }

            Vector2Int frente =
                DireccionFrontal(orientacionRobot);

            Vector2Int desplazamiento =
                comando == Direccion.Arriba
                    ? frente
                    : -frente;

            Vector2Int siguiente =
                posicionRobot + desplazamiento;

            if (!DentroDelTablero(siguiente))
            {
                FinalizarProgramaConError(
                    "El robot intentó avanzar fuera del tablero. La ejecución se detuvo.",
                    i
                );

                yield break;
            }

            if (obstaculos.Contains(siguiente))
            {
                FinalizarProgramaConError(
                    "El robot encontró un obstáculo y se detuvo. Revisa el siguiente paso.",
                    i
                );

                yield break;
            }

            Vector3 origenMundo =
                CentroMundo(posicionRobot);

            Vector3 destinoMundo =
                CentroMundo(siguiente);

            float tiempo = 0f;
            float duracion =
                Mathf.Max(0.01f, duracionPorCasilla);

            while (tiempo < duracion)
            {
                if (pausado)
                {
                    yield return null;
                    continue;
                }

                tiempo += Time.deltaTime;

                float t = Mathf.SmoothStep(
                    0f,
                    1f,
                    Mathf.Clamp01(tiempo / duracion)
                );

                imagenRobot.position =
                    Vector3.Lerp(
                        origenMundo,
                        destinoMundo,
                        t
                    );

                ActualizarEstrellaTransportada();

                yield return null;
            }

            posicionRobot = siguiente;

            imagenRobot.position =
                CentroMundo(posicionRobot);

            ActualizarEstrellaTransportada();

            if (estrellaCargada == -1)
            {
                RecogerEstrella();
            }

            if (estrellaCargada >= 0 &&
                posicionRobot == puntoEntrega)
            {
                yield return AnimarEntrega();
            }
        }

        rutinaPrograma = null;
        ejecutando = false;
        pausado = false;

        if (estrellasDepositadas >= cantidadEstrellas)
        {
            GanarEjercicio();
        }
        else
        {
            MostrarMensaje(
                "¡Buen intento! Entregaste " +
                estrellasDepositadas + " de " +
                cantidadEstrellas +
                " estrellas. Revisa tu programa."
            );
        }

        ActualizarBotones();
    }

    private IEnumerator EsperarConPausa(float segundos)
    {
        float tiempo = 0f;

        while (tiempo < segundos)
        {
            if (!pausado)
                tiempo += Time.deltaTime;

            yield return null;
        }
    }

    private void FinalizarProgramaConError(
        string mensaje,
        int indiceComando)
    {
        ejecutando = false;
        pausado = false;
        rutinaPrograma = null;

        MostrarMensaje(
            mensaje +
            " (Instrucción " +
            (indiceComando + 1) + ")"
        );

        ActualizarBotones();
    }

    // =====================================================
    // RECOGER ESTRELLAS
    // =====================================================

    private void RecogerEstrella()
    {
        if (estrellaCargada >= 0)
            return;

        for (int i = 0; i < posicionesEstrellas.Count; i++)
        {
            RectTransform estrella = estrellasSuelo[i];

            if (estrella == null ||
                !estrella.gameObject.activeSelf)
                continue;

            if (posicionRobot == posicionesEstrellas[i])
            {
                estrellaCargada = i;

                estrella.gameObject.SetActive(false);

                ActualizarEstrellaTransportada();

                MostrarMensaje(
                    "¡Estrella recogida! Ahora llévala al destino."
                );

                return;
            }
        }
    }

    private void ActualizarEstrellaTransportada()
    {
        if (estrellaCargada < 0)
        {
            imagenEstrellaCargada.gameObject.SetActive(false);
            return;
        }

        imagenEstrellaCargada.gameObject.SetActive(true);

        imagenEstrellaCargada.position =
            imagenRobot.position +
            Vector3.up * (tamanoCasilla * 0.35f);

        imagenEstrellaCargada.sizeDelta =
            Vector2.one * tamanoCasilla * 0.40f;
    }

    // =====================================================
    // ENTREGAR ESTRELLAS
    // =====================================================

    private IEnumerator AnimarEntrega()
    {
        if (estrellaCargada < 0 ||
            posicionRobot != puntoEntrega)
        {
            yield break;
        }

        estrellaCargada = -1;
        imagenEstrellaCargada.gameObject.SetActive(false);

        int indice = estrellasDepositadas;

        GameObject objeto = CrearImagen(
            "Estrella_Entregada_" + (indice + 1),
            panelEstrellas,
            disenoActual.estrella
        );

        RectTransform rect =
            objeto.GetComponent<RectTransform>();

        ColocarEnCasilla(rect, puntoEntrega, 0.40f);

        Vector3 desplazamiento = Vector3.zero;
        float distancia = tamanoCasilla * 0.18f;

        if (cantidadEstrellas == 2)
        {
            desplazamiento = new Vector3(
                indice == 0 ? -distancia : distancia,
                0f,
                0f
            );
        }
        else if (cantidadEstrellas == 3)
        {
            float angulo =
                (90f + indice * 120f) * Mathf.Deg2Rad;

            desplazamiento = new Vector3(
                Mathf.Cos(angulo) * distancia,
                Mathf.Sin(angulo) * distancia,
                0f
            );
        }

        rect.position += desplazamiento;

        objetosEstrellas.Add(objeto);
        estrellasEntregadas.Add(rect);

        Vector3 escalaOriginal = rect.localScale;

        float tiempo = 0f;
        float duracion =
            Mathf.Max(0.1f, duracionAnimacionEstrella);

        while (tiempo < duracion)
        {
            if (pausado)
            {
                yield return null;
                continue;
            }

            tiempo += Time.deltaTime;

            float t = Mathf.Clamp01(tiempo / duracion);

            float escala =
                1f +
                (escalaMaximaEstrella - 1f) *
                Mathf.Sin(t * Mathf.PI);

            rect.localScale =
                escalaOriginal * escala;

            yield return null;
        }

        rect.localScale = escalaOriginal;

        estrellasDepositadas++;

        MostrarMensaje(
            "¡Estrella entregada! " +
            estrellasDepositadas + "/" +
            cantidadEstrellas
        );
    }

    // =====================================================
    // PAUSA
    // =====================================================

    public void AlternarPausa()
    {
        if (!ejecutando || victoria)
            return;

        pausado = !pausado;

        MostrarMensaje(
            pausado ? "PAUSA" : ""
        );

        ActualizarBotones();
    }

    private void DetenerPrograma()
    {
        if (rutinaPrograma != null)
        {
            StopCoroutine(rutinaPrograma);
            rutinaPrograma = null;
        }

        ejecutando = false;
        pausado = false;
    }

    // =====================================================
    // PRESENTACION Y VICTORIA
    // =====================================================

    private void MostrarPresentacion()
    {
        panelVictoria.SetActive(false);
        panelPresentacion.SetActive(true);

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
                        "¡Recoge una estrella y llévala al destino!";
                    break;

                case 2:
                    txtObjetivo.text =
                        "¡Transporta una estrella evitando los obstáculos!";
                    break;

                case 3:
                    txtObjetivo.text =
                        "¡Recoge y entrega las 2 estrellas!";
                    break;

                case 4:
                    txtObjetivo.text =
                        "¡Transporta las 3 estrellas hasta el destino!";
                    break;
            }
        }
    }

    public void ComenzarEjercicio()
    {
        panelPresentacion.SetActive(false);
        juegoIniciado = true;
        ActualizarBotones();
    }

    private void GanarEjercicio()
    {
        victoria = true;
        ejecutando = false;
        pausado = false;

        string clave =
            "MundoPujllay_TransportaEstrellas_Progreso";

        int anterior = PlayerPrefs.GetInt(clave, 0);

        PlayerPrefs.SetInt(
            clave,
            Mathf.Max(anterior, ejercicioActual)
        );

        PlayerPrefs.Save();

        if (txtExcelente != null)
            txtExcelente.text = "¡Excelente!";

        if (txtLlegasteEstrella != null)
        {
            txtLlegasteEstrella.text =
                cantidadEstrellas == 1
                    ? "¡Entregaste la estrella!"
                    : "¡Entregaste todas las estrellas!";
        }

        panelVictoria.SetActive(true);

        ActualizarProgreso();
        ActualizarBotones();
    }

    public void SiguienteEjercicio()
    {
        if (!victoria)
            return;

        if (ejercicioActual < TOTAL_EJERCICIOS)
        {
            CargarEjercicio(ejercicioActual + 1);
        }
        else if (!string.IsNullOrWhiteSpace(escenaAlTerminar))
        {
            SceneManager.LoadScene(escenaAlTerminar);
        }
        else
        {
            CargarEjercicio(1);
        }
    }

    // =====================================================
    // PROGRESO
    // =====================================================

    private void CrearProgreso()
    {
        circulosProgreso.Clear();
        numerosProgreso.Clear();

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
            GameObject objeto = CrearImagen(
                "Progreso_" + (i + 1),
                panelProgreso,
                ObtenerSpriteCircular()
            );

            LayoutElement elemento =
                objeto.AddComponent<LayoutElement>();

            elemento.preferredWidth = 65f;
            elemento.preferredHeight = 65f;

            GameObject texto = new GameObject(
                "Numero",
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
            numero.alignment = TextAlignmentOptions.Center;
            numero.raycastTarget = false;

            // Contorno amarillo alrededor de cada bolita de progreso.
            // El componente Outline sigue el borde circular del sprite.
            Outline marco = objeto.GetComponent<Outline>();
            if (marco == null)
                marco = objeto.AddComponent<Outline>();
            marco.effectColor = new Color(1f, 0.80f, 0.12f, 1f);
            marco.effectDistance = new Vector2(2.5f, 2.5f);
            marco.useGraphicAlpha = false;

            circulosProgreso.Add(
                objeto.GetComponent<Image>()
            );

            numerosProgreso.Add(numero);
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

        Vector2 centro = new Vector2(31.5f, 31.5f);
        float radio = 30f;

        for (int y = 0; y < resolucion; y++)
        {
            for (int x = 0; x < resolucion; x++)
            {
                float distancia = Vector2.Distance(
                    new Vector2(x, y),
                    centro
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
        for (int i = 0; i < circulosProgreso.Count; i++)
        {
            bool activo = (i + 1) <= ejercicioActual;

            circulosProgreso[i].color = activo
                ? new Color(1f, 0.78f, 0.2f)
                : new Color(0.70f, 0.75f, 0.82f);

            numerosProgreso[i].color = activo
                ? new Color(0.3f, 0.2f, 0.1f)
                : Color.white;
        }
    }

    // =====================================================
    // BOTONES Y UTILIDADES
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

        if (btnPausa != null)
            btnPausa.interactable = ejecutando;
    }

    private void MostrarMensaje(string mensaje)
    {
        if (txtMensaje != null)
            txtMensaje.text = mensaje;
    }

    private void LimpiarObjetos(List<GameObject> lista)
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

    public void ReiniciarEjercicio()
    {
        CargarEjercicio(ejercicioActual);
    }

    public void VolverAlInicioDeMision()
    {
        CargarEjercicio(1);
    }

    private void OnDisable()
    {
        DetenerPrograma();
    }
}
