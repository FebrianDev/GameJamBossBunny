using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    private AudioSource audioSource;
    private bool soundIsOn;

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("AudioManager");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            var check = PlayerPrefs.GetInt(Constant.KEY_SOUND, 0);
            soundIsOn = (check == 0) ? true : false;
            audioSource = GetComponent<AudioSource>();
            if (soundIsOn)
            {
                audioSource.Play();
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
    }
}