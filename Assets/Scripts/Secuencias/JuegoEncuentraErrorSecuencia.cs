using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoEncuentraErrorSecuencia : MonoBehaviour
{
    [Header("Textos")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;
    public TMP_Text txtResultado;

    [Header("Elementos de la secuencia")]
    public Button[] botonesPasos;

    [Header("Botones")]
    public Button btnComprobar;
    public Button btnSiguiente;

    private int ejercicioActual = 0;
    private int pasoSeleccionado = -1;

    private string[] preguntas =
    {
        "¿Qué color rompe la secuencia?",
        "¿Qué número rompe la secuencia?",
        "¿Qué movimiento rompe la secuencia?"
    };

    private string[,] secuencias =
    {
        {
            "AZUL",
            "ROJO",
            "AZUL",
            "VERDE",
            "AZUL"
        },

        {
            "1",
            "2",
            "3",
            "1",
            "5"
        },

        {
            "ARRIBA",
            "ARRIBA",
            "ABAJO",
            "ARRIBA",
            "IZQUIERDA"
        }
    };

    // Posición del elemento incorrecto
    // Recuerda: Unity empieza a contar desde 0.
    private int[] erroresCorrectos =
    {
        3,  // VERDE
        4,  // 5
        4   // IZQUIERDA
    };

    private string[] explicaciones =
    {
        "La secuencia alterna AZUL - ROJO. VERDE debería ser ROJO.",
        "La secuencia repite 1 - 2 - 3. El 5 debería ser 2.",
        "La secuencia repite ARRIBA - ARRIBA - ABAJO. IZQUIERDA debería ser ARRIBA."
    };

    void Start()
    {
        for (int i = 0; i < botonesPasos.Length; i++)
        {
            int indice = i;

            botonesPasos[i].onClick.AddListener(
                () => SeleccionarPaso(indice)
            );
        }

        btnComprobar.onClick.AddListener(Comprobar);
        btnSiguiente.onClick.AddListener(SiguienteEjercicio);

        CargarEjercicio();
    }

    void CargarEjercicio()
    {
        pasoSeleccionado = -1;

        txtPregunta.text = preguntas[ejercicioActual];

        txtProgreso.text =
            "Ejercicio " + (ejercicioActual + 1) + " de 3";

        txtResultado.text = "";

        for (int i = 0; i < botonesPasos.Length; i++)
        {
            botonesPasos[i]
                .GetComponentInChildren<TMP_Text>()
                .text = secuencias[ejercicioActual, i];

            botonesPasos[i].interactable = true;
        }

        btnComprobar.gameObject.SetActive(true);
        btnSiguiente.gameObject.SetActive(false);
    }

    void SeleccionarPaso(int indice)
    {
        pasoSeleccionado = indice;

        txtResultado.text =
            "Seleccionaste: " +
            botonesPasos[indice]
            .GetComponentInChildren<TMP_Text>()
            .text;
    }

    void Comprobar()
    {
        if (pasoSeleccionado == -1)
        {
            txtResultado.text =
                "Selecciona el elemento que crees que está mal.";
            return;
        }

        if (pasoSeleccionado == erroresCorrectos[ejercicioActual])
        {
            txtResultado.text =
                "⭐ ¡Correcto!\n" +
                explicaciones[ejercicioActual];

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);

            foreach (Button boton in botonesPasos)
            {
                boton.interactable = false;
            }
        }
        else
        {
            txtResultado.text =
                "❌ Ese elemento sigue correctamente la secuencia. Inténtalo otra vez.";
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
        txtPregunta.text = "🏆 ¡MISIÓN COMPLETADA!";
        txtProgreso.text = "";

        txtResultado.text =
            "Encontraste los errores de las 3 secuencias.";

        foreach (Button boton in botonesPasos)
        {
            boton.gameObject.SetActive(false);
        }

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);
    }
}