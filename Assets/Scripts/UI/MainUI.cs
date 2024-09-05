using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MainUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text stageName;
    [SerializeField]
    TMP_Text pauseStageName;
    [SerializeField]
    TMP_Text enemyCount;
    [SerializeField]
    TMP_Text timeLimit;

    [SerializeField]
    private GameObject pauseMenuUI;
    [SerializeField]
    private GameObject speedButton;

    [SerializeField]
    private Slider bgmSlider;
    [SerializeField]
    private Slider sfxSlider;

    [SerializeField]
    private Toggle muteBGMToggle;
    [SerializeField]
    private Toggle muteSFXToggle;

    private void Start()
    {
        // 슬라이더 값을 불러온 값으로 초기화
        bgmSlider.value = PlayerPrefs.GetFloat("BGM", 0.75f); // 기본값 0.75
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 0.75f); // 기본값 0.75

        // 음소거 상태 불러오기
        bool isBGMMuted = PlayerPrefs.GetInt("BGMMute", 0) == 1;
        muteBGMToggle.isOn = isBGMMuted;
        bool isSFXMuted = PlayerPrefs.GetInt("SFXMute", 0) == 1;
        muteSFXToggle.isOn = isSFXMuted;
    }


    public void SetUIData(string stageName, string enemyCount, float timeLimit)
    {
        this.stageName.text = stageName;
        pauseStageName.text = stageName;
        this.enemyCount.text = enemyCount;
        this.timeLimit.text = Mathf.Round(timeLimit).ToString();
    }

    public void SetEnemyCount(string enemyCount)
    {
        this.enemyCount.text = enemyCount;
    }
    public void SetTimeLimit(float timeLimit)
    {
        int min = 0;
        int sec;

        while (timeLimit >= 60f)
        {
            timeLimit -= 60;
            min += 1;
        }
        sec = (int)timeLimit;
        

        this.timeLimit.text = string.Format("{0:D2}:{1:D2}",min, sec);
    }

    public void ActivePauseMenu(bool active)
    {
        if(pauseMenuUI.activeSelf != active)
        {
            pauseMenuUI.SetActive(active);
        }
    }

    public void ChangeSpeedButton(GameSpeed gameSpeed)
    {
        switch (gameSpeed)
        {
            case GameSpeed.None:
                break;
            case GameSpeed.Normal:
                speedButton.GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f);
                Debug.Log(speedButton.transform.childCount);
                for(int i = 0; i < speedButton.transform.childCount; i++)
                {
                    speedButton.transform.GetChild(i).gameObject.SetActive(false);
                    speedButton.transform.GetChild(i).GetComponent<Image>().color = new Color(0.1f, 0.2f, 0.3f);
                }
                speedButton.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case GameSpeed.Fast:
                speedButton.GetComponent<Image>().color = new Color(0.4f, 1f, 1f);
                Debug.Log(speedButton.transform.childCount);
                for (int i = 0; i < speedButton.transform.childCount; i++)
                {
                    speedButton.transform.GetChild(i).gameObject.SetActive(false);
                    speedButton.transform.GetChild(i).GetComponent<Image>().color = new Color(0.1f, 0.4f, 0.6f);
                }
                for (int i = 3; i < 5; i++)
                {
                    speedButton.transform.GetChild(i).gameObject.SetActive(true);

                }
                break;
            case GameSpeed.Fastest:
                speedButton.GetComponent<Image>().color = new Color(1f, 0.9f, 0f);
                Debug.Log(speedButton.transform.childCount);
                for (int i = 0; i < speedButton.transform.childCount; i++)
                {
                    speedButton.transform.GetChild(i).gameObject.SetActive(false);
                    speedButton.transform.GetChild(i).GetComponent<Image>().color = new Color(0.6f, 0.3f, 0f);
                }
                for(int i = 0; i<3; i++)
                {
                    speedButton.transform.GetChild(i).gameObject.SetActive(true);

                }
                break;
        }
        GameSettingData.Instance.SetGameSpeed((int)gameSpeed);
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
