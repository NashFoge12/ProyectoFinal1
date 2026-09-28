using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class JuegoOrdenarSecuencia : MonoBehaviour
{
    [Header("Textos")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;
    public TMP_Text txtResultado;

    [Header("Tarjetas")]
    public Button[] botonesPasos;

    [Header("Botones")]
    public Button btnComprobar;
    public Button btnSiguiente;

    private int ejercicioActual = 0;
    private int tarjetaSeleccionada = -1;

    private string[] preguntas =
    {
        "Ordena el ciclo de vida de la mariposa",
        "Ordena el crecimiento de una planta",
        "Ordena las partes del día"
    };

    private string[,] secuenciasCorrectas =
    {
        {
            "Huevo",
            "Oruga",
            "Capullo",
            "Mariposa",
            "Mariposa pone huevos"
        },

        {
            "Semilla",
            "Brote",
            "Planta pequeña",
            "Planta grande",
            "Flor"
        },

        {
            "Amanecer",
            "Mañana",
            "Mediodía",
            "Tarde",
            "Noche"
        }
    };

    void Start()
    {
        for (int i = 0; i < botonesPasos.Length; i++)
        {
            int indice = i;

            botonesPasos[i].onClick.AddListener(
                () => SeleccionarTarjeta(indice)
            );
        }

        btnComprobar.onClick.AddListener(ComprobarSecuencia);
        btnSiguiente.onClick.AddListener(SiguienteEjercicio);

        CargarEjercicio();
    }

    void CargarEjercicio()
    {
        tarjetaSeleccionada = -1;

        txtResultado.text = "";

        txtPregunta.text = preguntas[ejercicioActual];

        txtProgreso.text =
            "Ejercicio " + (ejercicioActual + 1) + " de 3";

        List<string> elementos = new List<string>();

        for (int i = 0; i < botonesPasos.Length; i++)
        {
            elementos.Add(secuenciasCorrectas[ejercicioActual, i]);
        }

        Mezclar(elementos);

        for (int i = 0; i < botonesPasos.Length; i++)
        {
            botonesPasos[i]
                .GetComponentInChildren<TMP_Text>()
                .text = elementos[i];

            botonesPasos[i].interactable = true;
        }

        btnComprobar.gameObject.SetActive(true);
        btnSiguiente.gameObject.SetActive(false);
    }

    void Mezclar(List<string> lista)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            int posicionAleatoria = Random.Range(i, lista.Count);

            string temporal = lista[i];
            lista[i] = lista[posicionAleatoria];
            lista[posicionAleatoria] = temporal;
        }
    }

    void SeleccionarTarjeta(int indice)
    {
        if (tarjetaSeleccionada == -1)
        {
            tarjetaSeleccionada = indice;

            txtResultado.text =
                "Seleccionaste: " +
                botonesPasos[indice]
                .GetComponentInChildren<TMP_Text>()
                .text;
        }
        else
        {
            IntercambiarTarjetas(
                tarjetaSeleccionada,
                indice
            );

            tarjetaSeleccionada = -1;

            txtResultado.text =
                "Tarjetas intercambiadas.";
        }
    }

    void IntercambiarTarjetas(int primera, int segunda)
    {
        TMP_Text textoPrimero =
            botonesPasos[primera]
            .GetComponentInChildren<TMP_Text>();

        TMP_Text textoSegundo =
            botonesPasos[segunda]
            .GetComponentInChildren<TMP_Text>();

        string temporal = textoPrimero.text;

        textoPrimero.text = textoSegundo.text;
        textoSegundo.text = temporal;
    }

    void ComprobarSecuencia()
    {
        for (int i = 0; i < botonesPasos.Length; i++)
        {
            string textoActual =
                botonesPasos[i]
                .GetComponentInChildren<TMP_Text>()
                .text;

            if (textoActual != secuenciasCorrectas[ejercicioActual, i])
            {
                txtResultado.text =
                    "❌ La secuencia todavía no está correcta.";

                return;
            }
        }

        txtResultado.text =
            "⭐ ¡Correcto! Ordenaste la secuencia.";

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(true);

        foreach (Button boton in botonesPasos)
        {
            boton.interactable = false;
        }
    }

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

    void FinalizarMision()
    {
        txtPregunta.text =
            "🏆 ¡MISIÓN COMPLETADA!";

        txtProgreso.text = "";

        txtResultado.text =
            "Ordenaste correctamente las 3 secuencias.";

        foreach (Button boton in botonesPasos)
        {
            boton.gameObject.SetActive(false);
        }

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);
    }
}