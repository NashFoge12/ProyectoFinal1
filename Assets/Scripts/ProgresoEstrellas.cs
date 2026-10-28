
using UnityEngine;

public static class ProgresoEstrellas
{
    // Cada misión tiene 3 ejercicios.
    private const int MAX_ESTRELLAS = 3;

    // =========================
    // OBTENER ESTRELLAS
    // =========================

    public static int ObtenerEstrellas(
        string categoria,
        int mision)
    {
        string clave = CrearClave(categoria, mision);

        return Mathf.Clamp(
            PlayerPrefs.GetInt(clave, 0),
            0,
            MAX_ESTRELLAS
        );
    }

    // =========================
    // GUARDAR EJERCICIO
    // =========================

    public static void GuardarEjercicio(
        string categoria,
        int mision,
        int ejercicioCompletado)
    {
        // IMPORTANTE:
        // En modo Repasar no se guardan estrellas.
        if (NavegacionEscenas.EsModoRepaso)
        {
            return;
        }

        string clave = CrearClave(categoria, mision);

        int progresoAnterior =
            ObtenerEstrellas(categoria, mision);

        // Nunca perder las estrellas ya ganadas.
        int nuevoProgreso = Mathf.Clamp(
            Mathf.Max(
                progresoAnterior,
                ejercicioCompletado
            ),
            0,
            MAX_ESTRELLAS
        );

        PlayerPrefs.SetInt(clave, nuevoProgreso);
        PlayerPrefs.Save();
    }

    // =========================
    // CREAR CLAVE DE GUARDADO
    // =========================

    private static string CrearClave(
        string categoria,
        int mision)
    {
        return "MundoPujllay_Estrellas_" +
               categoria + "_Mision" + mision;
    }
}
