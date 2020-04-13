using System.Collections;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    [System.Serializable]
    public abstract class BaseNode
    {
        // Delegate that returns the state of the node
        // "A delegate in C# can be thought of as a function pointer of sorts. 
        // You can also think of a delegate as a variable containing 
        // (or more accurately, pointing to) a function. 
        // This allows you to set the function to be called at runtime."
        public delegate NodeStates NodeReturn();

        // The current state of the node
        public NodeStates m_nodeState;

        public NodeStates nodesState { get { return m_nodeState; } }

        // The constructor for the node
        public BaseNode() { }

        // Implementing classes use this method to evaluate the desired set of conditions
        public abstract NodeStates Evaluate();
    }
}