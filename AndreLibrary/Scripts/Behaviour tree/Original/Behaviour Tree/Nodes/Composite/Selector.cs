using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    public class Selector : BaseNode
    {
        // The child nodes for this selector
        protected List<BaseNode> m_nodes = new List<BaseNode>();

        // The constructor requires a list of child nodes to be passed in
        public Selector(List<BaseNode> _nodes)
        {
            m_nodes = _nodes;
        }

        // If any of thechildren reports a success, the selectore will 
        // immediately report a success upwards. If all children fail,
        // it will report a failure instead
        public override NodeStates Evaluate()
        {
            foreach (BaseNode node in m_nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        continue;
                    case NodeStates.SUCCESS:
                        return m_nodeState;
                    case NodeStates.RUNNING:
                        m_nodeState = NodeStates.RUNNING;
                        return m_nodeState;
                    default:
                        continue;
                }
            }
            m_nodeState = NodeStates.FAILURE;
            return m_nodeState;
        }

    }
}
