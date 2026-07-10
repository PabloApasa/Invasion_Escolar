using UnityEngine;

public class Bala2D : MonoBehaviour
{
    [SerializeField] private float velocidadBala = 15f;
    [SerializeField] private float tiempoDeVida = 3f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Forzar a la bala a ignorar al Player para que no se autodetecte
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            Collider2D colliderBala = GetComponent<Collider2D>();
            Collider2D colliderJugador = jugador.GetComponent<Collider2D>();
            if (colliderBala != null && colliderJugador != null)
            {
                Physics2D.IgnoreCollision(colliderBala, colliderJugador);
            }
        }

        Destroy(gameObject, tiempoDeVida);
    }

    //  ESTA ES LA FUNCIÓN QUE TE FALTA Y QUE SOLUCIONA EL ERROR 
    public void DispararHacia(Vector2 direccion)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direccion * velocidadBala;

            // Opcional: rotar la bala hacia donde viaja
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si choca con un enemigo
        if (collision.CompareTag("Enemigo"))
        {
            // Buscamos si el enemigo tiene el componente de vida
            VidaEnemigo saludEnemigo = collision.GetComponent<VidaEnemigo>();

            if (saludEnemigo != null)
            {
                saludEnemigo.RecibirDanio(); // Le quita 1 de vida al enemigo
            }

            Destroy(gameObject); // La bala estalla e impacta
        }
        // Si choca con una pared
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstaculos"))
        {
            Destroy(gameObject);
        }
    }
}