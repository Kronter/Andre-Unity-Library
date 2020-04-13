using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    public class DebugLog : BaseNode
    {
        string m_ToPrint = "This is a debug.log";

        // The constructor requires a string to print
        public DebugLog(string _ToPrint)
        {
            m_ToPrint = _ToPrint;
        }

        // Reports a success and prints out what was given to it
        public override NodeStates Evaluate()
        {
            Debug.Log(m_ToPrint.ToString());
            m_nodeState = NodeStates.SUCCESS;
            return m_nodeState;
        }
    }
}
