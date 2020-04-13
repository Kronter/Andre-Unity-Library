using UnityEngine;

namespace Andre.AutoTiling
{
    [CreateAssetMenu(fileName = "New TileAsset", menuName = "Auto Tile/8 Bit/TileAsset")]
    public class TileAssetEightBit : TileAsset
    {
    }

    public class TileAsset : ScriptableObject
    {
        [PreviewEightBit]
        public Sprite[] tiles;
    }
}
