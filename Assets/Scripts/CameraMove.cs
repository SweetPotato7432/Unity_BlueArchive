using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //private float offsetX = -11.0f;
    //private float offsetY = 9.0f;
    //private float offsetZ = -21.0f;

    // 카메라 이동 속도
    [SerializeField]
    private float cameraSpeed;
    // 카메라 줌 시간
    [SerializeField]
    private float zoomDampTime;
    // 현재 줌 속도 저장
    private float zoomSpeed;

    // 카메라 목표 위치
    private Vector3 targetPos;
    // 카메라 목표 사이즈
    private float requiredSize;

    // 메인 카메라 및 시네머신 가상 카메라
    [SerializeField]
    private Camera mainCamera;
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        // 카메라 컴포넌트 초기화
        mainCamera = GetComponentInChildren<Camera>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //카메라 초기 위치 및 크기 설정
        InitializeCameraPosionAndSize();
    }

    void LateUpdate()
    {
        // 캐릭터들의 중점과 적절한 사이즈 확인
        FindAveragePosAndSize();
        // 카메라 이동
        MoveCamera();
        // 시네머신 가상 카메라 업데이트
        UpdateVirtualCamera();
    }

    private void InitializeCameraPosionAndSize()
    {
        // 카메라 초기 위치 및 크기 설정
        FindAveragePosAndSize();
        transform.position = targetPos;
        mainCamera.orthographicSize = requiredSize;

        // 시네머신 카메라 초기 설정
        if(virtualCamera != null)
        {
            virtualCamera.transform.position = transform.position;
            virtualCamera.m_Lens.OrthographicSize = requiredSize;
        }    
    }

    private void MoveCamera()
    {
        // 카메라 이동
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * cameraSpeed);

        // 카메라 줌
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, requiredSize, ref zoomSpeed, zoomDampTime);
    }
    
    // 캐릭터들의 평균 위치와 카메라 크기 계산
    private void FindAveragePosAndSize()
    {
        Vector3 averagePos = new Vector3();

        float size = 0f;

        GameObject[] characters = GetAllCharacters();

        if (characters.Length == 0)
        {
            return;
        }

        Bounds bounds = new Bounds(characters[0].transform.position, Vector3.zero);

        foreach (GameObject character in characters)
        {
            if (!character.gameObject.activeSelf) continue;

            bounds.Encapsulate(character.transform.position);
        }

        // 카메라 위치 설정
        averagePos = bounds.center;

        averagePos.x -= 10;
        averagePos.y = transform.position.y;
        averagePos.z = transform.position.z;

        targetPos = averagePos;

        // 카메라 크기 설정
        Vector3 desiredLocalPos = transform.InverseTransformPoint(averagePos);

        foreach(GameObject character in characters)
        {
            if (!character.gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(character.transform.position);

            Vector3 desiredPosToTarget = targetLocalPos-desiredLocalPos;

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size,Mathf.Abs((desiredPosToTarget.x)/mainCamera.aspect));
        }
        size -= 3f;

        size = Mathf.Max(size, 6.5f);

        requiredSize = size;
    }

    // 모든 캐릭터 리스트 화
    private GameObject[] GetAllCharacters()
    {
        GameObject[] charactersAlly = GameObject.FindGameObjectsWithTag("Ally");
        GameObject[] charactersEnemy = GameObject.FindGameObjectsWithTag("Enemy");

        List<GameObject> combinedList = new List<GameObject>();
        combinedList.AddRange(charactersAlly);
        combinedList.AddRange(charactersEnemy);

        return combinedList.ToArray();
    }

    // 시네머신 가상 카메라 업데이트
    private void UpdateVirtualCamera()
    {
        if(virtualCamera != null) 
        {
            virtualCamera.transform.position = transform.position;
            virtualCamera.m_Lens.Orthographic = mainCamera.orthographic;
            virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(virtualCamera.m_Lens.OrthographicSize, requiredSize, ref zoomSpeed, zoomDampTime);
        }
    }



}
