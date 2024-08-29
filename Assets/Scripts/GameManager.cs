using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour
{
    

    private GameObject[] allies;
    [SerializeField]
    private GameObject[] enemies;

    private Vector3 alliesAveragePos;
    //private Vector3 enemiesAveragePos;

    [SerializeField]
    private GameObject[] spawnPoint;
    private int spawnPointCount = 0;

    [SerializeField]
    private int enemyCount;

    //�׽�Ʈ ��
    [SerializeField]
    private GameObject[] enemySpawn;

    //MainUI
    //���Ŀ� ���������� �ν��Ͻ� �� ���
    [SerializeField]
    private MainUI mainUI;

    private float limitTime;

    // �̱���
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance==null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    Debug.Log("No singleton obj");
                }
            }
            return _instance;
        }

    }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        // �ν��Ͻ��� �����ϴ� ��� ���� ����� �ν��Ͻ� ����
        else
        {
            _instance = this;
            // �� ��ȯ�� ���� ����
            DontDestroyOnLoad(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        limitTime = 180f;
        foreach(GameObject enemySpawn in enemySpawn)
        {
            enemyCount += enemySpawn.transform.childCount;
        }
        mainUI.SetUIData("stage 1", enemyCount.ToString(), limitTime);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyCount == 0 || limitTime <= 0)
        {
            // ���� ����
            GameFinish();
        }

        allies = GameObject.FindGameObjectsWithTag("Ally");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        allies = FilterSpecialCharacters(allies);

        alliesAveragePos =  AveragePos(ref allies);

        if (enemies.Length == 0 && spawnPoint.Length > spawnPointCount && alliesAveragePos.x > spawnPoint[spawnPointCount].transform.position.x)
        {
            Debug.Log("����");
            // �׽�Ʈ
            enemySpawn[spawnPointCount].SetActive(true);
            spawnPointCount++;
            Debug.Log(spawnPointCount);

        }

        if (enemies.Length <= 0)
        {
            foreach (GameObject ally in allies)
            {
                StrikerCharacter allyStriker = ally.GetComponent<StrikerCharacter>();

                allyStriker.ChangeState(States.Idle);
            }

            if (InvokeCheck(allies))
            {
                foreach (GameObject ally in allies)
                {
                    StrikerCharacter allyStriker = ally.GetComponent<StrikerCharacter>();

                    allyStriker.ChangeState(States.Move);
                }
            }
        }

        // ���ѽð� ����
        limitTime -= Time.deltaTime;
        if (limitTime <= 0)
        {
            limitTime = 0;
        }
        mainUI.SetTimeLimit(Mathf.Round(limitTime));
    }

    // ĳ���͵��� �� �߾� ��ġ ���� �޼���
    private Vector3 AveragePos(ref GameObject[] characters)
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
    // �迭���� ����� ĳ���� ����
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
    // ĳ���͵� Idle���� Move ���·� ��ȯ
    public bool InvokeCheck(GameObject[] Characters)
    {
        foreach (GameObject character in Characters)
        {
            StrikerCharacter Striker = character.GetComponent<StrikerCharacter>();

            if (Striker.IsInvoking())
            {
                return false;
            }
        }
        return true;
    }
    // ĳ���� ���
    public void CharacterDead(string tag)
    {
        if (tag == "Enemy")
        {
            enemyCount--;
            mainUI.SetEnemyCount((enemyCount.ToString()));

        }
        else if (tag == "Ally")
        {

        }
    }

    // ���� �Ͻ� ����
    public void PauseGame()
    {
        Debug.Log("pause");
        Time.timeScale = 0f;
        mainUI.ActivePauseMenu(true);
    }

    // ���� �Ϸ� ����
    private void GameFinish()
    {
        Debug.Log("���� �Ϸ�");
        // ���� ����

    }

    // ���� �Ͻ� ���� ����
    public void ResumeGame()
    {
        Debug.Log("Resume");
        Time.timeScale = 1f;
        mainUI.ActivePauseMenu(false);
    }

    // ���� �����
    public void RestartGame()
    {
        Debug.Log("�����");
        ResetBattleData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ResetBattleData()
    {
    }

    // ���� ����
    public void GameQuit()
    {
        Debug.Log("���� ����");
        Application.Quit();
    }


}
