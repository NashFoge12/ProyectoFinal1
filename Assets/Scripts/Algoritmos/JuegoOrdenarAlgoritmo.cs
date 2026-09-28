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
        public string texto;
        public Sprite imagen;
    }

    [Header("Interfaz")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;
    public TMP_Text txtResultado;

    [Header("Tarjetas")]
    public Button[] botonesPasos;
    public Image[] imagenesPasos;

    [Header("Botones")]
    public Button btnComprobar;
    public Button btnSiguiente;

    // ================================
    // SPRITES - LAVARSE LOS DIENTES
    // ================================

    [Header("Sprites - Lavarse los dientes")]
    public Sprite sprTomarCepillo;
    public Sprite sprPonerPasta;
    public Sprite sprCepillarDientes;
    public Sprite sprEnjuagarBoca;
    public Sprite sprGuardarCepillo;

    // ================================
    // SPRITES - PREPARAR MOCHILA
    // ================================

    [Header("Sprites - Preparar mochila")]
    public Sprite sprRevisarHorario;
    public Sprite sprAbrirMochila;
    public Sprite sprColocarCuadernos;
    public Sprite sprColocarLapices;
    public Sprite sprCerrarMochila;

    // ================================
    // SPRITES - REGAR PLANTA
    // ================================

    [Header("Sprites - Regar planta")]
    public Sprite sprBuscarRegadera;
    public Sprite sprLlenarRegadera;
    public Sprite sprAcercarsePlanta;
    public Sprite sprRegarPlanta;
    public Sprite sprGuardarRegadera;


    private int ejercicioActual = 0;
    private int tarjetaSeleccionada = -1;

    private List<Tarjeta[]> ejercicios =
        new List<Tarjeta[]>();

    private List<string[]> respuestasCorrectas =
        new List<string[]>();


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

        CargarEjercicio();
    }


    // =====================================
    // CREAR LOS 3 EJERCICIOS
    // =====================================

    void CrearEjercicios()
    {
        // EJERCICIO 1
        // LAVARSE LOS DIENTES

        ejercicios.Add(new Tarjeta[]
        {
            new Tarjeta
            {
                id = "enjuagar",
                texto = "Enjuagar la boca",
                imagen = sprEnjuagarBoca
            },

            new Tarjeta
            {
                id = "tomar",
                texto = "Tomar el cepillo",
                imagen = sprTomarCepillo
            },

            new Tarjeta
            {
                id = "guardar",
                texto = "Guardar el cepillo",
                imagen = sprGuardarCepillo
            },

            new Tarjeta
            {
                id = "pasta",
                texto = "Poner pasta dental",
                imagen = sprPonerPasta
            },

            new Tarjeta
            {
                id = "cepillar",
                texto = "Cepillar los dientes",
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


        // EJERCICIO 2
        // PREPARAR LA MOCHILA

        ejercicios.Add(new Tarjeta[]
        {
            new Tarjeta
            {
                id = "lapices",
                texto = "Colocar los lápices",
                imagen = sprColocarLapices
            },

            new Tarjeta
            {
                id = "cerrar",
                texto = "Cerrar la mochila",
                imagen = sprCerrarMochila
            },

            new Tarjeta
            {
                id = "horario",
                texto = "Revisar el horario",
                imagen = sprRevisarHorario
            },

            new Tarjeta
            {
                id = "abrir",
                texto = "Abrir la mochila",
                imagen = sprAbrirMochila
            },

            new Tarjeta
            {
                id = "cuadernos",
                texto = "Colocar los cuadernos",
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


        // EJERCICIO 3
        // REGAR UNA PLANTA

        ejercicios.Add(new Tarjeta[]
        {
            new Tarjeta
            {
                id = "regar",
                texto = "Regar la planta",
                imagen = sprRegarPlanta
            },

            new Tarjeta
            {
                id = "guardarRegadera",
                texto = "Guardar la regadera",
                imagen = sprGuardarRegadera
            },

            new Tarjeta
            {
                id = "llenar",
                texto = "Llenar la regadera",
                imagen = sprLlenarRegadera
            },

            new Tarjeta
            {
                id = "buscar",
                texto = "Buscar la regadera",
                imagen = sprBuscarRegadera
            },

            new Tarjeta
            {
                id = "acercarse",
                texto = "Acercarse a la planta",
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


    // =====================================
    // CARGAR EJERCICIO
    // =====================================

    void CargarEjercicio()
    {
        tarjetaSeleccionada = -1;

        txtResultado.text = "";

        btnSiguiente.gameObject.SetActive(false);
        btnComprobar.gameObject.SetActive(true);

        txtProgreso.text =
            "Ejercicio " + (ejercicioActual + 1) + " de 3";


        if (ejercicioActual == 0)
        {
            txtPregunta.text =
                "Ordena los pasos para lavarse los dientes";
        }
        else if (ejercicioActual == 1)
        {
            txtPregunta.text =
                "Ordena los pasos para preparar la mochila";
        }
        else
        {
            txtPregunta.text =
                "Ordena los pasos para regar una planta";
        }


        for (int i = 0; i < botonesPasos.Length; i++)
        {
            Tarjeta tarjeta =
                ejercicios[ejercicioActual][i];

            // TEXTO
            botonesPasos[i]
                .GetComponentInChildren<TMP_Text>()
                .text = tarjeta.texto;

            // IMAGEN
            imagenesPasos[i].sprite =
                tarjeta.imagen;

            // Guardamos el ID dentro del botón
            botonesPasos[i].name =
                tarjeta.id;
        }
    }


    // =====================================
    // SELECCIONAR E INTERCAMBIAR
    // =====================================

    void SeleccionarTarjeta(int indice)
    {
        if (tarjetaSeleccionada == -1)
        {
            tarjetaSeleccionada = indice;

            txtResultado.text =
                "Ahora selecciona otra tarjeta para intercambiarlas.";
        }
        else
        {
            IntercambiarTarjetas(
                tarjetaSeleccionada,
                indice
            );

            tarjetaSeleccionada = -1;

            txtResultado.text = "";
        }
    }


    void IntercambiarTarjetas(int indice1, int indice2)
    {
        // ========================
        // INTERCAMBIAR TEXTO
        // ========================

        TMP_Text texto1 =
            botonesPasos[indice1]
            .GetComponentInChildren<TMP_Text>();

        TMP_Text texto2 =
            botonesPasos[indice2]
            .GetComponentInChildren<TMP_Text>();

        string textoTemporal = texto1.text;

        texto1.text = texto2.text;
        texto2.text = textoTemporal;


        // ========================
        // INTERCAMBIAR SPRITE
        // ========================

        Sprite spriteTemporal =
            imagenesPasos[indice1].sprite;

        imagenesPasos[indice1].sprite =
            imagenesPasos[indice2].sprite;

        imagenesPasos[indice2].sprite =
            spriteTemporal;


        // ========================
        // INTERCAMBIAR ID
        // ========================

        string idTemporal =
            botonesPasos[indice1].name;

        botonesPasos[indice1].name =
            botonesPasos[indice2].name;

        botonesPasos[indice2].name =
            idTemporal;
    }


    // =====================================
    // COMPROBAR
    // =====================================

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
                "⭐ ¡Excelente! Ordenaste correctamente el algoritmo.";

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);
        }
        else
        {
            txtResultado.text =
                "❌ Aún no está correcto. ¡Inténtalo otra vez!";
        }
    }


    // =====================================
    // SIGUIENTE
    // =====================================

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


    // =====================================
    // FINAL
    // =====================================

    void FinalizarMision()
    {
        txtPregunta.text =
            "🏆 ¡MISIÓN COMPLETADA!";

        txtProgreso.text = "";

        txtResultado.text =
            "Has ordenado correctamente los 3 algoritmos.";

        foreach (Button boton in botonesPasos)
        {
            boton.gameObject.SetActive(false);
        }

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);
    }
}