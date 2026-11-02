
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [Header("Barra de carga")]
    public Image barraRelleno;

    [Header("Textos")]
    public TMP_Text textoPorcentaje;
    public TMP_Text textoCarga;

    [Header("Configuración")]
    public string escenaMenu = "MenuPrincipal";
    public float duracionCarga = 5f;

    private void Start()
    {
        StartCoroutine(CargarJuego());
    }

    IEnumerator CargarJuego()
    {
        float progreso = 0f;

        barraRelleno.fillAmount = 0f;
        textoPorcentaje.text = "0%";
        textoCarga.text = "Preparando tu aventura...";

        while (progreso < 1f)
        {
            progreso += Time.deltaTime / duracionCarga;
            progreso = Mathf.Clamp01(progreso);

            barraRelleno.fillAmount = progreso;

            int porcentaje = Mathf.RoundToInt(progreso * 100);
            textoPorcentaje.text = porcentaje + "%";

            yield return null;
        }

        textoCarga.text = "¡Tu aventura está lista!";

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(escenaMenu);
    }
}
