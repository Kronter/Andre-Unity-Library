using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    public class FakeState : BaseNode
    {
        // Child node to evaluate
        BaseNode m_node;
        NodeStates m_StateToChange = NodeStates.SUCCESS;

        public BaseNode node { get { return m_node; } }
        public NodeStates StateToChange { get { return m_StateToChange; } set { m_StateToChange = value; } }

        // The constructor requires the child node that this Fake State decorator wraps,
        // the node state tha you would like it to report
        public FakeState(BaseNode _node, NodeStates _StateToChange)
        {
            m_node = _node;
            m_StateToChange = _StateToChange;
        }

        // Reports the node state chosen by the user no matter what the child reports.
        // Running reports as running
        public override NodeStates Evaluate()
        {
            switch (m_node.Evaluate())
            {
                case NodeStates.FAILURE:
                    m_nodeState = m_StateToChange;
                    return m_nodeState;
                case NodeStates.SUCCESS:
                    m_nodeState = m_StateToChange;
                    return m_nodeState;
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;
            }
            m_nodeState = m_StateToChange;
            return m_nodeState;
        }
    }
}
