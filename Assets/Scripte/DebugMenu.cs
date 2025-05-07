using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class DebugMenu : MonoBehaviour
{
    public GameObject DebugMen;
    public GameObject playercam;
    public Slider Camslider;

    public InputAction DebugButton;

    bool ActiveMenu = false;
    bool Pressed = false;

    void OnEnable()
    {
        DebugButton.Enable();
    }

    void OnDisable()
    {
        DebugButton.Disable();
    }

    void Start()
    {
        if(playercam != null){
        playercam.SendMessage("SetMouseSens", PlayerPrefs.GetFloat("CamSens", 0.3f));
        Camslider.value = PlayerPrefs.GetFloat("CamSens", 0.3f);
        }
    }

    void Update()
    {
        if(DebugButton.ReadValue<float>() == 1f && !Pressed)
        {
            Pressed = true;
            ActiveMenu = !ActiveMenu;
            Time.timeScale = ActiveMenu?0f:1f;
            DebugMen.SetActive(ActiveMenu);
            Cursor.lockState = ActiveMenu? CursorLockMode.None : CursorLockMode.Locked;
        }
        if(DebugButton.ReadValue<float>() == 0f && Pressed)
        {
            Pressed = false;
        }
    }

    public void MouseSens(float sens)
    {
        playercam.SendMessage("SetMouseSens", PlayerPrefs.GetFloat("CamSens", 0.3f));
        PlayerPrefs.SetFloat("CamSens", sens);
    }

    public void LoadOtherScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
    public void ResetScene()
    {
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
