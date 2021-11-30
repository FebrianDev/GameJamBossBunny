using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject gamePanel, settingPanel;

    // Buttons
    [SerializeField] private GameObject selectJoystickButton;
    [SerializeField] private GameObject selectButtonButton;

    [SerializeField] private GameObject soundOn;
    [SerializeField] private GameObject soundOff;

    public Image gamePanelImage, settingPanelImage;

    private int sound;


    private AudioSource audio;
    
    private void Awake()
    {
       audio = GameObject.FindWithTag("AudioManager").GetComponent<AudioSource>();
    }

    private void Start()
    {
        sound = PlayerPrefs.GetInt(Constant.KEY_SOUND, 0);
        showSettingPanel();
        DefaultSound();
        Debug.Log(sound);
    }

    public void showSettingPanel()
    {
        settingPanel.gameObject.SetActive(true);
        gamePanel.gameObject.SetActive(false);
        gamePanelImage.color = new Color32(88, 47, 0, 255);
        settingPanelImage.color = new Color32(255, 255, 255, 255);
    }

    public void showGamePanel()
    {
        gamePanel.gameObject.SetActive(true);
        settingPanel.gameObject.SetActive(false);

        settingPanelImage.color = new Color32(88, 47, 0, 255);
        gamePanelImage.color = new Color32(255, 255, 255, 255);
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
            audio.Pause();
            soundOn.SetActive(false);
            soundOff.SetActive(true);
            sound = 1;
        }
        else
        {
            audio.Play();
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