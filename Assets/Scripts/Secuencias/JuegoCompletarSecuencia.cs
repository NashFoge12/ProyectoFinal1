using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoCompletarSecuencia : MonoBehaviour
{
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

    public Sprite fondoColores;
    public Sprite fondoFlechas;
    public Sprite fondoNumeros;


    // =====================================================
    // TITULOS EN IMAGEN
    // =====================================================

    [Header("Titulos")]
    public Image imagenTitulo;

    public Sprite tituloColores;
    public Sprite tituloFlechas;
    public Sprite tituloNumeros;


    // =====================================================
    // SECUENCIAS EN IMAGEN
    // =====================================================

    [Header("Secuencia principal")]
    public Image imagenSecuencia;

    public Sprite secuenciaColores;
    public Sprite secuenciaFlechas;
    public Sprite secuenciaNumeros;


    // =====================================================
    // BOTONES DE OPCIONES
    // =====================================================

    [Header("Botones de opciones")]
    public Button[] botonesOpciones;


    // =====================================================
    // OPCIONES EJERCICIO 1 - COLORES
    // =====================================================

    [Header("Opciones - Colores")]

    // ROJO - AZUL
    public Sprite coloresCorrecta;

    // AZUL - ROJO
    public Sprite coloresIncorrecta1;

    // ROJO - ROJO
    public Sprite coloresIncorrecta2;


    // =====================================================
    // OPCIONES EJERCICIO 2 - FLECHAS
    // =====================================================

    [Header("Opciones - Flechas")]

    // ARRIBA - ABAJO
    public Sprite flechasCorrecta;

    // ABAJO - ARRIBA
    public Sprite flechasIncorrecta1;

    // ARRIBA - ARRIBA
    public Sprite flechasIncorrecta2;


    // =====================================================
    // OPCIONES EJERCICIO 3 - NUMEROS
    // =====================================================

    [Header("Opciones - Numeros")]

    // 2 - 3
    public Sprite numerosCorrecta;

    // 3 - 2
    public Sprite numerosIncorrecta1;

    // 1 - 2
    public Sprite numerosIncorrecta2;


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

    private int opcionSeleccionada = -1;

    // Guarda la posición donde está la respuesta correcta
    private int indiceCorrectoActual = -1;


    // =====================================================
    // POSICIONES DE LAS RESPUESTAS CORRECTAS
    //
    // Ejemplo:
    //
    // 2, 0, 1
    //
    // Ejercicio 1 -> botón 3
    // Ejercicio 2 -> botón 1
    // Ejercicio 3 -> botón 2
    //
    // Cada partida cambia.
    // =====================================================

    private List<int> posicionesCorrectas =
        new List<int>();


    // =====================================================
    // PREGUNTAS
    // =====================================================

    private string[] preguntas =
    {
        "Descubre el patrón y completa lo que falta.",
        "Observa la repetición y completa la secuencia.",
        "Encuentra los números que faltan."
    };


    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        // =================================================
        // COMPROBAR QUE EXISTAN 3 BOTONES
        // =================================================

        if (botonesOpciones == null ||
            botonesOpciones.Length < 3)
        {
            Debug.LogError(
                "Debes asignar 3 botones en Botones Opciones."
            );

            return;
        }


        // =================================================
        // CONECTAR LOS BOTONES DE RESPUESTA
        // =================================================

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            int indice = i;

            botonesOpciones[i].onClick.AddListener(
                () => SeleccionarOpcion(indice)
            );
        }


        // =================================================
        // BOTÓN COMPROBAR
        // =================================================

        btnComprobar.onClick.AddListener(
            ComprobarRespuesta
        );


        // =================================================
        // BOTÓN SIGUIENTE
        // =================================================

        btnSiguiente.onClick.AddListener(
            SiguienteEjercicio
        );


        // =================================================
        // PREPARAR POSICIONES DE RESPUESTAS CORRECTAS
        // =================================================

        PrepararPosicionesCorrectas();


        // =================================================
        // CARGAR PRIMER EJERCICIO
        // =================================================

        CargarEjercicio();
    }


    // =====================================================
    // PREPARAR POSICIONES CORRECTAS
    // =====================================================

    void PrepararPosicionesCorrectas()
    {
        posicionesCorrectas.Clear();


        // Tenemos 3 posiciones posibles:
        //
        // 0 = Btn_Opcion1
        // 1 = Btn_Opcion2
        // 2 = Btn_Opcion3

        posicionesCorrectas.Add(0);
        posicionesCorrectas.Add(1);
        posicionesCorrectas.Add(2);


        // =================================================
        // MEZCLAR LAS 3 POSICIONES
        // =================================================

        for (int i = 0; i < posicionesCorrectas.Count; i++)
        {
            int posicionAleatoria =
                Random.Range(
                    i,
                    posicionesCorrectas.Count
                );


            int temporal =
                posicionesCorrectas[i];


            posicionesCorrectas[i] =
                posicionesCorrectas[posicionAleatoria];


            posicionesCorrectas[posicionAleatoria] =
                temporal;
        }
    }


    // =====================================================
    // CARGAR EJERCICIO
    // =====================================================

    void CargarEjercicio()
    {
        // =================================================
        // REINICIAR SELECCIÓN
        // =================================================

        opcionSeleccionada = -1;

        indiceCorrectoActual = -1;


        // =================================================
        // VOLVER A MOSTRAR SECUENCIA
        // =================================================

        if (imagenSecuencia != null)
        {
            imagenSecuencia
                .gameObject
                .SetActive(true);
        }


        // =================================================
        // MENSAJE
        // =================================================

        if (txtResultado != null)
        {
            txtResultado.text =
                "Selecciona la opción que completa la secuencia.";
        }


        // =================================================
        // PREGUNTA
        // =================================================

        if (txtPregunta != null)
        {
            txtPregunta.text =
                preguntas[ejercicioActual];
        }


        // =================================================
        // PROGRESO
        // =================================================

        if (txtProgreso != null)
        {
            txtProgreso.text =
                "Ejercicio\n" +
                (ejercicioActual + 1) +
                " de 3";
        }


        // =================================================
        // EJERCICIO 1
        // COLORES
        // =================================================

        if (ejercicioActual == 0)
        {
            CambiarImagen(
                imagenFondo,
                fondoColores,
                false
            );


            CambiarImagen(
                imagenTitulo,
                tituloColores,
                false
            );


            CambiarImagen(
                imagenSecuencia,
                secuenciaColores,
                false
            );
        }


        // =================================================
        // EJERCICIO 2
        // FLECHAS
        // =================================================

        else if (ejercicioActual == 1)
        {
            CambiarImagen(
                imagenFondo,
                fondoFlechas,
                false
            );


            CambiarImagen(
                imagenTitulo,
                tituloFlechas,
                false
            );


            CambiarImagen(
                imagenSecuencia,
                secuenciaFlechas,
                false
            );
        }


        // =================================================
        // EJERCICIO 3
        // NUMEROS
        // =================================================

        else if (ejercicioActual == 2)
        {
            CambiarImagen(
                imagenFondo,
                fondoNumeros,
                false
            );


            CambiarImagen(
                imagenTitulo,
                tituloNumeros,
                false
            );


            CambiarImagen(
                imagenSecuencia,
                secuenciaNumeros,
                false
            );
        }


        // =================================================
        // CARGAR OPCIONES
        // =================================================

        CargarOpciones();


        // =================================================
        // BOTONES
        // =================================================

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


    // =====================================================
    // CARGAR OPCIONES
    // =====================================================

    void CargarOpciones()
    {
        // =================================================
        // OBTENER SPRITES DEL EJERCICIO ACTUAL
        // =================================================

        Sprite correcta = null;

        Sprite incorrecta1 = null;

        Sprite incorrecta2 = null;


        // =================================================
        // EJERCICIO 1 - COLORES
        // =================================================

        if (ejercicioActual == 0)
        {
            correcta =
                coloresCorrecta;


            incorrecta1 =
                coloresIncorrecta1;


            incorrecta2 =
                coloresIncorrecta2;
        }


        // =================================================
        // EJERCICIO 2 - FLECHAS
        // =================================================

        else if (ejercicioActual == 1)
        {
            correcta =
                flechasCorrecta;


            incorrecta1 =
                flechasIncorrecta1;


            incorrecta2 =
                flechasIncorrecta2;
        }


        // =================================================
        // EJERCICIO 3 - NUMEROS
        // =================================================

        else if (ejercicioActual == 2)
        {
            correcta =
                numerosCorrecta;


            incorrecta1 =
                numerosIncorrecta1;


            incorrecta2 =
                numerosIncorrecta2;
        }


        // =================================================
        // OBTENER POSICIÓN CORRECTA
        // =================================================

        int posicionCorrecta =
            posicionesCorrectas[ejercicioActual];


        // Guardamos dónde está
        indiceCorrectoActual =
            posicionCorrecta;


        // =================================================
        // BUSCAR LAS OTRAS DOS POSICIONES
        // =================================================

        List<int> posicionesIncorrectas =
            new List<int>();


        for (int i = 0; i < 3; i++)
        {
            if (i != posicionCorrecta)
            {
                posicionesIncorrectas.Add(i);
            }
        }


        // =================================================
        // MEZCLAR TAMBIÉN LAS INCORRECTAS
        // =================================================

        if (Random.value > 0.5f)
        {
            int temporal =
                posicionesIncorrectas[0];


            posicionesIncorrectas[0] =
                posicionesIncorrectas[1];


            posicionesIncorrectas[1] =
                temporal;
        }


        // =================================================
        // ACTIVAR LOS 3 BOTONES
        // =================================================

        for (int i = 0; i < 3; i++)
        {
            botonesOpciones[i]
                .gameObject
                .SetActive(true);


            botonesOpciones[i]
                .interactable = true;
        }


        // =================================================
        // COLOCAR SPRITE CORRECTO
        // =================================================

        botonesOpciones[posicionCorrecta]
            .image
            .sprite = correcta;


        // IMPORTANTE:
        // Lo dejamos FALSE para que mantenga
        // el tamaño del botón que configuraste.
        botonesOpciones[posicionCorrecta]
            .image
            .preserveAspect = false;


        // =================================================
        // COLOCAR INCORRECTA 1
        // =================================================

        botonesOpciones[posicionesIncorrectas[0]]
            .image
            .sprite = incorrecta1;


        botonesOpciones[posicionesIncorrectas[0]]
            .image
            .preserveAspect = false;


        // =================================================
        // COLOCAR INCORRECTA 2
        // =================================================

        botonesOpciones[posicionesIncorrectas[1]]
            .image
            .sprite = incorrecta2;


        botonesOpciones[posicionesIncorrectas[1]]
            .image
            .preserveAspect = false;
    }


    // =====================================================
    // SELECCIONAR OPCIÓN
    // =====================================================

    void SeleccionarOpcion(int indice)
    {
        opcionSeleccionada =
            indice;


        if (txtResultado != null)
        {
            txtResultado.text =
                "Opción seleccionada. Pulsa COMPROBAR.";
        }
    }


    // =====================================================
    // COMPROBAR RESPUESTA
    // =====================================================

    void ComprobarRespuesta()
    {
        // =================================================
        // NO HA SELECCIONADO NINGUNA
        // =================================================

        if (opcionSeleccionada == -1)
        {
            if (txtResultado != null)
            {
                txtResultado.text =
                    "Selecciona una opción primero.";
            }

            return;
        }


        // =================================================
        // RESPUESTA CORRECTA
        // =================================================

        if (opcionSeleccionada ==
            indiceCorrectoActual)
        {
            if (txtResultado != null)
            {
                txtResultado.text =
                    "⭐ ¡Correcto! Completaste la secuencia.";
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


            // Bloquear las respuestas
            for (int i = 0; i < 3; i++)
            {
                botonesOpciones[i]
                    .interactable = false;
            }
        }


        // =================================================
        // RESPUESTA INCORRECTA
        // =================================================

        else
        {
            if (txtResultado != null)
            {
                txtResultado.text =
                    "❌ Esa no es la respuesta. ¡Inténtalo otra vez!";
            }


            // Permitir seleccionar otra
            opcionSeleccionada = -1;
        }
    }


    // =====================================================
    // SIGUIENTE EJERCICIO
    // =====================================================

    void SiguienteEjercicio()
    {
        ejercicioActual++;


        // =================================================
        // TODAVÍA QUEDAN EJERCICIOS
        // =================================================

        if (ejercicioActual < 3)
        {
            CargarEjercicio();
        }


        // =================================================
        // TERMINÓ LOS 3
        // =================================================

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
        // =================================================
        // MENSAJE FINAL
        // =================================================

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
                "⭐ Completaste correctamente las 3 secuencias.";
        }


        // =================================================
        // OCULTAR SECUENCIA
        // =================================================

        if (imagenSecuencia != null)
        {
            imagenSecuencia
                .gameObject
                .SetActive(false);
        }


        // =================================================
        // OCULTAR LAS 3 OPCIONES
        // =================================================

        for (int i = 0; i < 3; i++)
        {
            botonesOpciones[i]
                .gameObject
                .SetActive(false);
        }


        // =================================================
        // OCULTAR COMPROBAR
        // =================================================

        if (btnComprobar != null)
        {
            btnComprobar
                .gameObject
                .SetActive(false);
        }


        // =================================================
        // OCULTAR SIGUIENTE
        // =================================================

        if (btnSiguiente != null)
        {
            btnSiguiente
                .gameObject
                .SetActive(false);
        }
    }
}