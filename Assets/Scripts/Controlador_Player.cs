using UnityEngine;

public class JugadorTopDown : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;
    private Vector2 movimiento;

    [Header("Efectos Visuales")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    // Aquí pondremos los sprites correspondientes a cada dirección
    [SerializeField] private Sprite spriteFrente;
    [SerializeField] private Sprite spriteEspalda;
    [SerializeField] private Sprite spritePerfil; // El de un costado (normalmente mirando a la derecha)

    // Componentes internos
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 1. Capturar la entrada en dos ejes (X para lados, Y para arriba/abajo)
        movimiento.x = Input.GetAxisRaw("Horizontal");
        movimiento.y = Input.GetAxisRaw("Vertical");

        // Normalizamos el vector para que no camine más rápido cuando va en diagonal
        if (movimiento.magnitude > 1)
        {
            movimiento.Normalize();
        }

        // 2. Controlar qué Sprite mostrar según la dirección
        CambiarSpriteSegunDireccion();
    }

    private void FixedUpdate()
    {
        // 3. Aplicar el movimiento directamente a las físicas (sin gravedad)
        rb.linearVelocity = movimiento * velocidad;
    }

    private void CambiarSpriteSegunDireccion()
    {
        // Si se mueve hacia arriba (se va de espalda)
        if (movimiento.y > 0)
        {
            spriteRenderer.sprite = spriteEspalda;
        }
        // Si se mueve hacia abajo (viene de frente, se le ve el rostro)
        else if (movimiento.y < 0)
        {
            spriteRenderer.sprite = spriteFrente;
        }
        // Si se mueve hacia los lados
        else if (movimiento.x != 0)
        {
            spriteRenderer.sprite = spritePerfil;
            // Volteamos el sprite horizontalmente si va a la izquierda
            spriteRenderer.flipX = (movimiento.x < 0);
        }
    }

    // ==========================================
    // SISTEMA DE DAÑO Y CONEXIÓN CON GAMEMANAGER
    // ==========================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            RecibirDanio(20f); // Le avisa al GameManager para restar vida
        }
    }

    public void RecibirDanio(float cantidad)
    {
        Debug.Log("¡El jugador recibió daño!");
        GameManager.Instance.ModificarVida(-cantidad);
    }
}