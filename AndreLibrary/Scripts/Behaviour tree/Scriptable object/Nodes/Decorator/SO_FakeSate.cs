using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CreateAssetMenu(fileName = "New Node", menuName = "AI/Behaviour Tree/Decorator/SO_FakeSate")]
    public class SO_FakeSate : SO_Node
    {
        // Child node to evaluate
        [SerializeField]
        SO_Node m_node;
        [SerializeField]
        NodeStates m_StateToChange = NodeStates.SUCCESS;

        public NodeStates StateToChange { get { return m_StateToChange; } set { m_StateToChange = value; } }

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

        // Reports the node state chosen by the user no matter what the child reports.
        // Running reports as running
        public override NodeStates Evaluate()
        {
            if (m_node != null)
            {

                switch (m_node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        m_nodeState = m_StateToChange;
                        return m_nodeState;
                    case NodeStates.SUCCESS:
                        m_nodeState = m_StateToChange;
                        return m_nodeState;
                    case NodeStates.RUNNING:
                        m_nodeState = NodeStates.RUNNING;
                        return m_nodeState;
                }
            }
            m_nodeState = m_StateToChange;
            return m_nodeState;
        }
        protected override void DrawButtonGUI()
        {
            square.height = 130;
            GUI.Box(square, nameNode, style);
            Vector2 pos = square.position;
            Vector2 size = new Vector2(40, 40);
            pos.x = square.x + (square.width / 2) - 20;
            pos.y = square.y + square.height - 103;
            Rect rect = new Rect(pos, size);
            GUI.DrawTexture(rect, Icon);


            GUIStyle tmpStyle = new GUIStyle();
            tmpStyle.border = new RectOffset(12, 12, 12, 12);
            tmpStyle.alignment = TextAnchor.UpperCenter;
            tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            pos = square.position;
            size = new Vector2(square.width, 40);
            pos.x = square.x;
            pos.y = square.y + square.height - 63;
            rect = new Rect(pos, size);
            GUI.Label(rect, Type, tmpStyle);


            tmpStyle.border = new RectOffset(12, 12, 12, 12);
            tmpStyle.alignment = TextAnchor.UpperCenter;
            tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            pos = square.position;
            size = new Vector2(square.width, 40);
            pos.x = square.x;
            pos.y = square.y + square.height - 48;
            rect = new Rect(pos, size);
            GUI.Label(rect, "State To Fake", tmpStyle);

            pos = square.position;
            size = new Vector2(20, 20);
            size.x = square.width - 35;
            size.y = 18;
            pos.x = square.x + 20;
            pos.y = square.y + square.height - 35;
            rect = new Rect(pos, size);

            NodeStates tmpState = (NodeStates)EditorGUI.EnumPopup(rect, m_StateToChange);
            if (tmpState != m_StateToChange)
            {
                m_StateToChange = tmpState;
            }
        }

        public override void SetNodeIcon()
        {
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AndreLibrary/Editor/BT/FakeStateNode_Icon.png");
        }
    }
}
