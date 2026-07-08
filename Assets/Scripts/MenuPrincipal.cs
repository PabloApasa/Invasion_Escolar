using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Option")]
    public Slider volumen;
    public Slider FXvolumen;
    public Toggle mute;
    public AudioMixer mixer;
    public AudioSource fxSource;
    public AudioClip clickSound;
    private float lastVolume;
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject optionPanel;
    public GameObject playPanel;

    private void Awake()
    {
        volumen.onValueChanged.AddListener(ChangeVolumenMaster);
        FXvolumen.onValueChanged.AddListener(ChangeVolumenFX);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetMute()
    {

        if (mute.isOn)
        {
            mixer.GetFloat("VolMaster", out float lastVolume);
            mixer.SetFloat("VolMaster", -80);
        }
        else
            mixer.SetFloat("VolMaster", lastVolume);

    }
    public void OpenPanel(GameObject panel)
    {
        mainPanel.SetActive(false);
        optionPanel.SetActive(false);
        PlaySoundButton();
        panel.SetActive(true);
    }
    public void ChangeVolumenMaster(float v)
    {
        mixer.SetFloat("VolMaster", v);
    }
    public void ChangeVolumenFX(float v)
    {
        mixer.SetFloat("VolFX", v);
    }
    public void CambiarNivel1()
    {
        SceneManager.LoadScene("Nivel_1");
    }

    public void CambiarNivel2()
    {
        SceneManager.LoadScene("Nivel_2");
    }
    public void PlaySoundButton()
    {
        fxSource.PlayOneShot(clickSound);
    }
}