using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    private float offsetX = -11.0f;
    private float offsetY = 9.0f;
    private float offsetZ = -21.0f;

    [SerializeField]
    private float CameraSpeed = 10.0f;

    private float zoomSpeed;

    private Vector3 targetPos;
    private float requiredSize;


    [SerializeField]
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponentInChildren<Camera>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // ī�޶� �ʱ� ��ġ �� ũ�� ����
        FindAveragePosAndSize();
        transform.position = targetPos;
        mainCamera.orthographicSize = requiredSize;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ĳ���͵��� ������ ������ ������ Ȯ��
        FindAveragePosAndSize();
        // ī�޶� �̵�
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * CameraSpeed);

        // ī�޶� ��
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, requiredSize, ref zoomSpeed, 0.5f);
    }

    private void FindAveragePosAndSize()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        float size = 0f;

        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");

        // ī�޶� ��ġ ����
        for(int i = 0; i < characters.Length; i++)
        {
            if (!characters[i].gameObject.activeSelf)
                continue;

            averagePos += characters[i].transform.position;
            numTargets++;

        }

        if(numTargets>0)
        {
            averagePos/=numTargets;
        }
        averagePos.x -= 10;
        averagePos.y = transform.position.y;
        averagePos.z = transform.position.z;

        targetPos = averagePos;

        // ī�޶� ũ�� ����
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
