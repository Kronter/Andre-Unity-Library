using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CreateAssetMenu(fileName = "New Node", menuName = "AI/Behaviour Tree/Debug/SO_TestNode")]
    public class SO_TestNode : SO_Node
    {
        public string Message;
        public NodeStates NodeState;
        private static readonly GUIContent AddText = new GUIContent("Save Message", "Save Message");

        public void OnEnable()
        {
            SwapableNodes = false;
            NodeAdd = false;
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
            square.height = 172;
            GUI.Box(square, nameNode, style);
            Vector2 pos = square.position;
            Vector2 size = new Vector2(40, 40);
            pos.x = square.x + (square.width / 2) - 15;
            pos.y = square.y + square.height - 140;
            Rect rect = new Rect(pos, size);
            GUI.DrawTexture(rect, Icon);

            GUIStyle tmpStyle = new GUIStyle();
            tmpStyle.border = new RectOffset(12, 12, 12, 12);
            tmpStyle.alignment = TextAnchor.UpperCenter;
            tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            pos = square.position;
            size = new Vector2(square.width, 40);
            pos.x = square.x;
            pos.y = square.y + square.height - 98;
            rect = new Rect(pos, size);
            GUI.Label(rect, Type, tmpStyle);

            pos = square.position;
            size = new Vector2(20, 20);
            size.x = square.width - 40;
            size.y = 50;
            pos.x = square.x + 20;
            pos.y = square.y + square.height - 84;
            rect = new Rect(pos, size);

            Message = GUI.TextArea(rect, Message);
            rect.height = 18;
            rect.y = square.y + square.height - 32;
            if (GUI.Button(rect, AddText, EditorStyles.miniButton))
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        // If any of thechildren reports a success, the selectore will 
        // immediately report a success upwards. If all children fail,
        // it will report a failure instead
        public override NodeStates Evaluate()
        {
            m_nodeState = NodeState;
            if (m_nodeState == NodeStates.SUCCESS)
                Debug.Log(Message);
            return m_nodeState;
        }

        public override void Initialize(GameObject obj)
        {
        }

        public override void SetNodeIcon()
        {
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AndreLibrary/Editor/BT/DebugNode_Icon.png");
        }
    }
}