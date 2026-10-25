using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerfilJugadorManager : MonoBehaviour
{
    [Header("DATOS DEL JUGADOR")]
    [SerializeField] private TMP_InputField inputNombre;
    [SerializeField] private TMP_Text txtNombreJugador;
    [SerializeField] private TMP_Text txtMensaje;

    [Header("AVATARES PREDEFINIDOS (ORDEN: ROBOT, ZORRO, PANDA, RANA, MARIPOSA, UNICORNIO)")]
    [SerializeField] private Button[] botonesAvatares = new Button[6];
    [SerializeField] private Sprite[] spritesAvatares = new Sprite[6];
    [SerializeField] private Image imgAvatarSeleccionado;
    [SerializeField] private Image[] bordesSeleccion = new Image[6];
    [SerializeField] private Color colorBordeSeleccionado = new Color(0.42f, 0.46f, 1f);
    [SerializeField] private Color colorBordeNormal = new Color(1f, 1f, 1f, 0f);

    [Header("BOTONES")]
    [SerializeField] private Button btnSubirFoto;
    [SerializeField] private Button btnGuardarPerfil;

    [Header("ESTADISTICAS")]
    [SerializeField] private TMP_Text txtXP;
    [SerializeField] private TMP_Text txtMisiones;
    [SerializeField, Min(1)] private int totalMisiones = 12;

    [Header("FOTO PERSONALIZADA")]
    [Tooltip("Para abrir galeria Android o explorador PC instala un selector de imagenes y conecta su resultado a CargarFotoDesdeRuta.")]
    [SerializeField] private bool mostrarAvisoSiNoHaySelector = true;

    private const string ClaveNombre = "Perfil_Nombre";
    private const string ClaveAvatar = "Perfil_Avatar";
    private const string ClaveXP = "Perfil_XP";
    private const string ClaveMisiones = "Perfil_MisionesCompletadas";
    private const string NombreFoto = "avatar_personalizado.png";
    private const int AvatarFoto = -1;

    private int avatarActual;
    private Texture2D texturaFoto;
    private Sprite spriteFoto;
    private Coroutine rutinaMensaje;

    private string RutaFoto => Path.Combine(Application.persistentDataPath, NombreFoto);

    private void Awake()
    {
        for (int i = 0; i < botonesAvatares.Length; i++)
        {
            int indice = i;
            if (botonesAvatares[i] != null)
                botonesAvatares[i].onClick.AddListener(() => SeleccionarAvatar(indice));
        }

        if (btnGuardarPerfil != null) btnGuardarPerfil.onClick.AddListener(GuardarPerfil);
        if (btnSubirFoto != null) btnSubirFoto.onClick.AddListener(SolicitarFoto);
    }

    private void Start()
    {
        CargarPerfil();
        ActualizarEstadisticas();
    }

    public void SeleccionarAvatar(int indice)
    {
        if (indice < 0 || indice >= spritesAvatares.Length || spritesAvatares[indice] == null)
        {
            MostrarMensaje("Este avatar aun no tiene imagen asignada.");
            return;
        }

        avatarActual = indice;
        ActualizarAvatar();
    }

    private void ActualizarAvatar()
    {
        if (imgAvatarSeleccionado != null)
        {
            Sprite sprite = avatarActual == AvatarFoto ? spriteFoto :
                (avatarActual >= 0 && avatarActual < spritesAvatares.Length ? spritesAvatares[avatarActual] : null);
            imgAvatarSeleccionado.sprite = sprite;
            imgAvatarSeleccionado.enabled = sprite != null;
            imgAvatarSeleccionado.preserveAspect = true;
        }

        for (int i = 0; i < bordesSeleccion.Length; i++)
            if (bordesSeleccion[i] != null)
                bordesSeleccion[i].color = i == avatarActual ? colorBordeSeleccionado : colorBordeNormal;
    }

    public void GuardarPerfil()
    {
        string nombre = inputNombre != null ? inputNombre.text.Trim() : PlayerPrefs.GetString(ClaveNombre, "Explorador");
        if (string.IsNullOrWhiteSpace(nombre)) nombre = "Explorador";
        if (nombre.Length > 20) nombre = nombre.Substring(0, 20);

        PlayerPrefs.SetString(ClaveNombre, nombre);
        PlayerPrefs.SetInt(ClaveAvatar, avatarActual);
        PlayerPrefs.Save();

        if (inputNombre != null) inputNombre.text = nombre;
        if (txtNombreJugador != null) txtNombreJugador.text = nombre;
        MostrarMensaje("¡Perfil guardado!");
    }

    public void CargarPerfil()
    {
        string nombre = PlayerPrefs.GetString(ClaveNombre, "Explorador");
        avatarActual = PlayerPrefs.GetInt(ClaveAvatar, 0);

        if (inputNombre != null) inputNombre.text = nombre;
        if (txtNombreJugador != null) txtNombreJugador.text = nombre;
        if (txtMensaje != null) txtMensaje.text = "";

        if (avatarActual == AvatarFoto && !CargarFotoGuardada()) avatarActual = 0;
        ActualizarAvatar();
    }

    public void ActualizarEstadisticas()
    {
        // Estas claves son provisionales: integraremos la lectura real de ProgresoEstrellas
        // cuando dispongamos de su codigo. No se suman misiones inventadas.
        int xp = Mathf.Max(0, PlayerPrefs.GetInt(ClaveXP, 0));
        int misiones = Mathf.Clamp(PlayerPrefs.GetInt(ClaveMisiones, 0), 0, totalMisiones);
        if (txtXP != null) txtXP.text = xp + " XP";
        if (txtMisiones != null) txtMisiones.text = misiones + " / " + totalMisiones;
    }

    // Otros scripts pueden llamar a estos metodos al completar ejercicios o misiones.
    // Se debe evitar otorgar XP repetido: la comprobacion de primera vez se hara
    // desde el sistema de progreso existente.
    public static void EstablecerXP(int xp)
    {
        PlayerPrefs.SetInt(ClaveXP, Mathf.Max(0, xp));
        PlayerPrefs.Save();
    }

    public static void EstablecerMisionesCompletadas(int misiones)
    {
        PlayerPrefs.SetInt(ClaveMisiones, Mathf.Max(0, misiones));
        PlayerPrefs.Save();
    }

    public void SolicitarFoto()
    {
        if (mostrarAvisoSiNoHaySelector)
            MostrarMensaje("Falta conectar el selector de fotos del dispositivo.");
    }

    // Llamar desde el complemento de galeria una vez obtenida una ruta local valida.
    public void CargarFotoDesdeRuta(string ruta)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ruta) || !File.Exists(ruta))
            {
                MostrarMensaje("No se pudo encontrar la imagen.");
                return;
            }

            byte[] bytes = File.ReadAllBytes(ruta);
            Texture2D nuevaTextura = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!nuevaTextura.LoadImage(bytes))
            {
                Destroy(nuevaTextura);
                MostrarMensaje("No se pudo abrir la imagen.");
                return;
            }

            // Se guarda una copia PNG local para que el avatar no dependa de la galeria.
            byte[] png = nuevaTextura.EncodeToPNG();
            File.WriteAllBytes(RutaFoto, png);
            ReemplazarFoto(nuevaTextura);
            avatarActual = AvatarFoto;
            ActualizarAvatar();
            MostrarMensaje("Foto seleccionada. Pulsa Guardar perfil.");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("No se pudo cargar la foto: " + ex.Message);
            MostrarMensaje("No se pudo cargar la foto.");
        }
    }

    private bool CargarFotoGuardada()
    {
        try
        {
            if (!File.Exists(RutaFoto)) return false;
            byte[] bytes = File.ReadAllBytes(RutaFoto);
            Texture2D nuevaTextura = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!nuevaTextura.LoadImage(bytes))
            {
                Destroy(nuevaTextura);
                return false;
            }
            ReemplazarFoto(nuevaTextura);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning("No se pudo recuperar la foto: " + ex.Message);
            return false;
        }
    }

    private void ReemplazarFoto(Texture2D nuevaTextura)
    {
        if (spriteFoto != null) Destroy(spriteFoto);
        if (texturaFoto != null) Destroy(texturaFoto);
        texturaFoto = nuevaTextura;
        spriteFoto = Sprite.Create(texturaFoto,
            new Rect(0, 0, texturaFoto.width, texturaFoto.height),
            new Vector2(0.5f, 0.5f), 100f);
    }

    private void MostrarMensaje(string mensaje)
    {
        if (txtMensaje == null) return;
        if (rutinaMensaje != null) StopCoroutine(rutinaMensaje);
        txtMensaje.text = mensaje;
        rutinaMensaje = StartCoroutine(LimpiarMensaje());
    }

    private IEnumerator LimpiarMensaje()
    {
        yield return new WaitForSeconds(3f);
        if (txtMensaje != null) txtMensaje.text = "";
        rutinaMensaje = null;
    }

    private void OnDestroy()
    {
        if (spriteFoto != null) Destroy(spriteFoto);
        if (texturaFoto != null) Destroy(texturaFoto);
    }
}
