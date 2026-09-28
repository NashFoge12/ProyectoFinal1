using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoCompletarAlgoritmo : MonoBehaviour
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
        "¿Qué paso falta para completar el algoritmo?",
        "¿Qué paso falta para completar el algoritmo?",
        "¿Qué paso falta para completar el algoritmo?"
    };

    private string[] algoritmos =
    {
        "LAVARSE LOS DIENTES\n\n1. Tomar el cepillo\n2. __________\n3. Cepillar los dientes\n4. Enjuagar la boca\n5. Guardar el cepillo",

        "PREPARAR LA MOCHILA\n\n1. Revisar el horario\n2. Abrir la mochila\n3. __________\n4. Colocar los lápices\n5. Cerrar la mochila",

        "REGAR UNA PLANTA\n\n1. Buscar la regadera\n2. __________\n3. Acercarse a la planta\n4. Regar la planta\n5. Guardar la regadera"
    };

    private string[,] opciones =
    {
        {
            "Poner pasta dental",
            "Guardar el cepillo",
            "Secar las manos"
        },

        {
            "Colocar los cuadernos",
            "Abrir la puerta",
            "Cerrar la mochila"
        },

        {
            "Llenar la regadera",
            "Guardar la regadera",
            "Recoger la planta"
        }
    };

    private string[] respuestasCorrectas =
    {
        "Poner pasta dental",
        "Colocar los cuadernos",
        "Llenar la regadera"
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
        txtSecuencia.text = algoritmos[ejercicioActual];

        txtProgreso.text =
            "Ejercicio " + (ejercicioActual + 1) + " de 3";

        List<string> opcionesMezcladas = new List<string>();

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            opcionesMezcladas.Add(opciones[ejercicioActual, i]);
        }

        Mezclar(opcionesMezcladas);

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            botonesOpciones[i]
                .GetComponentInChildren<TMP_Text>()
                .text = opcionesMezcladas[i];

            botonesOpciones[i].interactable = true;
        }

        btnComprobar.gameObject.SetActive(true);
        btnSiguiente.gameObject.SetActive(false);
    }

    void Mezclar(List<string> lista)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            int aleatorio = Random.Range(i, lista.Count);

            string temporal = lista[i];
            lista[i] = lista[aleatorio];
            lista[aleatorio] = temporal;
        }
    }

    void SeleccionarOpcion(int indice)
    {
        opcionSeleccionada = indice;

        txtResultado.text =
            "Opción seleccionada: " +
            botonesOpciones[indice]
            .GetComponentInChildren<TMP_Text>()
            .text;
    }

    void ComprobarRespuesta()
    {
        if (opcionSeleccionada == -1)
        {
            txtResultado.text =
                "Selecciona una opción primero.";
            return;
        }

        string respuestaElegida =
            botonesOpciones[opcionSeleccionada]
            .GetComponentInChildren<TMP_Text>()
            .text;

        if (respuestaElegida == respuestasCorrectas[ejercicioActual])
        {
            txtResultado.text =
                "⭐ ¡Correcto! Completaste el algoritmo.";

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
                "❌ Ese paso no corresponde. Inténtalo otra vez.";
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
            "Completaste correctamente los 3 algoritmos.";

        foreach (Button boton in botonesOpciones)
        {
            boton.gameObject.SetActive(false);
        }

        btnComprobar.gameObject.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);
    }
}