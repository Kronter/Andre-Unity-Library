using UnityEngine;
using UnityEditor;

namespace Andre.AutoTiling
{
    [CustomEditor(typeof(TileAssetObjects)), CanEditMultipleObjects]
    public class TileAssetObjectEditor : Editor
    {
        TileAssetObjects myTarget;
        public override void OnInspectorGUI()
        {
            myTarget = (TileAssetObjects)target;

            serializedObject.Update();
            EditorList.Show(serializedObject.FindProperty("tiles"), EditorListOption.Buttons | EditorListOption.ListLabel, EditorListButtonOption.All);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
