using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    public class BreakPoint : BaseNode
    {
        // The constructor 
        public BreakPoint() { }

        // Break point node, returns success when told to continue on
        // To continue on from break point Press "c"
        public override NodeStates Evaluate()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                m_nodeState = NodeStates.SUCCESS;
                return m_nodeState;
            }
            else
            {
                m_nodeState = NodeStates.RUNNING;
                return m_nodeState;
            }
        }
    }
}
