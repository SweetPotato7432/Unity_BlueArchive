using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadUI : MonoBehaviour
{
    private string parentTag;

    private Vector3 movePos;

    [SerializeField]
    private GameObject reloadBG;

    // Start is called before the first frame update
    void Awake()
    {
        parentTag = transform.parent.tag;
        if (parentTag == "Enemy")
        {
            movePos = new Vector3(-1.5f, 0, 0);
            reloadBG.transform.localScale = new Vector3(-reloadBG.transform.localScale.x,reloadBG.transform.localScale.y,reloadBG.transform.localScale.z);
        }
        else if (parentTag == "Ally")
        {
            movePos = new Vector3(1.5f, 0, 0);
        }
    }

    void OnEnable()
    {
        Debug.Log(movePos);
        Vector3 targetPos = transform.parent.transform.position - movePos;

        targetPos.y += 1;
        transform.position = targetPos;

        //Vector3 targetPos = transform.parent.transform.position - transform.parent.transform.forward * 1.5f;
        //targetPos.y += 1f;
        //transform.position = targetPos;
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 정면을 바라보는 UI
        Quaternion cameraRotation = Camera.main.transform.rotation;

        transform.rotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);
    }
}
