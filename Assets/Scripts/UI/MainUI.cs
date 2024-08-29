using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text stageName;
    [SerializeField]
    TMP_Text enemyCount;
    [SerializeField]
    TMP_Text timeLimit;

    public GameObject pauseMenuUI;


    public void SetUIData(string stageName, string enemyCount, float timeLimit)
    {
        this.stageName.text = stageName;
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
        int sec=0;

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

}
