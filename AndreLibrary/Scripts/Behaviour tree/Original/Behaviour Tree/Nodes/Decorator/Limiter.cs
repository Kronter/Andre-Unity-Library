using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.AI.BehaviourTree
{
    public class Limiter : BaseNode
    {
        // Child node to evaluate
        BaseNode m_node;
        NodeStates m_StateToReturn = NodeStates.SUCCESS;
        int m_numTimesToEvaluate = 1;
        int m_numTimesHasEvaluated = 0;

        public BaseNode node { get { return m_node; } }
        public int numTimesHasEvaluated { get { return m_numTimesHasEvaluated; } }
        // Resets the node, so that the child node can be evaluated again
        public void ResetNode() { m_numTimesHasEvaluated = 0; }

        // Resets the node, so that the child node can be evaluated again
        // allows you to reset the amount of times you would like the child node to be evaluated
        public void ResetNode(int _numTimesEvaluate)
        {
            m_numTimesHasEvaluated = 0;
            m_numTimesToEvaluate = _numTimesEvaluate;
            if (m_numTimesToEvaluate <= 0)
                m_numTimesToEvaluate = 1;
        }

        // Resets the node, so that the child node can be evaluated again
        // allows you to reset the amount of times you would like the child node to be evaluated
        // and what you would like the node to evaluate once the limit has reached
        public void ResetNode(int _numTimesEvaluate, NodeStates _StateToReturn)
        {
            m_numTimesHasEvaluated = 0;
            m_StateToReturn = _StateToReturn;
            m_numTimesToEvaluate = _numTimesEvaluate;
            if (m_numTimesToEvaluate <= 0)
                m_numTimesToEvaluate = 1;
        }

        // The constructor requires the child node that this Limiter decorator wraps,
        // the max number of times that you would like the node to be evaluated
        // this node is mostly used to make sure AI does not get stuck in infinte loops
        // and what you would like the node to evaluate once the limit has reached
        public Limiter(BaseNode _node, int _numTimesEvaluate, NodeStates _StateToReturn)
        {
            m_node = _node;
            m_StateToReturn = _StateToReturn;
            m_numTimesToEvaluate = _numTimesEvaluate;
            if (m_numTimesToEvaluate <= 0)
                m_numTimesToEvaluate = 1;
        }

        // Reports a success if the child succeeds and a failure if the child fail.
        // Running reports as running
        // adds one to number of times evaluate, if it is greater or equal to the number of 
        // times asked to evaluate than just returns what you have chosen in contructor and 
        // doesn't evaluate child node
        public override NodeStates Evaluate()
        {
            if (m_numTimesHasEvaluated <= m_numTimesToEvaluate)
            {
                switch (m_node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        m_nodeState = NodeStates.FAILURE;
                        m_numTimesHasEvaluated++;
                        return m_nodeState;
                    case NodeStates.SUCCESS:
                        m_nodeState = NodeStates.SUCCESS;
                        m_numTimesHasEvaluated++;
                        return m_nodeState;
                    case NodeStates.RUNNING:
                        m_nodeState = NodeStates.RUNNING;
                        m_numTimesHasEvaluated++;
                        return m_nodeState;
                }
            }
            m_nodeState = m_StateToReturn;
            m_numTimesHasEvaluated++;
            return m_nodeState;
        }
    }
}