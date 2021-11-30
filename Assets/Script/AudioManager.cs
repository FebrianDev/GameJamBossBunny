﻿using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class AudioManager : MonoBehaviourPunCallbacks
{
	public static AudioManager instance;
	// List of the sound
	public Sound[] sounds;
	// Sound is on or not
	public static bool soundIsOn;
	// Ui Button
	[SerializeField] private GameObject turnOffSoundButton;
	[SerializeField] private GameObject turnOnSoundButton;

	[SerializeField] private bool TurnOnBGM;

	void Awake()
	{
		// Add audio source each sound object
		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
			s.source.playOnAwake = false;
		}

		var check = PlayerPrefs.GetInt(Constant.KEY_SOUND, 0);
		soundIsOn = (check == 0) ? true : false;

        if (TurnOnBGM)
        {
			Play("BGM");
        }
	}

    public void CheckUI()
    {
		// Check ui is empty or not
        if (turnOffSoundButton == null)
        {
			turnOffSoundButton = GameObject.FindWithTag("TurnOffSound"); 
        }
        if (turnOnSoundButton == null)
        {
			turnOnSoundButton = GameObject.FindWithTag("TurnOnSound");
		}

		// Set Ui button
		if (soundIsOn)
		{
			turnOffSoundButton.SetActive(true);
			turnOnSoundButton.SetActive(false);
		}
		else
		{
			turnOffSoundButton.SetActive(false);
			turnOnSoundButton.SetActive(true);
		}
	}

	public void TurnOffSound()
    {
		// Set bool
		soundIsOn = false;
		// Turn off every active sound
		foreach (Sound s in sounds)
		{
			if (s.source.isPlaying == true)
			{
				s.source.Pause();
			}
		}
		// Set Ui
		turnOffSoundButton.SetActive(false);
		turnOnSoundButton.SetActive(true);
	}

	public void TurnOnSound()
    {
		// Set bool
		soundIsOn = true;
		// Start BGM again
		Play("BGM");
		// Set ui button
		turnOffSoundButton.SetActive(true);
		turnOnSoundButton.SetActive(false);
	}

	// This method is to play sound
	[PunRPC]
	public void Play(string sound)
	{
		// Only if sound is turned on
		if (soundIsOn)
		{
			// Find the sound based on given string
			Sound s = Array.Find(sounds, item => item.name == sound);
			if (s == null)
			{
				// Print warning if sound not found
				Debug.LogWarning("Sound: " + name + " not found!");
				return;
			}
			// Adjust pitch and volume
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			
			// Check the sound and Play
			if(sound == "BGM" && !s.source.isPlaying)
            {
				s.source.Play();
            }
            else if (sound != "BGM")
            {
				s.source.Play();
			}
			
		}
	}

	// Method to stop sound
	[PunRPC]
	public void Stop(string sound)
	{
		if (soundIsOn)
		{
			Sound s = Array.Find(sounds, item => item.name == sound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + name + " not found!");
				return;
			}

			s.source.volume = s.volume;
			s.source.pitch = s.pitch;

			s.source.Stop();
		}
	}
}
