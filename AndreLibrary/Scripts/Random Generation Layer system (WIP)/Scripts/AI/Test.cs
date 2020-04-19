using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject target;
    [SerializeField] private MovementComponent movementComponent;
    [SerializeField] private AnimatorController animatorController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementComponent.destination = target.transform.position;
        if (Vector3.Distance(target.transform.position, transform.position) <= movementComponent.m_stopDistance)
            animatorController.animationState = AnimationState.Attack;
        else
            animatorController.animationState = AnimationState.Idle;
    }
}
