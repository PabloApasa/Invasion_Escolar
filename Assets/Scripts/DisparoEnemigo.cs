using UnityEngine;

public class DisparoEnemigo : MonoBehaviour
{
    [Header("Configuración del Disparo")]
    public GameObject prefabBala;       // Arrastra aquí el Prefab de la bala del enemigo
    public Transform puntoDeDisparo;    // (Opcional) De dónde sale la bala
    public float cadenciaDeDisparo = 2f; // Segundos que tarda entre cada disparo

    private Transform jugador;
    private float temporizador;

    void Start()
    {
        // Buscamos al jugador en la escena mediante su etiqueta (Tag) "Player"
        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");

        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
        }
        else
        {
            Debug.LogError("El enemigo no pudo encontrar a ningún objeto con la etiqueta 'Player'.");
        }
    }

    void Update()
    {
        // Si no hay jugador, el enemigo no hace nada
        if (jugador == null) return;

        // El temporizador avanza con el tiempo del juego
        temporizador += Time.deltaTime;

        // Cuando el temporizador alcanza la cadencia, dispara y se reinicia
        if (temporizador >= cadenciaDeDisparo)
        {
            EjecutarDisparo();
            temporizador = 0f;
        }
    }

    void EjecutarDisparo()
    {
        if (prefabBala == null) return;

        // 1. Definimos el origen del disparo
        Vector3 origen = puntoDeDisparo != null ? puntoDeDisparo.position : transform.position;

        // 2. Calculamos la dirección (Posición del Jugador - Origen del Enemigo) y normalizamos
        Vector2 direccion = ((Vector2)jugador.position - (Vector2)origen).normalized;

        // 3. Instanciamos la bala
        GameObject nuevaBala = Instantiate(prefabBala, origen, Quaternion.identity);

        // 4. Le pasamos la dirección al script Bala2D
        Bala2D scriptBala = nuevaBala.GetComponent<Bala2D>();
        if (scriptBala != null)
        {
            scriptBala.DispararHacia(direccion);
        }
    }
}