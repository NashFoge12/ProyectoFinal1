
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerfilJugadorManager : MonoBehaviour
{
    [Header("Nombre")]
    public TMP_InputField inputNombre;
    public TMP_Text txtNombreJugador;
    public TMP_Text txtMensaje;

    [Header("Imagenes principales")]
    public Image imgAvatarSeleccionado;
    public Image imgFotoPersonal;
    public Image imgMarcoFoto;

    [Header("Avatares")]
    public Button[] botonesAvatares = new Button[6];
    public Sprite[] spritesAvatares = new Sprite[6];

    [Header("Botones")]
    public Button btnSubirFoto;
    public Button btnGuardarPerfil;

    [Header("Texto del pie")]
    public TMP_Text txtPie;

    [Header("Estadisticas")]
    public TMP_Text txtXP;
    public TMP_Text txtMisiones;
    public int totalMisiones = 15;

    [Header("Experiencia")]
    public int xpPorEstrella = 10;

    private const string KEY_NOMBRE = "Perfil_Nombre";
    private const string KEY_TIPO = "Perfil_TipoVisual";
    private const string KEY_AVATAR = "Perfil_Avatar";
    private const string ARCHIVO_FOTO = "avatar_personalizado.png";

    private int tipoVisual = 0;
    private int avatarActual = 2;

    private Sprite spriteFotoPersonal;
    private Texture2D texturaFotoPersonal;
    private bool fotoCargada = false;

    // Estas categorías corresponden a las 15 misiones.
    private readonly string[] categorias =
    {
        "Algoritmos",
        "Secuencias",
        "Patrones",
        "Descomposicion",
        "DetectarErrores"
    };

    private string RutaFoto =>
        Path.Combine(Application.persistentDataPath, ARCHIVO_FOTO);

    private void Awake()
    {
        ConfigurarCampoNombre();
        ConectarBotones();
    }

    private void Start()
    {
        if (txtPie != null)
            txtPie.text = "¡Sigue explorando y aprendiendo!";

        CargarPerfil();
        ActualizarEstadisticas();
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
            ActualizarEstadisticas();
    }

    private void OnDestroy()
    {
        LiberarFotoActual();
    }

    // ------------------------------------------------
    // CAMPO DE NOMBRE
    // ------------------------------------------------

    private void ConfigurarCampoNombre()
    {
        if (inputNombre == null)
        {
            Debug.LogError("No se asignó Input_Nombre.");
            return;
        }

        inputNombre.interactable = true;
        inputNombre.readOnly = false;
        inputNombre.enabled = true;
    }

    // ------------------------------------------------
    // BOTONES
    // ------------------------------------------------

    private void ConectarBotones()
    {
        if (botonesAvatares != null)
        {
            for (int i = 0; i < botonesAvatares.Length; i++)
            {
                int indice = i;

                if (botonesAvatares[i] != null)
                {
                    botonesAvatares[i].onClick.AddListener(
                        () => SeleccionarAvatar(indice)
                    );
                }
            }
        }

        if (btnSubirFoto != null)
            btnSubirFoto.onClick.AddListener(AbrirGaleria);

        if (btnGuardarPerfil != null)
            btnGuardarPerfil.onClick.AddListener(GuardarPerfil);
    }

    // ------------------------------------------------
    // SELECCIONAR AVATAR
    // ------------------------------------------------

    public void SeleccionarAvatar(int indice)
    {
        if (spritesAvatares == null ||
            indice < 0 ||
            indice >= spritesAvatares.Length ||
            spritesAvatares[indice] == null)
        {
            MostrarMensaje("No se encontró la imagen del avatar.");
            return;
        }

        avatarActual = indice;
        tipoVisual = 1;

        ActualizarVisual();

        PlayerPrefs.SetInt(KEY_TIPO, tipoVisual);
        PlayerPrefs.SetInt(KEY_AVATAR, avatarActual);
        PlayerPrefs.Save();

        MostrarMensaje("¡Avatar seleccionado!");
    }

    // ------------------------------------------------
    // ABRIR GALERIA
    // ------------------------------------------------

    public void AbrirGaleria()
    {
        NativeGallery.GetImageFromGallery(
            (ruta) =>
            {
                if (string.IsNullOrEmpty(ruta))
                {
                    Debug.Log("Selección de fotografía cancelada.");
                    return;
                }

                CargarFotoDesdeRuta(ruta);
            },
            "Elige tu foto de perfil",
            "image/*"
        );
    }

    // ------------------------------------------------
    // CARGAR FOTO DESDE GALERIA
    // ------------------------------------------------

    public void CargarFotoDesdeRuta(string ruta)
    {
        if (string.IsNullOrEmpty(ruta))
        {
            MostrarMensaje("No se seleccionó ninguna fotografía.");
            return;
        }

        Texture2D textura = null;

        try
        {
            textura = NativeGallery.LoadImageAtPath(
                ruta,
                -1,
                false,
                false,
                false
            );

            // Segundo intento mediante lectura directa.
            if (textura == null && File.Exists(ruta))
            {
                byte[] datosOriginales = File.ReadAllBytes(ruta);

                Texture2D alternativa = new Texture2D(
                    2,
                    2,
                    TextureFormat.RGBA32,
                    false
                );

                if (alternativa.LoadImage(datosOriginales))
                    textura = alternativa;
                else
                    Destroy(alternativa);
            }

            if (textura == null)
            {
                MostrarMensaje("No se pudo abrir esta imagen. Prueba con JPG o PNG.");
                Debug.LogWarning("No se pudo cargar: " + ruta);
                return;
            }

            byte[] datosPNG = textura.EncodeToPNG();

            if (datosPNG == null || datosPNG.Length == 0)
            {
                Destroy(textura);
                MostrarMensaje("No se pudo convertir la fotografía.");
                return;
            }

            File.WriteAllBytes(RutaFoto, datosPNG);

            ReemplazarFoto(textura);
            textura = null;

            tipoVisual = 0;

            PlayerPrefs.SetInt(KEY_TIPO, 0);
            PlayerPrefs.Save();

            ActualizarVisual();

            MostrarMensaje("¡Fotografía guardada correctamente!");
        }
        catch (Exception e)
        {
            if (textura != null)
                Destroy(textura);

            Debug.LogError("Error al guardar fotografía: " + e);
            MostrarMensaje("No se pudo guardar la fotografía.");
        }
    }

    // ------------------------------------------------
    // REEMPLAZAR FOTO ACTUAL
    // ------------------------------------------------

    private void ReemplazarFoto(Texture2D textura)
    {
        LiberarFotoActual();

        texturaFotoPersonal = textura;

        spriteFotoPersonal = Sprite.Create(
            texturaFotoPersonal,
            new Rect(
                0,
                0,
                texturaFotoPersonal.width,
                texturaFotoPersonal.height
            ),
            new Vector2(0.5f, 0.5f),
            100f
        );

        fotoCargada = true;
    }

    // ------------------------------------------------
    // LIBERAR FOTO ANTERIOR
    // ------------------------------------------------

    private void LiberarFotoActual()
    {
        if (imgFotoPersonal != null &&
            imgFotoPersonal.sprite == spriteFotoPersonal)
        {
            imgFotoPersonal.sprite = null;
        }

        if (spriteFotoPersonal != null)
        {
            Destroy(spriteFotoPersonal);
            spriteFotoPersonal = null;
        }

        if (texturaFotoPersonal != null)
        {
            Destroy(texturaFotoPersonal);
            texturaFotoPersonal = null;
        }

        fotoCargada = false;
    }

    // ------------------------------------------------
    // RECUPERAR FOTO GUARDADA
    // ------------------------------------------------

    private void CargarFotoGuardada()
    {
        if (!File.Exists(RutaFoto))
            return;

        try
        {
            byte[] datos = File.ReadAllBytes(RutaFoto);

            Texture2D textura = new Texture2D(
                2,
                2,
                TextureFormat.RGBA32,
                false
            );

            if (!textura.LoadImage(datos))
            {
                Destroy(textura);
                Debug.LogWarning("No se pudo recuperar la fotografía.");
                return;
            }

            ReemplazarFoto(textura);
        }
        catch (Exception e)
        {
            Debug.LogError("Error al recuperar foto: " + e.Message);
        }
    }

    // ------------------------------------------------
    // ACTUALIZAR VISUAL
    // ------------------------------------------------

    private void ActualizarVisual()
    {
        bool mostrarFoto = (tipoVisual == 0);

        if (imgFotoPersonal != null)
        {
            imgFotoPersonal.gameObject.SetActive(mostrarFoto);
            imgFotoPersonal.enabled = mostrarFoto;
            imgFotoPersonal.raycastTarget = false;

            if (mostrarFoto && fotoCargada &&
                spriteFotoPersonal != null)
            {
                imgFotoPersonal.sprite = spriteFotoPersonal;

                // Se conserva la apariencia anterior.
                imgFotoPersonal.preserveAspect = false;
                imgFotoPersonal.color = Color.white;
            }
        }

        if (imgMarcoFoto != null)
        {
            imgMarcoFoto.gameObject.SetActive(mostrarFoto);
            imgMarcoFoto.enabled = mostrarFoto;
            imgMarcoFoto.raycastTarget = false;
        }

        if (imgAvatarSeleccionado != null)
        {
            imgAvatarSeleccionado.gameObject.SetActive(!mostrarFoto);
            imgAvatarSeleccionado.enabled = !mostrarFoto;
            imgAvatarSeleccionado.raycastTarget = false;

            if (!mostrarFoto &&
                spritesAvatares != null &&
                avatarActual >= 0 &&
                avatarActual < spritesAvatares.Length &&
                spritesAvatares[avatarActual] != null)
            {
                imgAvatarSeleccionado.sprite =
                    spritesAvatares[avatarActual];

                imgAvatarSeleccionado.preserveAspect = true;
                imgAvatarSeleccionado.color = Color.white;
            }
        }

        if (mostrarFoto &&
            imgFotoPersonal != null &&
            imgMarcoFoto != null &&
            imgFotoPersonal.transform.parent ==
            imgMarcoFoto.transform.parent)
        {
            imgFotoPersonal.transform.SetAsFirstSibling();
            imgMarcoFoto.transform.SetAsLastSibling();
        }
    }

    // ------------------------------------------------
    // CARGAR PERFIL
    // ------------------------------------------------

    public void CargarPerfil()
    {
        string nombre = PlayerPrefs.GetString(KEY_NOMBRE, "");

        if (inputNombre != null)
        {
            inputNombre.text = nombre;
            inputNombre.interactable = true;
            inputNombre.readOnly = false;
        }

        if (txtNombreJugador != null)
        {
            txtNombreJugador.text =
                string.IsNullOrWhiteSpace(nombre)
                ? "Jugador"
                : nombre;
        }

        tipoVisual = PlayerPrefs.GetInt(KEY_TIPO, 0);
        avatarActual = PlayerPrefs.GetInt(KEY_AVATAR, 2);

        CargarFotoGuardada();

        if (tipoVisual == 1)
        {
            if (spritesAvatares == null ||
                avatarActual < 0 ||
                avatarActual >= spritesAvatares.Length ||
                spritesAvatares[avatarActual] == null)
            {
                tipoVisual = fotoCargada ? 0 : 1;
            }
        }

        if (tipoVisual == 0 && !fotoCargada)
            tipoVisual = 1;

        ActualizarVisual();
    }

    // ------------------------------------------------
    // GUARDAR PERFIL
    // ------------------------------------------------

    public void GuardarPerfil()
    {
        if (inputNombre == null)
        {
            MostrarMensaje("No se encontró el campo del nombre.");
            return;
        }

        string nombre = inputNombre.text.Trim();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            MostrarMensaje("Escribe tu nombre antes de guardar.");
            inputNombre.ActivateInputField();
            return;
        }

        PlayerPrefs.SetString(KEY_NOMBRE, nombre);
        PlayerPrefs.SetInt(KEY_TIPO, tipoVisual);
        PlayerPrefs.SetInt(KEY_AVATAR, avatarActual);
        PlayerPrefs.Save();

        if (txtNombreJugador != null)
            txtNombreJugador.text = nombre;

        MostrarMensaje("¡Perfil guardado correctamente!");

        inputNombre.DeactivateInputField();
    }

    // ------------------------------------------------
    // EXPERIENCIA Y MISIONES
    // ------------------------------------------------

    public void ActualizarEstadisticas()
    {
        int totalEstrellas = ContarEstrellasObtenidas();

        // Cada estrella equivale a 10 XP.
        int experiencia = totalEstrellas * xpPorEstrella;

        if (txtXP != null)
        {
            txtXP.text = experiencia + " XP";
        }
        else
        {
            Debug.LogWarning("No se asignó Txt_XP en el Inspector.");
        }

        int misionesCompletadas = ContarMisionesCompletadas();

        if (txtMisiones != null)
        {
            txtMisiones.text =
                misionesCompletadas + " / " + totalMisiones;
        }
        else
        {
            Debug.LogWarning("No se asignó Txt_Misiones en el Inspector.");
        }

        Debug.Log(
            "Perfil actualizado: " +
            totalEstrellas + " estrellas | " +
            experiencia + " XP | " +
            misionesCompletadas + " / " + totalMisiones + " misiones"
        );
    }

    // Suma las estrellas guardadas en todas las misiones.
    private int ContarEstrellasObtenidas()
    {
        int total = 0;

        foreach (string categoria in categorias)
        {
            for (int mision = 1; mision <= 3; mision++)
            {
                total += ProgresoEstrellas.ObtenerEstrellas(
                    categoria,
                    mision
                );
            }
        }

        return total;
    }

    // Una misión cuenta como completada cuando tiene 3 estrellas.
    private int ContarMisionesCompletadas()
    {
        int total = 0;

        foreach (string categoria in categorias)
        {
            for (int mision = 1; mision <= 3; mision++)
            {
                int estrellas = ProgresoEstrellas.ObtenerEstrellas(
                    categoria,
                    mision
                );

                if (estrellas >= 3)
                    total++;
            }
        }

        return Mathf.Clamp(total, 0, totalMisiones);
    }

    // ------------------------------------------------
    // MENSAJES
    // ------------------------------------------------

    private void MostrarMensaje(string mensaje)
    {
        if (txtMensaje != null)
            txtMensaje.text = mensaje;

        Debug.Log("[PerfilJugador] " + mensaje);
    }
}
