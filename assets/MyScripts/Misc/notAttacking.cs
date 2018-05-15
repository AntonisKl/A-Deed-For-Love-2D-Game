using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class notAttacking : StateMachineBehaviour {

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // set weight to 0 to let the movement animation to be shown
        animator.SetLayerWeight(1, 0);
    }
}
