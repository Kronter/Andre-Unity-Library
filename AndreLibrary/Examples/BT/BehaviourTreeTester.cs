using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeTester : MonoBehaviour
{
    public Andre.AI.BehaviourTree.Scriptable.SO_BehaviourTree BT;
    public bool test = false;
    public int Number = 0;

    public delegate void TimerCheck(bool Running);
    public static event TimerCheck onTimerCheck;


    private void Awake()
    {
        BT.Initialize(this.gameObject);
    }

    private void OnDisable()
    {
        BT.OnBTDisable();
    }

    // Start is called before the first frame update
    void Update()
    {
        //if (!test)
        //    return;

        Andre.AI.BehaviourTree.NodeStates state = BT.root.Evaluate();

        if (state == Andre.AI.BehaviourTree.NodeStates.RUNNING)
        {
            Debug.Log("Running!");
            return;
        }

        if (state == Andre.AI.BehaviourTree.NodeStates.SUCCESS)
            Debug.Log($"Success! : {Number}");
        else if (state == Andre.AI.BehaviourTree.NodeStates.FAILURE)
            Debug.Log("Fail!");

        //test = false;
    }
    bool RanTimer = false;
    bool turnEnded = false;

    public void timerCheck()
    {
        if (turnEnded)
        {
            onTimerCheck?.Invoke(turnEnded);
            return;
        }
        if (!RanTimer)
            StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        RanTimer = true;
        turnEnded = false;
        Number++;
        yield return new WaitForSeconds(2);
        RanTimer = false;
        turnEnded = true;
    }
}
