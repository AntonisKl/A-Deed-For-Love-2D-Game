using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacking : StateMachineBehaviour {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // set weight to 1 to let the attacking animation to be shown
        animator.SetLayerWeight(1, 1);
    }
}
