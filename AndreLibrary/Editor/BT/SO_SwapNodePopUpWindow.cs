using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    public class SO_SwapNodePopUpWindow : EditorWindow
    {
        private static SO_Node AssetToUpdate;
        string assetName = null;
        string nodeAName = null;
        string nodeBName = null;
        int nodeAPos = 0;
        int nodeBPos = 0;
        private static Texture tex;
        int tab = 0;
        int nodeASelected = 0;
        int nodeBSelected = 0;
        string[] nodeAOptions;
        string[] nodeBOptions;
        public static void OpenEditorWindow(SO_Node Asset, Rect pos)
        {
            SO_SwapNodePopUpWindow window = ScriptableObject.CreateInstance<SO_SwapNodePopUpWindow>();
            Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AndreLibrary/Editor/BT_Icon.png");
            GUIContent titleContent = new GUIContent("Add Node" + Asset.name, icon);
            AssetToUpdate = Asset;
            window.titleContent = titleContent;
            window.position = pos;
            tex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AndreLibrary/Editor/BT_Lighter_Grey_Blue.jpg");
            window.PopulateNodeOptions();
            window.ShowModalUtility();
        }

        void PopulateNodeOptions()
        {
            nodeAOptions = new string[AssetToUpdate.Children.Count];
            nodeBOptions = new string[AssetToUpdate.Children.Count];

            for (int i = 0; i < AssetToUpdate.Children.Count; i++)
            {
                nodeAOptions[i] = AssetToUpdate.Children[i].name;
                nodeBOptions[i] = AssetToUpdate.Children[i].name;
            }
        }

        private void OnGUI()
        {
            if(tex == null)
                tex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AndreLibrary/Editor/BT_Lighter_Grey_Blue.jpg");

            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);
            GUILayout.BeginVertical("HelpBox");
            GUILayout.Space(10);
            tab = GUILayout.Toolbar(tab, new string[] { "Picker","Names", "Position"});
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            switch (tab)
            {
                case 0:
                    GUILayout.Space(10);
                    GUILayout.Label("Node A", style, GUILayout.ExpandWidth(true));
                    nodeASelected = EditorGUILayout.Popup(nodeASelected, nodeAOptions);
                    GUILayout.Space(5);
                    GUILayout.Label("Node B", style, GUILayout.ExpandWidth(true));
                    nodeBSelected = EditorGUILayout.Popup(nodeBSelected, nodeBOptions);
                    GUILayout.Space(2.5f);
                    if (nodeASelected != nodeBSelected)
                    {
                        GUILayout.Space(2.5f);
                        if (GUILayout.Button("Swap Nodes"))
                        {
                            AssetToUpdate.MoveNodePositions(AssetToUpdate.Children[nodeASelected].name, AssetToUpdate.Children[nodeBSelected].name);
                            this.Close();
                        }
                        GUILayout.Space(2.5f);
                    }
                    else
                        GUILayout.Space(27.5f);
                    break;
                case 1:
                    GUILayout.Space(10);
                    GUILayout.Label("Node A Name", style, GUILayout.ExpandWidth(true));
                    nodeAName = GUILayout.TextField(nodeAName);
                    GUILayout.Space(5);
                    GUILayout.Label("Node B Name", style, GUILayout.ExpandWidth(true));
                    nodeBName = GUILayout.TextField(nodeBName);
                    GUILayout.Space(2.5f);
                    if ((!string.IsNullOrEmpty(nodeAName) && !string.IsNullOrWhiteSpace(nodeAName))
                        && (!string.IsNullOrEmpty(nodeBName) && !string.IsNullOrWhiteSpace(nodeBName))
                        && nodeAName != nodeBName)
                    {
                        if (AssetToUpdate.ContainsName(nodeAName) && AssetToUpdate.ContainsName(nodeBName))
                        {
                            GUILayout.Space(2.5f);
                            if (GUILayout.Button("Swap Nodes"))
                            {
                                AssetToUpdate.MoveNodePositions(nodeAName, nodeBName);
                                this.Close();
                            }
                            GUILayout.Space(2.5f);
                        }
                        else
                        {
                            GUILayout.Space(27.5f);
                        }
                    }
                    else
                        GUILayout.Space(27.5f);
                    break;
                case 2:
                    GUILayout.Space(10);
                    GUILayout.Label("Node A Position", style, GUILayout.ExpandWidth(true));
                    nodeAPos = EditorGUILayout.IntField(nodeAPos);
                    GUILayout.Space(5);
                    GUILayout.Label("Node B Position", style, GUILayout.ExpandWidth(true));
                    nodeBPos = EditorGUILayout.IntField(nodeBPos);
                    GUILayout.Space(2.5f);
                    if ((nodeAPos > 0 && AssetToUpdate.Children.Count >= nodeAPos)
                        && (nodeBPos > 0 && AssetToUpdate.Children.Count >= nodeBPos)
                        && nodeAPos != nodeBPos)
                    {
                        GUILayout.Space(2.5f);
                        if (GUILayout.Button("Swap Nodes"))
                        {
                            AssetToUpdate.MoveNodePositions(AssetToUpdate.Children[nodeAPos-1].name, AssetToUpdate.Children[nodeBPos-1].name);
                            this.Close();
                        }
                        GUILayout.Space(2.5f);
                    }
                    else
                        GUILayout.Space(27.5f);
                    break;
                default:
                    break;
            }
            GUILayout.EndVertical();
        }
    }
}
