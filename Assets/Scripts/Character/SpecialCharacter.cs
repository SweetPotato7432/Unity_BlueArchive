using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCharacter : MonoBehaviour
{
    // ½ºÅÈ »ý¼º
    private Stat stat;

    [SerializeField]
    private UnitCode unitCode;

    private void Awake()
    {
        stat = Stat.setUnitStat(unitCode);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] Characters = GameObject.FindGameObjectsWithTag(this.tag);
        foreach (GameObject character in Characters) 
        {
            StrikerCharacter strikerCharcater = character.GetComponent<StrikerCharacter>();
            if(strikerCharcater != null)
            {
                strikerCharcater.SpecialCharacterStatCalculate(stat);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
