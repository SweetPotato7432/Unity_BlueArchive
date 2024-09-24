using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    float maxHp;
    float curHp;

    [SerializeField]
    Image barImage;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 정면을 바라보는 UI
        Quaternion cameraRotation = Camera.main.transform.rotation;

        transform.rotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

        //this.gameObject.transform.eulerAngles = new Vector3(18, 27, 0);
    }

    public void SetHp(Stat stat)
    {
        maxHp = stat.MaxHp;
        curHp = stat.CurHp;
        barImage.fillAmount = curHp / maxHp;
    }

    public void setEnemyColor(Stat stat)
    {
        switch (stat.DefendTypeCode)
        {
            case DefendTypeCode.Normal:
                barImage.color = new Color(0.17f, 0.27f, 0.38f, 1f);
                break;
            case DefendTypeCode.Light:
                barImage.color = new Color(0.94f, 0.36f, 0.38f, 1f);
                break;
            case DefendTypeCode.Heavy:
                barImage.color = new Color(0.92f, 0.80f, 0.20f, 1f);
                break;
            case DefendTypeCode.Special:
                barImage.color = new Color(0f,0.67f,096f,1f);
                break;
            case DefendTypeCode.Elastic:
                barImage.color = new Color(1f, 0.45f, 0.80f, 1f);
                break;
        }
    }

    public void setAllyColor()
    {
        barImage.color = new Color(0.749f,0.921f,0.149f,1f); 
    }

    public void UpdateHp()
    {

    }
}
