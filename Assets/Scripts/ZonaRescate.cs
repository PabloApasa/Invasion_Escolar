using TMPro;
using UnityEngine;

public class ZonaRescate : MonoBehaviour
{
    [Header("Configuración de Rescate")]
    public int rehenesRescatados = 0;

    [Header("UI Canvas")]
    public TextMeshProUGUI textoContador; 

    void Start()
    {
        ActualizarTexto();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rehen"))
        {
            rehenesRescatados++;

            ActualizarTexto();   

    

            Destroy(collision.gameObject); 
        }
    }

    void ActualizarTexto()
    {
        if (textoContador != null)
        {

            textoContador.text = "Rehenes Salvados: " + rehenesRescatados;
        }
    }
}