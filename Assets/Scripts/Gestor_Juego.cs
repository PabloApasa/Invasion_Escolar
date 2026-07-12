using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Singleton para acceder al GameManager desde cualquier otro script fácilmente
    public static GameManager Instance { get; private set; }

    [Header("Configuración del Temporizador")]
    [SerializeField] private float tiempoRestante = 120f; // 2 minutos (120 segundos)
    [SerializeField] private TextMeshProUGUI textoTemporizador;

    [Header("Configuración de Condiciones")]
    [SerializeField] private int enemigosTotales;
    [SerializeField] private int personasPorSalvar;
    [SerializeField] private TextMeshProUGUI textoPersonasRestantes;

    [Header("Configuración del Jugador (Vida)")]
    [SerializeField] private float vidaMaxima = 100f;
    private float vidaActual;
    [SerializeField] private Slider barraVida; // Barra de vida clásica (UI Slider)

    [Header("Paneles de Estado (UI)")]
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private GameObject panelDerrota;
    [SerializeField] private GameObject panelInicio; // NUEVO: Panel de historia/instrucciones
    [SerializeField] private GameObject panelPausa;

    [Header("Configuración de Sonido")]
    public Slider volumen;
    public Slider FXvolumen;
    public AudioMixer mixer;

    private bool juegoTerminado = false;

    private void Awake()
    {
        // Asegurar que solo exista un GameManager
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        vidaActual = vidaMaxima;
        ActualizarUI();

        // Desactivar paneles al iniciar el nivel
        if (panelVictoria) panelVictoria.SetActive(false);
        if (panelDerrota) panelDerrota.SetActive(false);

        // Mostrar el panel de inicio y pausar el juego
        if (panelInicio != null)
        {
            panelInicio.SetActive(true);
            Time.timeScale = 0f; // Congela todo el juego
        }
        else
        {
            Time.timeScale = 1f; // Por si acaso no asignas un panel, el juego fluye normal
        }

        // ==========================================
        // CARGAR EL VOLUMEN GUARDADO EN EL MIXER
        // ==========================================
        // Nota: Asumimos 0f por defecto porque el AudioMixer suele usar decibelios (0 es el máximo normal)
        float masterGuardado = PlayerPrefs.GetFloat("VolMasterGuardado", 0f);
        float fxGuardado = PlayerPrefs.GetFloat("VolFXGuardado", 0f);

        // Actualizamos los Sliders visuales
        if (volumen != null) volumen.value = masterGuardado;
        if (FXvolumen != null) FXvolumen.value = fxGuardado;

        // Actualizamos el Mixer
        if (mixer != null)
        {
            mixer.SetFloat("VolMaster", masterGuardado);
            mixer.SetFloat("VolFX", fxGuardado);
        }
    }

    private void Update()
    {
        if (juegoTerminado) return;

        ManejarTemporizador();
    }

    // El script DialogoInicio llamará a esta función al terminar de leer
    public void ComenzarJuego()
    {
        if (panelInicio != null)
        {
            panelInicio.SetActive(false); // Oculta el panel
        }
        Time.timeScale = 1f; // Descongela el juego y arranca la acción
    }

    public void Nivel02()
    {
        SceneManager.LoadScene("Nivel-02");
    }

    public void VolverMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // ==========================================
    // FUNCIONES DE VOLUMEN CON GUARDADO
    // ==========================================
    public void ChangeVolumenMaster(float v)
    {
        if (mixer != null) mixer.SetFloat("VolMaster", v);
        PlayerPrefs.SetFloat("VolMasterGuardado", v); // ¡Se guarda en memoria!
    }

    public void ChangeVolumenFX(float v)
    {
        if (mixer != null) mixer.SetFloat("VolFX", v);
        PlayerPrefs.SetFloat("VolFXGuardado", v); // ¡Se guarda en memoria!
    }

    private void ManejarTemporizador()
    {
        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarTextoTiempo(tiempoRestante);
        }
        else
        {
            tiempoRestante = 0;
            ActualizarTextoTiempo(tiempoRestante);
            ProvocarDerrota();
        }
    }

    private void ActualizarTextoTiempo(float tiempo)
    {
        if (textoTemporizador == null) return;
        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        textoTemporizador.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    // Método público para aplicar daño (valores negativos) o curación (valores positivos)
    public void ModificarVida(float cantidad)
    {
        if (juegoTerminado) return;

        vidaActual = Mathf.Clamp(vidaActual + cantidad, 0, vidaMaxima);
        if (barraVida) barraVida.value = vidaActual / vidaMaxima;

        if (vidaActual <= 0)
        {
            ProvocarDerrota();
        }
    }

    // Llama a esto desde el script de tus enemigos cuando mueran
    public void EliminarEnemigo()
    {
        if (juegoTerminado) return;

        enemigosTotales--;
        VerificarCondicionesVictoria();
    }

    // Llama a esto desde el script de las personas cuando las salves
    public void SalvarPersona()
    {
        if (juegoTerminado) return;

        personasPorSalvar--;
        ActualizarUI();
        VerificarCondicionesVictoria();
    }

    private void VerificarCondicionesVictoria()
    {
        // Condición: Si eliminas a todos los enemigos O salvas a todas las personas -> Victoria
        if (enemigosTotales <= 0 || personasPorSalvar <= 0)
        {
            ProvocarVictoria();
        }
    }

    private void ActualizarUI()
    {
        if (textoPersonasRestantes != null)
        {
            textoPersonasRestantes.text = "Personas restantes: " + personasPorSalvar;
        }
        if (barraVida)
        {
            barraVida.value = vidaActual / vidaMaxima;
        }
    }

    private void ProvocarVictoria()
    {
        juegoTerminado = true;
        Time.timeScale = 0f; // Detiene el paso del tiempo, físicas y lógicas del juego
        if (panelVictoria) panelVictoria.SetActive(true);
        Debug.Log("¡Victoria!");
    }

    private void ProvocarDerrota()
    {
        juegoTerminado = true;
        Time.timeScale = 0f; // Detiene el juego
        if (panelDerrota) panelDerrota.SetActive(true);
        Debug.Log("¡Perdiste!");
    }

    // Función útil para conectar a un botón de "Reintentar" en tu panel de derrota/victoria
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f; // MUY IMPORTANTE restablecer el tiempo antes de recargar
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ====================================================================
    // FUNCIÓN EXTRA: Permite al Generador de Enemigos leer la cantidad
    // ====================================================================
    public int GetEnemigosTotales()
    {
        return enemigosTotales;
    }
}