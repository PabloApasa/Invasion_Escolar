using UnityEngine;

public class EnemigoPerseguidor : MonoBehaviour
{
    [Header("Estadísticas")]
    [SerializeField] private float vidaMaxima = 50f;
    private float vidaActual;

    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadMax = 3.5f;
    [SerializeField] private float fuerzaGiroMax = 5f;

    [Header("Sensores de Obstáculos (Vectores)")]
    [SerializeField] private float longitudSensor = 1.5f;
    [SerializeField] private LayerMask capaObstaculos; // Asigna aquí la capa de tus paredes

    [Header("Efectos Visuales (Sprites)")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite spriteFrente;
    [SerializeField] private Sprite spriteEspalda;
    [SerializeField] private Sprite spritePerfil; // Mirando a la derecha

    [Header("Efectos de Sonido")]
    [SerializeField] private AudioClip sonidoAtaque;
    [SerializeField] private AudioClip sonidoMuerte;
    private AudioSource audioSource;

    private Transform objetivoRehen;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        vidaActual = vidaMaxima;
        BuscarNuevoObjetivo();
    }

    private void Update()
    {
        CambiarSpriteSegunMovimiento();
    }

    private void FixedUpdate()
    {
        // Si el rehén que perseguía desapareció (porque fue salvado), busca otro
        if (objetivoRehen == null || !objetivoRehen.gameObject.activeInHierarchy)
        {
            BuscarNuevoObjetivo();
            if (objetivoRehen == null)
            {
                rb.linearVelocity = Vector2.zero; // No hay rehenes, se queda quieto
                return;
            }
        }

        // 1. Calcular el Vector Deseado hacia el objetivo (Atracción)
        Vector2 posicionActual = transform.position;
        Vector2 posicionObjetivo = objetivoRehen.position;
        Vector2 direccionDeseada = (posicionObjetivo - posicionActual).normalized;

        // 2. Modificar el Vector Deseado si hay obstáculos cerca (Evitación Matemática)
        direccionDeseada = EvitarObstaculos(direccionDeseada);

        // 3. Aplicar Fuerza de Dirección (Steering Force)
        Vector2 velocidadDeseada = direccionDeseada * velocidadMax;
        Vector2 fuerzaDireccion = velocidadDeseada - rb.linearVelocity;

        // Limitamos la fuerza para que el giro sea suave y orgánico
        fuerzaDireccion = Vector2.ClampMagnitude(fuerzaDireccion, fuerzaGiroMax);

        // Aplicamos la fuerza al Rigidbody
        rb.AddForce(fuerzaDireccion);
    }

    private Vector2 EvitarObstaculos(Vector2 direccionBase)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direccionBase, longitudSensor, capaObstaculos);

        if (hit.collider != null)
        {
            Vector2 normalObstaculo = hit.normal;
            float productoPunto = Vector2.Dot(direccionBase, normalObstaculo);

            if (productoPunto < 0)
            {
                Vector2 fuerzaDesvio = normalObstaculo * (-productoPunto) * 2f;
                return (direccionBase + fuerzaDesvio).normalized;
            }
        }
        return direccionBase;
    }

    private void CambiarSpriteSegunMovimiento()
    {
        Vector2 vel = rb.linearVelocity;
        if (vel.magnitude < 0.1f) return;

        if (Mathf.Abs(vel.y) > Mathf.Abs(vel.x))
        {
            if (vel.y > 0) spriteRenderer.sprite = spriteEspalda;
            else spriteRenderer.sprite = spriteFrente;
        }
        else
        {
            spriteRenderer.sprite = spritePerfil;
            spriteRenderer.flipX = (vel.x < 0);
        }
    }

    private void BuscarNuevoObjetivo()
    {
        GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");

        if (jugadorObj != null && jugadorObj.activeInHierarchy)
        {
            objetivoRehen = jugadorObj.transform;
        }
        else
        {
            objetivoRehen = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 dir = rb != null && rb.linearVelocity.magnitude > 0.1f ? (Vector3)rb.linearVelocity.normalized : transform.up;
        Gizmos.DrawLine(transform.position, transform.position + dir * longitudSensor);
    }

    // ==========================================
    // SISTEMA DE COLISIONES Y ATAQUE
    // ==========================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            JugadorTopDown jugador = collision.gameObject.GetComponent<JugadorTopDown>();
            if (jugador != null)
            {
                // Reproducir sonido de ataque
                if (audioSource != null && sonidoAtaque != null)
                {
                    audioSource.PlayOneShot(sonidoAtaque);
                }

                jugador.RecibirDanio(20f);
            }
        }
        else if (collision.gameObject.CompareTag("Rehen"))
        {
            Debug.Log("¡Un enemigo atrapó a un rehén!");
        }
    }

    // ==========================================
    // SISTEMA DE VIDA Y MUERTE
    // ==========================================
    public void RecibirDanio(float daño)
    {
        vidaActual -= daño;

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        // 1. Reproducir sonido de muerte usando un parlante temporal (así no se corta al destruir el objeto)
        if (sonidoMuerte != null)
        {
            AudioSource.PlayClipAtPoint(sonidoMuerte, transform.position);
        }

        // 2. Le avisamos al GameManager que reste un enemigo (usando el método que ya tienes)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EliminarEnemigo();
        }

        // 3. Destruimos al enemigo
        Destroy(gameObject);
    }
}