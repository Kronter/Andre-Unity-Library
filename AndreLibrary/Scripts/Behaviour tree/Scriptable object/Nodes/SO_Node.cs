using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Andre.AI.BehaviourTree.Scriptable
{
    public abstract class SO_Node : ScriptableObject
    {
        // editor
        [HideInInspector]
        public GUIStyle style;
        [HideInInspector]
        public GUIStyle defaultNodeStyle;
        [HideInInspector]
        public GUIStyle selectedNodeStyle;
        [HideInInspector]
        public Rect square;
        [HideInInspector]
        public string nameNode;
        [HideInInspector]
        public string NodeStyleName;
        [HideInInspector]
        public bool isSelected;
        [HideInInspector]
        public bool SwapableNodes = true;
        [HideInInspector]
        public bool NodeAdd = true;
        [HideInInspector]
        public string Type = "Base";
        [HideInInspector]
        public int nodeType;
        [HideInInspector]
        public SO_Node parent;
        [HideInInspector]
        public SO_BehaviourTree btParent;
        protected Texture2D Icon;
        //  editor

        [HideInInspector]
        public List<SO_Node> Children = new List<SO_Node>();

        [HideInInspector]
        // The current state of the node
        public NodeStates m_nodeState = NodeStates.OFF;

        public NodeStates nodesState { get { return m_nodeState; } }

        // Implementing classes use this method to evaluate the desired set of conditions
        public abstract NodeStates Evaluate();

        public abstract bool ContainsName(string Node);

        public abstract void MoveNodePositions(string NodeA, string NodeB);

        public virtual void AddNode(SO_Node node)
        {
            SetNodeStyle(node);
        }

        public virtual void OnBTDisable()
        {
            m_nodeState = NodeStates.OFF;
            foreach (var node in Children)
            {
                node.OnBTDisable();
            }
        }

        public virtual void SetNodeStyle(SO_Node node)
        {
            if (node.nodeType == 0)
            {
                node.square = new Rect(40, 40, 150, 80);
            }
            else
            {
                node.parent = this;
                node.btParent = btParent;
                node.square = new Rect(this.square.x, this.square.y - -80, 150, 80);
            }

            node.defaultNodeStyle = new GUIStyle();
            node.NodeStyleName = GetNodeTypeStyle(node, false);
            node.defaultNodeStyle.normal.background = EditorGUIUtility.Load($"builtin skins/darkskin/images/{ GetNodeTypeStyle(node, false)}.png") as Texture2D;
            node.defaultNodeStyle.border = new RectOffset(12, 12, 12, 12);
            node.defaultNodeStyle.alignment = TextAnchor.UpperCenter;
            node.defaultNodeStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            node.style = node.defaultNodeStyle;

            node.selectedNodeStyle = new GUIStyle();
            node.selectedNodeStyle.normal.background = EditorGUIUtility.Load($"builtin skins/darkskin/images/{ GetNodeTypeStyle(node, true)}.png") as Texture2D;
            node.selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
            node.selectedNodeStyle.alignment = TextAnchor.UpperCenter;
            node.selectedNodeStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);

            //node.Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AndreLibrary/Editor/DebugNode_Icon.png");
            node.SetNodeIcon();

            node.nameNode = "\n" + node.name; //+ "\n" + node.Type;
        }

        public virtual string GetNodeTypeStyle(SO_Node node, bool On)
        {
            switch (node.nodeType)
            {
                case 0:
                    //root
                    return On ? "node1 on" : "node1";
                case 1:
                    //composite
                    return On ? "node3 on" : "node3";
                case 2:
                    //decorator
                    return On ? "node5 on" : "node5";
                case 3:
                    //leaf
                    return On ? "node2 on" : "node2";
                case 4:
                    //debug
                    return On ? "node6 on" : "node6";
            }
            return On ? "node1 on" : "node1";
        }

        public abstract void SetNodeIcon();

        public abstract void RemoveNode(string Node);

        public abstract void Initialize(GameObject obj);

        public abstract void DestroyNode(Object obj, bool allowDestroyingAssets = false);

        public T Create<T>(string name)
      where T : SO_Node
        {
            T node = CreateInstance<T>();
            node.name = name;
            return node;
        }

        public bool ListContainsByName(string name, List<SO_Node> List)
        {
            foreach (var item in List)
            {
                if (item.name == name)
                    return true;
            }
            return false;
        }

        public int GetInListByName(string name, List<SO_Node> List)
        {
            foreach (var item in List)
            {
                if (item.name == name)
                    return List.IndexOf(item);
            }
            return 0;
        }

        public void Drag(Vector2 delta)
        {
            square.position += delta;
        }

        public void DragWithChildren(Vector2 delta)
        {
            square.position += delta;
            foreach (var node in Children)
            {
                node.DragWithChildren(delta);
            }
        }

        public void SetNewPos(Vector2 delta)
        {
            Vector2 ParentDifDelta = delta - square.position;
            square.position = delta;
            foreach (var node in Children)
            {
                Vector2 newPos = ParentDifDelta + node.square.position;
                node.SetNewPos(newPos);
            }
        }

        public SO_Node ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        if (square.Contains(e.mousePosition))
                        {
                            GUI.changed = true;
                            isSelected = false;
                            style = selectedNodeStyle;
                            return this;
                        }
                        else
                        {
                            GUI.changed = true;
                            isSelected = false;
                            style = defaultNodeStyle;
                        }
                    }
                    else if (e.button == 0)
                    {
                        if (square.Contains(e.mousePosition))
                        {
                            GUI.changed = true;
                            isSelected = true;
                            style = selectedNodeStyle;
                        }
                        else
                        {
                            GUI.changed = true;
                            isSelected = false;
                            style = defaultNodeStyle;
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isSelected)
                    {
                        if (e.shift)
                            DragWithChildren(e.delta);
                        else
                            Drag(e.delta);

                        e.Use();
                    }
                    break;
            }

            if (isSelected)
                return this;
            return null;
        }

        public void ChangeName(string assetName)
        {
            name = assetName;
            nameNode = "\n" + name;
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this), ImportAssetOptions.ForceUpdate);
        }

        public void RemoveSelf()
        {
            if (parent == null)
                return;
            RemoveNode(name);
        }

        public void DrawInEditor(Event e, int i = 0)
        {
            if(Icon == null)
            {
                SetNodeIcon();
            }
            DrawConnections();
            DrawButtonGUI();
            if (i > 0)
            {
                GUIStyle tmpStyle = new GUIStyle();
                tmpStyle.alignment = TextAnchor.UpperCenter;
                tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
                Vector2 pos = square.position;
                Vector2 size = new Vector2(20, 20);
                size.x = square.width;
                size.y = 18;
                pos.x = (square.x - (square.width / 2)) + 20;
                pos.y = (square.y + square.height) - square.height + 10;
                Rect rect = new Rect(pos, size);
                GUI.Label(rect, i.ToString(), tmpStyle);
            }
            int z = 1;
            foreach (var node in Children)
            {
                node.DrawInEditor(e, z);
                z++;
            }
           
        }

        public SO_Node ProcessNodeEvents(Event e)
        {
            SO_Node UpdateAsset = null;
            Children.Reverse();
            foreach (var node in Children)
            {
                SO_Node tmp = node.ProcessNodeEvents(e);
                if (tmp != null)
                    UpdateAsset = tmp;
            }
            Children.Reverse();
            if (UpdateAsset == null)
                UpdateAsset = ProcessEvents(e);
            return UpdateAsset;
        }

        protected virtual void DrawButtonGUI()
        {
            GUIStyle tmpStyle = new GUIStyle();
            tmpStyle.border = new RectOffset(12, 12, 12, 12);
            tmpStyle.alignment = TextAnchor.UpperCenter;
            tmpStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
           Vector2 pos = square.position;
           Vector2 size = new Vector2(square.width, 40);
            pos.x = square.x;
            pos.y = square.y + square.height - 25;
            Rect rect = new Rect(pos, size);
            GUI.Label(rect, Type, tmpStyle);
        }

        protected void OnDisable()
        {
            m_nodeState = NodeStates.OFF;
        }

        public List<SO_Node> GetChildren()
        {
            List<SO_Node> tmp = new List<SO_Node>();
            foreach (var node in Children)
            {
                tmp.Add(node);
                foreach (var Cnode in node.Children)
                    tmp.Add(Cnode);
            }

            return tmp;
        }

        public void DrawConnections()
        {
            foreach (var node in Children)
            {
                Color color;
                if (node.m_nodeState == NodeStates.SUCCESS)
                    color = Color.green;
                else if (node.m_nodeState == NodeStates.FAILURE)
                    color = Color.red;
                else if (node.m_nodeState == NodeStates.RUNNING)
                    color = Color.yellow;
                else
                    color = Color.white;

                Handles.DrawBezier(square.center, node.square.center, square.center, node.square.center, color, null, 4f);

                node.DrawConnections();
            }
        }
    }
}