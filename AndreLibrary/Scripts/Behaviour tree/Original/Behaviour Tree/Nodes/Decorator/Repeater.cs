using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    public class Repeater : BaseNode
    {
        // Child node to evaluate
        BaseNode m_node;
        int m_repeatNumber = 1;

        public BaseNode node { get { return m_node; } }

        // The constructor requires the child node that this Repeater decorator wraps,
        // and the number of times wanted for it to repeat evaluating
        // if the number given is 0 or less than 0 than makes it 1 to avoid any bugs
        public Repeater(BaseNode _node, int _repeatNumber)
        {
            m_node = _node;
            m_repeatNumber = _repeatNumber;
            if (m_repeatNumber <= 0)
                m_repeatNumber = 1;
        }

        // Reports a success if the child succeeds and a failure if the child fails.
        // repeates evaluation of child the number of times set in constructor  - 1
        // so that it can return the last state on the last evaluation
        // Running reports as running
        public override NodeStates Evaluate()
        {
            for (int i = 0; i < m_repeatNumber - 1; i++)
            {
                m_node.Evaluate();
            }
            switch (m_node.Evaluate())
            {
                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;
                case NodeStates.SUCCESS:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;
            }
            m_nodeState = NodeStates.SUCCESS;
            return m_nodeState;
        }
    }
}