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

    //테스트 용
    [SerializeField]
    private GameObject[] enemySpawn;
    //MainUI
    //추후에 프리팹으로 인스턴스 해 사용
    [SerializeField]
    private MainUI mainUI;

    private float limitTime;

    private GameSpeed gameSpeed;


    private void Awake()
    {
        mainUI = GameObject.Find("MainUI").GetComponent<MainUI>();

        // 게임 제한 시간 설정
        limitTime = 180f;

        List<GameObject> result = new List<GameObject>();

        // 적 스폰 지역 불러오기
        foreach (Transform child in GameObject.Find("EnemySpawn").transform)
        {
            if (child.gameObject.CompareTag("Spawn"))
            {
                result.Add(child.gameObject);
            }
        }
        enemySpawn = result.ToArray();

        // 적 스폰 시점 불러오기
        spawnPoint = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        spawnPoint = spawnPoint.Skip(1).ToArray();

        // 각 적 스폰 지역에 있는 적 숫자 카운터 세기
        foreach (GameObject enemySpawn in enemySpawn)
        {
            enemyCount += enemySpawn.transform.childCount;
        }
        mainUI.SetUIData("stage 1", enemyCount.ToString(), limitTime);

        // 게임 속도 설정
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
            // 게임 종료
            GameFinish();
        }

        allies = GameObject.FindGameObjectsWithTag("Ally");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        allies = FilterSpecialCharacters(allies);

        alliesAveragePos =  AveragePos(ref allies);

        if (enemies.Length == 0 && spawnPoint.Length > spawnPointCount && alliesAveragePos.x > spawnPoint[spawnPointCount].transform.position.x)
        {
            Debug.Log("생성");
            // 테스트
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

        // 제한시간 감소
        limitTime -= Time.deltaTime;
        if (limitTime <= 0)
        {
            limitTime = 0;
        }
        mainUI.SetTimeLimit(Mathf.Round(limitTime));
    }

    // 캐릭터들의 정 중앙 위치 설정 메서드
    private Vector3 AveragePos(ref GameObject[] characters)
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        // 카메라 위치 설정
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
    // 배열에서 스페셜 캐릭터 제외
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
    // 캐릭터들 Idle에서 Move 상태로 전환
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
    // 캐릭터 사망
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

    // 게임 일시 정지
    public void PauseGame()
    {
        Debug.Log("pause");
        Time.timeScale = 0f;
        mainUI.ActivePauseMenu(true);
    }

    // 게임 완료 시점
    private void GameFinish()
    {
        Debug.Log("게임 완료");
        // 승패 구분

    }

    // 게임 일시 정지 해제
    public void ResumeGame()
    {
        Debug.Log("Resume");
        ChangeSpeed();
        mainUI.ActivePauseMenu(false);
    }

    // 게임 재시작
    public void RestartGame()
    {
        Debug.Log("재시작");
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
        // 1배속, 1.5배속, 2배속으로 변경
        ChangeSpeed();

        // 배속 버튼 단계에 따른 버튼 색 변경
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

    // 게임 종료
    public void GameQuit()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }


}
