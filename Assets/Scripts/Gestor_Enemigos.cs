using UnityEngine;
using System.Collections; // OBLIGATORIO para poder usar Corrutinas (IEnumerator)

public class GeneradorEnemigos : MonoBehaviour
{
    [Header("Referencia del Enemigo")]
    [SerializeField] private GameObject prefabEnemigo; // El enemigo original guardado como Prefab

    [Header("Área de Generación")]
    [SerializeField] private Vector2 rangoX = new Vector2(-8f, 8f); // Mínimo y Máximo en el eje X
    [SerializeField] private Vector2 rangoY = new Vector2(-4f, 4f); // Mínimo y Máximo en el eje Y

    [Header("Configuración del Tiempo")]
    [SerializeField] private float tiempoEntreEnemigos = 30f; // Tiempo de espera en segundos

    private void Start()
    {
        if (prefabEnemigo == null)
        {
            Debug.LogError("¡No has asignado el Prefab del enemigo en el Generador!");
            return;
        }

        // Iniciamos la corrutina que controla la aparición progresiva
        StartCoroutine(GenerarEnemigosPocoAPoco());
    }

    private IEnumerator GenerarEnemigosPocoAPoco()
    {
        // Esperamos un momento inicial para asegurarnos de que el GameManager ya está listo
        yield return new WaitForSeconds(0.2f);

        // Accedemos a la variable del GameManager para saber el límite de enemigos
        int cantidadTotal = GameManager.Instance.GetEnemigosTotales();
        Debug.Log($"Iniciando generación de oleada. Se crearán {cantidadTotal} enemigos en total, uno cada {tiempoEntreEnemigos} segundos.");

        for (int i = 0; i < cantidadTotal; i++)
        {
            // 1. Calcular una posición aleatoria dentro de los rangos establecidos
            float posX = Random.Range(rangoX.x, rangoX.y);
            float posY = Random.Range(rangoY.x, rangoY.y);
            Vector3 posicionAleatoria = new Vector3(posX, posY, 0f);

            // 2. Crear el enemigo en esa posición
            Instantiate(prefabEnemigo, posicionAleatoria, Quaternion.identity);
            Debug.Log($"[Generador] Enemigo número {i + 1}/{cantidadTotal} ha aparecido.");

            // 3. MATEMÁTICA DEL TIEMPO: Pausamos este script por 30 segundos antes de continuar con el siguiente ciclo del 'for'
            // Solo pausamos en los intermedios, si es el último enemigo ya no necesita esperar
            if (i < cantidadTotal - 1)
            {
                yield return new WaitForSeconds(tiempoEntreEnemigos);
            }
        }

        Debug.Log("¡Todos los enemigos de la oleada han sido generados!");
    }

    // Dibuja un rectángulo verde en el editor para que puedas ver el área donde nacerán los enemigos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 centro = new Vector3((rangoX.x + rangoX.y) / 2, (rangoY.x + rangoY.y) / 2, 0f);
        Vector3 tamanio = new Vector3(rangoX.y - rangoX.x, rangoY.y - rangoY.x, 1f);
        Gizmos.DrawWireCube(centro, tamanio);
    }
}