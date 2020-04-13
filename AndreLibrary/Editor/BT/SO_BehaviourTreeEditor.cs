using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CustomEditor(typeof(SO_BehaviourTree))]
    public class SO_BehaviourTreeEditor : Editor
    {
        NODEOPTIONS op;
        SO_BehaviourTree myTarget;
        string name = null;
        string assetName = null;
        int tab = 0;

        public override void OnInspectorGUI()
        {
            myTarget = (SO_BehaviourTree)target;

            DrawDefaultInspector();

            GUILayout.Space(20);

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

        public SO_Node GetNodeType(NODEOPTIONS options, string name)
        {
            SO_Node node = null;
            switch (options)
            {
                case NODEOPTIONS.Selector:
                    node = myTarget.Create<SO_Selector>(name);
                    node.Type = "Selector";
                    break;
                case NODEOPTIONS.Sequence:
                    node = myTarget.Create<SO_Sequence>(name);
                    node.Type = "Sequence";
                    break;
                case NODEOPTIONS.BooleanSelector:
                    node = myTarget.Create<SO_BooleanSelector>(name);
                    node.Type = "BSelector";
                    break;
                case NODEOPTIONS.FailRepeater:
                    node = myTarget.Create<SO_FailRepeater>(name);
                    node.SwapableNodes = false;
                    node.Type = "FRepeater";
                    break;
                case NODEOPTIONS.FakeSate:
                    node = myTarget.Create<SO_FakeSate>(name);
                    node.SwapableNodes = false;
                    node.Type = "FakeSate";
                    break;
                case NODEOPTIONS.InfiniteRepeater:
                    node = myTarget.Create<SO_InfiniteRepeater>(name);
                    node.SwapableNodes = false;
                    node.Type = "IRepeater";
                    break;
                case NODEOPTIONS.Inverter:
                    node = myTarget.Create<SO_Inverter>(name);
                    node.SwapableNodes = false;
                    node.Type = "Inverter";
                    break;
                case NODEOPTIONS.Limiter:
                    node = myTarget.Create<SO_Limiter>(name);
                    node.SwapableNodes = false;
                    node.Type = "Limiter";
                    break;
                case NODEOPTIONS.Repeater:
                    node = myTarget.Create<SO_Repeater>(name);
                    node.SwapableNodes = false;
                    node.Type = "Repeater";
                    break;
                case NODEOPTIONS.Leaf:
                    node = myTarget.Create<SO_LeafNode>(name);
                    node.SwapableNodes = false;
                    node.Type = "Leaf";
                    break;
                case NODEOPTIONS.Debug:
                    node = myTarget.Create<SO_TestNode>(name);
                    node.SwapableNodes = false;
                    node.Type = "Debug";
                    break;
                default:
                    break;
            }
            node.btParent = myTarget;
            node.nodeType = (int)NODETYPE.Root;
            return node;
        }
    }

    public enum NODEOPTIONS
    {
        None,
        Selector,
        Sequence,
        BooleanSelector,
        FailRepeater,
        FakeSate,
        InfiniteRepeater,
        Inverter,
        Limiter,
        Repeater,
        Leaf,
        Debug,
    }


    public enum ROOTNODEOPTIONS
    {
        Selector = 1,
        Sequence,
        BooleanSelector,
    }

    public enum SIDEMENUNODEOPTIONS
    {
        Selector = 1,
        Sequence,
        BooleanSelector,
        FailRepeater,
        FakeSate,
        InfiniteRepeater,
        Inverter,
        Limiter,
        Repeater,
        Leaf,
        Debug,
    }

    public enum NODETYPE
    {
        Root,
        Composite,
        Decorator,
        Leaf,
        Debug,
    }
}