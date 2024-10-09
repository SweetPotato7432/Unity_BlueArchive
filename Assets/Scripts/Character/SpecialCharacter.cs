using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCharacter : MonoBehaviour
{
    GameManager gameManager;

    // ���� ����
    private Stat stat;

    [SerializeField]
    private UnitCode unitCode;

    private void Awake()
    {

        // ĳ���� ���� ����
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        stat = Stat.setUnitStat(unitCode);

        if (this.tag == "Ally")
        {
            gameManager.RecordDamage(stat, 0);
        }
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
