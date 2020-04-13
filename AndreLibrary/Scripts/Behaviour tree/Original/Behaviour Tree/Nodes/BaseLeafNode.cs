using System.Collections;
using System;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    public class BaseLeafNode : BaseNode
    {
        // Method signature for the action
        // "A delegate in C# can be thought of as a function pointer of sorts. 
        // You can also think of a delegate as a variable containing 
        // (or more accurately, pointing to) a function. 
        // This allows you to set the function to be called at runtime."
        public delegate NodeStates BaseLeafNodeDelegate();

        // The delegate that is called to evaluate this node
        BaseLeafNodeDelegate m_action;

        // Because this node contains no logic itself, the logic must be passed in
        // in the form of a delegate. As the signature states, the action needs to
        // return a NodeStates enum
        public BaseLeafNode(BaseLeafNodeDelegate _action)
        {
            m_action = _action;
        }

        // Evaluates the node using the passed n delegate and reports the resulting
        // state as appropriate
        public override NodeStates Evaluate()
        {
            switch (m_action())
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
                default:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;
            }
        }

    }
}
