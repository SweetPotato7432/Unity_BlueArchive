using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 엄폐물과 관련된 스크립트

public class CoverObject : MonoBehaviour
{
    private GameObject coveredCharacter;

    private float maxHp;
    private float curHp;

    // 엄폐자 있는지 체크
    public bool isOccupied;

    private GameObject instance;

    // Start is called before the first frame update
    void Start()
    {
        

        isOccupied = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TryCover()
    {

    }
}