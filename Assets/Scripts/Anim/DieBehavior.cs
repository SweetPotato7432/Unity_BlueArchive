using UnityEngine;

public class DieBehavior : StateMachineBehaviour
{
     override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.gameObject);
    }
}
