using UnityEngine;
using UnityEngine.AI;
using System;
using Pathfinding;
using Pathfinding.RVO;

public class RootMotionWalk : MovementComponent
{
   // [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject animatedObject;
    [SerializeField] private Animator animator;
    [SerializeField] private RichAI agent;
    [SerializeField] private RVOController controller;
    Vector3 deltaPosition;

    // Start is called before the first frame update
    void Start()
    {
        //if (!agent)
        //    Debug.LogError($"You have forgotten to set navmeshAgent on {transform.name}");

        if (!agent)
            Debug.LogError($"You have forgotten to set agent on {transform.name}");

        if (!animatedObject)
            Debug.LogError($"You have forgotten to set animatedObject on {transform.name}");

        //agent.updatePosition = false;
        //agent.canMove = false;
        //agent.updatePosition = false;
        //seeker.updatePosition = false;
    }

    // Update is called once per frame
    void Update()
    {
        agent.maxSpeed = speed;
        agent.endReachedDistance = m_stopDistance;
        AgentMove();
        AgentArrived();
    }

    void AgentMove()
    {
        //if (seeker.destination == m_destination)
        //    return;

        if (!m_walking)
            return;

        agent.destination = m_destination;

        Vector3 nextPosition;
        Quaternion nextRotation;
        agent.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
        if (!controller)
            deltaPosition = new Vector3(animator.rootPosition.x, nextPosition.y, animator.rootPosition.z);
        //deltaPosition = nextPosition;
        else
        {
            deltaPosition = new Vector3(animator.rootPosition.x, nextPosition.y, animator.rootPosition.z);
            //deltaPosition = nextPosition;
            Vector3 delta = controller.CalculateMovementDelta(animatedObject.transform.position, Time.deltaTime);
            deltaPosition = new Vector3(deltaPosition.x + delta.x, deltaPosition.y, deltaPosition.z + delta.z);
        }

        agent.FinalizeMovement(deltaPosition, nextRotation);
        transform.rotation = nextRotation;

        //if (agent.destination == m_destination)
        //    return;

        //agent.destination = m_destination;
        //if (agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial)
        //{
        //    m_walking = false;
        //    agent.isStopped = true;
        //    return;
        //}

        //if (agent.isStopped)
        //    agent.isStopped = false;

        //m_walking = true;
        //agent.nextPosition = animatedObject.transform.position;
        //transform.rotation = agent.transform.rotation;
    }

    void AgentArrived()
    {
        float dist = Vector3.Distance(animatedObject.transform.position, m_destination);
        if ((agent.reachedDestination && dist <= m_stopDistance ) || Vector3.Distance(animatedObject.transform.position, m_destination) <= m_stopDistance)
        {
            m_walking = false;
            agent.canMove = false;
            agent.updateRotation = false;
            //if(controller)
            //    controller.enabled = false;
        }
        else if (m_canMove)
        {
            m_walking = true;
            agent.canMove = true;
            agent.updateRotation = true;
            //if (controller)
            //    controller.enabled = true;
        }

        //float dist = agent.remainingDistance;
        //if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= m_stopDistance)
        //{
        //    m_walking = false;
        //}
    }

    //private void OnAnimatorMove()
    //{
    //    if (Time.deltaTime <= 0) return;

    //    Vector3 nextPosition;
    //    Quaternion nextRotation;

    //    seeker.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
    //    //seeker.FinalizeMovement(new Vector3(_animator.rootPosition.x, nextPosition.y, _animator.rootPosition.z), nextRotation);

    //}
}
