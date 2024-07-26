using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 엄폐물에 부착된 스크립트
public class CoverObject : MonoBehaviour
{
    // 엄폐물의 최대 체력과 현재 체력
    // 최대 체력은 Prefab에서 설정
    [SerializeField]
    private float maxHp;
    [SerializeField]
    private float curHp;

    // 엄폐물이 사용 중인지 여부
    public bool isOccupied;

    // 캐릭터 탐지 레이어
    private LayerMask targetLayer;

    

    // 엄폐 위치
    [SerializeField]
    private GameObject backSpot;
    [SerializeField]
    private GameObject frontSpot;
    public GameObject coverSpot;

    // 엄폐물 사용 캐릭터
    [SerializeField]
    private GameObject useCharacter;

    // Start is called before the first frame update
    void Start()
    {
        targetLayer = LayerMask.GetMask("Character");
        isOccupied = false;

        // 위치 설정
        backSpot = transform.GetChild(0).gameObject;
        frontSpot = transform.GetChild(1).gameObject;

        // 체력 설정
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

    // 엄폐 위치 선택
    private GameObject SelectSpot(Transform characterTransform)
    {

        float backDistance = Vector3.Distance(characterTransform.position, backSpot.transform.position);
        float frontDistance = Vector3.Distance(characterTransform.position, frontSpot.transform.position);

        return (backDistance<frontDistance) ? backSpot:frontSpot;
    }

    // 엄폐물에 엄폐가 가능한지 확인
    public bool CanCover(GameObject coverUser, Stat userStat, string targetTag)
    {
        float targetDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        // 엄폐 위치 선택
        coverSpot = SelectSpot(coverUser.transform);

        // 엄폐 가능 여부 확인
        

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

    // 엄폐물 사용하는 캐릭터 설정
    public void GetUsedCharacter(GameObject character)
    {
        isOccupied = true;
        useCharacter = character;
    }

    // 현재 엄폐물을 사용하는 캐릭터 확인
    public bool CheckUser(GameObject character)
    {
        return isOccupied && useCharacter == character;
    }

    // 엄폐 종료 시점 호출
    public void StopCover()
    {
        isOccupied = false;
        useCharacter = null;
        coverSpot = null;
    }

    // 엄폐물이 피해를 받을때 사용
    public void TakeDamage(Stat attakerStat)
    {
        float damageRatio = 1f / (1f / (1000f / 0.6f));
        float damage = attakerStat.Atk*damageRatio;
        curHp -= damage;
        Debug.Log("공격받음!");
    }

    // 엄폐물 파괴
    private void OnDestroy()
    {
        if(useCharacter != null)
        {
            useCharacter.GetComponent<StrikerCharacter>().LeaveCover();
        }
        
        StopCover();
    }
}
