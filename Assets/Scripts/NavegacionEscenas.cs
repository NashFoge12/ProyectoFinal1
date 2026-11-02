
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavegacionEscenas : MonoBehaviour
{
    // false = Jugar normalmente
    // true = Repasar sin guardar estrellas
    public static bool EsModoRepaso = false;

    // MENÚ PRINCIPAL

    public void IrAMenuPrincipal()
    {
        EsModoRepaso = false;
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void IrAJugar()
    {
        EsModoRepaso = false;
        SceneManager.LoadScene("PensamientoComputacional");
    }

    public void IrARepasar()
    {
        EsModoRepaso = true;
        SceneManager.LoadScene("PensamientoComputacional");
    }

    // PERFIL Y LOGROS

    public void IrAPerfilJugador()
    {
        SceneManager.LoadScene("PerfilJugador");
    }

    public void IrALogros()
    {
        SceneManager.LoadScene("Logros");
    }

    public void IrAPensamientoComputacional()
    {
        SceneManager.LoadScene("PensamientoComputacional");
    }

    // ALGORITMOS

    public void IrAAlgoritmos()
    {
        SceneManager.LoadScene("Algoritmos");
    }

    public void IrAMision1Algoritmos()
    {
        SceneManager.LoadScene("Mision1_Algoritmos");
    }

    public void IrAMision2Algoritmos()
    {
        SceneManager.LoadScene("Mision2_Algoritmos");
    }

    public void IrAMision3Algoritmos()
    {
        SceneManager.LoadScene("Mision3_Algoritmos");
    }

    // SECUENCIAS

    public void IrASecuencias()
    {
        SceneManager.LoadScene("Secuencias");
    }

    public void IrAMision1Secuencias()
    {
        SceneManager.LoadScene("Mision1_Secuencias");
    }

    public void IrAMision2Secuencias()
    {
        SceneManager.LoadScene("Mision2_Secuencias");
    }

    public void IrAMision3Secuencias()
    {
        SceneManager.LoadScene("Mision3_Secuencias");
    }

    // PATRONES

    public void IrAPatrones()
    {
        SceneManager.LoadScene("Patrones");
    }

    public void IrAMision1Patrones()
    {
        SceneManager.LoadScene("Mision1_Patrones");
    }

    public void IrAMision2Patrones()
    {
        SceneManager.LoadScene("Mision2_Patrones");
    }

    public void IrAMision3Patrones()
    {
        SceneManager.LoadScene("Mision3_Patrones");
    }

    // DETECTAR ERRORES

    public void IrADetectarErrores()
    {
        SceneManager.LoadScene("Detectar Errores");
    }

    public void IrAMision1DetectarErrores()
    {
        SceneManager.LoadScene("Mision1_DetectarErrores");
    }

    public void IrAMision2DetectarErrores()
    {
        SceneManager.LoadScene("Mision2_DetectarErrores");
    }

    public void IrAMision3DetectarErrores()
    {
        SceneManager.LoadScene("Mision3_DetectarErrores");
    }

    // PROGRAMACIÓN

    public void IrAProgramacion()
    {
        SceneManager.LoadScene("Programacion");
    }

    public void IrAMision1Programacion()
    {
        SceneManager.LoadScene("Mision1_Programacion");
    }

    public void IrAMision2Programacion()
    {
        SceneManager.LoadScene("Mision2_Programacion");
    }

    public void IrAMision3Programacion()
    {
        SceneManager.LoadScene("Mision3_Programacion");
    }

    // DESCOMPOSICIÓN

    public void IrADescomposicion()
    {
        SceneManager.LoadScene("Descomposicion");
    }

    public void IrAMision1Descomposicion()
    {
        SceneManager.LoadScene("Mision1_Descomposicion");
    }

    public void IrAMision2Descomposicion()
    {
        SceneManager.LoadScene("Mision2_Descomposicion");
    }

    public void IrAMision3Descomposicion()
    {
        SceneManager.LoadScene("Mision3_Descomposicion");
    }

    // SALIR DEL JUEGO

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo de Mundo Pujllay...");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
