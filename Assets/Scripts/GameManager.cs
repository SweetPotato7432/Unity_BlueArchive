using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour
{
    private int alleyLayer;
    private GameObject[] allies;
    private GameObject[] enemies;

    private Vector3 alliesAveragePos;

    [SerializeField]
    private GameObject spawnPoint;

    //�׽�Ʈ ��
    [SerializeField]
    private GameObject enemy;
    

    // Start is called before the first frame update
    void Start()
    {
        alleyLayer = LayerMask.NameToLayer("Alley");
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
        List<GameObject> alliesList = new List<GameObject>();
        List<GameObject> enemyList = new List<GameObject>();
        foreach(GameObject character in characters)
        {
            if(character.layer == alleyLayer)
            {
                alliesList.Add(character);
            }
            else
            {
                enemyList.Add(character);
            }
        }
        allies = alliesList.ToArray();
        enemies = enemyList.ToArray();

        alliesAveragePos =  AveragePos(ref allies);
        
        if(alliesAveragePos.x > spawnPoint.transform.position.x)
        {
            enemy.SetActive(true);
        }

    }

    // ĳ���͵��� �� �߾� ��ġ ���� �޼���
    public Vector3 AveragePos(ref GameObject[] characters)
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        // ī�޶� ��ġ ����
        for (int i = 0; i < characters.Length; i++)
        {
            if (!characters[i].gameObject.activeSelf)
                continue;

            averagePos += characters[i].transform.position;
            numTargets++;

        }

        if (numTargets > 0)
        {
            averagePos /= numTargets;
        }

        return averagePos;
    }
}
