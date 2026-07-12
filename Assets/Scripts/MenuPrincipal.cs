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

    private void Start()
    {
        // 1. CARGAR LOS DATOS GUARDADOS
        float masterGuardado = PlayerPrefs.GetFloat("VolMasterGuardado", 0f);
        float fxGuardado = PlayerPrefs.GetFloat("VolFXGuardado", 0f);

        // Cargar el estado del botón mute (1 significa sí, 0 significa no)
        bool isMuted = PlayerPrefs.GetInt("MuteGuardado", 0) == 1;

        // 2. ACTUALIZAR LOS SLIDERS Y EL TOGGLE VISUALMENTE
        if (volumen != null) volumen.value = masterGuardado;
        if (FXvolumen != null) FXvolumen.value = fxGuardado;
        if (mute != null) mute.isOn = isMuted;

        // 3. APLICAR AL AUDIO MIXER
        if (mixer != null)
        {
            // Si estaba muteado, lo bajamos a -80. Si no, le ponemos el volumen guardado.
            mixer.SetFloat("VolMaster", isMuted ? -80f : masterGuardado);
            mixer.SetFloat("VolFX", fxGuardado);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetMute()
    {
        if (mute.isOn)
        {
            // Le quitamos el "float" extra para que use la variable de arriba
            mixer.GetFloat("VolMaster", out lastVolume);
            mixer.SetFloat("VolMaster", -80f);

            // Guardamos que el juego está muteado
            PlayerPrefs.SetInt("MuteGuardado", 1);
        }
        else
        {
            // Al desmutear, usamos el valor donde esté el slider
            mixer.SetFloat("VolMaster", volumen.value);

            // Guardamos que el juego ya no está muteado
            PlayerPrefs.SetInt("MuteGuardado", 0);
        }
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
        // Solo cambiamos el sonido si NO está muteado
        if (!mute.isOn)
        {
            mixer.SetFloat("VolMaster", v);
        }

        // Guardamos el volumen en la memoria usando LA MISMA LLAVE que el GameManager
        PlayerPrefs.SetFloat("VolMasterGuardado", v);
    }

    public void ChangeVolumenFX(float v)
    {
        mixer.SetFloat("VolFX", v);

        // Guardamos en la memoria
        PlayerPrefs.SetFloat("VolFXGuardado", v);
    }

    public void CambiarNivel1()
    {
        SceneManager.LoadScene("NIvel-01");
    }

    public void CambiarNivel2()
    {
        SceneManager.LoadScene("NIvel-02");
    }

    public void PlaySoundButton()
    {
        fxSource.PlayOneShot(clickSound);
    }
}