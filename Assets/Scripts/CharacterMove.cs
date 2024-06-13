using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMove : MonoBehaviour
{// Start is called before the first frame update

    enum States
    {
        None,
        Move,
        Cover,
        Attack,
        Reload
    };

    [SerializeField]
    private States currentState;

    private NavMeshAgent agent;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private GameObject closestTarget;
    private float closestDistance;

    private LayerMask targetLayer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {

        currentState = States.Move;
        targetLayer = LayerMask.NameToLayer("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case States.Move:
                {
                    if (agent.isStopped)
                    {
                        agent.isStopped = false;
                    }
                    closestDistance = Mathf.Infinity;
                    closestTarget = null;

                    // �̵�
                    agent.SetDestination(target.position);

                    // �� Ž��
                    int layerMask = (1 << targetLayer);

                    Collider[] cols = Physics.OverlapSphere(transform.position, 15, layerMask);
                    foreach (Collider col in cols)
                    {
                        float distance = Vector3.Distance(col.transform.position, transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTarget = col.gameObject;
                        }
                    }
                    if (cols.Length >= 1)
                    {
                        // ���� ���� Ȯ��

                        // ���� Ȯ�� �� ����
                        currentState = States.Attack;
                        if (!agent.isStopped)
                        {
                            agent.isStopped = true;
                        }
                        
                    }
                    break;
                }
            case States.Cover:
                {
                    break;
                }
            case States.Attack:
                {
                    // ����

                    // Ž������ ��ȯ
                    if (closestTarget == null)
                    {
                        currentState = States.Move;
                    }
                    break;
                }
            case States.Reload:
                {
                    // ������
                    break;
                }
        }
    }
}
