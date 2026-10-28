
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
    public int totalMisiones = 12;

    private const string KEY_NOMBRE = "Perfil_Nombre";
    private const string KEY_TIPO = "Perfil_TipoVisual";
    private const string KEY_AVATAR = "Perfil_Avatar";
    private const string KEY_XP = "Perfil_XP";
    private const string KEY_MISIONES = "Perfil_MisionesCompletadas";
    private const string ARCHIVO_FOTO = "avatar_personalizado.png";

    // 0 = foto personal; 1 = avatar predisenado.
    // Por defecto, se muestra la foto con su marco.
    private int tipoVisual = 0;
    private int avatarActual = 2;

    private Sprite spriteFotoPersonal;
    private bool fotoCargada = false;

    private string RutaFoto =>
        Path.Combine(Application.persistentDataPath, ARCHIVO_FOTO);

    private void Awake()
    {
        ConectarBotones();
    }

    private void Start()
    {
        // Texto fijo del pie.
        if (txtPie != null)
            txtPie.text = "¡Sigue explorando y aprendiendo!";

        CargarPerfil();
        ActualizarEstadisticas();
    }

    private void OnDestroy()
    {
        if (spriteFotoPersonal != null)
            Destroy(spriteFotoPersonal);
    }

    // ------------------------------------------------
    // CONECTAR BOTONES
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
                        () => SeleccionarAvatar(indice));
                }
            }
        }

        if (btnSubirFoto != null)
        {
            btnSubirFoto.onClick.RemoveListener(AbrirGaleria);
            btnSubirFoto.onClick.AddListener(AbrirGaleria);
        }

        if (btnGuardarPerfil != null)
        {
            btnGuardarPerfil.onClick.RemoveListener(GuardarPerfil);
            btnGuardarPerfil.onClick.AddListener(GuardarPerfil);
        }
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
            MostrarMensaje("No se encontro la imagen del avatar.");
            return;
        }

        avatarActual = indice;
        tipoVisual = 1;

        ActualizarVisual();

        // Guardar la seleccion inmediatamente.
        PlayerPrefs.SetInt(KEY_TIPO, 1);
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
                    return;

                CargarFotoDesdeRuta(ruta);
            },
            "Elige tu foto de perfil",
            "image/*"
        );
    }

    // ------------------------------------------------
    // CARGAR FOTO PERSONAL
    // ------------------------------------------------

    public void CargarFotoDesdeRuta(string ruta)
    {
        if (string.IsNullOrEmpty(ruta) || !File.Exists(ruta))
        {
            MostrarMensaje("No se encontro la fotografia.");
            return;
        }

        try
        {
            byte[] datos = File.ReadAllBytes(ruta);

            if (CargarFotoDesdeBytes(datos))
            {
                try
                {
                    File.WriteAllBytes(RutaFoto, datos);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(
                        "No se pudo guardar la copia de la foto: " +
                        e.Message);
                }

                tipoVisual = 0;

                PlayerPrefs.SetInt(KEY_TIPO, 0);
                PlayerPrefs.Save();

                ActualizarVisual();
                MostrarMensaje("¡Fotografia actualizada!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error al cargar la fotografia: " + e.Message);
            MostrarMensaje("No se pudo cargar la fotografia.");
        }
    }

    // Carga la imagen sin cambiar el avatar o modo guardado.
    private bool CargarFotoDesdeBytes(byte[] datos)
    {
        Texture2D textura = new Texture2D(
            2, 2, TextureFormat.RGBA32, false);

        if (!textura.LoadImage(datos))
        {
            Destroy(textura);
            return false;
        }

        Sprite nuevoSprite = Sprite.Create(
            textura,
            new Rect(0, 0, textura.width, textura.height),
            new Vector2(0.5f, 0.5f),
            100f
        );

        if (spriteFotoPersonal != null)
            Destroy(spriteFotoPersonal);

        spriteFotoPersonal = nuevoSprite;
        fotoCargada = true;

        return true;
    }

    // ------------------------------------------------
    // ACTUALIZAR INTERFAZ
    // ------------------------------------------------

    private void ActualizarVisual()
    {
        bool mostrarFoto = (tipoVisual == 0);

        // FOTO PERSONAL
        if (imgFotoPersonal != null)
        {
            imgFotoPersonal.gameObject.SetActive(mostrarFoto);
            imgFotoPersonal.enabled = mostrarFoto;
            imgFotoPersonal.raycastTarget = false;

            if (mostrarFoto && fotoCargada &&
                spriteFotoPersonal != null)
            {
                imgFotoPersonal.sprite = spriteFotoPersonal;
                imgFotoPersonal.preserveAspect = false;
                imgFotoPersonal.color = Color.white;
            }
        }

        // MARCO: visible con la foto, oculto con los avatares.
        if (imgMarcoFoto != null)
        {
            imgMarcoFoto.gameObject.SetActive(mostrarFoto);
            imgMarcoFoto.enabled = mostrarFoto;
            imgMarcoFoto.raycastTarget = false;
        }

        // AVATAR PREDEFINIDO
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

        // Orden de dibujo para la foto y el marco.
        // Se aplica cuando ambos comparten el mismo padre.
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
    // CARGAR PERFIL GUARDADO
    // ------------------------------------------------

    public void CargarPerfil()
    {
        string nombre = PlayerPrefs.GetString(KEY_NOMBRE, "");

        if (inputNombre != null)
            inputNombre.text = nombre;

        if (txtNombreJugador != null)
        {
            txtNombreJugador.text =
                string.IsNullOrEmpty(nombre) ? "Jugador" : nombre;
        }

        // Leer la seleccion guardada ANTES de cargar la foto.
        tipoVisual = PlayerPrefs.GetInt(KEY_TIPO, 0);
        avatarActual = PlayerPrefs.GetInt(KEY_AVATAR, 2);

        // Recuperar la foto sin modificar tipoVisual.
        if (File.Exists(RutaFoto))
        {
            try
            {
                byte[] datos = File.ReadAllBytes(RutaFoto);
                CargarFotoDesdeBytes(datos);
            }
            catch (Exception e)
            {
                Debug.LogWarning(
                    "No se pudo recuperar la foto: " + e.Message);
            }
        }

        // Validar el avatar guardado.
        if (tipoVisual == 1)
        {
            if (spritesAvatares == null ||
                avatarActual < 0 ||
                avatarActual >= spritesAvatares.Length ||
                spritesAvatares[avatarActual] == null)
            {
                tipoVisual = 0;
                PlayerPrefs.SetInt(KEY_TIPO, 0);
                PlayerPrefs.Save();
            }
        }

        ActualizarVisual();
    }

    // ------------------------------------------------
    // BOTON GUARDAR
    // ------------------------------------------------

    public void GuardarPerfil()
    {
        string nombre = inputNombre != null
            ? inputNombre.text.Trim()
            : "";

        if (string.IsNullOrEmpty(nombre))
            nombre = "Jugador";

        // Guardar todos los datos del perfil.
        PlayerPrefs.SetString(KEY_NOMBRE, nombre);
        PlayerPrefs.SetInt(KEY_TIPO, tipoVisual);
        PlayerPrefs.SetInt(KEY_AVATAR, avatarActual);
        PlayerPrefs.Save();

        if (txtNombreJugador != null)
            txtNombreJugador.text = nombre;

        // Confirmar en consola y en pantalla.
        Debug.Log(
            "Perfil guardado. Nombre: " + nombre +
            ", tipo visual: " + tipoVisual +
            ", avatar: " + avatarActual);

        MostrarMensaje("¡Perfil guardado correctamente!");
    }

    // ------------------------------------------------
    // ESTADISTICAS
    // ------------------------------------------------

    public void ActualizarEstadisticas()
    {
        int xp = PlayerPrefs.GetInt(KEY_XP, 0);
        int misiones = PlayerPrefs.GetInt(KEY_MISIONES, 0);

        if (txtXP != null)
            txtXP.text = Mathf.Max(0, xp) + " XP";

        if (txtMisiones != null)
        {
            txtMisiones.text =
                Mathf.Clamp(misiones, 0, totalMisiones) +
                " / " + totalMisiones;
        }
    }

    public void EstablecerEstadisticas(int xp, int misiones)
    {
        PlayerPrefs.SetInt(KEY_XP, Mathf.Max(0, xp));

        PlayerPrefs.SetInt(
            KEY_MISIONES,
            Mathf.Clamp(misiones, 0, totalMisiones));

        PlayerPrefs.Save();
        ActualizarEstadisticas();
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
