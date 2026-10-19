
using UnityEngine;

public static class ProgresoEstrellas
{
    // Cada misión tiene 3 ejercicios.
    private const int MAX_ESTRELLAS = 3;

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

    public static void GuardarEjercicio(
        string categoria,
        int mision,
        int ejercicioCompletado)
    {
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

    private static string CrearClave(
        string categoria,
        int mision)
    {
        return "MundoPujllay_Estrellas_" +
               categoria + "_Mision" + mision;
    }
}
