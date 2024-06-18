using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //private float offsetX = -11.0f;
    //private float offsetY = 9.0f;
    //private float offsetZ = -21.0f;

    [SerializeField]
    private float CameraSpeed = 10.0f;
    // 현재 줌 속도 저장
    private float zoomSpeed;

    private Vector3 targetPos;
    private float requiredSize;


    [SerializeField]
    private Camera mainCamera;

    private GameManager gameManager;

    private void Awake()
    {
        mainCamera = GetComponentInChildren<Camera>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = (GameObject.Find("GameManager")).GetComponent<GameManager>();
        // 카메라 초기 위치 및 크기 설정
        FindAveragePosAndSize();
        transform.position = targetPos;
        mainCamera.orthographicSize = requiredSize;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 캐릭터들의 중점과 적절한 사이즈 확인
        FindAveragePosAndSize();
        // 카메라 이동
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * CameraSpeed);

        // 카메라 줌
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, requiredSize, ref zoomSpeed, 0.5f);
    }

    private void FindAveragePosAndSize()
    {
        Vector3 averagePos = new Vector3();
        //int numTargets = 0;

        float size = 0f;

        GameObject[] charactersAlly = GameObject.FindGameObjectsWithTag("Ally");
        GameObject[] charactersEnemy = GameObject.FindGameObjectsWithTag("Enemy");

        List<GameObject> combinedList = new List<GameObject>();
        combinedList.AddRange(charactersAlly);
        combinedList.AddRange(charactersEnemy);

        GameObject[] characters = combinedList.ToArray();

        // 카메라 위치 설정
        averagePos = gameManager.AveragePos(ref characters);

        averagePos.x -= 10;
        averagePos.y = transform.position.y;
        averagePos.z = transform.position.z;

        targetPos = averagePos;

        // 카메라 크기 설정
        Vector3 desiredLocalPos = transform.InverseTransformPoint(averagePos);

        for(int i = 0;i < characters.Length;i++)
        {
            if (!characters[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(characters[i].transform.position);

            Vector3 desiredPosToTarget = targetLocalPos-desiredLocalPos;

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size,Mathf.Abs((desiredPosToTarget.x)/mainCamera.aspect));
        }
        size -= 4f;

        size = Mathf.Max(size, 6.5f);

        requiredSize = size;
    }

    

}
