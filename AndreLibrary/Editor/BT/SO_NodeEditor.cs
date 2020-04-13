using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CustomEditor(typeof(SO_Node), true)]
    public class SO_NodeEditor : Editor
    {
        NODEOPTIONS op;
        SO_Node myTarget;
        string name = null;
        string nodeAName = null;
        string nodeBName = null;
        string assetName = null;
        int tab = 0;
        public override void OnInspectorGUI()
        {
            myTarget = (SO_Node)target;
            assetName = EditorGUILayout.TextField("New asset name: ", assetName);
            if (!string.IsNullOrEmpty(assetName) && !string.IsNullOrWhiteSpace(assetName)
                && assetName != myTarget.name)
            {
                if (GUILayout.Button("Change Name"))
                {
                    myTarget.ChangeName(assetName);
                }
            }

            GUILayout.Space(20);

            //serializedObject.Update();
           // EditorList.Show(serializedObject.FindProperty("m_nodes"), EditorListOption.ElementLabels | EditorListOption.Buttons);
            //serializedObject.ApplyModifiedProperties();

            DrawDefaultInspector();

            GUILayout.Space(20);

            if (myTarget.SwapableNodes)
            {
                tab = GUILayout.Toolbar(tab, new string[] { "Add", "Swap", "Remove" });
                switch (tab)
                {
                    case 0:
                        op = (NODEOPTIONS)EditorGUILayout.EnumPopup("Node to create:", op);
                        if (op != NODEOPTIONS.None)
                        {
                            name = EditorGUILayout.TextField("Node Name: ", name);
                            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name) && !myTarget.ContainsName(name))
                            {
                                if (GUILayout.Button("Add Node"))
                                {
                                    SO_Node node = GetNodeType(op, name);
                                    if (node != null)
                                        myTarget.AddNode(node);
                                }
                            }
                        }
                        break;
                    case 1:
                        if (!myTarget.NodeAdd)
                            return;
                        nodeAName = EditorGUILayout.TextField("Node A Name: ", nodeAName);
                        nodeBName = EditorGUILayout.TextField("Node B Name: ", nodeBName);
                        if ((!string.IsNullOrEmpty(nodeAName) && !string.IsNullOrWhiteSpace(nodeAName) && myTarget.ContainsName(nodeAName))
                            && (!string.IsNullOrEmpty(nodeBName) && !string.IsNullOrWhiteSpace(nodeBName) && myTarget.ContainsName(nodeBName)))
                        {
                            if (GUILayout.Button("Swap Nodes"))
                            {
                                myTarget.MoveNodePositions(nodeAName, nodeBName);
                            }
                        }
                        break;
                    case 2:
                        name = EditorGUILayout.TextField("Node Name: ", name);
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name) && myTarget.ContainsName(name))
                        {
                            if (GUILayout.Button("Remove Nodes"))
                            {
                                myTarget.RemoveNode(name);
                                //AssetDatabase.Refresh();
                               // AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(myTarget), ImportAssetOptions.ForceUpdate);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (!myTarget.NodeAdd)
                    return;

                tab = GUILayout.Toolbar(tab, new string[] { "Add", "Remove" });
                switch (tab)
                {
                    case 0:
                        op = (NODEOPTIONS)EditorGUILayout.EnumPopup("Node to create:", op);
                        if (op != NODEOPTIONS.None)
                        {
                            name = EditorGUILayout.TextField("Node Name: ", name);
                            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name) && !myTarget.ContainsName(name))
                            {
                                if (GUILayout.Button("Add Node"))
                                {
                                    SO_Node node = GetNodeType(op, name);
                                    if (node != null)
                                        myTarget.AddNode(node);
                                }
                            }
                        }
                        break;

                    case 1:
                        name = EditorGUILayout.TextField("Node Name: ", name);
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name) && myTarget.ContainsName(name))
                        {
                            if (GUILayout.Button("Remove Nodes"))
                            {
                                myTarget.RemoveNode(name);
                                AssetDatabase.Refresh();
                                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(myTarget), ImportAssetOptions.ForceUpdate);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public SO_Node GetNodeType(NODEOPTIONS options, string name)
        {
            SO_Node node = null;
            switch (options)
            {
                case NODEOPTIONS.Selector:
                    node = myTarget.Create<SO_Selector>(name);
                    node.Type = "Selector";
                    node.nodeType = (int)NODETYPE.Composite;
                    break;
                case NODEOPTIONS.Sequence:
                    node = myTarget.Create<SO_Sequence>(name);
                    node.Type = "Sequence";
                    node.nodeType = (int)NODETYPE.Composite;
                    break;
                case NODEOPTIONS.BooleanSelector:
                    node = myTarget.Create<SO_BooleanSelector>(name);
                    node.Type = "BSelector";
                    node.nodeType = (int)NODETYPE.Composite;
                    break;
                case NODEOPTIONS.FailRepeater:
                    node = myTarget.Create<SO_FailRepeater>(name);
                    node.SwapableNodes = false;
                    node.Type = "FRepeater";
                    node.nodeType = (int)NODETYPE.Decorator;
                    break;
                case NODEOPTIONS.FakeSate:
                    node = myTarget.Create<SO_FakeSate>(name);
                    node.SwapableNodes = false;
                    node.Type = "FakeSate";
                    node.nodeType = (int)NODETYPE.Decorator;
                    break;
                case NODEOPTIONS.InfiniteRepeater:
                    node = myTarget.Create<SO_InfiniteRepeater>(name);
                    node.SwapableNodes = false;
                    node.Type = "IRepeater";
                    node.nodeType = (int)NODETYPE.Decorator;
                    break;
                case NODEOPTIONS.Inverter:
                    node = myTarget.Create<SO_Inverter>(name);
                    node.SwapableNodes = false;
                    node.Type = "Inverter";
                    node.nodeType = (int)NODETYPE.Decorator;
                    break;
                case NODEOPTIONS.Limiter:
                    node = myTarget.Create<SO_Limiter>(name);
                    node.SwapableNodes = false;
                    node.Type = "Limiter";
                    node.nodeType = (int)NODETYPE.Decorator;
                    break;
                case NODEOPTIONS.Repeater:
                    node = myTarget.Create<SO_Repeater>(name);
                    node.SwapableNodes = false;
                    node.Type = "Repeater";
                    node.nodeType = (int)NODETYPE.Decorator;
                    break;
                case NODEOPTIONS.Leaf:
                    node = myTarget.Create<SO_LeafNode>(name);
                    node.SwapableNodes = false;
                    node.Type = "Leaf";
                    node.nodeType = (int)NODETYPE.Leaf;
                    break;
                case NODEOPTIONS.Debug:
                    node = myTarget.Create<SO_TestNode>(name);
                    node.SwapableNodes = false;
                    node.Type = "Debug";
                    node.nodeType = (int)NODETYPE.Debug;
                    break;
                default:
                    break;
            }

            return node;
        }
    }
}