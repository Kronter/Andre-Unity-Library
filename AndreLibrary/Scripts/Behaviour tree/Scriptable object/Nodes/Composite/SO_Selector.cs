using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CreateAssetMenu(fileName = "New Node", menuName = "AI/Behaviour Tree/Composite/SO_Selector")]
    public class SO_Selector : SO_Node
    {
        // The child nodes for this selector
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

        // If any of the children reports a success, the selectore will 
        // immediately report a success upwards. If all children fail,
        // it will report a failure instead
        public override NodeStates Evaluate()
        {
            foreach (SO_Node node in m_nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        continue;
                    case NodeStates.SUCCESS:
                        m_nodeState = NodeStates.SUCCESS;
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
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AndreLibrary/Editor/BT/SelectorNode_Icon.png");
        }
    }
}
