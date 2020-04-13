using UnityEngine;

namespace Andre.AutoTiling
{
    [CreateAssetMenu(fileName = "New TileAsset", menuName = "Auto Tile/Object/TileAsset")]
    public class TileAssetObjects : ScriptableObject
    {
        public GameObject[] tiles;
    }
}
