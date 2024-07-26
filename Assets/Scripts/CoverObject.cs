using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���󹰿� ������ ��ũ��Ʈ
public class CoverObject : MonoBehaviour
{
    // ������ �ִ� ü�°� ���� ü��
    // �ִ� ü���� Prefab���� ����
    [SerializeField]
    private float maxHp;
    [SerializeField]
    private float curHp;

    // ������ ��� ������ ����
    public bool isOccupied;

    // ĳ���� Ž�� ���̾�
    private LayerMask targetLayer;

    

    // ���� ��ġ
    [SerializeField]
    private GameObject backSpot;
    [SerializeField]
    private GameObject frontSpot;
    public GameObject coverSpot;

    // ���� ��� ĳ����
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

        // ü�� ����
        curHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if(curHp <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    // ���� ��ġ ����
    private GameObject SelectSpot(Transform characterTransform)
    {

        float backDistance = Vector3.Distance(characterTransform.position, backSpot.transform.position);
        float frontDistance = Vector3.Distance(characterTransform.position, frontSpot.transform.position);

        return (backDistance<frontDistance) ? backSpot:frontSpot;
    }

    // ���󹰿� ���� �������� Ȯ��
    public bool CanCover(GameObject coverUser, Stat userStat, string targetTag)
    {
        float targetDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        // ���� ��ġ ����
        coverSpot = SelectSpot(coverUser.transform);

        // ���� ���� ���� Ȯ��
        

        Collider[] cols = Physics.OverlapSphere(coverSpot.transform.position, userStat.Range, targetLayer);
        foreach (Collider col in cols)
        {
            if (col.CompareTag(targetTag))
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

    // ���� ����ϴ� ĳ���� ����
    public void GetUsedCharacter(GameObject character)
    {
        isOccupied = true;
        useCharacter = character;
    }

    // ���� ������ ����ϴ� ĳ���� Ȯ��
    public bool CheckUser(GameObject character)
    {
        return isOccupied && useCharacter == character;
    }

    // ���� ���� ���� ȣ��
    public void StopCover()
    {
        isOccupied = false;
        useCharacter = null;
        coverSpot = null;
    }

    // ������ ���ظ� ������ ���
    public void TakeDamage(Stat attakerStat)
    {
        float damageRatio = 1f / (1f / (1000f / 0.6f));
        float damage = attakerStat.Atk*damageRatio;
        curHp -= damage;
        Debug.Log("���ݹ���!");
    }

    // ���� �ı�
    private void OnDestroy()
    {
        if(useCharacter != null)
        {
            useCharacter.GetComponent<StrikerCharacter>().LeaveCover();
        }
        
        StopCover();
    }
}
