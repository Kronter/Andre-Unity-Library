using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CreateAssetMenu(fileName = "New Node", menuName = "AI/Behaviour Tree/Decorator/SO_InfiniteRepeater")]
    public class SO_InfiniteRepeater : SO_Node
    {
        // Child node to evaluate
        [SerializeField]
        SO_Node m_node;
        bool m_repeat = true;

        public bool repeat { get { return m_repeat; } set { m_repeat = value; } }

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

        // Continues to keep evaluating child node indefinitely.
        // Untill the user sets repeat to false
        // then it reports a success
        public override NodeStates Evaluate()
        {
            while (m_repeat)
            {
                m_node.Evaluate();
            }
            m_nodeState = NodeStates.SUCCESS;
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
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AndreLibrary/Editor/BT/InfiniteRepeaterNode_Icon.png");
        }
    }
}
