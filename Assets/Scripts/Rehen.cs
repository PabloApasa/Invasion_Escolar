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

    void Start()
    {
        if (iconoBoton != null) iconoBoton.SetActive(false);

        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }
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
                transform.position = Vector2.MoveTowards(transform.position, jugador.position, velocidadSeguimiento * Time.deltaTime);
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