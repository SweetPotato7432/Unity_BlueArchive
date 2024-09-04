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
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAudioSettings(); // PlayerPrefs에서 설정을 불러오기
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
        float bgmVolume = PlayerPrefs.GetFloat("BGM", 0.75f); // 기본값 0.75
        float sfxVolume = PlayerPrefs.GetFloat("SFX", 0.75f); // 기본값 0.75

        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }

    // BGM 볼륨 조절 (AudioMixer의 BGM 그룹 볼륨 조절)
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", volume); 
        SaveAudioSettings(volume, PlayerPrefs.GetFloat("SFX", 0.75f));
    }

    // SFX 볼륨 조절 (AudioMixer의 SFX 그룹 볼륨 조절)
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", volume);
        SaveAudioSettings(PlayerPrefs.GetFloat("BGM", 0.75f), volume);
    }
}
