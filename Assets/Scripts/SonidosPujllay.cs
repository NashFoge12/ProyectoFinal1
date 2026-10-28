
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SonidosPujllay : MonoBehaviour
{
    public static SonidosPujllay Instancia;

    [Header("AUDIO MIXER")]
    [SerializeField] private AudioMixerGroup grupoSonidos;

    [Header("VOLUMEN DE EFECTOS")]
    [Range(0f, 1f)]
    [SerializeField] private float intensidad = 0.35f;

    [Header("BOTONES")]
    [SerializeField] private bool sonidoAutomaticoBotones = true;

    private AudioSource fuenteAudio;

    private AudioClip sonidoClic;
    private AudioClip sonidoCorrecto;
    private AudioClip sonidoIncorrecto;
    private AudioClip sonidoCelebracion;

    private readonly HashSet<Button> botonesRegistrados =
        new HashSet<Button>();

    private const int FRECUENCIA = 44100;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;

        DontDestroyOnLoad(gameObject);

        fuenteAudio = gameObject.AddComponent<AudioSource>();

        fuenteAudio.playOnAwake = false;
        fuenteAudio.loop = false;
        fuenteAudio.spatialBlend = 0f;
        fuenteAudio.volume = 1f;
        fuenteAudio.outputAudioMixerGroup = grupoSonidos;

        GenerarSonidos();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void Start()
    {
        RegistrarBotones();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        botonesRegistrados.Clear();
        RegistrarBotones();
    }

    private void RegistrarBotones()
    {
        if (!sonidoAutomaticoBotones)
            return;

        Button[] botones =
            FindObjectsByType<Button>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (Button boton in botones)
        {
            if (boton == null)
                continue;

            if (botonesRegistrados.Contains(boton))
                continue;

            boton.onClick.AddListener(ReproducirClic);

            botonesRegistrados.Add(boton);
        }
    }

    private void GenerarSonidos()
    {
        sonidoClic = CrearMelodia(
            "Pujllay_Clic",
            new float[] { 740f, 980f },
            0.045f,
            0.10f
        );

        sonidoCorrecto = CrearMelodia(
            "Pujllay_Correcto",
            new float[] { 523.25f, 659.25f, 783.99f },
            0.12f,
            0.18f
        );

        sonidoIncorrecto = CrearMelodia(
            "Pujllay_Incorrecto",
            new float[] { 392f, 329.63f },
            0.15f,
            0.13f
        );

        sonidoCelebracion = CrearMelodia(
            "Pujllay_Celebracion",
            new float[]
            {
                523.25f,
                659.25f,
                783.99f,
                1046.50f
            },
            0.13f,
            0.22f
        );
    }

    private AudioClip CrearMelodia(
        string nombre,
        float[] notas,
        float duracionNota,
        float duracionCola
    )
    {
        float duracionTotal =
            notas.Length * duracionNota + duracionCola;

        int cantidadMuestras =
            Mathf.CeilToInt(duracionTotal * FRECUENCIA);

        float[] datos = new float[cantidadMuestras];

        for (int n = 0; n < notas.Length; n++)
        {
            float frecuenciaNota = notas[n];

            int inicio =
                Mathf.RoundToInt(n * duracionNota * FRECUENCIA);

            int muestrasNota =
                Mathf.RoundToInt(
                    (duracionNota + duracionCola) * FRECUENCIA
                );

            for (int i = 0; i < muestrasNota; i++)
            {
                int posicion = inicio + i;

                if (posicion >= datos.Length)
                    break;

                float tiempo = i / (float)FRECUENCIA;

                float progreso =
                    i / (float)Mathf.Max(1, muestrasNota - 1);

                float ataque =
                    Mathf.Clamp01(tiempo / 0.008f);

                float decaimiento =
                    Mathf.Pow(1f - progreso, 2.5f);

                float envolvente = ataque * decaimiento;

                float ondaPrincipal =
                    Mathf.Sin(
                        2f * Mathf.PI * frecuenciaNota * tiempo
                    );

                float armonico =
                    Mathf.Sin(
                        2f * Mathf.PI * frecuenciaNota * 2f * tiempo
                    ) * 0.18f;

                float muestra =
                    (ondaPrincipal + armonico) *
                    envolvente *
                    0.24f;

                datos[posicion] += muestra;
            }
        }

        for (int i = 0; i < datos.Length; i++)
        {
            datos[i] = Mathf.Clamp(datos[i], -1f, 1f);
        }

        AudioClip clip = AudioClip.Create(
            nombre,
            cantidadMuestras,
            1,
            FRECUENCIA,
            false
        );

        clip.SetData(datos, 0);

        return clip;
    }

    private void Reproducir(AudioClip clip)
    {
        if (fuenteAudio == null || clip == null)
            return;

        fuenteAudio.PlayOneShot(clip, intensidad);
    }

    public void ReproducirClic()
    {
        Reproducir(sonidoClic);
    }

    public void ReproducirCorrecto()
    {
        Reproducir(sonidoCorrecto);
    }

    public void ReproducirIncorrecto()
    {
        Reproducir(sonidoIncorrecto);
    }

    public void ReproducirCelebracion()
    {
        Reproducir(sonidoCelebracion);
    }
}
