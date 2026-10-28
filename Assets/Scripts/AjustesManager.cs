
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class AjustesManager : MonoBehaviour
{
    // ========================================
    // PANEL DE AJUSTES
    // ========================================

    [Header("PANEL DE AJUSTES")]
    [SerializeField] private GameObject panelAjustesCompleto;
    [SerializeField] private Button btnCerrar;

    // ========================================
    // SOBRE NOSOTROS
    // ========================================

    [Header("SOBRE NOSOTROS")]
    [SerializeField] private GameObject panelSobreNosotros;
    [SerializeField] private Button btnSobreNosotros;
    [SerializeField] private Button btnCerrarSobreNosotros;

    // ========================================
    // MUSICA
    // ========================================

    [Header("MUSICA")]
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private TMP_Text txtPorcentajeMusica;

    // ========================================
    // SONIDOS
    // ========================================

    [Header("SONIDOS")]
    [SerializeField] private Slider sliderSonidos;
    [SerializeField] private TMP_Text txtPorcentajeSonidos;

    // ========================================
    // AUDIO MIXER
    // ========================================

    [Header("AUDIO MIXER")]
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField]
    private string parametroMusica = "VolumenMusica";

    [SerializeField]
    private string parametroSonidos = "VolumenSonidos";

    // ========================================
    // VALORES INICIALES
    // ========================================

    [Header("VALORES INICIALES")]
    [Range(0f, 100f)]
    [SerializeField] private float volumenInicialMusica = 40f;

    [Range(0f, 100f)]
    [SerializeField] private float volumenInicialSonidos = 50f;

    [Tooltip("Activalo para borrar los volumenes guardados una vez. Se desactiva automaticamente al iniciar.")]
    [SerializeField] private bool restablecerVolumenesUnaVez = false;

    // ========================================
    // PIE DE PAGINA
    // ========================================

    [Header("PIE DE PAGINA")]
    [SerializeField] private TMP_Text txtPiePagina;

    [SerializeField]
    private Color colorCorazon =
        new Color(0.66f, 0.33f, 0.97f, 1f);

    private GameObject corazonPiePagina;

    // ========================================
    // GUARDADO
    // ========================================

    private const string CLAVE_MUSICA =
        "MundoPujllay_VolumenMusica";

    private const string CLAVE_SONIDOS =
        "MundoPujllay_VolumenSonidos";

    // ========================================
    // INICIALIZACION
    // ========================================

    private void Awake()
    {
        // Botones

        if (btnCerrar != null)
            btnCerrar.onClick.AddListener(CerrarAjustes);

        if (btnSobreNosotros != null)
            btnSobreNosotros.onClick.AddListener(
                AbrirSobreNosotros
            );

        if (btnCerrarSobreNosotros != null)
            btnCerrarSobreNosotros.onClick.AddListener(
                CerrarSobreNosotros
            );

        // Slider de musica

        if (sliderMusica != null)
        {
            sliderMusica.minValue = 0f;
            sliderMusica.maxValue = 100f;
            sliderMusica.wholeNumbers = true;
            sliderMusica.interactable = true;
            sliderMusica.direction =
                Slider.Direction.LeftToRight;

            sliderMusica.onValueChanged.AddListener(
                CambiarVolumenMusica
            );
        }

        // Slider de sonidos

        if (sliderSonidos != null)
        {
            sliderSonidos.minValue = 0f;
            sliderSonidos.maxValue = 100f;
            sliderSonidos.wholeNumbers = true;
            sliderSonidos.interactable = true;
            sliderSonidos.direction =
                Slider.Direction.LeftToRight;

            sliderSonidos.onValueChanged.AddListener(
                CambiarVolumenSonidos
            );
        }

        // Pie de pagina

        PrepararPiePagina();
    }

    private void Start()
    {
        // Restablecer configuracion si se solicita

        if (restablecerVolumenesUnaVez)
        {
            PlayerPrefs.DeleteKey(CLAVE_MUSICA);
            PlayerPrefs.DeleteKey(CLAVE_SONIDOS);
            PlayerPrefs.Save();

            restablecerVolumenesUnaVez = false;
        }

        // Recuperar los valores guardados

        float musica = PlayerPrefs.GetFloat(
            CLAVE_MUSICA,
            volumenInicialMusica
        );

        float sonidos = PlayerPrefs.GetFloat(
            CLAVE_SONIDOS,
            volumenInicialSonidos
        );

        musica = Mathf.Clamp(musica, 0f, 100f);
        sonidos = Mathf.Clamp(sonidos, 0f, 100f);

        // Colocar los sliders en el porcentaje correcto

        if (sliderMusica != null)
            sliderMusica.SetValueWithoutNotify(musica);

        if (sliderSonidos != null)
            sliderSonidos.SetValueWithoutNotify(sonidos);

        // Actualizar textos y volumenes reales

        AplicarVolumenMusica(musica);
        AplicarVolumenSonidos(sonidos);

        // Cerrar paneles al iniciar

        if (panelSobreNosotros != null)
            panelSobreNosotros.SetActive(false);

        if (panelAjustesCompleto != null)
            panelAjustesCompleto.SetActive(false);
    }

    private void OnDestroy()
    {
        if (btnCerrar != null)
            btnCerrar.onClick.RemoveListener(CerrarAjustes);

        if (btnSobreNosotros != null)
            btnSobreNosotros.onClick.RemoveListener(
                AbrirSobreNosotros
            );

        if (btnCerrarSobreNosotros != null)
            btnCerrarSobreNosotros.onClick.RemoveListener(
                CerrarSobreNosotros
            );

        if (sliderMusica != null)
            sliderMusica.onValueChanged.RemoveListener(
                CambiarVolumenMusica
            );

        if (sliderSonidos != null)
            sliderSonidos.onValueChanged.RemoveListener(
                CambiarVolumenSonidos
            );
    }

    // ========================================
    // ABRIR Y CERRAR AJUSTES
    // ========================================

    public void AbrirAjustes()
    {
        if (panelAjustesCompleto != null)
            panelAjustesCompleto.SetActive(true);

        if (panelSobreNosotros != null)
            panelSobreNosotros.SetActive(false);
    }

    public void CerrarAjustes()
    {
        if (panelSobreNosotros != null)
            panelSobreNosotros.SetActive(false);

        if (panelAjustesCompleto != null)
            panelAjustesCompleto.SetActive(false);
    }

    // ========================================
    // SOBRE NOSOTROS
    // ========================================

    public void AbrirSobreNosotros()
    {
        if (panelSobreNosotros != null)
            panelSobreNosotros.SetActive(true);
    }

    public void CerrarSobreNosotros()
    {
        if (panelSobreNosotros != null)
            panelSobreNosotros.SetActive(false);
    }

    // ========================================
    // MUSICA
    // ========================================

    public void CambiarVolumenMusica(float valor)
    {
        valor = Mathf.Clamp(valor, 0f, 100f);

        AplicarVolumenMusica(valor);

        PlayerPrefs.SetFloat(CLAVE_MUSICA, valor);
        PlayerPrefs.Save();
    }

    private void AplicarVolumenMusica(float valor)
    {
        valor = Mathf.Clamp(valor, 0f, 100f);

        if (txtPorcentajeMusica != null)
        {
            txtPorcentajeMusica.text =
                Mathf.RoundToInt(valor) + "%";
        }

        if (audioMixer != null)
        {
            bool aplicado = audioMixer.SetFloat(
                parametroMusica,
                ConvertirADecibelios(valor)
            );

            if (!aplicado)
            {
                Debug.LogWarning(
                    "No se encontro el parametro de musica: " +
                    parametroMusica
                );
            }
        }
    }

    // ========================================
    // SONIDOS
    // ========================================

    public void CambiarVolumenSonidos(float valor)
    {
        valor = Mathf.Clamp(valor, 0f, 100f);

        AplicarVolumenSonidos(valor);

        PlayerPrefs.SetFloat(CLAVE_SONIDOS, valor);
        PlayerPrefs.Save();
    }

    private void AplicarVolumenSonidos(float valor)
    {
        valor = Mathf.Clamp(valor, 0f, 100f);

        if (txtPorcentajeSonidos != null)
        {
            txtPorcentajeSonidos.text =
                Mathf.RoundToInt(valor) + "%";
        }

        if (audioMixer != null)
        {
            bool aplicado = audioMixer.SetFloat(
                parametroSonidos,
                ConvertirADecibelios(valor)
            );

            if (!aplicado)
            {
                Debug.LogWarning(
                    "No se encontro el parametro de sonidos: " +
                    parametroSonidos
                );
            }
        }
    }

    // ========================================
    // CONVERTIR PORCENTAJE A DECIBELIOS
    // ========================================

    private float ConvertirADecibelios(float porcentaje)
    {
        if (porcentaje <= 0f)
            return -80f;

        float volumen = porcentaje / 100f;

        return Mathf.Clamp(
            20f * Mathf.Log10(volumen),
            -80f,
            0f
        );
    }

    // ========================================
    // PIE DE PAGINA
    // ========================================

    private void PrepararPiePagina()
    {
        if (txtPiePagina == null)
            return;

        txtPiePagina.text =
            "Hecho con     para exploradores";

        txtPiePagina.alignment =
            TextAlignmentOptions.Center;

        CrearCorazonPiePagina();
    }

    private void CrearCorazonPiePagina()
    {
        if (txtPiePagina == null)
            return;

        Transform existente =
            txtPiePagina.transform.Find(
                "Corazon_PiePagina"
            );

        if (existente != null)
        {
            corazonPiePagina = existente.gameObject;

            CorazonDibujado corazonExistente =
                corazonPiePagina.GetComponent<CorazonDibujado>();

            if (corazonExistente != null)
                corazonExistente.color = colorCorazon;

            return;
        }

        corazonPiePagina = new GameObject(
            "Corazon_PiePagina",
            typeof(RectTransform),
            typeof(CanvasRenderer)
        );

        corazonPiePagina.transform.SetParent(
            txtPiePagina.transform,
            false
        );

        RectTransform rect =
            corazonPiePagina.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.anchoredPosition =
            new Vector2(-60f, 0f);

        rect.sizeDelta = new Vector2(30f, 30f);
        rect.localScale = Vector3.one;

        CorazonDibujado corazon =
            corazonPiePagina.AddComponent<CorazonDibujado>();

        corazon.color = colorCorazon;
        corazon.raycastTarget = false;
    }

    // ========================================
    // CORAZON DIBUJADO POR CODIGO
    // ========================================

    private class CorazonDibujado : MaskableGraphic
    {
        protected override void OnPopulateMesh(
            VertexHelper vh
        )
        {
            vh.Clear();

            const int puntos = 80;

            float ancho = rectTransform.rect.width;
            float alto = rectTransform.rect.height;

            UIVertex vertice = UIVertex.simpleVert;
            vertice.color = color;
            vertice.position = Vector3.zero;

            vh.AddVert(vertice);

            for (int i = 0; i <= puntos; i++)
            {
                float t =
                    i * Mathf.PI * 2f / puntos;

                float x =
                    16f * Mathf.Pow(
                        Mathf.Sin(t), 3f
                    );

                float y =
                    13f * Mathf.Cos(t)
                    - 5f * Mathf.Cos(2f * t)
                    - 2f * Mathf.Cos(3f * t)
                    - Mathf.Cos(4f * t);

                vertice.position = new Vector3(
                    x * ancho / 36f,
                    y * alto / 34f,
                    0f
                );

                vh.AddVert(vertice);
            }

            for (int i = 1; i <= puntos; i++)
            {
                vh.AddTriangle(
                    0,
                    i,
                    i + 1
                );
            }
        }
    }
}
