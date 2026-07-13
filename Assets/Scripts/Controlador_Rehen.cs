using UnityEngine;

public class RehenTopDown : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    [SerializeField] private float distanciaDeteccion = 2f;
    [SerializeField] private float distanciaSeguridad = 1.2f;
    [SerializeField] private float velocidadSeguimiento = 4f;

    private Transform transJugador;
    private bool siguiendoAlJugador = false;
    private bool rescatado = false;
    private Rigidbody2D rb;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

        // Activar seguimiento con tecla E
        if (!siguiendoAlJugador && distanciaAlJugador <= distanciaDeteccion)
        {
            siguiendoAlJugador = true;
            Debug.Log("¡Rehén comenzó a seguirte!");
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