using System;
using UnityEditor;
using UnityEngine;

namespace Andre.AutoTiling
{
    [CustomEditor(typeof(TileAssetEightBit)), CanEditMultipleObjects]
    public class TileAssetEightBitEditor : Editor
    {
        private TileAssetEightBit myTarget;
        private int tab = 0;
        private Texture2D currentSprite;
        private Texture2D previousSprite;
        private object[] assets;

        public override void OnInspectorGUI()
        {
            myTarget = (TileAssetEightBit)target;
            if (myTarget.tiles == null)
                myTarget.tiles = new Sprite[48];
            serializedObject.Update();
            tab = GUILayout.Toolbar(tab, new string[] { "Tiles", "Sprite Sheet" });
            switch (tab)
            {
                case 0:
                    GUILayout.Space(20);
                    GUILayout.Label("N = North, E = East, W = West, S = South \n\nTile names show by which tiles of same type are near");
                    GUILayout.Space(20);
                    serializedObject.Update();
                    EditorList.Show(serializedObject.FindProperty("tiles"), EditorListOption.Buttons, EditorListButtonOption.Move);
                    serializedObject.ApplyModifiedProperties();
                    break;

                case 1:

                    currentSprite = EditorGUILayout.ObjectField("Select sprite Sheet", currentSprite, typeof(Texture2D), false) as Texture2D;
                    if (currentSprite != null)
                    {
                        if (currentSprite != previousSprite || assets == null)
                        {
                            assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(currentSprite));
                            Sprite[] sprites = Array.ConvertAll(assets, item => item as Sprite);


                            if (sprites.Length == 0)
                            {
                                EditorGUILayout.HelpBox(currentSprite.name + " has no sprites", MessageType.Error);
                            }
                            else
                            {
                                if (sprites.Length > 48)
                                {
                                    EditorGUILayout.HelpBox(currentSprite.name + " has too many sprites, they may not be placed properly consider placing them manualy", MessageType.Warning);
                                }
                                if (sprites.Length <= 16)
                                {
                                    EditorGUILayout.HelpBox(currentSprite.name + " has too few sprites, they may not be placed properly consider using FourBit Asset or placing them manualy", MessageType.Warning);
                                }
                                else if (sprites.Length < 48)
                                {
                                    EditorGUILayout.HelpBox(currentSprite.name + " has too few sprites, they may not be placed properly", MessageType.Warning);
                                }
                                if (GUILayout.Button("Add Sprites"))
                                {
                                    previousSprite = currentSprite;
                                    for (int i = 0; i < sprites.Length; i++)
                                    {
                                        if (i > myTarget.tiles.Length - 1)
                                        {
                                            break;
                                        }

                                        myTarget.tiles[i] = sprites[i];

                                    }
                                    EditorUtility.SetDirty(myTarget);
                                    AssetDatabase.SaveAssets();
                                    serializedObject.ApplyModifiedProperties();
                                    assets = null;
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
