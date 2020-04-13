using UnityEngine;

namespace Andre.Utils.Pixels
{
    public class SpriteToMeshRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite;
        private MeshRenderer mesh;

        // Start is called before the first frame update
        private void Start()
        {
            mesh = GetComponent<MeshRenderer>();
        }

        private void LateUpdate()
        {
            Texture2D croppedTexture = new Texture2D((int)sprite.sprite.textureRect.width, (int)sprite.sprite.textureRect.height);
            Color[] pixels = sprite.sprite.texture.GetPixels((int)sprite.sprite.textureRect.x, (int)sprite.sprite.textureRect.y, (int)sprite.sprite.textureRect.width, (int)sprite.sprite.textureRect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();

            mesh.material.mainTexture = croppedTexture;
        }
    }
}
