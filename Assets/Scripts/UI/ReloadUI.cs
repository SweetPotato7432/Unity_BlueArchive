using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadUI : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        Vector3 targetPos = transform.parent.transform.position - new Vector3(1.5f, 0, 0);

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
