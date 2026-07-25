using UnityEngine;
using UnityEngine.SceneManagement;

public class Creditos : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void VolverMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void SalirJuego()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego..."); 
    }
}