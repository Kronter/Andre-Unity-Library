using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CreateAssetMenu(fileName = "New Node", menuName = "AI/Behaviour Tree/Composite/SO_BooleanSelector")]
    public class SO_BooleanSelector : SO_Node
    {
        // The child nodes for this selector
        public SO_Node[] m_nodes = new SO_Node[3];

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
            int i = 1;
            foreach (var obj in m_nodes)
            {
                if (obj == null)
                    break;
                i++;
            }
            if (i > m_nodes.Length)
                return;
            m_nodes[i - 1] = node;
            base.AddNode(node);
            Children.Add(node);
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
        }

        public override void MoveNodePositions(string NodeAName, string NodeBName)
        {
            int NodeAIndex = 0;
            int NodeBIndex = 0;
            int i = 0;
            foreach (var node in m_nodes)
            {
                if (node.name == NodeAName)
                    NodeAIndex = i;

                if (node.name == NodeBName)
                    NodeBIndex = i;
                i++;
            }

            if (NodeAIndex == 0 && NodeBIndex == 0)
                return;

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
                    return false;
                if (Node == node.name)
                    return true;
            }
            return false;
        }


        public override void RemoveNode(string Node)
        {
            int i = 0;
            foreach (var node in m_nodes)
            {
                if (Node == node.name)
                {
                    Children.Remove(node);
                    m_nodes[i] = null;
                    node.DestroyNode(node, true);
                    return;
                }
                i++;
            }
        }

        public override void DestroyNode(Object obj, bool allowDestroyingAssets = false)
        {
            int i = 0;
            foreach (var node in Children)
            {
                m_nodes[i] = null;
                node.DestroyNode(node, true);
            }
            i++;

            Children.Clear();
            DestroyImmediate(obj, true);
        }

        // If any of the children reports a success, the selector will 
        // immediately report a success upwards. If all children fail,
        // it will report a failure instead
        public override NodeStates Evaluate()
        {

            switch (m_nodes[0].Evaluate())
            {
                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodes[1].Evaluate();
                case NodeStates.SUCCESS:
                    m_nodeState = NodeStates.SUCCESS;
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
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AndreLibrary/Editor/BT/BooleanSelectorNode_Icon.png");
        }
    }
}