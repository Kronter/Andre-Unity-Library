using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    public class BooleanSelector : BaseNode
    {
        // The child nodes for this selector
        protected BaseNode[] m_nodes = new BaseNode[3];

        /// <summary>
        /// The constructor requires a list of child nodes to be passed in
        /// can only take 3 nodes in array, first node is the node that will 
        /// be evaluated to figure out which of the otehr tow you will go to
        /// </summary>
        /// <param name="_nodes"></param>
        public BooleanSelector(BaseNode[] _nodes)
        {
            m_nodes = _nodes;
        }

        // If any of thechildren reports a success, the selectore will 
        // immediately report a success upwards. If all children fail,
        // it will report a failure instead
        public override NodeStates Evaluate()
        {

            switch (m_nodes[0].Evaluate())
            {
                case NodeStates.FAILURE:
                    return m_nodes[1].Evaluate();
                case NodeStates.SUCCESS:
                    return m_nodes[2].Evaluate();
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;
                default:
                    break;
            }
            m_nodeState = NodeStates.FAILURE;
            return m_nodeState;
        }

    }
}