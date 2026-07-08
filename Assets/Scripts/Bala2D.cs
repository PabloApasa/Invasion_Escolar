using UnityEngine;

public class Bala2D : MonoBehaviour
{
    public float velocidad = 15f;
    public float tiempoDeVida = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Se destruye automáticamente después de 'tiempoDeVida' segundos
        Destroy(gameObject, tiempoDeVida);
    }

    // Esta función la llamaremos desde el otro script justo al disparar
    public void DispararHacia(Vector2 direccion)
    {
        if (rb != null)
        {
            // CAMBIO AQUÍ: Usamos linearVelocity en lugar de velocity
            rb.linearVelocity = direccion * velocidad;

            // Rotamos la bala para que apunte visualmente hacia donde va
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si lo que chocamos tiene la etiqueta "Enemigo"
        if (collision.CompareTag("Enemigo"))
        {
            // Buscamos el Rigidbody2D del enemigo para poder empujarlo
            Rigidbody2D rbEnemigo = collision.GetComponent<Rigidbody2D>();

            if (rbEnemigo != null)
            {
                // 1. Calculamos la dirección del impacto (la misma hacia la que viaja la bala)
                Vector2 direccionImpacto = rb.linearVelocity.normalized;

                // 2. Definimos qué tan fuerte es el empuje (puedes cambiar este número)
                float fuerzaDeRetroceso = 5f;

                // 3. Le aplicamos un empujón (Impulso) al enemigo
                rbEnemigo.AddForce(direccionImpacto * fuerzaDeRetroceso, ForceMode2D.Impulse);
            }
        }

        // La bala se destruye siempre al chocar con cualquier cosa (con o sin Rigidbody)
        Destroy(gameObject);
    }
}

