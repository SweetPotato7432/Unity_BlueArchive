using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ¾öÆó¹°°ú °ü·ÃµÈ ½ºÅ©¸³Æ®

public class CoverObject : MonoBehaviour
{
    private GameObject coveredCharacter;

    private float maxHp;
    private float curHp;

    // ¾öÆóÀÚ ÀÖ´ÂÁö Ã¼Å©
    public bool isOccupied;

    // Ä³¸¯ÅÍ Å½Áö ·¹ÀÌ¾î
    private LayerMask targetLayer;

    // °¡±î¿î ¸ñÇ¥¿ÍÀÇ °Å¸®
    private float targetDistance;

    // ¾öÆó À§Ä¡
    [SerializeField]
    private GameObject backSpot;
    [SerializeField]
    private GameObject frontSpot;

    public GameObject coverSpot;

    // Start is called before the first frame update
    void Start()
    {
        targetLayer = LayerMask.GetMask("Character");

        isOccupied = false;

        // À§Ä¡ ÀúÀå
        backSpot = transform.GetChild(0).gameObject;
        frontSpot = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ¾öÆó À§Ä¡ ÁöÁ¤
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

    // ¾öÆó¹°¿¡¼­ °¡Àå °¡±î¿î Àû È®ÀÎ
    public bool CanCover(GameObject coverUser, Stat userStat, string targetTag)
    {
        targetDistance = Mathf.Infinity;
        // ¾öÆó À§Ä¡
        coverSpot = SelectSpot(coverUser.transform);

        // °¡±î¿î Àû È®ÀÎ
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
                    //targetCharacter = closestTarget.GetComponent<Character>();
                }
            }
        }
        if (closestEnemy == null)
        {
            Debug.Log("ÀÌµ¿X");
            return false;
        }
        else
        {
            Debug.Log("ÀÌµ¿");
            return true;
        }
    }
<<<<<<< HEAD

    public void GetUsedCharacter(GameObject character)
    {
        isOccupied = true;
        useCharacter = character;
    }


    public bool CheckUser(GameObject character)
    {
        if(isOccupied && useCharacter == character)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void StopCover()
    {
        isOccupied = false;
        useCharacter = null;
        coverSpot = null;
    }
=======
>>>>>>> parent of 0940890 (ì—„í ê¸°ëŠ¥ êµ¬í˜„ ë° ìˆ˜ì •)
}