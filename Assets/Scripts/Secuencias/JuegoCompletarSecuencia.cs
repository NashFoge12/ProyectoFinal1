using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoCompletarSecuencia : MonoBehaviour
{
    [Header("Textos")]
    public TMP_Text txtPregunta;
    public TMP_Text txtSecuencia;
    public TMP_Text txtProgreso;
    public TMP_Text txtResultado;

    [Header("Opciones")]
    public Button[] botonesOpciones;

    [Header("Botones")]
    public Button btnComprobar;
    public Button btnSiguiente;

    private int ejercicioActual = 0;
    private int opcionSeleccionada = -1;

    private string[] preguntas =
    {
        "Descubre el patrón y completa lo que falta",
        "Observa la repetición y completa la secuencia",
        "Encuentra los números que faltan"
    };

    private string[] secuencias =
    {
        "AZUL - ROJO - AZUL - ROJO - AZUL - __ - __",
        "↑ - ↑ - ↓ - ↑ - ↑ - ↓ - ↑ - __ - __",
        "1 - 2 - 3 - 1 - 2 - 3 - 1 - __ - __"
    };

    private string[,] opciones =
    {
        { "ROJO - AZUL", "AZUL - ROJO", "ROJO - ROJO" },
        { "↑ - ↓", "↓ - ↑", "↑ - ↑" },
        { "2 - 3", "3 - 2", "1 - 2" }
    };

    private int[] respuestasCorrectas =
    {
        0,
        0,
        0
    };

    void Start()
    {
        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            int indice = i;

            botonesOpciones[i].onClick.AddListener(
                () => SeleccionarOpcion(indice)
            );
        }

        btnComprobar.onClick.AddListener(ComprobarRespuesta);
        btnSiguiente.onClick.AddListener(SiguienteEjercicio);

        CargarEjercicio();
    }

    void CargarEjercicio()
    {
        opcionSeleccionada = -1;

        txtResultado.text = "";

        txtPregunta.text = preguntas[ejercicioActual];
        txtSecuencia.text = secuencias[ejercicioActual];

        txtProgreso.text =
            "Ejercicio " + (ejercicioActual + 1) + " de 3";

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            botonesOpciones[i]
                .GetComponentInChildren<TMP_Text>()
                .text = opciones[ejercicioActual, i];

            botonesOpciones[i].interactable = true;
        }

        btnComprobar.gameObject.SetActive(true);
        btnSiguiente.gameObject.SetActive(false);
    }

    void SeleccionarOpcion(int indice)
    {
        opcionSeleccionada = indice;

        txtResultado.text =
            "Opción seleccionada: " +
            botonesOpciones[indice]
            .GetComponentInChildren<TMP_Text>().text;
    }

    void ComprobarRespuesta()
    {
        if (opcionSeleccionada == -1)
        {
            txtResultado.text =
                "Selecciona una opción primero.";
            return;
        }

        if (opcionSeleccionada ==
            respuestasCorrectas[ejercicioActual])
        {
            txtResultado.text =
                "⭐ ¡Correcto! Descubriste el patrón.";

            btnComprobar.gameObject.SetActive(false);
            btnSiguiente.gameObject.SetActive(true);

            foreach (Button boton in botonesOpciones)
            {
                boton.interactable = false;
            }
        }
        else
        {
            txtResultado.text =
                "❌ Esa no es la respuesta. Inténtalo otra vez.";
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
        txtSecuencia.text = "";
        txtProgreso.text = "";

        txtResultado.text =
            "Completaste correctamente las 3 secuencias.";

        foreach (Button boton in botonesOpciones)
        {
            boton.gameObject.SetActive(false);
        }

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);
    }
}