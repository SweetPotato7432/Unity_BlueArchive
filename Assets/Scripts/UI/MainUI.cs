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


    public void SetUIData(string stageName, string enemyCount, string timeLimit)
    {
        this.stageName.text = stageName;
        this.enemyCount.text = stageName;
        this.timeLimit.text = stageName;
    }
}
