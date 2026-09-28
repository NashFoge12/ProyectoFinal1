using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoDivideProblema : MonoBehaviour
{
    // =====================================================
    // CLASE PARA CADA TARJETA
    // =====================================================

    [System.Serializable]
    public class Tarjeta
    {
        public Sprite imagen;
        public bool esCorrecta;

        public Tarjeta(Sprite imagenNueva, bool correcta)
        {
            imagen = imagenNueva;
            esCorrecta = correcta;
        }
    }


    // =====================================================
    // TEXTOS
    // =====================================================

    [Header("Textos")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;
    public TMP_Text txtResultado;


    // =====================================================
    // FONDOS
    // =====================================================

    [Header("Fondos")]
    public Image imagenFondo;

    public Sprite fondoColegio;
    public Sprite fondoPlanta;
    public Sprite fondoRobot;


    // =====================================================
    // TITULOS
    // =====================================================

    [Header("Titulos en imagen")]
    public Image imagenTitulo;

    public Sprite tituloColegio;
    public Sprite tituloPlanta;
    public Sprite tituloRobot;


    // =====================================================
    // IMAGEN DEL PROBLEMA
    // =====================================================

    [Header("Imagen del problema")]
    public Image imagenProblema;

    public Sprite problemaColegio;
    public Sprite problemaPlanta;
    public Sprite problemaRobot;


    // =====================================================
    // BOTONES / TARJETAS
    // =====================================================

    [Header("Tarjetas")]
    public Button[] botonesPartes;


    // =====================================================
    // CHECKS DE SELECCION
    //
    // Deben estar en el mismo orden que botonesPartes.
    // =====================================================

    [Header("Checks de seleccion")]
    public GameObject[] checksSeleccion;


    // =====================================================
    // EJERCICIO 1 - COLEGIO
    // =====================================================

    [Header("Ejercicio 1 - Colegio")]

    public Sprite colegioVestirse;
    public Sprite colegioMochila;
    public Sprite colegioDesayunar;

    public Sprite colegioVideojuegos;
    public Sprite colegioPlaya;


    // =====================================================
    // EJERCICIO 2 - PLANTA
    // =====================================================

    [Header("Ejercicio 2 - Planta")]

    public Sprite plantaRegar;
    public Sprite plantaLuz;
    public Sprite plantaRevisarHojas;

    public Sprite plantaTelevision;
    public Sprite plantaFutbol;


    // =====================================================
    // EJERCICIO 3 - ROBOT
    // =====================================================

    [Header("Ejercicio 3 - Robot")]

    public Sprite robotObservarCamino;
    public Sprite robotIdentificarObstaculos;
    public Sprite robotPlanearMovimientos;

    public Sprite robotPintar;
    public Sprite robotMusica;


    // =====================================================
    // BOTONES DE CONTROL
    // =====================================================

    [Header("Botones")]
    public Button btnComprobar;
    public Button btnSiguiente;


    // =====================================================
    // VARIABLES INTERNAS
    // =====================================================

    private int ejercicioActual = 0;

    // Guarda si cada botón está seleccionado.
    private bool[] seleccionadas;

    // Guarda qué tarjeta está actualmente en cada botón.
    private Tarjeta[] tarjetasActuales;

    // Cantidad máxima que debe seleccionar.
    private const int CANTIDAD_CORRECTAS = 3;


    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        // -------------------------------------------------
        // COMPROBAR BOTONES
        // -------------------------------------------------

        if (botonesPartes == null ||
            botonesPartes.Length != 5)
        {
            Debug.LogError(
                "JuegoDivideProblema necesita exactamente 5 botones."
            );

            return;
        }


        // -------------------------------------------------
        // CREAR ARRAYS
        // -------------------------------------------------

        seleccionadas =
            new bool[botonesPartes.Length];

        tarjetasActuales =
            new Tarjeta[botonesPartes.Length];


        // -------------------------------------------------
        // CONECTAR LOS 5 BOTONES
        // -------------------------------------------------

        for (int i = 0; i < botonesPartes.Length; i++)
        {
            int indice = i;

            botonesPartes[i].onClick.AddListener(
                () => SeleccionarTarjeta(indice)
            );
        }


        // -------------------------------------------------
        // BOTÓN COMPROBAR
        // -------------------------------------------------

        if (btnComprobar != null)
        {
            btnComprobar.onClick.AddListener(
                ComprobarRespuesta
            );
        }


        // -------------------------------------------------
        // BOTÓN SIGUIENTE
        // -------------------------------------------------

        if (btnSiguiente != null)
        {
            btnSiguiente.onClick.AddListener(
                SiguienteEjercicio
            );
        }


        // -------------------------------------------------
        // CARGAR PRIMER EJERCICIO
        // -------------------------------------------------

        CargarEjercicio();
    }


    // =====================================================
    // CARGAR EJERCICIO
    // =====================================================

    void CargarEjercicio()
    {
        // -------------------------------------------------
        // REINICIAR SELECCIONES
        // -------------------------------------------------

        for (int i = 0; i < seleccionadas.Length; i++)
        {
            seleccionadas[i] = false;
        }


        ActualizarChecks();


        // -------------------------------------------------
        // MENSAJE
        // -------------------------------------------------

        if (txtResultado != null)
        {
            txtResultado.text =
                "Selecciona las 3 partes que ayudan a resolver el problema.";
        }


        // -------------------------------------------------
        // PROGRESO
        // -------------------------------------------------

        if (txtProgreso != null)
        {
            txtProgreso.text =
                "Ejercicio\n" +
                (ejercicioActual + 1) +
                " de 3";
        }


        // =================================================
        // EJERCICIO 1 - COLEGIO
        // =================================================

        if (ejercicioActual == 0)
        {
            if (txtPregunta != null)
            {
                txtPregunta.text =
                    "¿En qué partes podemos dividir prepararnos para ir al colegio?";
            }


            CambiarImagen(
                imagenFondo,
                fondoColegio,
                false
            );


            CambiarImagen(
                imagenTitulo,
                tituloColegio,
                false
            );


            CambiarImagen(
                imagenProblema,
                problemaColegio,
                false
            );
        }


        // =================================================
        // EJERCICIO 2 - PLANTA
        // =================================================

        else if (ejercicioActual == 1)
        {
            if (txtPregunta != null)
            {
                txtPregunta.text =
                    "¿En qué partes podemos dividir el cuidado de una planta?";
            }


            CambiarImagen(
                imagenFondo,
                fondoPlanta,
                false
            );


            CambiarImagen(
                imagenTitulo,
                tituloPlanta,
                false
            );


            CambiarImagen(
                imagenProblema,
                problemaPlanta,
                false
            );
        }


        // =================================================
        // EJERCICIO 3 - ROBOT
        // =================================================

        else
        {
            if (txtPregunta != null)
            {
                txtPregunta.text =
                    "¿Qué partes necesitamos para que el robot llegue a la meta?";
            }


            CambiarImagen(
                imagenFondo,
                fondoRobot,
                false
            );


            CambiarImagen(
                imagenTitulo,
                tituloRobot,
                false
            );


            CambiarImagen(
                imagenProblema,
                problemaRobot,
                false
            );
        }


        // -------------------------------------------------
        // CREAR Y MEZCLAR TARJETAS
        // -------------------------------------------------

        PrepararTarjetas();


        // -------------------------------------------------
        // ACTIVAR BOTONES
        // -------------------------------------------------

        for (int i = 0; i < botonesPartes.Length; i++)
        {
            botonesPartes[i]
                .gameObject
                .SetActive(true);

            botonesPartes[i]
                .interactable = true;
        }


        // -------------------------------------------------
        // COMPROBAR / SIGUIENTE
        // -------------------------------------------------

        if (btnComprobar != null)
        {
            btnComprobar
                .gameObject
                .SetActive(true);
        }


        if (btnSiguiente != null)
        {
            btnSiguiente
                .gameObject
                .SetActive(false);
        }
    }


    // =====================================================
    // PREPARAR TARJETAS
    // =====================================================

    void PrepararTarjetas()
    {
        List<Tarjeta> tarjetas =
            new List<Tarjeta>();


        // =================================================
        // EJERCICIO 1
        // =================================================

        if (ejercicioActual == 0)
        {
            tarjetas.Add(
                new Tarjeta(colegioVestirse, true)
            );

            tarjetas.Add(
                new Tarjeta(colegioMochila, true)
            );

            tarjetas.Add(
                new Tarjeta(colegioDesayunar, true)
            );

            tarjetas.Add(
                new Tarjeta(colegioVideojuegos, false)
            );

            tarjetas.Add(
                new Tarjeta(colegioPlaya, false)
            );
        }


        // =================================================
        // EJERCICIO 2
        // =================================================

        else if (ejercicioActual == 1)
        {
            tarjetas.Add(
                new Tarjeta(plantaRegar, true)
            );

            tarjetas.Add(
                new Tarjeta(plantaLuz, true)
            );

            tarjetas.Add(
                new Tarjeta(plantaRevisarHojas, true)
            );

            tarjetas.Add(
                new Tarjeta(plantaTelevision, false)
            );

            tarjetas.Add(
                new Tarjeta(plantaFutbol, false)
            );
        }


        // =================================================
        // EJERCICIO 3
        // =================================================

        else
        {
            tarjetas.Add(
                new Tarjeta(robotObservarCamino, true)
            );

            tarjetas.Add(
                new Tarjeta(robotIdentificarObstaculos, true)
            );

            tarjetas.Add(
                new Tarjeta(robotPlanearMovimientos, true)
            );

            tarjetas.Add(
                new Tarjeta(robotPintar, false)
            );

            tarjetas.Add(
                new Tarjeta(robotMusica, false)
            );
        }


        // =================================================
        // MEZCLAR LAS 5 TARJETAS
        // =================================================

        for (int i = 0; i < tarjetas.Count; i++)
        {
            int aleatorio =
                Random.Range(i, tarjetas.Count);


            Tarjeta temporal =
                tarjetas[i];


            tarjetas[i] =
                tarjetas[aleatorio];


            tarjetas[aleatorio] =
                temporal;
        }


        // =================================================
        // COLOCAR EN LOS BOTONES
        // =================================================

        for (int i = 0; i < botonesPartes.Length; i++)
        {
            tarjetasActuales[i] =
                tarjetas[i];


            botonesPartes[i]
                .image
                .sprite =
                tarjetas[i].imagen;


            // Igual que en tus otros juegos:
            // evita que un botón quede más pequeño.
            botonesPartes[i]
                .image
                .preserveAspect = false;
        }
    }


    // =====================================================
    // SELECCIONAR / DESELECCIONAR TARJETA
    // =====================================================

    void SeleccionarTarjeta(int indice)
    {
        // -------------------------------------------------
        // SI YA ESTABA SELECCIONADA
        // LA QUITAMOS
        // -------------------------------------------------

        if (seleccionadas[indice])
        {
            seleccionadas[indice] = false;

            ActualizarChecks();


            if (txtResultado != null)
            {
                txtResultado.text =
                    "Selecciona las 3 partes correctas.";
            }

            return;
        }


        // -------------------------------------------------
        // CONTAR CUÁNTAS YA ESTÁN SELECCIONADAS
        // -------------------------------------------------

        int cantidadSeleccionada =
            ContarSeleccionadas();


        // -------------------------------------------------
        // NO PERMITIR MÁS DE 3
        // -------------------------------------------------

        if (cantidadSeleccionada >= CANTIDAD_CORRECTAS)
        {
            if (txtResultado != null)
            {
                txtResultado.text =
                    "Ya seleccionaste 3 tarjetas. Puedes quitar una si deseas cambiarla.";
            }

            return;
        }


        // -------------------------------------------------
        // SELECCIONAR
        // -------------------------------------------------

        seleccionadas[indice] = true;


        ActualizarChecks();


        cantidadSeleccionada =
            ContarSeleccionadas();


        if (txtResultado != null)
        {
            if (cantidadSeleccionada < 3)
            {
                txtResultado.text =
                    "Seleccionaste " +
                    cantidadSeleccionada +
                    " de 3.";
            }
            else
            {
                txtResultado.text =
                    "¡Ya elegiste 3! Pulsa COMPROBAR.";
            }
        }
    }


    // =====================================================
    // CONTAR SELECCIONADAS
    // =====================================================

    int ContarSeleccionadas()
    {
        int cantidad = 0;


        for (int i = 0; i < seleccionadas.Length; i++)
        {
            if (seleccionadas[i])
            {
                cantidad++;
            }
        }


        return cantidad;
    }


    // =====================================================
    // ACTUALIZAR CHECKS
    // =====================================================

    void ActualizarChecks()
    {
        if (checksSeleccion == null)
        {
            return;
        }


        for (int i = 0; i < checksSeleccion.Length; i++)
        {
            if (checksSeleccion[i] == null)
            {
                continue;
            }


            bool mostrar =
                i < seleccionadas.Length &&
                seleccionadas[i];


            checksSeleccion[i]
                .SetActive(mostrar);
        }
    }


    // =====================================================
    // COMPROBAR
    // =====================================================

    void ComprobarRespuesta()
    {
        // -------------------------------------------------
        // PRIMERO DEBE ELEGIR EXACTAMENTE 3
        // -------------------------------------------------

        if (ContarSeleccionadas() != CANTIDAD_CORRECTAS)
        {
            if (txtResultado != null)
            {
                txtResultado.text =
                    "Debes seleccionar exactamente 3 tarjetas.";
            }

            return;
        }


        // -------------------------------------------------
        // REVISAR LAS 3 SELECCIONADAS
        // -------------------------------------------------

        bool todoCorrecto = true;


        for (int i = 0; i < botonesPartes.Length; i++)
        {
            // Solo revisamos las seleccionadas
            if (seleccionadas[i])
            {
                if (!tarjetasActuales[i].esCorrecta)
                {
                    todoCorrecto = false;

                    break;
                }
            }
        }


        // =================================================
        // CORRECTO
        // =================================================

        if (todoCorrecto)
        {
            if (txtResultado != null)
            {
                txtResultado.text =
                    "⭐ ¡Excelente! Dividiste el problema en partes más pequeñas.";
            }


            // Bloquear tarjetas
            foreach (Button boton in botonesPartes)
            {
                boton.interactable = false;
            }


            // Ocultar comprobar
            if (btnComprobar != null)
            {
                btnComprobar
                    .gameObject
                    .SetActive(false);
            }


            // Mostrar siguiente
            if (btnSiguiente != null)
            {
                btnSiguiente
                    .gameObject
                    .SetActive(true);
            }
        }


        // =================================================
        // INCORRECTO
        // =================================================

        else
        {
            if (txtResultado != null)
            {
                txtResultado.text =
                    "❌ Una de esas partes no ayuda a resolver el problema. ¡Inténtalo otra vez!";
            }


            // Desmarcar todo para volver a intentar
            for (int i = 0; i < seleccionadas.Length; i++)
            {
                seleccionadas[i] = false;
            }


            ActualizarChecks();
        }
    }


    // =====================================================
    // SIGUIENTE EJERCICIO
    // =====================================================

    void SiguienteEjercicio()
    {
        ejercicioActual++;


        if (ejercicioActual < 3)
        {
            CargarEjercicio();
        }
        else
        {
            FinalizarMision();
        }
    }


    // =====================================================
    // FINALIZAR MISIÓN
    // =====================================================

    void FinalizarMision()
    {
        if (txtPregunta != null)
        {
            txtPregunta.text =
                "🏆 ¡MISIÓN COMPLETADA!";
        }


        if (txtProgreso != null)
        {
            txtProgreso.text = "";
        }


        if (txtResultado != null)
        {
            txtResultado.text =
                "⭐ Aprendiste a dividir problemas grandes en partes más pequeñas.";
        }


        // -------------------------------------------------
        // OCULTAR IMAGEN DEL PROBLEMA
        // -------------------------------------------------

        if (imagenProblema != null)
        {
            imagenProblema
                .gameObject
                .SetActive(false);
        }


        // -------------------------------------------------
        // OCULTAR TARJETAS
        // -------------------------------------------------

        foreach (Button boton in botonesPartes)
        {
            boton
                .gameObject
                .SetActive(false);
        }


        // -------------------------------------------------
        // OCULTAR CHECKS
        // -------------------------------------------------

        if (checksSeleccion != null)
        {
            foreach (GameObject check in checksSeleccion)
            {
                if (check != null)
                {
                    check.SetActive(false);
                }
            }
        }


        // -------------------------------------------------
        // OCULTAR BOTONES
        // -------------------------------------------------

        if (btnComprobar != null)
        {
            btnComprobar
                .gameObject
                .SetActive(false);
        }


        if (btnSiguiente != null)
        {
            btnSiguiente
                .gameObject
                .SetActive(false);
        }
    }


    // =====================================================
    // CAMBIAR IMAGEN
    // =====================================================

    void CambiarImagen(
        Image imagen,
        Sprite spriteNuevo,
        bool mantenerProporcion
    )
    {
        if (imagen == null ||
            spriteNuevo == null)
        {
            return;
        }


        imagen.sprite =
            spriteNuevo;


        imagen.preserveAspect =
            mantenerProporcion;
    }
}