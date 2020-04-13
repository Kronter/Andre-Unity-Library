using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    public class SO_LeafNode : SO_Node
    {
        // The delegate that is called to evaluate this node
        public SO_Action m_action;

        public void OnEnable()
        {
            SwapableNodes = false;
            NodeAdd = false;
        }


        public override void Initialize(GameObject obj = null)
        {
            if(obj !=null)
            m_action.Initialize(obj);
        }

        public override void AddNode(SO_Node node)
        {
        }

        public override void MoveNodePositions(string NodeAName, string NodeBName)
        {
        }

        public override bool ContainsName(string name)
        {
            return false;
        }

        public override void RemoveNode(string name)
        {
        }

        public override void DestroyNode(Object obj, bool allowDestroyingAssets = false)
        {
            DestroyImmediate(obj, true);
        }

        protected override void DrawButtonGUI()
        {
            square.width = 175;
            square.height = 115;
            GUI.Box(square, nameNode, style);
            Vector2 pos = square.position;
            Vector2 size = new Vector2(40, 40);
            pos.x = square.x + (square.width / 2) - 20;
            pos.y = square.y + square.height - 88;
            Rect rect = new Rect(pos, size);
            GUI.DrawTexture(rect, Icon);

            GUIStyle tmpStyle = new GUIStyle();
            tmpStyle.border = new RectOffset(12, 12, 12, 12);
            tmpStyle.alignment = TextAnchor.UpperCenter;
            tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            pos = square.position;
            size = new Vector2(square.width, 40);
            pos.x = square.x;
            pos.y = square.y + square.height - 48;
            rect = new Rect(pos, size);
            GUI.Label(rect, Type, tmpStyle);

            pos = square.position;
            size = new Vector2(20, 20);
            size.x = square.width - 35;
            size.y = 18;
            pos.x = square.x + 20;
            pos.y = square.y + square.height - 35;
            rect = new Rect(pos, size);

            SO_Action tmp = EditorGUI.ObjectField(rect, "", m_action, typeof(SO_Action), true) as SO_Action;
            if (tmp != m_action)
            {
                m_action = tmp;
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        // Evaluates the node using the passed n delegate and reports the resulting
        // state as appropriate
        public override NodeStates Evaluate()
        {
            Type = m_action.name;
            switch (m_action.Action())
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
                default:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;
            }
        }

        public override void SetNodeIcon()
        {
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AndreLibrary/Editor/BT/LeafNode_Icon.png");
        }
    }
}
