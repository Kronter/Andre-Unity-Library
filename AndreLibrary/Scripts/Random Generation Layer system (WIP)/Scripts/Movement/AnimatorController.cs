using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private MovementComponent movementComponent;
    protected AnimationState m_animState = AnimationState.Idle;
    public AnimationState animationState { set { m_animState = value; } }

    void Start()
    {
        if (!movementComponent)
            Debug.LogError($"You have forgotten to set movementComponent on {transform.name}");
        if (!animator)
            Debug.LogError($"You have forgotten to set animator on {transform.name}");
    }

    private void Update()
    {
        if (movementComponent.walking && movementComponent.canMove)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                m_animState = AnimationState.Run;
            else
                m_animState = AnimationState.Walk;
        }
        else if (m_animState == AnimationState.Attack)
            movementComponent.canMove = false;
        else
            m_animState = AnimationState.Idle;

        animator.SetBool(AnimationHash.Walk, m_animState == AnimationState.Walk);
        animator.SetBool(AnimationHash.Run, m_animState == AnimationState.Run);
        animator.SetBool(AnimationHash.Attack, m_animState == AnimationState.Attack);

    }

    public void SetCanMove(bool _canMove)
    {
        movementComponent.canMove = _canMove;
    }
}
