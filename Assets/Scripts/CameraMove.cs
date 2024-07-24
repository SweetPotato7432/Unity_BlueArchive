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

    // ī�޶� �̵� �ӵ�
    [SerializeField]
    private float cameraSpeed;
    // ī�޶� �� �ð�
    [SerializeField]
    private float zoomDampTime;
    // ���� �� �ӵ� ����
    private float zoomSpeed;

    // ī�޶� ��ǥ ��ġ
    private Vector3 targetPos;
    // ī�޶� ��ǥ ������
    private float requiredSize;

    // ���� ī�޶� �� �ó׸ӽ� ���� ī�޶�
    [SerializeField]
    private Camera mainCamera;
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        // ī�޶� ������Ʈ �ʱ�ȭ
        mainCamera = GetComponentInChildren<Camera>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //ī�޶� �ʱ� ��ġ �� ũ�� ����
        InitializeCameraPosionAndSize();
    }

    void LateUpdate()
    {
        // ĳ���͵��� ������ ������ ������ Ȯ��
        FindAveragePosAndSize();
        // ī�޶� �̵�
        MoveCamera();
        // �ó׸ӽ� ���� ī�޶� ������Ʈ
        UpdateVirtualCamera();
    }

    private void InitializeCameraPosionAndSize()
    {
        // ī�޶� �ʱ� ��ġ �� ũ�� ����
        FindAveragePosAndSize();
        transform.position = targetPos;
        mainCamera.orthographicSize = requiredSize;

        // �ó׸ӽ� ī�޶� �ʱ� ����
        if(virtualCamera != null)
        {
            virtualCamera.transform.position = transform.position;
            virtualCamera.m_Lens.OrthographicSize = requiredSize;
        }    
    }

    private void MoveCamera()
    {
        // ī�޶� �̵�
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * cameraSpeed);

        // ī�޶� ��
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, requiredSize, ref zoomSpeed, zoomDampTime);
    }
    
    // ĳ���͵��� ��� ��ġ�� ī�޶� ũ�� ���
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

        // ī�޶� ��ġ ����
        averagePos = bounds.center;

        averagePos.x -= 10;
        averagePos.y = transform.position.y;
        averagePos.z = transform.position.z;

        targetPos = averagePos;

        // ī�޶� ũ�� ����
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

    // ��� ĳ���� ����Ʈ ȭ
    private GameObject[] GetAllCharacters()
    {
        GameObject[] charactersAlly = GameObject.FindGameObjectsWithTag("Ally");
        GameObject[] charactersEnemy = GameObject.FindGameObjectsWithTag("Enemy");

        List<GameObject> combinedList = new List<GameObject>();
        combinedList.AddRange(charactersAlly);
        combinedList.AddRange(charactersEnemy);

        return combinedList.ToArray();
    }

    // �ó׸ӽ� ���� ī�޶� ������Ʈ
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
