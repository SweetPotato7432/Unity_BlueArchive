using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���󹰰� ���õ� ��ũ��Ʈ

public class CoverObject : MonoBehaviour
{
    private GameObject coveredCharacter;

    private float maxHp;
    private float curHp;

    // ������ �ִ��� üũ
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