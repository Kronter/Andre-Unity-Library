using UnityEditor;
using UnityEngine;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CreateAssetMenu(fileName = "New Node", menuName = "AI/Behaviour Tree/Decorator/SO_FailRepeater")]
    public class SO_FailRepeater : SO_Node
    {
        // Child node to evaluate
        [SerializeField]
        SO_Node m_node;
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

        // keeps repeating the evaluation of its child until the child reports a failure
        // Reports a fail once done.
        public override NodeStates Evaluate()
        {
            bool failed = false;
            while (!failed)
            {
                switch (m_node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        failed = true;
                        break;
                    case NodeStates.SUCCESS:
                        continue;
                    case NodeStates.RUNNING:
                        continue;
                }
                m_currRepeatNumber++;
            }
            m_nodeState = NodeStates.FAILURE;
            return m_nodeState;
        }

        protected override void DrawButtonGUI()
        {
            if (!debug)
            {
                square.height = 95;
                GUI.Box(square, nameNode, style);
            }
            else
            {
                square.height = 115;
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
                size = new Vector2(50, 50);
                pos.x = square.x + (square.width / 2) - 25;
                pos.y = square.y + square.height - 72;
                rect = new Rect(pos, size);
                GUI.DrawTexture(rect, Icon);

                tmpStyle = new GUIStyle();
                tmpStyle.border = new RectOffset(12, 12, 12, 12);
                tmpStyle.alignment = TextAnchor.UpperCenter;
                tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                pos = square.position;
                size = new Vector2(square.width, 40);
                pos.x = square.x;
                pos.y = square.y + square.height - 25;
                rect = new Rect(pos, size);
                GUI.Label(rect, Type, tmpStyle);
            }
            else
            {
                pos = square.position;
                size = new Vector2(50, 50);
                pos.x = square.x + (square.width / 2) - 25;
                pos.y = square.y + square.height - 92;
                rect = new Rect(pos, size);
                GUI.DrawTexture(rect, Icon);

                tmpStyle = new GUIStyle();
                tmpStyle.border = new RectOffset(12, 12, 12, 12);
                tmpStyle.alignment = TextAnchor.UpperCenter;
                tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                pos = square.position;
                size = new Vector2(square.width, 40);
                pos.x = square.x;
                pos.y = square.y + square.height - 45;
                rect = new Rect(pos, size);
                GUI.Label(rect, Type, tmpStyle);

                tmpStyle.border = new RectOffset(12, 12, 12, 12);
                tmpStyle.alignment = TextAnchor.UpperCenter;
                tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                pos = square.position;
                size = new Vector2(square.width, 40);
                pos.x = square.x;
                pos.y = square.y + square.height - 25;
                rect = new Rect(pos, size);
                GUI.Label(rect, $"Current Cycle: {m_currRepeatNumber}", tmpStyle);
            }
        }

        public override void SetNodeIcon()
        {
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AndreLibrary/Editor/BT/FailRepeaterNode_Icon.png");
        }
    }
}
