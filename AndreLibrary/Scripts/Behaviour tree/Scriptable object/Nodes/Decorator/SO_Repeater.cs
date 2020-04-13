using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CreateAssetMenu(fileName = "New Node", menuName = "AI/Behaviour Tree/Decorator/SO_Repeater")]
    public class SO_Repeater : SO_Node
    {
        // Child node to evaluate
        [SerializeField]
        SO_Node m_node;
        [SerializeField]
        int m_repeatNumber = 1;
        [SerializeField]
        int m_currRepeatNumber = 0;
        bool debug = false;

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
            m_currRepeatNumber = 0;
        }

        public override void AddNode(SO_Node node)
        {
            base.AddNode(node);
            m_node = node;
            Children.Add(node);
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
            DestroyNode(m_node, true);
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

        // Reports a success if the child succeeds and a failure if the child fails.
        // repeates evaluation of child the number of times set in constructor  - 1
        // so that it can return the last state on the last evaluation
        // Running reports as running
        public override NodeStates Evaluate()
        {
            m_currRepeatNumber = 0;
            for (int i = 0; i < m_repeatNumber - 1; i++)
            {
                m_node.Evaluate();
                m_currRepeatNumber++;
            }
            switch (m_node.Evaluate())
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
            }
            m_nodeState = NodeStates.SUCCESS;
            return m_nodeState;
        }

        protected override void DrawButtonGUI()
        {

            if (!debug)
            {
                square.height = 130;
                GUI.Box(square, nameNode, style);
            }
            else
            {
                square.height = 150;
                GUI.Box(square, nameNode, style);
            }

            Vector2 pos = square.position;
            Vector2 size = new Vector2(20, 20);
            pos.x = square.x + (square.width) - 25;
            pos.y = square.y + 10;
            Rect rect = new Rect(pos, size);
            GUIStyle tmpStyle = new GUIStyle();
            debug = EditorGUI.Toggle(rect, debug);

            if (!debug)
            {
                pos = square.position;
                size = new Vector2(40, 40);
                pos.x = square.x + (square.width / 2) - 20;
                pos.y = square.y + square.height - 105;
                rect = new Rect(pos, size);
                GUI.DrawTexture(rect, Icon);

                tmpStyle = new GUIStyle();
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
                GUI.Label(rect, "Total Cycles", tmpStyle);

                pos = square.position;
                size = new Vector2(20, 20);
                size.x = square.width - 35;
                size.y = 18;
                pos.x = square.x + 20;
                pos.y = square.y + square.height - 35;
                rect = new Rect(pos, size);

                int tmp = EditorGUI.IntField(rect, m_repeatNumber);
                if (tmp >= 0 && tmp != m_repeatNumber)
                {
                    m_repeatNumber = tmp;
                }
            }
            else
            {
                pos = square.position;
                size = new Vector2(40, 40);
                pos.x = square.x + (square.width / 2) - 20;
                pos.y = square.y + square.height - 125;
                rect = new Rect(pos, size);
                GUI.DrawTexture(rect, Icon);

                tmpStyle = new GUIStyle();
                tmpStyle.border = new RectOffset(12, 12, 12, 12);
                tmpStyle.alignment = TextAnchor.UpperCenter;
                tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                pos = square.position;
                size = new Vector2(square.width, 40);
                pos.x = square.x;
                pos.y = square.y + square.height - 83;
                rect = new Rect(pos, size);
                GUI.Label(rect, Type, tmpStyle);

                tmpStyle.border = new RectOffset(12, 12, 12, 12);
                tmpStyle.alignment = TextAnchor.UpperCenter;
                tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                pos = square.position;
                size = new Vector2(square.width, 40);
                pos.x = square.x;
                pos.y = square.y + square.height - 68;
                rect = new Rect(pos, size);
                GUI.Label(rect, "Total Cycles", tmpStyle);

                pos = square.position;
                size = new Vector2(20, 20);
                size.x = square.width - 35;
                size.y = 18;
                pos.x = square.x + 20;
                pos.y = square.y + square.height - 55;
                rect = new Rect(pos, size);

                int tmp = EditorGUI.IntField(rect, m_repeatNumber);
                if (tmp >= 0 && tmp != m_repeatNumber)
                {
                    m_repeatNumber = tmp;
                }

                tmpStyle.border = new RectOffset(12, 12, 12, 12);
                tmpStyle.alignment = TextAnchor.UpperCenter;
                tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                pos = square.position;
                size = new Vector2(square.width, 40);
                pos.x = square.x;
                pos.y = square.y + square.height - 30;
                rect = new Rect(pos, size);
                GUI.Label(rect, $"Current Cycle: {m_currRepeatNumber}", tmpStyle);
            }
        }

        public override void SetNodeIcon()
        {
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AndreLibrary/Editor/BT/RepeaterNode_Icon.png");
        }
    }
 }
