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
    private Vector3 enemiesAveragePos;

    [SerializeField]
    private GameObject spawnPoint;

    //�׽�Ʈ ��
    [SerializeField]
    private GameObject enemy;
    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        allies = GameObject.FindGameObjectsWithTag("Ally");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        alliesAveragePos =  AveragePos(ref allies);
        enemiesAveragePos = AveragePos(ref enemies);

        // �Ʊ� ĳ���� ���� 
        if(enemies.Length != 0)
        {
            for(int i = 0; i<allies.Length; i++)
            {
                allies[i].GetComponent<Character>().SetDestination(enemiesAveragePos);
            }
        }
        else
        {
            for (int i = 0; i < allies.Length; i++)
            {
                allies[i].GetComponent<Character>().SetDestination(new Vector3(allies[i].transform.position.x+50, allies[i].transform.position.y, allies[i].transform.position.z));
            }
        }

        // �� ĳ���� ���� ����
        if(allies.Length != 0)
        {
            for(int i=0; i<enemies.Length; i++)
            {
                enemies[i].GetComponent<Character>().SetDestination(alliesAveragePos);
            }
        }
        else
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].GetComponent<Character>().SetDestination(new Vector3(enemies[i].transform.position.x - 50, enemies[i].transform.position.y, enemies[i].transform.position.z));
            }
        }

        if (alliesAveragePos.x > spawnPoint.transform.position.x)
        {
            // �׽�Ʈ
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
