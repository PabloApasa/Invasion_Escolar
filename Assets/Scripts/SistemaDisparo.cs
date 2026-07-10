using UnityEngine;

public class SistemaDisparo : MonoBehaviour
{
    [Header("Configuración del Arma")]
    [SerializeField] private GameObject prefabBala;
    [SerializeField] private Transform puntoDeDisparo;

    private OrientarPuntoDisparo orientador;

    void Awake()
    {
        // Buscamos el componente de orientación automática en el punto de disparo
        if (puntoDeDisparo != null)
        {
            orientador = puntoDeDisparo.GetComponent<OrientarPuntoDisparo>();
        }
    }

    void Update()
    {
        // Mantenemos tu detección de disparo (Clic izquierdo del mouse)
        if (Input.GetMouseButtonDown(0))
        {
            EjecutarDisparo();
        }
    }

    void EjecutarDisparo()
    {
        if (prefabBala == null) return;

        // 1. Definimos el origen exacto del disparo
        Vector3 origen = puntoDeDisparo != null ? puntoDeDisparo.position : transform.position;

        // 2. MATEMÁTICA: Si el orientador encontró un enemigo, usamos esa dirección. 
        // Si no, recurre a tu cálculo original por Mouse.
        Vector2 direccion;
        if (orientador != null)
        {
            direccion = orientador.DireccionObjetivo;
        }
        else
        {
            Vector3 posicionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            posicionMouse.z = 0f;
            direccion = ((Vector2)posicionMouse - (Vector2)origen).normalized;
        }

        // 3. Creamos la bala en la escena
        GameObject nuevaBala = Instantiate(prefabBala, origen, Quaternion.identity);

        // 4. Pasamos la dirección al script de la bala física
        Bala2D scriptBala = nuevaBala.GetComponent<Bala2D>();
        if (scriptBala != null)
        {
            scriptBala.DispararHacia(direccion);
        }
        else
        {
            // Soporte directo por si usas físicas Rigidbody2D nativas
            Rigidbody2D rbBala = nuevaBala.GetComponent<Rigidbody2D>();
            if (rbBala != null) rbBala.linearVelocity = direccion * 15f;
        }
    }
}