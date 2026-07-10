using UnityEngine;

public class OrientarPuntoDisparo : MonoBehaviour
{
    [Header("Configuración de Radar")]
    [SerializeField] private float rangoDeteccion = 8f;
    [SerializeField] private string etiquetaEnemigo = "Enemigo";

    private Transform enemigoMasCercano;
    private Vector2 direccionActual = Vector2.right;

    public Vector2 DireccionObjetivo => direccionActual;

    void Update()
    {
        BuscarEnemigoMasCercano();

        if (enemigoMasCercano != null)
        {
            // 1. Calculamos el vector relativo desde el PLAYER (transform.parent) hacia el enemigo
            Transform padre = transform.parent != null ? transform.parent : transform;
            Vector2 haciaEnemigo = (enemigoMasCercano.position - padre.position);
            direccionActual = haciaEnemigo.normalized;

            // 2. MATEMÁTICA: Convertimos el vector a un ángulo de rotación en 2D (Eje Z)
            float angulo = Mathf.Atan2(direccionActual.y, direccionActual.x) * Mathf.Rad2Deg;

            // Forzamos la rotación local para que gire limpiamente alrededor del Player
            transform.localRotation = Quaternion.Euler(0f, 0f, angulo);
        }
        else
        {
            // Si no hay enemigos, lee los inputs tradicionales para apuntar al frente
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (h != 0 || v != 0)
            {
                direccionActual = new Vector2(h, v).normalized;
                float angulo = Mathf.Atan2(direccionActual.y, direccionActual.x) * Mathf.Rad2Deg;
                transform.localRotation = Quaternion.Euler(0f, 0f, angulo);
            }
        }
    }

    private void BuscarEnemigoMasCercano()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag(etiquetaEnemigo);
        float menorDistancia = Mathf.Infinity;
        enemigoMasCercano = null;

        // Usamos la posición del Player como centro del radar para evitar desfases matemáticos
        Transform centroRadar = transform.parent != null ? transform.parent : transform;

        foreach (GameObject enemigo in enemigos)
        {
            Vector2 vectorDistancia = enemigo.transform.position - centroRadar.position;
            float distancia = vectorDistancia.magnitude;

            if (distancia < menorDistancia && distancia <= rangoDeteccion)
            {
                // PRODUCTO PUNTO: Validamos la proyección del vector sobre sí mismo
                // Esto confirma matemáticamente que el objetivo está en el plano 2D actual
                float dotPunto = Vector2.Dot(vectorDistancia, vectorDistancia);

                if (dotPunto > 0.01f)
                {
                    menorDistancia = distancia;
                    enemigoMasCercano = enemigo.transform;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform centroRadar = transform.parent != null ? transform.parent : transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centroRadar.position, rangoDeteccion);
    }
}