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
        barImage.fillAmount = curHp / maxHp;
        this.gameObject.transform.eulerAngles = new Vector3(18, 27, 0);
    }

    public void SetHp(Stat stat)
    {
        maxHp = stat.MaxHp;
        curHp = stat.CurHp;
    }

    public void UpdateHp()
    {

    }
}
