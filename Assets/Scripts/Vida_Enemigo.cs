using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    [Header("Sistema de Salud")]
    [SerializeField] private int impactosMaximos = 3; // Ocupa 3 golpes para morir
    private int impactosActuales;

    private void Start()
    {
        impactosActuales = impactosMaximos;
    }

    // Esta función pública la llamará la bala al chocar
    public void RecibirDanio()
    {
        impactosActuales--; // Restamos 1 vida
        Debug.Log($"[Enemigo] ¡Ay! Me quedan {impactosActuales} impactos de vida.");

        if (impactosActuales <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log($"¡Enemigo {gameObject.name} destruido por completo!");

        // Avisamos al GameManager (si lo usas para las oleadas) antes de destruir el objeto
        // GameManager.Instance.RestarEnemigoLista(); 

        Destroy(gameObject); // Elimina al enemigo del mapa
    }
}