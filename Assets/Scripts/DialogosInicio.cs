using TMPro;
using UnityEngine;

public class DialogosInicio : MonoBehaviour
{
    [Header("Configuración de Diálogo")]
    public TextMeshProUGUI textoHistoria; 

    [TextArea(3, 5)] 
    public string[] lineasDeDialogo; 

    private int indiceActual = 0;

    void Start()
    {
        Time.timeScale = 0f;

        if (lineasDeDialogo.Length > 0 && textoHistoria != null)
        {
            textoHistoria.text = lineasDeDialogo[0];
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            AvanzarDialogo();
        }
    }

    public void AvanzarDialogo()
    {
        indiceActual++; 

        if (indiceActual < lineasDeDialogo.Length)
        {
            textoHistoria.text = lineasDeDialogo[indiceActual];
        }
        else
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ComenzarJuego();
            }

            gameObject.SetActive(false);
        }
    }
}