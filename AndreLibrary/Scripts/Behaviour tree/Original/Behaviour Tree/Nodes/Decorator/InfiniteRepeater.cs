using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    public class InfiniteRepeater : BaseNode
    {
        // Child node to evaluate
        BaseNode m_node;
        bool m_repeat = true;

        public BaseNode node { get { return m_node; } }
        public bool repeat { get { return m_repeat; } set { m_repeat = value; } }


        // The constructor requires the child node that this Infinite Repeater decorator wraps
        public InfiniteRepeater(BaseNode _node)
        {
            m_node = _node;
        }

        // Continues to keep evaluating child node indefinitely.
        // Untill the user sets repeat to false
        // then it reports a success
        public override NodeStates Evaluate()
        {
            while (m_repeat)
            {
                m_node.Evaluate();
            }
            m_nodeState = NodeStates.SUCCESS;
            return m_nodeState;
        }
    }
}
