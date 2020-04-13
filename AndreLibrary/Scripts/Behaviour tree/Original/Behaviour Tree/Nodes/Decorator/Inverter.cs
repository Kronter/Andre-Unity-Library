using System.Collections;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    public class Inverter : BaseNode
    {
        // Child node to evaluate
        BaseNode m_node;

        public BaseNode node { get { return m_node; } }

        // The constructor requires the child node that this inverter decorator wraps
        public Inverter(BaseNode _node)
        {
            m_node = _node;
        }

        // Reports a success if the child fails and a failure if the child succeeds.
        // Running reports as running
        public override NodeStates Evaluate()
        {
            switch (m_node.Evaluate())
            {
                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
                case NodeStates.SUCCESS:
                    m_nodeState = NodeStates.FAILURE;
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
