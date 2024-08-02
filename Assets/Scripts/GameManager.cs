using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    //테스트 용
    [SerializeField]
    private GameObject[] enemySpawn;

    // 싱글톤
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

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
        if(_instance == null)
        {
            _instance = this;
        }
        // 인스턴스가 존재하는 경우 새로 생기는 인스턴스 삭제
        else if(_instance != this)
        {
            Destroy(gameObject);
        }
        // 씬 전환시 삭제 방지
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject enemySpawn in enemySpawn)
        {
            enemyCount += enemySpawn.transform.childCount;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyCount == 0)
        {
            // 게임 종료
            GameEnd();
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
        }
        else if (tag == "Ally")
        {

        }
    }
    

    // 게임 종료
    private void GameEnd()
    {
        Debug.Log("게임 종료");
    }

}
