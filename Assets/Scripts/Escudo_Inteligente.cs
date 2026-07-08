using UnityEngine;

public class EscudoInteligente : MonoBehaviour
{
    [Header("Configuración del Escudo")]
    [SerializeField] private float radioOrbita = 1.5f;     // Distancia respecto al jugador
    [SerializeField] private float velocidadRotacion = 5f; // Qué tan rápido reacciona el escudo
    [SerializeField] private float velocidadGiroPasivo = 40f; // Giro normal cuando no hay peligro
    [SerializeField] private string etiquetaProyectil = "ProyectilEnemigo";

    private Transform jugador;
    private Transform proyectilPeligroso;

    private void Start()
    {
        // El script asume que está dentro de un objeto hijo del Player
        jugador = transform.parent;

        if (jugador == null)
        {
            Debug.LogError("¡El objeto del Escudo debe ser HIJO del objeto Player!");
        }
    }

    private void Update()
    {
        if (jugador == null) return;

        // Mantener el centro del sistema orbital exactamente en la posición del jugador
        transform.localPosition = Vector3.zero;

        // Buscar si hay proyectiles peligrosos en el mapa
        BuscarProyectilMasCercano();

        if (proyectilPeligroso != null && proyectilPeligroso.gameObject.activeInHierarchy)
        {
            IntercetarProyectil();
        }
        else
        {
            // Movimiento pasivo: si no hay peligro, el escudo gira tranquilamente alrededor
            transform.Rotate(Vector3.forward * velocidadGiroPasivo * Time.deltaTime);
        }
    }

    private void IntercetarProyectil()
    {
        // 1. Vector desde el jugador hacia el escudo (Hacia dónde apunta el escudo actualmente)
        // Tomamos el primer hijo (el sprite visual del escudo) para saber su dirección relativa
        Transform visualEscudo = transform.GetChild(0);
        Vector2 vectorEscudo = (visualEscudo.position - jugador.position).normalized;

        // 2. Vector desde el jugador hacia el proyectil enemigo
        Vector2 vectorProyectil = (proyectilPeligroso.position - jugador.position).normalized;

        // 3. MATEMÁTICA: Producto Cruz en 2D (Calcula el sentido de giro menor)
        float productoCruz2D = (vectorEscudo.x * vectorProyectil.y) - (vectorEscudo.y * vectorProyectil.x);

        // Determinamos la dirección del giro basándonos en el signo del producto cruz
        float direccionGiro = productoCruz2D > 0 ? 1f : -1f;

        // Calculamos el ángulo restante entre el escudo y el proyectil
        float anguloFaltante = Vector2.Angle(vectorEscudo, vectorProyectil);

        // Si el ángulo es grande, rotamos el escudo usando la dirección del producto cruz
        if (anguloFaltante > 2f)
        {
            transform.Rotate(Vector3.forward * direccionGiro * velocidadRotacion * anguloFaltante * Time.deltaTime);
        }
    }

    private void BuscarProyectilMasCercano()
    {
        GameObject[] proyectiles = GameObject.FindGameObjectsWithTag(etiquetaProyectil);
        float menorDistancia = Mathf.Infinity;
        proyectilPeligroso = null;

        foreach (GameObject proj in proyectiles)
        {
            float distancia = Vector2.Distance(jugador.position, proj.transform.position);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                proyectilPeligroso = proj.transform;
            }
        }
    }
}