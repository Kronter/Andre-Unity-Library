using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Andre.AI.BehaviourTree.Scriptable
{
    public class SO_RenamePopUpWindow : EditorWindow
    {
        private static SO_Node AssetToUpdate;
        string assetName = null;
        private static Texture tex;
        public static void OpenEditorWindow(SO_Node Asset, Rect pos)
        {
            SO_RenamePopUpWindow window = ScriptableObject.CreateInstance<SO_RenamePopUpWindow>();
            Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AndreLibrary/Editor/BT_Icon.png");
            GUIContent titleContent = new GUIContent("Rename " + Asset.name, icon);
            AssetToUpdate = Asset;
            window.titleContent = titleContent;
            window.position = pos;
            tex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AndreLibrary/Editor/BT_Lighter_Grey_Blue.jpg");
            window.ShowModalUtility();
        }

        void OnGUI()
        {
            if (tex == null)
                tex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AndreLibrary/Editor/BT_Lighter_Grey_Blue.jpg");

            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);
            GUILayout.BeginVertical("HelpBox");
            GUILayout.Space(20);
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            GUILayout.Label("New Asset Name", style, GUILayout.ExpandWidth(true));
            assetName = GUILayout.TextField(assetName);
            GUILayout.Space(2.5f);
            if (!string.IsNullOrEmpty(assetName) && !string.IsNullOrWhiteSpace(assetName)
                && assetName != AssetToUpdate.name)
            {
                if (AssetToUpdate.parent == null)
                {
                    if (!AssetToUpdate.btParent.ContainsName(assetName))
                    {
                        if (GUILayout.Button("Change Name"))
                        {
                            AssetToUpdate.ChangeName(assetName);
                            this.Close();
                        }
                        GUILayout.Space(11.5f);
                    }
                }
                else
                {
                    if (!AssetToUpdate.parent.ContainsName(assetName))
                    {
                        if (GUILayout.Button("Change Name"))
                        {
                            AssetToUpdate.ChangeName(assetName);
                            this.Close();
                        }
                        GUILayout.Space(11.5f);
                    }
                }
            }
            else
                GUILayout.Space(32.5f);
            GUILayout.EndVertical();
        }

    }
}
