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

        allies = FilterSpecialCharacters(allies);
        enemies = FilterSpecialCharacters(enemies);

        alliesAveragePos =  AveragePos(ref allies);
        enemiesAveragePos = AveragePos(ref enemies);

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

    private GameObject[] FilterSpecialCharacters(GameObject[] characters)
    {
        List<GameObject> filteredList = new List<GameObject>();

        foreach (GameObject character in characters)
        {
            if (character.GetComponent<SpecialCharacter>() == null)
            {
                filteredList.Add(character);
            }
        }
        return filteredList.ToArray();
    }
}
