using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private GameObject Target;

    private float offsetX = -11.0f;
    private float offsetY = 9.0f;
    private float offsetZ = -21.0f;

    [SerializeField]
    private float CameraSpeed = 10.0f;

    private Vector3 TargetPos;


    // Start is called before the first frame update
    void Start()
    {
        TargetPos = new Vector3(
            Target.transform.position.x + offsetX,
            Target.transform.position.y + offsetY,
            Target.transform.position.z + offsetZ
            );

        transform.position = TargetPos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TargetPos = new Vector3(
            Target.transform.position.x + offsetX,
            Target.transform.position.y + offsetY,
            Target.transform.position.z + offsetZ
            );

        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime*CameraSpeed);
    }
}
