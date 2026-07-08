using UnityEngine;

public class AtaqueJugador : MonoBehaviour
{
    [Header("Configuración del Ataque")]
    [SerializeField] private float alcanceAtaque = 5f;
    [SerializeField] private float conoApuntado = 0.85f; // 1 es perfecto, 0.85 es un cono de ~30 grados
    [SerializeField] private GameObject prefabProyectil;
    [SerializeField] private Transform puntoDisparo; // Lugar desde donde sale la bala

    private JugadorTopDown jugadorMovimiento;

    private void Awake()
    {
        // Obtenemos el script de movimiento para saber hacia dónde está mirando
        jugadorMovimiento = GetComponent<JugadorTopDown>();
    }

    private void Update()
    {
        // Detectar cuando se presiona la barra espaciadora
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AtacarYDetectarEnemigo();
        }
    }

    private void AtacarYDetectarEnemigo()
    {
        // 1. Obtener el vector de dirección hacia donde mira el jugador
        // Si tu script JugadorTopDown guarda la última dirección de movimiento, úsala.
        // Si no, podemos deducirla de su escala o usar el vector de disparo.
        Vector2 direccionMirada = ObtenerDireccionMirada();

        // Instanciar el proyectil físicamente en el juego
        if (prefabProyectil != null && puntoDisparo != null)
        {
            GameObject bala = Instantiate(prefabProyectil, puntoDisparo.position, Quaternion.identity);
            Rigidbody2D rbBala = bala.GetComponent<Rigidbody2D>();
            if (rbBala != null)
            {
                rbBala.linearVelocity = direccionMirada * 10f; // Velocidad de la bala
            }
        }

        // 2. MATEMÁTICA: Buscar si hay un enemigo en el cono de ataque para "fijarlo" o hacer daño extra
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemigo");
        GameObject enemigoObjetivo = null;
        float mejorAlineacion = -1f;

        foreach (GameObject enemigo in enemigos)
        {
            // Vector desde el jugador hacia el enemigo
            Vector2 vectorHaciaEnemigo = (enemigo.transform.position - transform.position);
            float distancia = vectorHaciaEnemigo.magnitude;

            // Solo evaluar si está dentro del rango de alcance
            if (distancia <= alcanceAtaque)
            {
                Vector2 direccionHaciaEnemigo = vectorHaciaEnemigo.normalized;

                // PRODUCTO PUNTO: Comparamos la dirección de la mirada con la dirección al enemigo
                float productoPunto = Vector2.Dot(direccionMirada, direccionHaciaEnemigo);

                // Si el producto punto es mayor que nuestro límite (conoApuntado)
                // y es el enemigo que está más alineado de frente
                if (productoPunto > conoApuntado && productoPunto > mejorAlineacion)
                {
                    mejorAlineacion = productoPunto;
                    enemigoObjetivo = enemigo;
                }
            }
        }

        // Si la matemática detectó un enemigo justo al frente
        if (enemigoObjetivo != null)
        {
            Debug.Log($"¡Disparo certero al enemigo: {enemigoObjetivo.name}! Alineación: {mejorAlineacion}");
            // Aquí podrías hacer que la bala sea teledirigida hacia él o infligir daño instantáneo
        }
    }

    private Vector2 ObtenerDireccionMirada()
    {
        // Intentamos leer los Sprites activos para saber hacia dónde dispara
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && jugadorMovimiento != null)
        {
            // Lógica basada en qué sprite tienes puesto o los valores de tus inputs
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (horizontal != 0 || vertical != 0)
            {
                return new Vector2(horizontal, vertical).normalized;
            }
        }

        // Dirección por defecto (Derecha) si está quieto
        return transform.right;
    }

    // Dibujar el cono de ataque matemático en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alcanceAtaque);
    }
}