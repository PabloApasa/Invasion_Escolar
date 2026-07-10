using UnityEngine;
using UnityEngine.UI;
using TMPro; // Es el estándar de texto en Unity 6
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración del Temporizador")]
    [SerializeField] private float tiempoRestante = 120f;
    [SerializeField] private TextMeshProUGUI textoTemporizador;

    [Header("Configuración de Condiciones")]
    [SerializeField] private int enemigosTotales;
    [SerializeField] private int personasPorSalvar;
    [SerializeField] private TextMeshProUGUI textoPersonasRestantes;

    [Header("Configuración del Jugador (Vida)")]
    [SerializeField] private float vidaMaxima = 100f;
    private float vidaActual;
    [SerializeField] private Slider barraVida;

    [Header("Paneles de Estado (UI)")]
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private GameObject panelDerrota;
    [SerializeField] private GameObject panelInicio;

    private bool juegoTerminado = false;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        vidaActual = vidaMaxima;
        ActualizarUI();

        if (panelVictoria) panelVictoria.SetActive(false);
        if (panelDerrota) panelDerrota.SetActive(false);

        if (panelInicio != null)
        {
            panelInicio.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void Update()
    {
        if (juegoTerminado) return;

        ManejarTemporizador();
    }

    public void ComenzarJuego()
    {
        if (panelInicio != null)
        {
            panelInicio.SetActive(false);
        }
        Time.timeScale = 1f;
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

    public void EliminarEnemigo()
    {
        if (juegoTerminado) return;

        enemigosTotales--;
        VerificarCondicionesVictoria();
    }

    public void SalvarPersona()
    {
        if (juegoTerminado) return;

        personasPorSalvar--;
        ActualizarUI();
        VerificarCondicionesVictoria();
    }

    private void VerificarCondicionesVictoria()
    {
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
        Time.timeScale = 0f;
        if (panelVictoria) panelVictoria.SetActive(true);
        Debug.Log("¡Victoria!");
    }

    private void ProvocarDerrota()
    {
        juegoTerminado = true;
        Time.timeScale = 0f;
        if (panelDerrota) panelDerrota.SetActive(true);
        Debug.Log("¡Perdiste!");
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public int GetEnemigosTotales()
    {
        return enemigosTotales;
    }
}