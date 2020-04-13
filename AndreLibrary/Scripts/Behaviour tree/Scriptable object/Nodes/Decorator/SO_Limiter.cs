using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CreateAssetMenu(fileName = "New Node", menuName = "AI/Behaviour Tree/Decorator/SO_Limiter")]
    public class SO_Limiter : SO_Node
    {
        // Child node to evaluate
        [SerializeField]
        SO_Node m_node;
        [SerializeField]
        NodeStates m_StateToReturn = NodeStates.FAILURE;
        [SerializeField]
        int m_numTimesToEvaluate = 1;
        int m_numTimesHasEvaluated = 0;
        bool debug = false;

        public void OnEnable()
        {
            SwapableNodes = false;
            ResetNode(m_numTimesToEvaluate, m_StateToReturn);
        }

        public override void Initialize(GameObject obj)
        {
            foreach (var node in Children)
            {
                if (obj != null)
                    node.Initialize(obj);
            }
            ResetNode(m_numTimesToEvaluate, m_StateToReturn);
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

        // Reports a success if the child succeeds and a failure if the child fail.
        // Running reports as running
        // adds one to number of times evaluate, if it is greater or equal to the number of 
        // times asked to evaluate than just returns what you have chosen in contructor and 
        // doesn't evaluate child node
        public override NodeStates Evaluate()
        {
            if (m_numTimesHasEvaluated < m_numTimesToEvaluate)
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
                        //m_numTimesHasEvaluated++;
                        return m_nodeState;
                }
            }
            m_nodeState = m_StateToReturn;
            return m_nodeState;
        }

        protected override void DrawButtonGUI()
        {
            if (!debug)
            {
                square.height = 175;
                GUI.Box(square, nameNode, style);
            }
            else
            {
                square.height = 195;
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
                size = new Vector2(60, 60);
                pos.x = square.x + (square.width / 2) - 30;
                pos.y = square.y + square.height - 158;
                rect = new Rect(pos, size);
                GUI.DrawTexture(rect, Icon);

                tmpStyle.border = new RectOffset(12, 12, 12, 12);
                tmpStyle.alignment = TextAnchor.UpperCenter;
                tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                pos = square.position;
                size = new Vector2(square.width, 40);
                pos.x = square.x;
                pos.y = square.y + square.height - 103;
                rect = new Rect(pos, size);
                GUI.Label(rect, Type, tmpStyle);

                tmpStyle.border = new RectOffset(12, 12, 12, 12);
                tmpStyle.alignment = TextAnchor.UpperCenter;
                tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                pos = square.position;
                size = new Vector2(square.width, 40);
                pos.x = square.x;
                pos.y = square.y + square.height - 83;
                rect = new Rect(pos, size);
                GUI.Label(rect, "Total Cycles", tmpStyle);

                pos = square.position;
                size = new Vector2(20, 20);
                size.x = square.width - 35;
                size.y = 18;
                pos.x = square.x + 20;
                pos.y = square.y + square.height - 70;
                rect = new Rect(pos, size);

                int tmp = EditorGUI.IntField(rect, m_numTimesToEvaluate);
                if (tmp >= 0 && tmp != m_numTimesToEvaluate)
                {
                    m_numTimesToEvaluate = tmp;
                }
            }
            else
            {
                pos = square.position;
                size = new Vector2(60, 60);
                pos.x = square.x + (square.width / 2) - 30;
                pos.y = square.y + square.height - 178;
                rect = new Rect(pos, size);
                GUI.DrawTexture(rect, Icon);

                tmpStyle.border = new RectOffset(12, 12, 12, 12);
                tmpStyle.alignment = TextAnchor.UpperCenter;
                tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                pos = square.position;
                size = new Vector2(square.width, 40);
                pos.x = square.x;
                pos.y = square.y + square.height - 123;
                rect = new Rect(pos, size);
                GUI.Label(rect, Type, tmpStyle);

                tmpStyle.border = new RectOffset(12, 12, 12, 12);
                tmpStyle.alignment = TextAnchor.UpperCenter;
                tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                pos = square.position;
                size = new Vector2(square.width, 40);
                pos.x = square.x;
                pos.y = square.y + square.height - 103;
                rect = new Rect(pos, size);
                GUI.Label(rect, "Total Cycles", tmpStyle);

                pos = square.position;
                size = new Vector2(20, 20);
                size.x = square.width - 35;
                size.y = 18;
                pos.x = square.x + 20;
                pos.y = square.y + square.height - 90;
                rect = new Rect(pos, size);

                int tmp = EditorGUI.IntField(rect, m_numTimesToEvaluate);
                if (tmp >= 0 && tmp != m_numTimesToEvaluate)
                {
                    m_numTimesToEvaluate = tmp;
                }

                tmpStyle.border = new RectOffset(12, 12, 12, 12);
                tmpStyle.alignment = TextAnchor.UpperCenter;
                tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                pos = square.position;
                size = new Vector2(square.width, 40);
                pos.x = square.x;
                pos.y = square.y + square.height - 65;
                rect = new Rect(pos, size);
                GUI.Label(rect, $"Current Cycle: {m_numTimesHasEvaluated}", tmpStyle);
            }

            tmpStyle.border = new RectOffset(12, 12, 12, 12);
            tmpStyle.alignment = TextAnchor.UpperCenter;
            tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            pos = square.position;
            size = new Vector2(square.width, 40);
            pos.x = square.x;
            pos.y = square.y + square.height - 48;
            rect = new Rect(pos, size);
            GUI.Label(rect, "State At End", tmpStyle);

            pos = square.position;
            size = new Vector2(20, 20);
            size.x = square.width - 35;
            size.y = 18;
            pos.x = square.x + 20;
            pos.y = square.y + square.height - 35;
            rect = new Rect(pos, size);

            NodeStates tmpState = (NodeStates)EditorGUI.EnumPopup(rect, m_StateToReturn);
            if (tmpState != m_StateToReturn)
            {
                m_StateToReturn = tmpState;
            }
        }

        public override void SetNodeIcon()
        {
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AndreLibrary/Editor/BT/LimiterNode_Icon.png");
        }
    }
}