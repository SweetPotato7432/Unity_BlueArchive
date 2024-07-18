using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���󹰿� ������ ��ũ��Ʈ
public class CoverObject : MonoBehaviour
{
    private GameObject coveredCharacter;

    private float maxHp;
    private float curHp;

    // ������ ��� ������ Ȯ��
    public bool isOccupied;

    // ĳ���� Ž�� ���̾�
    private LayerMask targetLayer;

    // ������ �Ÿ�
    private float targetDistance;

    // ���� ��ġ
    [SerializeField]
    private GameObject backSpot;
    [SerializeField]
    private GameObject frontSpot;

    public GameObject coverSpot;

    [SerializeField]
    private GameObject useCharacter;

    // Start is called before the first frame update
    void Start()
    {
        targetLayer = LayerMask.GetMask("Character");
        isOccupied = false;

        // ��ġ ����
        backSpot = transform.GetChild(0).gameObject;
        frontSpot = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ���� ��ġ ����
    private GameObject SelectSpot(Transform characterTransform)
    {
        GameObject spot;

        float backDistance = Vector3.Distance(characterTransform.position, backSpot.transform.position);
        float frontDistance = Vector3.Distance(characterTransform.position, frontSpot.transform.position);
        if (backDistance < frontDistance)
        {
            spot = backSpot;
        }
        else
        {
            spot = frontSpot;
        }
        return spot;
    }

    // ���󹰿� ���� �������� Ȯ��
    public bool CanCover(GameObject coverUser, Stat userStat, string targetTag)
    {
        targetDistance = Mathf.Infinity;
        // ���� ��ġ ����
        coverSpot = SelectSpot(coverUser.transform);

        // ���� ���� ���� Ȯ��
        GameObject closestEnemy = null;

        Collider[] cols = Physics.OverlapSphere(coverSpot.transform.position, userStat.Range, targetLayer);
        foreach (Collider col in cols)
        {
            if (col.tag == targetTag)
            {
                float distance = Vector3.Distance(col.transform.position, transform.position);
                if (distance < targetDistance)
                {
                    targetDistance = distance;
                    closestEnemy = col.gameObject;
                }
            }
        }
        return closestEnemy != null;
    }

    public void GetUsedCharacter(GameObject character)
    {
        isOccupied = true;
        useCharacter = character;
    }

    public bool CheckUser(GameObject character)
    {
        return isOccupied && useCharacter == character;
    }

    public void StopCover()
    {
        isOccupied = false;
        useCharacter = null;
        coverSpot = null;
    }
}
