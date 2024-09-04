using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettingData : MonoBehaviour
{
    public static GameSettingData Instance { get; private set; }
    [SerializeField]
    private AudioMixer audioMixer;

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAudioSettings(); // PlayerPrefs���� ������ �ҷ�����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        Debug.Log("BGM" + PlayerPrefs.GetFloat("BGM"));
        Debug.Log("SFX" + PlayerPrefs.GetFloat("SFX"));
    }

    public void SaveAudioSettings(float bgmVolume, float sfxVolume)
    {
        PlayerPrefs.SetFloat("BGM", bgmVolume);
        PlayerPrefs.SetFloat("SFX", sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadAudioSettings()
    {
        float bgmVolume = PlayerPrefs.GetFloat("BGM", 0.75f); // �⺻�� 0.75
        float sfxVolume = PlayerPrefs.GetFloat("SFX", 0.75f); // �⺻�� 0.75

        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }

    // BGM ���� ���� (AudioMixer�� BGM �׷� ���� ����)
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", volume); 
        SaveAudioSettings(volume, PlayerPrefs.GetFloat("SFX", 0.75f));
    }

    // SFX ���� ���� (AudioMixer�� SFX �׷� ���� ����)
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", volume);
        SaveAudioSettings(PlayerPrefs.GetFloat("BGM", 0.75f), volume);
    }
}
