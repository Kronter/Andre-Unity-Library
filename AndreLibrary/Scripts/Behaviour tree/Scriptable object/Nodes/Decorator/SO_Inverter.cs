using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CreateAssetMenu(fileName = "New Node", menuName = "AI/Behaviour Tree/Decorator/SO_Inverter")]
    public class SO_Inverter : SO_Node
    {
        // Child node to evaluate
        [SerializeField]
        SO_Node m_node;

        public void OnEnable()
        {
            SwapableNodes = false;
        }

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
            Children.Add(node);
            m_node = node;
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
        }

        public override void MoveNodePositions(string NodeAName, string NodeBName)
        {
        }

        public override bool ContainsName(string name)
        {
            if (m_node == null)
                return false;
            return m_node.name == name;
        }

        public override void RemoveNode(string name)
        {
            if (m_node == null)
                return;

            Children.Remove(m_node);
            m_node.DestroyNode(m_node, true);
            m_node = null;
        }


        public override void DestroyNode(Object obj, bool allowDestroyingAssets = false)
        {
            foreach (var node in Children)
            {
                node.DestroyNode(node, true);
            }
            Children.Clear();
            m_node = null;
            DestroyImmediate(obj, true);
        }

        // Reports a success if the child fails and a failure if the child succeeds.
        // Running reports as running
        public override NodeStates Evaluate()
        {
            switch (m_node.Evaluate())
            {
                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
                case NodeStates.SUCCESS:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;
            }
            m_nodeState = NodeStates.SUCCESS;
            return m_nodeState;
        }

        protected override void DrawButtonGUI()
        {
            square.height = 95;
            GUI.Box(square, nameNode, style);
            Vector2 pos = square.position;
            Vector2 size = new Vector2(50, 50);
            pos.x = square.x + (square.width / 2) - 25;
            pos.y = square.y + square.height - 73;
            Rect rect = new Rect(pos, size);
            GUI.DrawTexture(rect, Icon);

            base.DrawButtonGUI();
        }

        public override void SetNodeIcon()
        {
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AndreLibrary/Editor/BT/InverterNode_Icon.png");
        }
    }
}
