using TMPro;
using UnityEngine;

public class DialogoInicio : MonoBehaviour
{
    [Header("Configuración de Diálogo")]
    public TextMeshProUGUI textoHistoria; // Arrastra aquí tu texto del Panel

    [TextArea(3, 5)] // Esto hace que la caja de texto en Unity sea más grande para escribir cómodo
    public string[] lineasDeDialogo; // Lista con todas las páginas de tu historia

    private int indiceActual = 0;// Índice para saber qué línea de diálogo estamos mostrando

    void Start()
    {
        // Al iniciar, mostramos la primera página de texto
        if (lineasDeDialogo.Length > 0 && textoHistoria != null)
        {
            textoHistoria.text = lineasDeDialogo[0];
        }
    }

    void Update()
    {
        // Avanzamos el diálogo si el jugador hace clic izquierdo o presiona Espacio/Enter
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            AvanzarDialogo();
        }
    }

    public void AvanzarDialogo()
    {
        indiceActual++; // Pasamos a la siguiente línea

        // Si todavía quedan textos por leer...
        if (indiceActual < lineasDeDialogo.Length)
        {
            textoHistoria.text = lineasDeDialogo[indiceActual];
        }
        else
        {
            // Si ya no quedan textos, cerramos el panel y empezamos a jugar
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ComenzarJuego();
            }
        }
    }
}