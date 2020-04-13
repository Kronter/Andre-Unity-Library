using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    public class Sequence : BaseNode
    {
        // The child nodes for this Sequence
        List<BaseNode> m_nodes = new List<BaseNode>();

        // The constructor requires a list of child nodes to be passed in
        public Sequence(List<BaseNode> _nodes)
        {
            m_nodes = _nodes;
        }

        // If any child node returns a failure, the entire node fails.
        // When all nodes return a success, the node reports a success.
        public override NodeStates Evaluate()
        {
            bool anyChildRunning = false;

            foreach (BaseNode node in m_nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        m_nodeState = NodeStates.FAILURE;
                        return m_nodeState;
                    case NodeStates.SUCCESS:
                        continue;
                    case NodeStates.RUNNING:
                        anyChildRunning = true;
                        continue;
                    default:
                        m_nodeState = NodeStates.SUCCESS;
                        return m_nodeState;
                }
            }
            m_nodeState = anyChildRunning ? NodeStates.RUNNING : NodeStates.SUCCESS;
            return m_nodeState;
        }
    }
}