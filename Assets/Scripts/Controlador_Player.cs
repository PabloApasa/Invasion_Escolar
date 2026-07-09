using UnityEngine;

public class JugadorTopDown : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;
    private Vector2 movimiento;

    [Header("Componentes Visuales")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sistema de Daño")]
    [SerializeField] private float tiempoInmunidad = 1.5f; // Segundos que es invencible tras un golpe
    private float temporizadorInmunidad;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Movimiento e Inputs
        movimiento.x = Input.GetAxisRaw("Horizontal");
        movimiento.y = Input.GetAxisRaw("Vertical");

        if (movimiento.magnitude > 1)
        {
            movimiento.Normalize();
        }

        Animacion_Player.UpdateMovementAnimation(movimiento);

        // Temporizador de Inmunidad
        if (temporizadorInmunidad > 0)
        {
            temporizadorInmunidad -= Time.deltaTime;

            // Efecto visual de parpadeo para indicar que es invencible
            spriteRenderer.color = (Mathf.Sin(Time.time * 20) > 0) ? new Color(1, 0.5f, 0.5f, 0.5f) : Color.white;
        }
        else
        {
            spriteRenderer.color = Color.white; // Vuelve a la normalidad
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movimiento * velocidad;
    }

    // ==========================================
    // SISTEMA DE DAÑO Y CONEXIÓN CON GAMEMANAGER
    // ==========================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cuando toca a un enemigo...
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            RecibirDanio(20f);
        }
    }

    // Si el enemigo se queda pegado, lo sigue atacando cada vez que se acaba la inmunidad
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            RecibirDanio(20f);
        }
    }

    public void RecibirDanio(float cantidad)
    {
        // Si el jugador es inmune, ignoramos el daño
        if (temporizadorInmunidad > 0) return;

        Debug.Log("¡El jugador recibió " + cantidad + " de daño!");

        // Restamos vida en el GameManager
        GameManager.Instance.ModificarVida(-cantidad);

        // Activamos la inmunidad temporal
        temporizadorInmunidad = tiempoInmunidad;
    }
}