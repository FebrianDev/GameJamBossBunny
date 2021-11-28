using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    private string keyName;
    void Start()
    {
        keyName = PlayerPrefs.GetString(Constant.KEY_NAME);

        StartCoroutine("StartSplashCreen");
    }

    IEnumerator StartSplashCreen()
    {
        yield return new WaitForSeconds(3);
        if (keyName.Equals(""))
        {
            SceneManager.LoadScene("Register");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
