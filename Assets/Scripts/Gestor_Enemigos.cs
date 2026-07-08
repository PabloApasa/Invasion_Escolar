using UnityEngine;

public class GeneradorEnemigos : MonoBehaviour
{
    [Header("Referencia del Enemigo")]
    [SerializeField] private GameObject prefabEnemigo; // El enemigo original guardado como Prefab

    [Header("Área de Generación")]
    [SerializeField] private Vector2 rangoX = new Vector2(-8f, 8f); // Mínimo y Máximo en el eje X
    [SerializeField] private Vector2 rangoY = new Vector2(-4f, 4f); // Mínimo y Máximo en el eje Y

    private void Start()
    {
        // Esperamos un cuadro para asegurarnos de que el GameManager ya se inició correctamente
        Invoke("GenerarOleada", 0.1f);
    }

    private void GenerarOleada()
    {
        if (prefabEnemigo == null)
        {
            Debug.LogError("¡No has asignado el Prefab del enemigo en el Generador!");
            return;
        }

        // Accedemos directamente a la variable del GameManager para saber cuántos crear
        int cantidadACrear = GameManager.Instance.GetEnemigosTotales();

        for (int i = 0; i < cantidadACrear; i++)
        {
            // Calcular una posición aleatoria dentro de los rangos establecidos
            float posX = Random.Range(rangoX.x, rangoX.y);
            float posY = Random.Range(rangoY.x, rangoY.y);
            Vector3 posicionAleatoria = new Vector3(posX, posY, 0f);

            // Crear el enemigo en esa posición
            Instantiate(prefabEnemigo, posicionAleatoria, Quaternion.identity);
        }

        Debug.Log("Se han generado " + cantidadACrear + " enemigos en el mapa.");
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