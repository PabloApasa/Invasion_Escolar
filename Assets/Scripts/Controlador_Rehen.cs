using UnityEngine;

public class RehenTopDown : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    [SerializeField] private float distanciaDeteccion = 2f;
    [SerializeField] private float distanciaSeguridad = 1.2f;
    [SerializeField] private float velocidadSeguimiento = 4f;

    [Header("Efectos Visuales (Sprites)")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite spriteFrente;
    [SerializeField] private Sprite spriteEspalda;
    [SerializeField] private Sprite spritePerfil; // Mirando a la derecha

    private Transform transJugador;
    private bool siguiendoAlJugador = false;
    private bool rescatado = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Si te olvidas de arrastrarlo, lo busca automáticamente en el objeto
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObj != null)
        {
            transJugador = jugadorObj.transform;
        }
    }

    private void Update()
    {
        if (rescatado || transJugador == null) return;

        float distanciaAlJugador = Vector2.Distance(transform.position, transJugador.position);

        if (!siguiendoAlJugador && distanciaAlJugador <= distanciaDeteccion)
        {
            siguiendoAlJugador = true;
            Debug.Log("¡Rehén comenzó a seguirte!");
        }

        // Controlar la orientación visual si se está moviendo
        if (siguiendoAlJugador)
        {
            CambiarSpriteSegunMovimiento();
        }
    }

    private void FixedUpdate()
    {
        if (siguiendoAlJugador && !rescatado && transJugador != null)
        {
            float distanciaAlJugador = Vector2.Distance(transform.position, transJugador.position);

            if (distanciaAlJugador > distanciaSeguridad)
            {
                Vector2 direccion = ((Vector2)transJugador.position - (Vector2)transform.position).normalized;
                rb.linearVelocity = direccion * velocidadSeguimiento;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private void CambiarSpriteSegunMovimiento()
    {
        // Usamos la velocidad actual del Rigidbody para saber hacia dónde camina
        Vector2 vel = rb.linearVelocity;

        // Si se detiene por la distancia de seguridad, no cambia de sprite
        if (vel.magnitude < 0.1f) return;

        // Evaluamos si el movimiento es más vertical o más horizontal
        if (Mathf.Abs(vel.y) > Mathf.Abs(vel.x))
        {
            // Movimiento Vertical
            if (vel.y > 0) spriteRenderer.sprite = spriteEspalda;
            else spriteRenderer.sprite = spriteFrente;
        }
        else
        {
            // Movimiento Horizontal
            spriteRenderer.sprite = spritePerfil;
            spriteRenderer.flipX = (vel.x < 0); // Lo voltea si va a la izquierda
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (siguiendoAlJugador && !rescatado && collision.CompareTag("ZonaRescate"))
        {
            CompletarRescate();
        }
    }

    private void CompletarRescate()
    {
        rescatado = true;
        siguiendoAlJugador = false;
        rb.linearVelocity = Vector2.zero;
        GameManager.Instance.SalvarPersona();
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
    }
}