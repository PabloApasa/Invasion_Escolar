using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject optionPanel;
    public GameObject playPanel;

    public void ExitGame()
    {
        Application.Quit();
    }

    public void CambiarNivel1()
    {
        SceneManager.LoadScene("NIvel-01");
    }

    public void CambiarNivel2()
    {
        SceneManager.LoadScene("NIvel-02");
    }

    public void mainMenu()
    {
        mainPanel.SetActive(true);
        optionPanel.SetActive(false);
        playPanel.SetActive(false);
    }

    public void optionMenu()
    {
        mainPanel.SetActive(false);
        optionPanel.SetActive(true);
        playPanel.SetActive(false);
    }

    public void playMenu()
    {
        mainPanel.SetActive(false);
        optionPanel.SetActive(false);
        playPanel.SetActive(true);
    }
}