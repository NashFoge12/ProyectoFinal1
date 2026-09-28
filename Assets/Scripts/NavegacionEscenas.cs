using UnityEngine;
using UnityEngine.SceneManagement;

public class NavegacionEscenas : MonoBehaviour
{
    // =========================
    // MENÚ PRINCIPAL
    // =========================

    public void IrAMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    // =========================
    // PENSAMIENTO COMPUTACIONAL
    // =========================

    public void IrAPensamientoComputacional()
    {
        SceneManager.LoadScene("PensamientoComputacional");
    }

    // =========================
    // ALGORITMOS
    // =========================

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

    // =========================
    // SECUENCIAS
    // =========================

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
}