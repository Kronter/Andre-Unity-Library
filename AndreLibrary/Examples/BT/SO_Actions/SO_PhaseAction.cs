using System.Collections;
using System.Collections.Generic;
using Andre.AI.BehaviourTree;
using UnityEngine;

[CreateAssetMenu(fileName = "New Node", menuName = "AI/Behaviour Tree/Actions/ Phase")]
public class SO_PhaseAction : Andre.AI.BehaviourTree.Scriptable.SO_Action
{
    NodeStates state = NodeStates.RUNNING;
    GameObject obj;

    private void OnDisable()
    {
        BehaviourTreeTester.onTimerCheck -= OnTimerEnd;
    }

    private void OnDestroy()
    {
        BehaviourTreeTester.onTimerCheck -= OnTimerEnd;
    }
    public override NodeStates Action()
    {
        //obj.GetComponent<BehaviourTreeTester>().timerCheck();
        //return state;
        return NodeStates.SUCCESS;
    }

    void OnTimerEnd(bool running)
    {
        state = running == true ? NodeStates.SUCCESS : NodeStates.RUNNING;
    }

    public override void Initialize(GameObject obj = null)
    {
        this.obj = obj;
        BehaviourTreeTester.onTimerCheck += OnTimerEnd;
        state = NodeStates.RUNNING;
    }
}
