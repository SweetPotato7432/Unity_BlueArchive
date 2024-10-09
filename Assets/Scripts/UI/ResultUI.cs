using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField]
    TMP_Text resultTime;
    [SerializeField]
    TMP_Text resultText;

    //������ ���� Dictionary
    private Dictionary<string, float> strikerDamageData = new Dictionary<string, float>();
    private Dictionary<string, float> specialDamageData = new Dictionary<string, float>();

    [SerializeField]
    GameObject chartGroup;
    [SerializeField]
    GameObject chartNameGroup;

    [SerializeField]
    private List<RectTransform> chartTransform = new List<RectTransform>();
    //private RectTransform[] chartTransform;
    [SerializeField]
    private List<TMP_Text> chartNameText = new List<TMP_Text>();

    public float minDamage = 0f; // �ּ� ������ ��
    public float maxDamage =0; // �ִ� ������ ��
    public float minBarLength = 40f; // �ּ� ���� (�ּ� �������� �� �׷��� ����)
    public float maxBarLength = 540f; // �ִ� ���� (�ִ� �������� �� �׷��� ����)

    //private TMP_Text[] chartNameText;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        foreach(Transform child in chartGroup.transform)
        {
            chartTransform.Add(child.GetComponent<RectTransform>());
        }

        foreach(Transform child in chartNameGroup.transform)
        {
            chartNameText.Add(child.GetComponent<TMP_Text>());
        }

    }

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
        // �ð� ����
        int min = 0;
        int sec;

        while (resultTime >= 60f)
        {
            resultTime -= 60;
            min += 1;
        }
        sec = (int)resultTime;


        this.resultTime.text = string.Format("{0:D2}:{1:D2}", min, sec);

        // ���� Ȯ��
        if (isWin)
        {
            resultText.gameObject.SetActive(true);
        }
        else
        {
            resultText.gameObject.SetActive(false);
        }

    }
    public void setDamageReport()
    {
        int count = 0;

        Dictionary<string, float> strikerData = gameManager.GetStrikerDamageData();
        Dictionary<string, float> specialData = gameManager.GetSpecialDamageData();

        foreach (KeyValuePair<string, float> entry in strikerData)
        {
            if (maxDamage < entry.Value)
            {
                maxDamage = entry.Value;
            }
        }
        foreach (KeyValuePair<string, float> entry in specialData)
        { 
            if (maxDamage < entry.Value)
            {
                maxDamage = entry.Value;
            }
        }
        Debug.Log(maxDamage);
        foreach (KeyValuePair<string, float> entry in strikerData)
        {
            Debug.Log(entry.Key + ":" + entry.Value);
            chartTransform[count].gameObject.SetActive(true);
            chartNameText[count].gameObject.SetActive(true);

            float newBarLength = Mathf.Lerp(minBarLength, maxBarLength, Mathf.InverseLerp(minDamage, maxDamage, entry.Value));
            chartTransform[count].sizeDelta = new Vector2(chartTransform[count].sizeDelta.x, newBarLength);

            chartTransform[count].GetComponentInChildren<TMP_Text>().text = ((int)entry.Value).ToString();

            chartNameText[count].text = entry.Key;

            count++;
        }
        count = 4;
        foreach (KeyValuePair<string, float> entry in specialData)
        {
            
            Debug.Log(entry.Key + ":" + entry.Value);
            chartTransform[count].gameObject.SetActive(true);
            chartNameText[count].gameObject.SetActive(true);

            float newBarLength = Mathf.Lerp(minBarLength, maxBarLength, Mathf.InverseLerp(minDamage, maxDamage, entry.Value));
            chartTransform[count].sizeDelta = new Vector2( chartTransform[count].sizeDelta.x, newBarLength);

            chartTransform[count].GetComponentInChildren<TMP_Text>().text = ((int)entry.Value).ToString();

            chartNameText[count].text = entry.Key;

            count++;
        }
    }

}
