using UnityEditor;
using UnityEngine;

namespace Andre.AutoTiling
{
    [CustomPropertyDrawer(typeof(PreviewEightBitAttribute))]
    public class PreviewEightBitTileDrawer : PropertyDrawer
    {
        private const float imageHeight = 40;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference &&
                (property.objectReferenceValue as Sprite) != null)
            {
                return EditorGUI.GetPropertyHeight(property, label, true) + imageHeight + 10;
            }
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private static string GetPath(SerializedProperty property)
        {
            string path = property.propertyPath;
            int index = path.LastIndexOf(".");
            return path.Substring(0, index + 1);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Draw the normal property field
            label = EditorGUI.BeginProperty(position, label, property);
            GUIContent title = new GUIContent();
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            title.text = GetTileLable(pos);
            Rect contentPosition = EditorGUI.PrefixLabel(position, title);
            contentPosition = EditorGUI.IndentedRect(position);
            contentPosition.width *= 0.5f;
            contentPosition.x = contentPosition.width * 0.75f;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(contentPosition, property, label, true);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                Sprite sprite = property.objectReferenceValue as Sprite;
                if (sprite != null)
                {
                    contentPosition.x += contentPosition.width;
                    contentPosition.width /= 3f;
                    contentPosition.y += EditorGUI.GetPropertyHeight(property, label, true) - imageHeight;
                    contentPosition.height = imageHeight;
                    EditorGUIUtility.labelWidth = 14f;
                    DrawTexturePreview(contentPosition, sprite);
                }
            }
            EditorGUI.EndProperty();
        }

        private void DrawTexturePreview(Rect position, Sprite sprite)
        {
            Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
            Vector2 size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

            Rect coords = sprite.textureRect;
            coords.x /= fullSize.x;
            coords.width /= fullSize.x;
            coords.y /= fullSize.y;
            coords.height /= fullSize.y;

            Vector2 ratio;
            ratio.x = position.width / size.x;
            ratio.y = position.height / size.y;
            float minRatio = Mathf.Min(ratio.x, ratio.y);

            Vector2 center = position.center;
            position.width = size.x * minRatio;
            position.height = size.y * minRatio;
            position.center = center;

            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }
        private string GetTileLable(int index)
        {
            string Name = " ";

            switch (index)
            {
                case 0:
                    Name = "None";
                    break;

                case 1:
                    Name = "N";
                    break;

                case 2:
                    Name = "W";
                    break;

                case 3:
                    Name = "W, N";
                    break;

                case 4:
                    Name = "W, N, NW";
                    break;

                case 5:
                    Name = "E";
                    break;

                case 6:
                    Name = "N, E";
                    break;

                case 7:
                    Name = "N, E, NE";
                    break;

                case 8:
                    Name = "W, E";
                    break;

                case 9:
                    Name = "N, E, W";
                    break;

                case 10:
                    Name = "N, E, W, NW";
                    break;

                case 11:
                    Name = "N, E, W, NE";
                    break;

                case 12:
                    Name = "N, E, W, NE, NW";
                    break;

                case 13:
                    Name = "S";
                    break;

                case 14:
                    Name = "N, S";
                    break;

                case 15:
                    Name = "S, W";
                    break;

                case 16:
                    Name = "N, S, W";
                    break;

                case 17:
                    Name = "N, S, W, NW";
                    break;

                case 18:
                    Name = "S, E";
                    break;

                case 19:
                    Name = "N, S, E";
                    break;

                case 20:
                    Name = "N, S, E, NE";
                    break;

                case 21:
                    Name = "S, W, E";
                    break;

                case 22:
                    Name = "N, S, W, E";
                    break;

                case 23:
                    Name = "N, S, W, E, NW, SE";
                    break;

                case 24:
                    Name = "N, S, W, E, NE";
                    break;

                case 25:
                    Name = "N, S, W, E, NW, NE";
                    break;

                case 26:
                    Name = "S, W, SW";
                    break;

                case 27:
                    Name = "N, S, W, SW";
                    break;

                case 28:
                    Name = "N, S, W, NW, SW";
                    break;

                case 29:
                    Name = "S, W, E, SW";
                    break;

                case 30:
                    Name = "N, S, W, E, SW, NE";
                    break;

                case 31:
                    Name = "N, S, W, E, NW, SW";
                    break;

                case 32:
                    Name = "N, S, W, E, SW, NE";
                    break;

                case 33:
                    Name = "N, S, W, E, NW, SW, NE";
                    break;

                case 34:
                    Name = "S, E, SE";
                    break;

                case 35:
                    Name = "N, S, E, SE";
                    break;

                case 36:
                    Name = "N, S, E, NE, SE";
                    break;

                case 37:
                    Name = "S, W, E, SE";
                    break;

                case 38:
                    Name = "N, S, W, E, SE";
                    break;

                case 39:
                    Name = "N, S, W, E, NW, SE";
                    break;

                case 40:
                    Name = "N, S, W, E, NE, SE";
                    break;

                case 41:
                    Name = "N, S, W, E, NW, NE, SE";
                    break;

                case 42:
                    Name = "S, W, E, SW, SE";
                    break;

                case 43:
                    Name = "N, S, W, E, SW, SE";
                    break;

                case 44:
                    Name = "N, S, W, E, NW, SW, SE";
                    break;

                case 45:
                    Name = "N, S, W, E, NE, SW, SE";
                    break;

                case 46:
                    Name = "None";
                    break;

                case 47:
                    Name = "All";
                    break;

                default:
                    Name = index.ToString();
                    break;
            }

            return Name + " Tile";
        }
    }
}
