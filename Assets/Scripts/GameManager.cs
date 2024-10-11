using System.Collections;
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
    [SerializeField]
    private ResultUI resultUI;

    private float limitTime;

    private bool gameFinish = false;

    private GameSpeed gameSpeed;

    //데미지 저장 Dictionary
    private Dictionary<string, float> strikerDamageData = new Dictionary<string, float>();
    private Dictionary<string, float> specialDamageData = new Dictionary<string, float>();

    // getter dictionary
    public Dictionary<string, float> GetStrikerDamageData()
    {
        return new Dictionary<string, float>(strikerDamageData);
    }

    public Dictionary<string, float> GetSpecialDamageData()
    {
        return new Dictionary<string, float>(specialDamageData);
    }


    private void Awake()
    {
        mainUI = GameObject.Find("MainUI").GetComponent<MainUI>();
        resultUI = GameObject.Find("ResultUI").GetComponent<ResultUI>();
        resultUI.gameObject.SetActive(false);

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
        // 스폰된 적을 모두 처치시
        if (enemies.Length <= 0 && !gameFinish)
        {
            bool result = true;

            foreach (GameObject ally in allies)
            {
                if (InvokeCheck(ally))
                {
                    StrikerCharacter allyStriker = ally.GetComponent<StrikerCharacter>();
                    allyStriker.ChangeState(States.Idle);
                }

            }

            foreach (GameObject ally in allies)
            {
                
                StrikerCharacter allyStriker = ally.GetComponent<StrikerCharacter>();
                if (!allyStriker.IsIdle())
                {
                    result = false;
                }

            }

            if (result)
            {
                foreach (GameObject ally in allies)
                {
                    StrikerCharacter allyStriker = ally.GetComponent<StrikerCharacter>();
                    allyStriker.ChangeState(States.Move);
                

                }
            }
        }

        if (!gameFinish)
        {
            // 제한시간 감소
            limitTime -= Time.deltaTime;
            if (limitTime <= 0)
            {
                limitTime = 0;
            }
            mainUI.SetTimeLimit(Mathf.Round(limitTime));
        }
        
    }

    private void LateUpdate()
    {
        if (enemyCount == 0 || limitTime <= 0 || allies.Length == 0)
        {
            // 게임 종료
            GameFinish();
        }
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
    public bool InvokeCheck(GameObject character)
    {
        StrikerCharacter Striker = character.GetComponent<StrikerCharacter>();

        if (Striker.IsInvoking())
        {
                return false;
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
        if (!gameFinish)
        {
            mainUI.gameObject.SetActive(false);
            gameFinish = true;
            StartCoroutine(RestoreGameSpeed());
        }

        Debug.Log("게임 완료");

        if (enemies.Length <= 0)
        {
            foreach (GameObject ally in allies)
            {

                if (InvokeCheck(ally))
                {
                    StrikerCharacter allyStriker = ally.GetComponent<StrikerCharacter>();
                    allyStriker.ChangeState(States.Idle);
                }

            }
        }
        
        if(allies.Length <= 0)
        {
            foreach (GameObject enemy in enemies)
            {
                if (InvokeCheck(enemy))
                {
                    StrikerCharacter enemyStriker = enemy.GetComponent<StrikerCharacter>();
                    enemyStriker.ChangeState(States.Idle);
                }

            }
        }
       
        // 승패 구분
        if (enemyCount != 0 || limitTime <= 0 ) 
        {
            Debug.Log("패배");
            resultUI.SetData(limitTime,false);
        }
        else
        {
            Debug.Log("승리");
            resultUI.SetData(limitTime,true);
        }
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

    private IEnumerator RestoreGameSpeed()
    {
        Time.timeScale = 0f;
        float startTime = Time.realtimeSinceStartup;
        float startScale = Time.timeScale;

        // 천천히 게임 속도를 원래대로 복구
        while (Time.timeScale < 1f)
        {
            Time.timeScale = Mathf.Lerp(startScale, 1f, (Time.realtimeSinceStartup - startTime) / 3f);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;  // 물리 연산도 타임스케일에 맞게 조정
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        resultUI.gameObject.SetActive(true);
    }

    public void RecordDamage(Stat stat, float damage)
    {
        if(stat.BattleTypeCode == BattleTypeCode.Striker)
        {
            if (strikerDamageData.ContainsKey(stat.Name))
            {
                strikerDamageData[stat.Name] += damage;
            }
            else
            {
                strikerDamageData.Add(stat.Name, damage);
            }
        }
        else if(stat.BattleTypeCode == BattleTypeCode.Special)
        {
            if (specialDamageData.ContainsKey(stat.Name))
            {
                specialDamageData[stat.Name] += damage;
            }
            else
            {
                specialDamageData.Add(stat.Name, damage);
            }
        }
       
    }


}
