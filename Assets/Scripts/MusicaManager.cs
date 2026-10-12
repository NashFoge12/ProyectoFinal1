
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicaManager : MonoBehaviour
{
    private static MusicaManager instancia;

    private AudioSource audioSource;

    void Awake()
    {
        // Evita tener varias musicas sonando.
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;

        // Mantener la musica entre escenas.
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        // Configurar musica de fondo.
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.spatialBlend = 0f;

        // Reproducir si tiene un audio asignado.
        if (audioSource.clip != null &&
            !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
