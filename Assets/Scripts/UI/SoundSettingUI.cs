using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingUI : MonoBehaviour
{
    [SerializeField]
    private Slider bgmSlider;
    [SerializeField]
    private Slider sfxSlider;

    [SerializeField]
    private Toggle muteBGMToggle;
    [SerializeField]
    private Toggle muteSFXToggle;

    // Start is called before the first frame update
    void Start()
    {
        // �����̴� ���� �ҷ��� ������ �ʱ�ȭ
        bgmSlider.value = PlayerPrefs.GetFloat("BGM", 0.75f); // �⺻�� 0.75
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 0.75f); // �⺻�� 0.75

        // ���Ұ� ���� �ҷ�����
        bool isBGMMuted = PlayerPrefs.GetInt("BGMMute", 0) == 1;
        muteBGMToggle.isOn = isBGMMuted;
        bool isSFXMuted = PlayerPrefs.GetInt("SFXMute", 0) == 1;
        muteSFXToggle.isOn = isSFXMuted;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBGMVolume()
    {
        float sound = bgmSlider.value;

        GameSettingData.Instance.SetBGMVolume(sound);
    }
    public void SetSFXVolume()
    {
        float sound = sfxSlider.value;

        GameSettingData.Instance.SetSFXVolume(sound);
    }

    public void SetBGMMute(bool isMuted)
    {
        GameSettingData.Instance.SetBGMMute(isMuted);
        Debug.Log(isMuted);
    }
    public void SetSFXMute(bool isMuted)
    {
        GameSettingData.Instance.SetSFXMute(isMuted);
    }
}
