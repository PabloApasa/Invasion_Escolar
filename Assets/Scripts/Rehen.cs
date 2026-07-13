using UnityEngine;
using TMPro;
public class Rehen : MonoBehaviour
{
    [Header("Configuración UI")]
    public GameObject iconoBoton; 

    [Header("Matemáticas de Interacción")]
    public float radioDeDeteccion = 3f;   
    public float distanciaDeParada = 1.5f;
    public float velocidadSeguimiento = 4f;

    private Transform jugador;
    private bool estaSiguiendo = false;
    private Animator animator;

    void Start()
    {
        if (iconoBoton != null) iconoBoton.SetActive(false);

        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }
        animator = GetComponent<Animator>(); // inicializar Animator
    }

    void Update()
    {
        if (jugador == null) return;

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        if (!estaSiguiendo)
        {
            if (distanciaAlJugador <= radioDeDeteccion)
            {
                if (iconoBoton != null) iconoBoton.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    estaSiguiendo = true;
                    animator.SetBool("isFollowing", true); // activa animaciones
                    if (iconoBoton != null) iconoBoton.SetActive(false);
                }
            }
            else
            {
                if (iconoBoton != null) iconoBoton.SetActive(false);
            }
        }
        else
        {
            if (distanciaAlJugador > distanciaDeParada)
            {
                Vector2 direccion = (jugador.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, jugador.position, velocidadSeguimiento * Time.deltaTime);

                animator.SetBool("isMoving", true);
                animator.SetFloat("velX", direccion.x);
                animator.SetFloat("velY", direccion.y);
            }
            else
            {
                animator.SetBool("isMoving", false);
                
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioDeDeteccion);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeParada);
    }
}