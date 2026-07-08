using UnityEngine;

public class EnemigoPerseguidor : MonoBehaviour
{
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

    private Transform objetivoRehen;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        BuscarNuevoObjetivo();
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
        // Lanzamos un rayo en nuestra dirección actual de movimiento
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direccionBase, longitudSensor, capaObstaculos);

        if (hit.collider != null)
        {
            // Vector Normal: Perpendicular a la superficie del obstáculo chocada
            Vector2 normalObstaculo = hit.normal;

            // PRODUCTO PUNTO: Medimos qué tan "de frente" vamos hacia el obstáculo.
            // Si da cercano a -1, vamos directo a colisionar de frente.
            float productoPunto = Vector2.Dot(direccionBase, normalObstaculo);

            if (productoPunto < 0) // Significa que nos dirigimos hacia la pared
            {
                // Calculamos una fuerza de desvío empujando al enemigo en la dirección de la Normal
                // Cuanto más de frente choquemos, más fuerte será el desvío.
                Vector2 fuerzaDesvio = normalObstaculo * (-productoPunto) * 2f;

                // Retornamos la nueva dirección combinada (Hacia el rehén + Esquivar Obstáculo)
                return (direccionBase + fuerzaDesvio).normalized;
            }
        }

        // Si no hay peligro inmediato, seguimos el vector original hacia el rehén
        return direccionBase;
    }

    private void CambiarSpriteSegunMovimiento()
    {
        Vector2 vel = rb.linearVelocity;

        // Si se mueve muy lento, que mantenga el último sprite puesto
        if (vel.magnitude < 0.1f) return;

        // Comparamos matemáticamente si el movimiento es más vertical o más horizontal
        if (Mathf.Abs(vel.y) > Mathf.Abs(vel.x))
        {
            // Movimiento principalmente Vertical
            if (vel.y > 0) spriteRenderer.sprite = spriteEspalda; // Camina hacia arriba
            else spriteRenderer.sprite = spriteFrente;           // Camina hacia abajo
        }
        else
        {
            // Movimiento principalmente Horizontal
            spriteRenderer.sprite = spritePerfil;
            spriteRenderer.flipX = (vel.x < 0); // Si va a la izquierda (X negativo), voltea el sprite
        }
    }

    private void BuscarNuevoObjetivo()
    {
        // Busca todas las personas/rehenes en la escena
        RehenTopDown[] rehenes = FindObjectsByType<RehenTopDown>(FindObjectsSortMode.None);

        float distanciaMasCercana = Mathf.Infinity;
        RehenTopDown rehenMasCercano = null;

        foreach (RehenTopDown rehen in rehenes)
        {
            if (rehen.gameObject.activeInHierarchy)
            {
                float distancia = Vector2.Distance(transform.position, rehen.transform.position);
                if (distancia < distanciaMasCercana)
                {
                    distanciaMasCercana = distancia;
                    rehenMasCercano = rehen;
                }
            }
        }

        if (rehenMasCercano != null)
        {
            objetivoRehen = rehenMasCercano.transform;
        }
    }

    // Dibujar los vectores sensores en el editor para depuración visual
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Dibuja la línea del sensor frontal según el vector de velocidad actual
        Vector3 dir = rb != null && rb.linearVelocity.magnitude > 0.1f ? (Vector3)rb.linearVelocity.normalized : transform.up;
        Gizmos.DrawLine(transform.position, transform.position + dir * longitudSensor);
    }

    // SI TOCA AL REHÉN O AL PLAYER
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si choca con el jugador, le hace daño usando el script correcto (JugadorTopDown)
        if (collision.gameObject.CompareTag("Player"))
        {
            JugadorTopDown jugador = collision.gameObject.GetComponent<JugadorTopDown>();
            if (jugador != null) 
            {
                jugador.RecibirDanio(20f); 
            }
        }
        // Si choca contra un rehén
        else if (collision.gameObject.CompareTag("Rehen"))
        {
            Debug.Log("¡Un enemigo atrapó a un rehén!");
            // Aquí puedes destruir al rehén o restarlo si quieres
        }
    }
}