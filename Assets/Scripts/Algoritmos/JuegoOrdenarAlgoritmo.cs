using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoOrdenarAlgoritmo : MonoBehaviour
{
    // =====================================================
    // DATOS DE UNA TARJETA
    // =====================================================

    [System.Serializable]
    public class Tarjeta
    {
        public string id;
        public Sprite imagen;
    }


    // =====================================================
    // TEXTOS DE LA INTERFAZ
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
    // TITULOS EN IMAGEN
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
    // BOTONES DE CONTROL
    // =====================================================

    [Header("Botones")]
    public Button btnComprobar;
    public Button btnSiguiente;


    // =====================================================
    // SPRITES - LAVARSE LOS DIENTES
    // =====================================================

    [Header("Sprites - Lavarse los dientes")]
    public Sprite sprTomarCepillo;
    public Sprite sprPonerPasta;
    public Sprite sprCepillarDientes;
    public Sprite sprEnjuagarBoca;
    public Sprite sprGuardarCepillo;


    // =====================================================
    // SPRITES - PREPARAR MOCHILA
    // =====================================================

    [Header("Sprites - Preparar mochila")]
    public Sprite sprRevisarHorario;
    public Sprite sprAbrirMochila;
    public Sprite sprColocarCuadernos;
    public Sprite sprColocarLapices;
    public Sprite sprCerrarMochila;


    // =====================================================
    // SPRITES - REGAR PLANTA
    // =====================================================

    [Header("Sprites - Regar planta")]
    public Sprite sprBuscarRegadera;
    public Sprite sprLlenarRegadera;
    public Sprite sprAcercarsePlanta;
    public Sprite sprRegarPlanta;
    public Sprite sprGuardarRegadera;


    // =====================================================
    // VARIABLES INTERNAS
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

        // Conectar automáticamente las tarjetas
        for (int i = 0; i < botonesPasos.Length; i++)
        {
            int indice = i;

            botonesPasos[i].onClick.AddListener(
                () => SeleccionarTarjeta(indice)
            );
        }

        btnComprobar.onClick.AddListener(Comprobar);

        btnSiguiente.onClick.AddListener(SiguienteEjercicio);

        CargarEjercicio();
    }


    // =====================================================
    // CREAR LOS 3 EJERCICIOS
    // =====================================================

    void CrearEjercicios()
    {
        // =================================================
        // EJERCICIO 1
        // LAVARSE LOS DIENTES
        // =================================================

        ejercicios.Add(new Tarjeta[]
        {
            new Tarjeta
            {
                id = "enjuagar",
                imagen = sprEnjuagarBoca
            },

            new Tarjeta
            {
                id = "tomar",
                imagen = sprTomarCepillo
            },

            new Tarjeta
            {
                id = "guardar",
                imagen = sprGuardarCepillo
            },

            new Tarjeta
            {
                id = "pasta",
                imagen = sprPonerPasta
            },

            new Tarjeta
            {
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


        // =================================================
        // EJERCICIO 2
        // PREPARAR LA MOCHILA
        // =================================================

        ejercicios.Add(new Tarjeta[]
        {
            new Tarjeta
            {
                id = "lapices",
                imagen = sprColocarLapices
            },

            new Tarjeta
            {
                id = "cerrar",
                imagen = sprCerrarMochila
            },

            new Tarjeta
            {
                id = "horario",
                imagen = sprRevisarHorario
            },

            new Tarjeta
            {
                id = "abrir",
                imagen = sprAbrirMochila
            },

            new Tarjeta
            {
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


        // =================================================
        // EJERCICIO 3
        // REGAR UNA PLANTA
        // =================================================

        ejercicios.Add(new Tarjeta[]
        {
            new Tarjeta
            {
                id = "regar",
                imagen = sprRegarPlanta
            },

            new Tarjeta
            {
                id = "guardarRegadera",
                imagen = sprGuardarRegadera
            },

            new Tarjeta
            {
                id = "llenar",
                imagen = sprLlenarRegadera
            },

            new Tarjeta
            {
                id = "buscar",
                imagen = sprBuscarRegadera
            },

            new Tarjeta
            {
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
        // Reiniciar selección
        tarjetaSeleccionada = -1;


        // =================================================
        // MENSAJE INICIAL
        // =====================================================

        txtResultado.text =
            "Toca dos tarjetas para intercambiarlas.";


        // =================================================
        // BOTONES
        // =====================================================

        btnComprobar.gameObject.SetActive(true);

        btnSiguiente.gameObject.SetActive(false);


        // =================================================
        // PROGRESO
        // =====================================================

        txtProgreso.text =
            "Ejercicio\n" +
            (ejercicioActual + 1) +
            " de " +
            ejercicios.Count;


        // =================================================
        // EJERCICIO 1
        // DIENTES
        // =====================================================

        if (ejercicioActual == 0)
        {
            txtPregunta.text =
                "Ordena los pasos para\nlavarse los dientes.";


            // Fondo
            if (imagenFondo != null &&
                fondoDientes != null)
            {
                imagenFondo.sprite =
                    fondoDientes;
            }


            // Título
            if (imagenTitulo != null &&
                tituloDientes != null)
            {
                imagenTitulo.sprite =
                    tituloDientes;
            }
        }


        // =================================================
        // EJERCICIO 2
        // MOCHILA
        // =====================================================

        else if (ejercicioActual == 1)
        {
            txtPregunta.text =
                "Ordena los pasos para\npreparar la mochila.";


            // Fondo
            if (imagenFondo != null &&
                fondoMochila != null)
            {
                imagenFondo.sprite =
                    fondoMochila;
            }


            // Título
            if (imagenTitulo != null &&
                tituloMochila != null)
            {
                imagenTitulo.sprite =
                    tituloMochila;
            }
        }


        // =================================================
        // EJERCICIO 3
        // PLANTA
        // =====================================================

        else
        {
            txtPregunta.text =
                "Ordena los pasos para\nregar una planta.";


            // Fondo
            if (imagenFondo != null &&
                fondoPlanta != null)
            {
                imagenFondo.sprite =
                    fondoPlanta;
            }


            // Título
            if (imagenTitulo != null &&
                tituloPlanta != null)
            {
                imagenTitulo.sprite =
                    tituloPlanta;
            }
        }


        // =================================================
        // CARGAR LAS 5 TARJETAS
        // =====================================================

        for (int i = 0; i < botonesPasos.Length; i++)
        {
            Tarjeta tarjeta =
                ejercicios[ejercicioActual][i];


            // Sprite completo de la tarjeta
            botonesPasos[i].image.sprite =
                tarjeta.imagen;


            // IMPORTANTE:
            // Lo dejamos en FALSE porque tus tarjetas
            // se ven correctamente así.
            botonesPasos[i].image.preserveAspect =
                false;


            // Guardar ID de la tarjeta
            botonesPasos[i].name =
                tarjeta.id;


            // Activar tarjeta
            botonesPasos[i].gameObject.SetActive(true);

            botonesPasos[i].interactable =
                true;
        }
    }


    // =====================================================
    // SELECCIONAR TARJETA
    // =====================================================

    void SeleccionarTarjeta(int indice)
    {
        // =================================================
        // PRIMERA TARJETA
        // =====================================================

        if (tarjetaSeleccionada == -1)
        {
            tarjetaSeleccionada = indice;


            txtResultado.text =
                "Ahora selecciona otra tarjeta para intercambiarlas.";


            return;
        }


        // =================================================
        // TOCÓ LA MISMA TARJETA
        // =====================================================

        if (tarjetaSeleccionada == indice)
        {
            tarjetaSeleccionada = -1;


            txtResultado.text =
                "Toca dos tarjetas para intercambiarlas.";


            return;
        }


        // =================================================
        // INTERCAMBIAR
        // =====================================================

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
        // =================================================
        // COMPROBACIÓN DE SEGURIDAD
        // =====================================================

        if (indice1 < 0 ||
            indice2 < 0 ||
            indice1 >= botonesPasos.Length ||
            indice2 >= botonesPasos.Length)
        {
            Debug.LogError(
                "Error: índice de tarjeta fuera de rango."
            );

            return;
        }


        // =================================================
        // INTERCAMBIAR SPRITES
        // =====================================================

        Sprite spriteTemporal =
            botonesPasos[indice1].image.sprite;


        botonesPasos[indice1].image.sprite =
            botonesPasos[indice2].image.sprite;


        botonesPasos[indice2].image.sprite =
            spriteTemporal;


        // =================================================
        // INTERCAMBIAR IDs
        // =====================================================

        string idTemporal =
            botonesPasos[indice1].name;


        botonesPasos[indice1].name =
            botonesPasos[indice2].name;


        botonesPasos[indice2].name =
            idTemporal;
    }


    // =====================================================
    // COMPROBAR RESPUESTA
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


        // =================================================
        // RESPUESTA CORRECTA
        // =====================================================

        if (correcto)
        {
            txtResultado.text =
                "¡Excelente! Ordenaste correctamente el algoritmo.";


            // Ocultar comprobar
            btnComprobar.gameObject.SetActive(false);


            // Mostrar siguiente
            btnSiguiente.gameObject.SetActive(true);


            // Bloquear tarjetas
            foreach (Button boton in botonesPasos)
            {
                boton.interactable =
                    false;
            }
        }


        // =================================================
        // RESPUESTA INCORRECTA
        // =====================================================

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
    // FINALIZAR MISIÓN
    // =====================================================

    void FinalizarMision()
    {
        txtPregunta.text =
            "¡MISIÓN COMPLETADA!";


        txtProgreso.text =
            "";


        txtResultado.text =
            "¡Excelente! Completaste los 3 algoritmos.";


        // =================================================
        // OCULTAR TARJETAS
        // =====================================================

        foreach (Button boton in botonesPasos)
        {
            boton.gameObject.SetActive(false);
        }


        // =================================================
        // OCULTAR BOTONES
        // =====================================================

        btnComprobar.gameObject.SetActive(false);

        btnSiguiente.gameObject.SetActive(false);
    }
}