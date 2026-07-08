using UnityEngine;
using UnityEngine.UI;
using TMPro; // Es el estándar de texto en Unity 6
using UnityEngine.SceneManagement;

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
    }

    private void Update()
    {
        if (juegoTerminado) return;

        ManejarTemporizador();
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