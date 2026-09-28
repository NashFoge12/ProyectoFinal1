using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoEncuentraErrorAlgoritmo : MonoBehaviour
{
    [Header("Textos")]
    public TMP_Text txtPregunta;
    public TMP_Text txtProgreso;
    public TMP_Text txtResultado;

    [Header("Pasos")]
    public Button[] botonesPasos;

    [Header("Botones")]
    public Button btnComprobar;
    public Button btnSiguiente;

    private int ejercicioActual = 0;
    private int pasoSeleccionado = -1;

    private string[,] pasos =
    {
        {
            "1. Tomar el cepillo",
            "2. Poner pasta dental",
            "3. Guardar el cepillo",
            "4. Cepillar los dientes",
            "5. Enjuagar la boca"
        },

        {
            "1. Revisar el horario",
            "2. Cerrar la mochila",
            "3. Abrir la mochila",
            "4. Colocar los cuadernos",
            "5. Colocar los lápices"
        },

        {
            "1. Buscar la regadera",
            "2. Llenar la regadera",
            "3. Guardar la regadera",
            "4. Acercarse a la planta",
            "5. Regar la planta"
        }
    };

    private int[] erroresCorrectos =
    {
        2,
        1,
        2
    };

    private string[] correcciones =
    {
        "Guardar el cepillo debe ir al final.",
        "Cerrar la mochila debe ir al final.",
        "Guardar la regadera debe hacerse después de regar."
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

        txtResultado.text = "";

        txtPregunta.text =
            "Encuentra el paso que está mal";

        txtProgreso.text =
            "Ejercicio " + (ejercicioActual + 1) + " de 3";

        for (int i = 0; i < botonesPasos.Length; i++)
        {
            botonesPasos[i]
                .GetComponentInChildren<TMP_Text>()
                .text = pasos[ejercicioActual, i];

            botonesPasos[i].interactable = true;
        }

        btnComprobar.gameObject.SetActive(true);
        btnSiguiente.gameObject.SetActive(false);
    }

    void SeleccionarPaso(int indice)
    {
        pasoSeleccionado = indice;

        txtResultado.text =
            "Paso seleccionado: " +
            botonesPasos[indice]
            .GetComponentInChildren<TMP_Text>()
            .text;
    }

    void Comprobar()
    {
        if (pasoSeleccionado == -1)
        {
            txtResultado.text =
                "Selecciona el paso que crees que está mal.";
            return;
        }

        if (pasoSeleccionado == erroresCorrectos[ejercicioActual])
        {
            txtResultado.text =
                "⭐ ¡Correcto! Encontraste el error.\n" +
                correcciones[ejercicioActual];

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
                "❌ Ese paso no es el error. Inténtalo otra vez.";
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
            "Encontraste correctamente los errores de los 3 algoritmos.";

        foreach (Button boton in botonesPasos)
        {
            boton.gameObject.SetActive(false);
        }

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);
    }
}