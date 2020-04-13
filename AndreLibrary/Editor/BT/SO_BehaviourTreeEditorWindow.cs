using UnityEditor;
using UnityEngine;

namespace Andre.AI.BehaviourTree.Scriptable
{
    public class SO_BehaviourTreeEditorWindow : EditorWindow
    {
        private SO_BehaviourTree BehaviourTree = null;
        private static SO_BehaviourTreeEditorWindow window;
        private Vector2 drag;
        private Vector2 offset;
        private SO_Node AssetToUpdate;
        private SO_Node SideMenuAssetToUpdate;
        Rect sideMenuSquare;
        private static Texture tex;
        private static Texture2D sidemenuTex;
        private static Texture2D sidemenuNodeTex;
        bool InContexMenu = false;

        private const float kZoomMin = 0.1f;
        private const float kZoomMax = 10.0f;

        private Rect _zoomArea;
        private float _zoom = 1.0f;
        private Vector2 _zoomCoordsOrigin = Vector2.zero;

        private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
        {
            return (screenCoords - _zoomArea.TopLeft()) / _zoom + _zoomCoordsOrigin;
        }

        [MenuItem("Window/AI/Behaviour Tree Editor")]
        public static SO_BehaviourTreeEditorWindow OpenBehaviourTreeEditorWindow(SO_BehaviourTree bt)
        {
            window = EditorWindow.GetWindow<SO_BehaviourTreeEditorWindow>("Behaviour Tree");
            Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AndreLibrary/Editor/BT_Icon.png");
            GUIContent titleContent = new GUIContent("Behaviour Tree", icon);
            window.titleContent = titleContent;
            tex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AndreLibrary/Editor/BT_Lighter_Black.jpg");
            sidemenuNodeTex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AndreLibrary/Editor/BT/BT_Light_Blue.jpg") as Texture2D;
            sidemenuTex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AndreLibrary/Editor/BT/BT_Blue_Grey.jpg") as Texture2D;
            return window;
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenBehaviourTree(int instanceID, int line)
        {
            SO_BehaviourTree Bt = EditorUtility.InstanceIDToObject(instanceID) as SO_BehaviourTree;

            if (Bt != null)
            {
                SO_BehaviourTreeEditorWindow editorWindow = OpenBehaviourTreeEditorWindow(Bt);
                editorWindow.BehaviourTree = Bt;
                editorWindow.SideMenuAssetToUpdate = Bt.root;
                return true;
            }
            return false;
        }

        private void OnGUI()
        {

            if (tex == null)
            {
                tex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AndreLibrary/Editor/BT/BT_Lighter_Black.jpg");
            }

            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);

            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            if (window != null)
                _zoomArea = new Rect(0, 0, window.position.width, window.position.height);
            if (BehaviourTree != null)
            {
                if (BehaviourTree.root != null)
                {
                    EditorZoomArea.Begin(_zoom, _zoomArea);
                    BehaviourTree.root.DrawInEditor(Event.current);
                    AssetToUpdate = BehaviourTree.root.ProcessNodeEvents(Event.current);
                    EditorZoomArea.End();
                }
                HelperWindow();
                ProcessEvents(Event.current);
                Repaint();
            }
        }

        private void OnSelectionChange()
        {
            if (window != null)
            {
                if (Selection.activeObject is SO_BehaviourTree)
                {
                    OnOpenBehaviourTree(Selection.instanceIDs[0], 0);
                }
            }
        }

        private void Update()
        {
            Repaint();
        }

        int tab = 0;
        float NumberOfChildren = 0;
        Vector2 scrollPos;
        private void HelperWindow()
        {
            if (window == null)
                return;

            if (AssetToUpdate != null)
                SideMenuAssetToUpdate = AssetToUpdate;
            if (SideMenuAssetToUpdate == null && BehaviourTree.root != null)
                SideMenuAssetToUpdate = BehaviourTree.root;

            GUIStyle defaultNodeStyle = new GUIStyle();
            defaultNodeStyle.normal.background = sidemenuTex;
            defaultNodeStyle.border = new RectOffset(0, 0, 0, 0);
            defaultNodeStyle.alignment = TextAnchor.UpperCenter;
            defaultNodeStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            float windowWidth = window.position.width * 0.2f;
            if (windowWidth < 200)
                windowWidth = 200;
            sideMenuSquare = new Rect(window.position.width - (windowWidth), 0, windowWidth, window.position.height);
            Vector2 pos = sideMenuSquare.position;
            float heightOffset = 20;
            GUI.Box(sideMenuSquare, "", defaultNodeStyle);
            defaultNodeStyle.normal.background = null;

            Vector2 size = new Vector2(windowWidth, sideMenuSquare.height +10);
            pos.x = window.position.width - (windowWidth);
            pos.y = sideMenuSquare.y + sideMenuSquare.height - (sideMenuSquare.height - heightOffset +20);
            Rect rect = new Rect(pos, size);
            NumberOfChildren = SideMenuAssetToUpdate.Children.Count - 1;
            float scrollheight = (8 * 60) + 150;
            if (scrollheight <= (8* 60))
                scrollheight= (NumberOfChildren * 60) + 265;
            Vector2 viewSize = new Vector2(windowWidth, scrollheight);
            Rect viewRect = new Rect(pos, viewSize);
            scrollPos = GUI.BeginScrollView(rect, scrollPos, viewRect, false, true);

            if (BehaviourTree.root == null)
            {
                GUI.Label(sideMenuSquare, "\nNo Nodes In Tree", defaultNodeStyle);
                SideMenuAddRoot(sideMenuSquare, pos, defaultNodeStyle, heightOffset);
                GUI.EndScrollView();
                return;
            }
            GUI.Label(sideMenuSquare, "\n" + SideMenuAssetToUpdate.name, defaultNodeStyle);
            if (SideMenuAssetToUpdate.nodeType == (int)NODETYPE.Leaf || SideMenuAssetToUpdate.nodeType == (int)NODETYPE.Debug)
            {
                heightOffset = heightOffset + 20;
                size = new Vector2(140, 20);
                pos.x = sideMenuSquare.x + (sideMenuSquare.width / 2) - 70;
                pos.y = sideMenuSquare.y + sideMenuSquare.height - (sideMenuSquare.height - heightOffset);
                rect = new Rect(pos, size);
                tab = GUI.Toolbar(rect, tab, new string[] { "Current", "        " });
                tab = 0;
                if (tab == 0)
                {
                    heightOffset = SideMenuChangeName(sideMenuSquare, pos, defaultNodeStyle, heightOffset);
                    SideMenuDeleteNode(sideMenuSquare, pos, defaultNodeStyle, heightOffset);
                }
            }
            else
            {
                heightOffset = heightOffset + 20;
                size = new Vector2(140, 20);
                pos.x = sideMenuSquare.x + (sideMenuSquare.width / 2) - 70;
                pos.y = sideMenuSquare.y + sideMenuSquare.height - (sideMenuSquare.height - heightOffset);
                rect = new Rect(pos, size);
                tab = GUI.Toolbar(rect, tab, new string[] { "Current", "Children"});
                if (tab == 0)
                {
                    heightOffset = SideMenuChangeName(sideMenuSquare, pos, defaultNodeStyle, heightOffset);
                    SideMenuDeleteNode(sideMenuSquare, pos, defaultNodeStyle, heightOffset);
                }
                if (tab ==1)
                {
                    heightOffset = ShowChildren(sideMenuSquare, pos, defaultNodeStyle, heightOffset);
                    heightOffset = SideMenuAddNode(sideMenuSquare, pos, defaultNodeStyle, heightOffset);
                }
            }
            GUI.EndScrollView();
        }

        ROOTNODEOPTIONS sideMenuRootOptions;
        private void SideMenuAddRoot(Rect square, Vector2 pos, GUIStyle defaultNodeStyle, float offset)
        {
            float heightOffset = offset + 20;
            Vector2 size = new Vector2(140, 20);
            pos.x = square.x + (square.width / 2) - 70;
            pos.y = square.y + square.height - (square.height - heightOffset);
            Rect rect = new Rect(pos, size);

            sideMenuRootOptions = (ROOTNODEOPTIONS)EditorGUI.EnumPopup(rect, sideMenuRootOptions);

            heightOffset = heightOffset + 20;
            size = new Vector2(140, 20);
            pos.x = square.x + (square.width / 2) - 70;
            pos.y = square.y + square.height - (square.height - heightOffset);
            rect = new Rect(pos, size);

            if (GUI.Button(rect, "Add Root Node"))
            {
                OnClickAddRoot(new Vector2(window.position.width / 2, window.position.height / 2), (NODEOPTIONS)sideMenuRootOptions);
            }
        }

        SIDEMENUNODEOPTIONS sideMenuNodeOptions;
        private float SideMenuAddNode(Rect square, Vector2 pos, GUIStyle defaultNodeStyle, float offset)
        {
            if (!CheckCanAdd(SideMenuAssetToUpdate))
                return 0;

            float heightOffset = offset + 30;
            Vector2 size = new Vector2(140, 20);
            pos.x = square.x + (square.width / 2) - 70;
            pos.y = square.y + square.height - (square.height - heightOffset);
            Rect rect = new Rect(pos, size);

            sideMenuNodeOptions = (SIDEMENUNODEOPTIONS)EditorGUI.EnumPopup(rect, sideMenuNodeOptions);

            heightOffset = heightOffset + 20;
            size = new Vector2(140, 20);
            pos.x = square.x + (square.width / 2) - 70;
            pos.y = square.y + square.height - (square.height - heightOffset);
            rect = new Rect(pos, size);

            if (GUI.Button(rect, "+"))
            {
                OnClickAddNode((NODEOPTIONS)sideMenuNodeOptions, SideMenuAssetToUpdate);
            }

            return heightOffset;
        }

        Vector2 scrollPosChildren;
        private float ShowChildren (Rect square, Vector2 pos, GUIStyle defaultNodeStyle, float offset)
        {
            float tmp = offset;
            int index = 0;
            if (NumberOfChildren >= 8)
            {
                Vector2 size = new Vector2(160, 8 * 60);
                pos.x = square.x + (square.width / 2) - 70; 
                pos.y = sideMenuSquare.y + sideMenuSquare.height - (sideMenuSquare.height - offset - 30);
                Rect rect = new Rect(pos, size);
                float scrollheight = NumberOfChildren * 60;
                Vector2 viewSize = new Vector2(120, scrollheight);
                Rect viewRect = new Rect(pos, viewSize);
                scrollPosChildren = GUI.BeginScrollView(rect, scrollPosChildren, viewRect, false, true);
            }
            for (int i = 0; i < SideMenuAssetToUpdate.Children.Count; i++)
            {
                tmp = ShowChild(square, pos, defaultNodeStyle, tmp, SideMenuAssetToUpdate.Children[i].name, index);
                index++;
            }
            if (NumberOfChildren >= 8)
            {
                GUI.EndScrollView();
                return (8 * 60) + 40;
            }
            return  tmp;
        }

        private float ShowChild(Rect square, Vector2 pos, GUIStyle defaultNodeStyle, float offset, string name, int index)
        {
            float temp = offset;
            temp = SideMenuMoveNodeUp(sideMenuSquare, pos, defaultNodeStyle, temp, index);
            Vector2 size = new Vector2(120, 45);
            pos.x = square.x + (square.width / 2) - 70;
            pos.y = square.y + square.height - (square.height - temp);
            Rect rect = new Rect(pos, size);
            GUI.Box(rect, sidemenuNodeTex);
            defaultNodeStyle.normal.textColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            GUI.Label(rect, name, defaultNodeStyle);
            defaultNodeStyle.normal.textColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            temp = SideMenuMoveNodeDown(sideMenuSquare, pos, defaultNodeStyle, temp, index);
            return SideMenuRemoveNode(sideMenuSquare, pos, defaultNodeStyle, temp,name);
        }

        private float SideMenuRemoveNode(Rect square, Vector2 pos, GUIStyle defaultNodeStyle, float offset, string name)
        {
            float heightOffset = offset + 15;
            Vector2 size = new Vector2(15, 15);
            pos.x = square.x + (square.width / 2) + 55;
            pos.y = square.y + square.height - (square.height - heightOffset);
            Rect rect = new Rect(pos, size);
            if (GUI.Button(rect, "-", EditorStyles.miniButton))
            {
                SideMenuAssetToUpdate.RemoveNode(name);
            }
            return heightOffset;
        }

        private float SideMenuMoveNodeUp(Rect square, Vector2 pos, GUIStyle defaultNodeStyle, float offset, int index)
        {
            float heightOffset = offset + 30;
            Vector2 size = new Vector2(15, 15);
            pos.x = square.x + (square.width / 2) + 55;
            pos.y = square.y + square.height - (square.height - heightOffset);
            Rect rect = new Rect(pos, size);

            if (index == 0)
            {
                GUI.Button(rect, "", EditorStyles.miniButton);
                return heightOffset;
            }

            if (GUI.Button(rect, "\u21D1", EditorStyles.miniButton))
            {
                SideMenuAssetToUpdate.MoveNodePositions(SideMenuAssetToUpdate.Children[index].name, SideMenuAssetToUpdate.Children[index -1 ].name);
            }
            return heightOffset;
        }

        private float SideMenuMoveNodeDown(Rect square, Vector2 pos, GUIStyle defaultNodeStyle, float offset, int index)
        {
            float heightOffset = offset + 15;
            Vector2 size = new Vector2(15, 15);
            pos.x = square.x + (square.width / 2) + 55;
            pos.y = square.y + square.height - (square.height - heightOffset);
            Rect rect = new Rect(pos, size);
            if (index >= SideMenuAssetToUpdate.Children.Count -1)
            {
                GUI.Button(rect, "", EditorStyles.miniButton);
                return heightOffset;
            }
            if (GUI.Button(rect, "\u21D3", EditorStyles.miniButton))
            {
                SideMenuAssetToUpdate.MoveNodePositions(SideMenuAssetToUpdate.Children[index].name, SideMenuAssetToUpdate.Children[index + 1].name);
            }
            return heightOffset;
        }


        private float SideMenuDeleteNode(Rect square, Vector2 pos, GUIStyle defaultNodeStyle, float offset)
        {
            float heightOffset = offset + 30;
            Vector2 size = new Vector2(140, 20);
            pos.x = square.x + (square.width / 2) - 70;
            pos.y = square.y + square.height - (square.height - heightOffset);
            Rect rect = new Rect(pos, size);
            if (GUI.Button(rect, "Delete Node"))
            {
                if(SideMenuAssetToUpdate == BehaviourTree.root)
                    BehaviourTree.RemoveNode(BehaviourTree.root.name);
                else
                    SideMenuAssetToUpdate.parent.RemoveNode(SideMenuAssetToUpdate.name);

                SideMenuAssetToUpdate = null;
                RefreshAsset();
            }
            return 0;
        }

        string sideMenuAssetName = null;
        private float SideMenuChangeName(Rect square, Vector2 pos, GUIStyle defaultNodeStyle, float offset)
        {
            // lable
            float heightOffset = offset + 30;
            Vector2 size = new Vector2(60, 30);
            pos.x = square.x + (square.width / 2) - 30;
            pos.y = square.y + square.height - (square.height - heightOffset);
            Rect rect = new Rect(pos, size);
            GUI.Label(rect, "Change Node Name", defaultNodeStyle);
            // name texture field
            heightOffset = heightOffset + 20;
            size = new Vector2(140, 20);
            pos.x = square.x + (square.width / 2) - 70;
            pos.y = square.y + square.height - (square.height - heightOffset);
            rect = new Rect(pos, size);
            sideMenuAssetName = GUI.TextField(rect, sideMenuAssetName);

            // button
            heightOffset = heightOffset + 25;
            size = new Vector2(140, 20);
            pos.x = square.x + (square.width / 2) - 70;
            pos.y = square.y + square.height - (square.height - heightOffset);
            rect = new Rect(pos, size);
            if (GUI.Button(rect, "Change Name"))
            {
                if (!string.IsNullOrEmpty(sideMenuAssetName) && !string.IsNullOrWhiteSpace(sideMenuAssetName)
               && sideMenuAssetName != SideMenuAssetToUpdate.name)
                {
                    if (SideMenuAssetToUpdate.parent == null)
                    {
                        if (!SideMenuAssetToUpdate.btParent.ContainsName(sideMenuAssetName))
                        {
                            SideMenuAssetToUpdate.ChangeName(sideMenuAssetName);
                            sideMenuAssetName = "";
                        }
                    }
                    else
                    {
                        if (!SideMenuAssetToUpdate.parent.ContainsName(sideMenuAssetName))
                        {
                            SideMenuAssetToUpdate.ChangeName(sideMenuAssetName);
                            sideMenuAssetName = "";
                        }
                    }
                }
            }

            return heightOffset;
        }

        private void RefreshAsset()
        {
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(BehaviourTree), ImportAssetOptions.ForceUpdate);
        }

        private void ProcessEvents(Event e)
        {
            drag = Vector2.zero;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        ProcessContextMenu(e.mousePosition);
                    }
                    break;
                case EventType.MouseDrag:
                    // drag around world/ canvas
                    if (e.button == 0 /*&& e.control*/ && AssetToUpdate == null)
                    {
                        OnDrag(e.delta);
                        InContexMenu = false;
                    }
                    break;
                case EventType.KeyDown:
                    if (e.control && AssetToUpdate != null)
                    {
                        if (e.keyCode == KeyCode.R)
                        {
                            OnClickRemoveNode();
                        }

                        if (e.keyCode == KeyCode.N)
                        {
                            OnClickRenameNode(e.mousePosition);
                        }

                        if (e.keyCode == KeyCode.A)
                        {
                            GenericMenu genericMenu = new GenericMenu();
                            AddNodeOptions(genericMenu, e.mousePosition);
                            genericMenu.ShowAsContext();
                        }

                        if (e.keyCode == KeyCode.S)
                        {
                            if (AssetToUpdate.SwapableNodes && AssetToUpdate.Children.Count > 1)
                            {
                                OnClickSwapNodes(e.mousePosition);
                            }
                        }
                    }
                    break;
                case EventType.ScrollWheel:
                    Vector2 screenCoordsMousePos = Event.current.mousePosition;
                    Vector2 delta = Event.current.delta;
                    Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(screenCoordsMousePos);
                    float zoomDelta = -delta.y / 150.0f;
                    float oldZoom = _zoom;
                    _zoom += zoomDelta;
                    _zoom = Mathf.Clamp(_zoom, kZoomMin, kZoomMax);
                    _zoomCoordsOrigin += (zoomCoordsMousePos - _zoomCoordsOrigin) - (oldZoom / _zoom) * (zoomCoordsMousePos - _zoomCoordsOrigin);

                    Event.current.Use();
                    break;
            }
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();

            if (BehaviourTree.root == null)
            {
                AddRootNodeOptions(genericMenu, mousePosition);
                InContexMenu = true;
            }
            else
            {
                if (AssetToUpdate == null)
                {
                    genericMenu.AddItem(new GUIContent("Remove Root Node"), false, () => OnClickRemoveRootNode());
                    InContexMenu = true;
                }
                else
                {
                    AddNodeOptions(genericMenu, mousePosition);

                    genericMenu.AddItem(new GUIContent("Remove Node"), false, () => OnClickRemoveNode());

                    genericMenu.AddItem(new GUIContent("Rename Node"), false, () => OnClickRenameNode(mousePosition));

                    if (AssetToUpdate.SwapableNodes && AssetToUpdate.Children.Count > 1)
                    {
                        genericMenu.AddItem(new GUIContent("Swap Children Node"), false, () => OnClickSwapNodes(mousePosition));
                    }
                    InContexMenu = true;
                }
            }

            genericMenu.ShowAsContext();
        }

        private void AddRootNodeOptions(GenericMenu genericMenu, Vector2 mousePosition)
        {
            genericMenu.AddItem(new GUIContent("Add Root Node / Sequence"), false, () => OnClickAddRoot(mousePosition, NODEOPTIONS.Sequence));
            genericMenu.AddItem(new GUIContent("Add Root Node / Selector"), false, () => OnClickAddRoot(mousePosition, NODEOPTIONS.Selector));
            genericMenu.AddItem(new GUIContent("Add Root Node / Boolean Selector"), false, () => OnClickAddRoot(mousePosition, NODEOPTIONS.BooleanSelector));
        }

        private void AddNodeOptions(GenericMenu genericMenu, Vector2 mousePosition)
        {
            if (CheckCanAdd(AssetToUpdate))
            {
                ////composites
                genericMenu.AddItem(new GUIContent("Add Node /  Composite / Sequence"), false, () => OnClickAddNode(NODEOPTIONS.Sequence, AssetToUpdate));
                genericMenu.AddItem(new GUIContent("Add Node /  Composite / Selector"), false, () => OnClickAddNode(NODEOPTIONS.Selector, AssetToUpdate));
                genericMenu.AddItem(new GUIContent("Add Node /  Composite / Boolean Selector"), false, () => OnClickAddNode(NODEOPTIONS.BooleanSelector, AssetToUpdate));

                //// selectors
                genericMenu.AddItem(new GUIContent("Add Node /  Decorator / Fail Repeater"), false, () => OnClickAddNode(NODEOPTIONS.FailRepeater, AssetToUpdate));
                genericMenu.AddItem(new GUIContent("Add Node /  Decorator / Fake Sate"), false, () => OnClickAddNode(NODEOPTIONS.FakeSate, AssetToUpdate));
                genericMenu.AddItem(new GUIContent("Add Node /  Decorator / Infinite Repeater"), false, () => OnClickAddNode(NODEOPTIONS.InfiniteRepeater, AssetToUpdate));
                genericMenu.AddItem(new GUIContent("Add Node /  Decorator / Inverter"), false, () => OnClickAddNode(NODEOPTIONS.Inverter, AssetToUpdate));
                genericMenu.AddItem(new GUIContent("Add Node /  Decorator / Limiter"), false, () => OnClickAddNode(NODEOPTIONS.Limiter, AssetToUpdate));
                genericMenu.AddItem(new GUIContent("Add Node /  Decorator / Repeater"), false, () => OnClickAddNode(NODEOPTIONS.Repeater, AssetToUpdate));

                //// leaf
                genericMenu.AddItem(new GUIContent("Add Node /  Leaf"), false, () => OnClickAddNode(NODEOPTIONS.Leaf, AssetToUpdate));

                //// Debug
                genericMenu.AddItem(new GUIContent("Add Node /  Debug"), false, () => OnClickAddNode(NODEOPTIONS.Debug, AssetToUpdate));
            }
        }

        private bool CheckCanAdd(SO_Node _AssetToUpdate)
        {
            if (_AssetToUpdate.nodeType == (int)NODETYPE.Leaf || _AssetToUpdate.nodeType == (int)NODETYPE.Debug)
            {
                return false;
            }

            if (_AssetToUpdate.nodeType == (int)NODETYPE.Decorator && _AssetToUpdate.Children.Count == 1)
            {
                return false;
            }

            if (_AssetToUpdate.Type == "BSelector" && _AssetToUpdate.Children.Count == 3)
            {
                return false;
            }

            return true;
        }

        private void OnClickRenameNode(Vector2 mousePosition)
        {
            Rect rect = new Rect(GUIUtility.GUIToScreenPoint(mousePosition), new Vector2(200, 85));
            SO_RenamePopUpWindow.OpenEditorWindow(AssetToUpdate, rect);
            InContexMenu = false;
        }

        private void OnClickSwapNodes(Vector2 mousePosition)
        {
            Rect rect = new Rect(GUIUtility.GUIToScreenPoint(mousePosition), new Vector2(200, 155));
            SO_SwapNodePopUpWindow.OpenEditorWindow(AssetToUpdate, rect);
            InContexMenu = false;
        }

        private void OnClickRemoveRootNode()
        {
            BehaviourTree.RemoveNode(BehaviourTree.root.name);
            RefreshAsset();
            InContexMenu = false;
        }

        private void OnClickRemoveNode()
        {
            if (AssetToUpdate == BehaviourTree.root)
            {
                OnClickRemoveRootNode();
            }
            else
            {
                AssetToUpdate.parent.RemoveNode(AssetToUpdate.name);
                RefreshAsset();
            }
            InContexMenu = false;
        }

        private void OnClickAddRoot(Vector2 mousePosition, NODEOPTIONS options)
        {
            SO_Node node = null;
            switch (options)
            {
                case NODEOPTIONS.Selector:
                    node = BehaviourTree.Create<SO_Selector>(name);
                    node.Type = "Selector";
                    node.nodeType = (int)NODETYPE.Root;
                    break;
                case NODEOPTIONS.Sequence:
                    node = BehaviourTree.Create<SO_Sequence>(name);
                    node.Type = "Sequence";
                    node.nodeType = (int)NODETYPE.Root;
                    break;
                case NODEOPTIONS.BooleanSelector:
                    node = BehaviourTree.Create<SO_BooleanSelector>(name);
                    node.Type = "BSelector";
                    node.nodeType = (int)NODETYPE.Root;
                    break;
            }
            node.name = "Root";
            node.btParent = BehaviourTree;
            BehaviourTree.AddNode(node, mousePosition);
            InContexMenu = false;
        }

        private void OnClickAddNode(NODEOPTIONS options, SO_Node _AssetToUpdate)
        {
            SO_Node node = null;
            switch (options)
            {
                case NODEOPTIONS.Selector:
                    node = _AssetToUpdate.Create<SO_Selector>(name);
                    node.Type = "Selector";
                    node.nodeType = (int)NODETYPE.Composite;
                    break;
                case NODEOPTIONS.Sequence:
                    node = _AssetToUpdate.Create<SO_Sequence>(name);
                    node.Type = "Sequence";
                    node.nodeType = (int)NODETYPE.Composite;
                    break;
                case NODEOPTIONS.BooleanSelector:
                    node = _AssetToUpdate.Create<SO_BooleanSelector>(name);
                    node.Type = "BSelector";
                    node.nodeType = (int)NODETYPE.Composite;
                    break;
                case NODEOPTIONS.FailRepeater:
                    node = _AssetToUpdate.Create<SO_FailRepeater>(name);
                    node.SwapableNodes = false;
                    node.Type = "FRepeater";
                    node.nodeType = (int)NODETYPE.Decorator;
                    break;
                case NODEOPTIONS.FakeSate:
                    node = _AssetToUpdate.Create<SO_FakeSate>(name);
                    node.SwapableNodes = false;
                    node.Type = "FakeSate";
                    node.nodeType = (int)NODETYPE.Decorator;
                    break;
                case NODEOPTIONS.InfiniteRepeater:
                    node = _AssetToUpdate.Create<SO_InfiniteRepeater>(name);
                    node.SwapableNodes = false;
                    node.Type = "IRepeater";
                    node.nodeType = (int)NODETYPE.Decorator;
                    break;
                case NODEOPTIONS.Inverter:
                    node = _AssetToUpdate.Create<SO_Inverter>(name);
                    node.SwapableNodes = false;
                    node.Type = "Inverter";
                    node.nodeType = (int)NODETYPE.Decorator;
                    break;
                case NODEOPTIONS.Limiter:
                    node = _AssetToUpdate.Create<SO_Limiter>(name);
                    node.SwapableNodes = false;
                    node.Type = "Limiter";
                    node.nodeType = (int)NODETYPE.Decorator;
                    break;
                case NODEOPTIONS.Repeater:
                    node = _AssetToUpdate.Create<SO_Repeater>(name);
                    node.SwapableNodes = false;
                    node.Type = "Repeater";
                    node.nodeType = (int)NODETYPE.Decorator;
                    break;
                case NODEOPTIONS.Leaf:
                    node = _AssetToUpdate.Create<SO_LeafNode>(name);
                    node.SwapableNodes = false;
                    node.Type = "Leaf";
                    node.nodeType = (int)NODETYPE.Leaf;
                    break;
                case NODEOPTIONS.Debug:
                    node = _AssetToUpdate.Create<SO_TestNode>(name);
                    node.SwapableNodes = false;
                    node.Type = "Debug";
                    node.nodeType = (int)NODETYPE.Debug;
                    break;
            }
            string tempBaseName = "New Node";
            string tempName = tempBaseName;
            int number = 0;
            while (_AssetToUpdate.ContainsName(tempName))
            {
                tempName = tempBaseName + $" {number}";
                number++;
            }
            node.name = tempName;
            _AssetToUpdate.AddNode(node);
            InContexMenu = false;
        }

        private void OnDrag(Vector2 delta)
        {
            if (InContexMenu)
                return; 

            drag = delta;

            if (BehaviourTree != null)
            {
                if (BehaviourTree.root != null)
                {
                    BehaviourTree.root.DragWithChildren(delta);
                }
            }
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            offset += drag * 0.5f;
            Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }
    }
}
