using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text resultTime;
    [SerializeField]
    TMP_Text resultText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(float resultTime, bool isWin)
    {
        // 시간 설정
        int min = 0;
        int sec;

        while (resultTime >= 60f)
        {
            resultTime -= 60;
            min += 1;
        }
        sec = (int)resultTime;


        this.resultTime.text = string.Format("{0:D2}:{1:D2}", min, sec);

        // 승패 확인
        if (isWin)
        {
            resultText.gameObject.SetActive(true);
        }
        else
        {
            resultText.gameObject.SetActive(false);
        }
    }
}
