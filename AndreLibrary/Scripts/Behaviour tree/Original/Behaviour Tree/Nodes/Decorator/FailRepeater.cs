using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    public class FailRepeater : BaseNode
    {
        // Child node to evaluate
        BaseNode m_node;

        public BaseNode node { get { return m_node; } }

        // The constructor requires the child node that this Fail Repeater decorator wraps
        public FailRepeater(BaseNode _node)
        {
            m_node = _node;
        }

        // keeps repeating the evaluation of its child until the child reports a failure
        // Reports a fail once done.
        public override NodeStates Evaluate()
        {
            bool failed = false;
            while (!failed)
            {
                switch (m_node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        failed = true;
                        break;
                    case NodeStates.SUCCESS:
                        continue;
                    case NodeStates.RUNNING:
                        continue;
                }
            }
            m_nodeState = NodeStates.FAILURE;
            return m_nodeState;
        }
    }
}