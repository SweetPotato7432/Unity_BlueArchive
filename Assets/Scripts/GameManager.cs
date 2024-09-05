using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    

    private GameObject[] allies;
    [SerializeField]
    private GameObject[] enemies;

    private Vector3 alliesAveragePos;
    //private Vector3 enemiesAveragePos;

    [SerializeField]
    private Transform[] spawnPoint;
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

    private GameSpeed gameSpeed;


    private void Awake()
    {
        mainUI = GameObject.Find("MainUI").GetComponent<MainUI>();

        // ���� ���� �ð� ����
        limitTime = 180f;

        List<GameObject> result = new List<GameObject>();

        // �� ���� ���� �ҷ�����
        foreach (Transform child in GameObject.Find("EnemySpawn").transform)
        {
            if (child.gameObject.CompareTag("Spawn"))
            {
                result.Add(child.gameObject);
            }
        }
        enemySpawn = result.ToArray();

        // �� ���� ���� �ҷ�����
        spawnPoint = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        spawnPoint = spawnPoint.Skip(1).ToArray();

        // �� �� ���� ������ �ִ� �� ���� ī���� ����
        foreach (GameObject enemySpawn in enemySpawn)
        {
            enemyCount += enemySpawn.transform.childCount;
        }
        mainUI.SetUIData("stage 1", enemyCount.ToString(), limitTime);

        // ���� �ӵ� ����
        //gameSpeed = GameSpeed.Normal;
        gameSpeed = (GameSpeed)PlayerPrefs.GetInt("GameSpeed",1);
        ChangeSpeed();
        mainUI.ChangeSpeedButton(gameSpeed);

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCount == 0 || limitTime <= 0)
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
        ChangeSpeed();
        mainUI.ActivePauseMenu(false);
    }

    // ���� �����
    public void RestartGame()
    {
        Debug.Log("�����");
        //ResetBattleData();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChageGameSpeedOnClick()
    {
        if(gameSpeed==GameSpeed.Fastest)
        {
            gameSpeed = GameSpeed.Normal;
        }
        else
        {
            gameSpeed++;
        }
        // 1���, 1.5���, 2������� ����
        ChangeSpeed();

        // ��� ��ư �ܰ迡 ���� ��ư �� ����
        mainUI.ChangeSpeedButton(gameSpeed);
    }

    private void ChangeSpeed()
    {
        switch (gameSpeed)
        {
            case GameSpeed.Normal:
                Time.timeScale = 1f;
                break;
            case GameSpeed.Fast:
                Time.timeScale = 1.5f;
                break;
            case GameSpeed.Fastest:
                Time.timeScale = 2f;

                break;
        }
    }

    // ���� ����
    public void GameQuit()
    {
        Debug.Log("���� ����");
        Application.Quit();
    }


}
