using UnityEngine;

public class Bala2DEnemigo : MonoBehaviour
{
    [Header("Configuración de la Bala")]
    [SerializeField] private float velocidad = 12f;          // Bajamos un poco la velocidad para darle tiempo a girar
    [SerializeField] private float velocidadGiro = 250f;     // Qué tan cerrado/rápido puede girar la bala
    [SerializeField] private float tiempoDeVida = 4f;

    private Rigidbody2D rb;
    private Transform jugador;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Buscamos al Player para tener su posición en tiempo real
        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
        }

        Destroy(gameObject, tiempoDeVida);
    }

    // Esta función se mantiene para darle el impulso inicial al nacer
    public void DispararHacia(Vector2 direccion)
    {
        if (rb != null)
        {
            rb.linearVelocity = direccion.normalized * velocidad;
            AlinearVisualmente(direccion);
        }
    }

    void FixedUpdate()
    {
        // Si el jugador murió o no existe, la bala sigue recta con su velocidad actual
        if (jugador == null) return;

        // 1. Vector de dirección actual de la bala (Hacia dónde va)
        Vector2 direccionActual = rb.linearVelocity.normalized;

        // 2. Vector de dirección hacia el objetivo (Hacia dónde está el Player)
        Vector2 direccionHaciaJugador = ((Vector2)jugador.position - (Vector2)transform.position).normalized;

        // 3. MATEMÁTICA: Producto cruz en 2D para saber si el jugador está a la izquierda o derecha
        float productoCruz2D = (direccionActual.x * direccionHaciaJugador.y) - (direccionActual.y * direccionHaciaJugador.x);

        // El signo del producto cruz nos dice hacia dónde rotar la velocidad
        float sentidoGiro = productoCruz2D > 0 ? 1f : -1f;

        // Calculamos el nuevo vector de velocidad aplicando el giro angular
        float anguloGiro = sentidoGiro * velocidadGiro * Time.fixedDeltaTime;

        // Rotamos físicamente el vector de velocidad usando trigonometría básica
        Vector2 nuevaDireccion;
        nuevaDireccion.x = direccionActual.x * Mathf.Cos(anguloGiro * Mathf.Deg2Rad) - direccionActual.y * Mathf.Sin(anguloGiro * Mathf.Deg2Rad);
        nuevaDireccion.y = direccionActual.x * Mathf.Sin(anguloGiro * Mathf.Deg2Rad) + direccionActual.y * Mathf.Cos(anguloGiro * Mathf.Deg2Rad);

        // Aplicamos la nueva velocidad calculada vectorialmente al Rigidbody2D
        rb.linearVelocity = nuevaDireccion.normalized * velocidad;

        // Ajustamos la rotación del Sprite para que mire hacia su nuevo vector de movimiento
        AlinearVisualmente(rb.linearVelocity);
    }

    private void AlinearVisualmente(Vector2 direccion)
    {
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si el objeto con el que chocó se llama "VisualEscudo" o tiene un Tag específico
        if (collision.gameObject.name == "VisualEscudo" || collision.CompareTag("Escudo"))
        {
            Debug.Log("¡Bala bloqueada por el escudo!");
            Destroy(gameObject); // Destruye la bala de inmediato
            return; // Corta la función aquí para que no siga evaluando el daño al jugador
        }

        // Si choca con el escudo inteligente o las paredes, se destruye
        if (collision.CompareTag("Player"))
        {
            JugadorTopDown scriptJugador = collision.GetComponent<JugadorTopDown>();
            if (scriptJugador != null)
            {
                scriptJugador.RecibirDanio(15f); // Hace daño al jugador
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstaculos"))
        {
            Destroy(gameObject); // Se rompe con las paredes
        }
    }
}