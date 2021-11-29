using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject gamePanel, settingPanel;

    private void Start()
    {
        showGamePanel();
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
}
