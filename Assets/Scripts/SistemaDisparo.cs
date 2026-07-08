using UnityEngine;
using System.Collections;

public class SistemaDisparo : MonoBehaviour
{
    [Header("Configuración del Arma")]
    public GameObject prefabBala;    // Arrastra aquí el Prefab de tu bala
    public Transform puntoDeDisparo; // De dónde sale la bala (opcional)

    void Update()
    {
        // 0 es el clic izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            EjecutarDisparo();
        }
    }

    void EjecutarDisparo()
    {
        if (prefabBala == null) return;

        // 1. Convertimos la posición del mouse de píxeles a coordenadas del juego
        Vector3 posicionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        posicionMouse.z = 0f; // Aseguramos que estamos en 2D

        // 2. Definimos de dónde sale la bala
        Vector3 origen = puntoDeDisparo != null ? puntoDeDisparo.position : transform.position;

        // 3. Calculamos la dirección (Destino - Origen) y la normalizamos (longitud 1)
        Vector2 direccion = ((Vector2)posicionMouse - (Vector2)origen).normalized;

        // 4. Creamos la bala en la escena
        GameObject nuevaBala = Instantiate(prefabBala, origen, Quaternion.identity);

        // 5. Buscamos el script de la bala y le pasamos la dirección
        Bala2D scriptBala = nuevaBala.GetComponent<Bala2D>();
        if (scriptBala != null)
        {
            scriptBala.DispararHacia(direccion);
        }
    }
}