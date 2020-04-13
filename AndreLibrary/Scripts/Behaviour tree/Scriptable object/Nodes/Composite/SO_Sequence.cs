using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CreateAssetMenu(fileName = "New Node", menuName = "AI/Behaviour Tree/Composite/SO_Sequence")]
    public class SO_Sequence : SO_Node
    {
        // The child nodes for this Sequence
        public List<SO_Node> m_nodes = new List<SO_Node>();

        public override void Initialize(GameObject obj)
        {
            foreach (var node in Children)
            {
                if (obj != null)
                    node.Initialize(obj);
            }
        }

        public override void AddNode(SO_Node node)
        {
            base.AddNode(node);
            m_nodes.Add(node);
            Children.Add(node);
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
        }

        public override void MoveNodePositions(string NodeAName, string NodeBName)
        {
            if (!ListContainsByName(NodeAName, m_nodes) || !ListContainsByName(NodeBName, m_nodes))
                return;
            int NodeAIndex = GetInListByName(NodeAName, m_nodes);
            int NodeBIndex = GetInListByName(NodeBName, m_nodes);

            SO_Node NodeA = m_nodes[NodeAIndex];
            SO_Node NodeB = m_nodes[NodeBIndex];
            Vector2 posNodeA = NodeA.square.position;
            Vector2 posNodeB = NodeB.square.position;

            m_nodes[NodeAIndex].SetNewPos(posNodeB);
            m_nodes[NodeBIndex].SetNewPos(posNodeA);
            m_nodes[NodeAIndex] = NodeB;
            m_nodes[NodeBIndex] = NodeA;
            Children[NodeAIndex] = NodeB;
            Children[NodeBIndex] = NodeA;
        }

        public override bool ContainsName(string Node)
        {
            foreach (var node in m_nodes)
            {
                if (node == null)
                    continue;

                if (Node == node.name)
                    return true;
            }
            return false;
        }


        public override void RemoveNode(string Node)
        {
            foreach (var node in m_nodes)
            {
                if (Node == node.name)
                {
                    Children.Remove(node);
                    m_nodes.Remove(node);
                    node.DestroyNode(node, true);
                    return;
                }
            }
        }

        public override void DestroyNode(Object obj, bool allowDestroyingAssets = false)
        {
            foreach (var node in Children)
            {
                node.DestroyNode(node, true);
            }
            Children.Clear();
            m_nodes.Clear();
            DestroyImmediate(obj, true);
        }

        // If any child node returns a failure, the entire node fails.
        // When all nodes return a success, the node reports a success.
        public override NodeStates Evaluate()
        {
            bool anyChildRunning = false;

            foreach (SO_Node node in m_nodes)
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

        protected override void DrawButtonGUI()
        {
            GUI.Box(square, nameNode, style);
            Vector2 pos = square.position;
            Vector2 size = new Vector2(60, 30);
            pos.x = square.x + (square.width / 2) - 30;
            pos.y = square.y + square.height - 53;
            Rect rect = new Rect(pos, size);
            GUI.DrawTexture(rect, Icon);

            base.DrawButtonGUI();
        }

        public override void SetNodeIcon()
        {
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AndreLibrary/Editor/BT/SequenceNode_Icon.png");
        }
    }
}
