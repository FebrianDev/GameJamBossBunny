using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public GameObject gamePanel, settingPanel;
    
    // Buttons
    [SerializeField] private GameObject selectJoystickButton;
    [SerializeField] private GameObject selectButtonButton;
    
    [SerializeField] private GameObject soundOn;
    [SerializeField] private GameObject soundOff;

    private int sound;
    
    private void Start()
    {
        sound = PlayerPrefs.GetInt(Constant.KEY_SOUND, 0);
        showGamePanel();
        DefaultSound();
        Debug.Log(sound);
    }

    public void showSettingPanel()
    {
        settingPanel.gameObject.SetActive(true);
        gamePanel.gameObject.SetActive(false);
    }

    public void showGamePanel()
    {
        gamePanel.gameObject.SetActive(true);
        settingPanel.gameObject.SetActive(false);
    }
    
    public void SelectController()
    {
        if (PlayerManager.isJoystick)
        {
            selectJoystickButton.SetActive(false);
            selectButtonButton.SetActive(true);
            PlayerManager.isJoystick = false;
        }
        else
        {
            selectJoystickButton.SetActive(true);
            selectButtonButton.SetActive(false);
            PlayerManager.isJoystick = true;
        }
    }

    private void DefaultSound()
    {
        if (sound == 1)
        {
            soundOn.SetActive(false);
            soundOff.SetActive(true);
            
        }
        else
        {
            soundOn.SetActive(true);
            soundOff.SetActive(false);
        }
    }
    
    public void Sound()
    {
        if (sound == 0)
        {
            soundOn.SetActive(false);
            soundOff.SetActive(true);
            sound = 1;
        }
        else
        {
            soundOn.SetActive(true);
            soundOff.SetActive(false);
            sound = 0;
        }
        
        PlayerPrefs.SetInt(Constant.KEY_SOUND, sound);
    }
    
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
