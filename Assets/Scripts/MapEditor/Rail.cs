using System.Collections.Generic;
using UnityEngine;
public enum RailBitmapType
{
    Center = 0,
    BottomToRight = 1,
    RightToEnd = 2,
    BottomToLeft = 3,
    LeftToEnd = 4,
    BottomToTop = 5,
    BottomToEnd = 6,
    TopToEnd = 7,
    TopToRight = 8,
    LeftToRight = 9,
    LeftToUp = 10
}

public class Rail : Tile
{
    [SerializeField] public List<Sprite> RailSprites;
    public RailBitmapType currentType;
    public bool isLocked = false;

    public Dictionary<RailBitmapType, string> RailBitmap = new Dictionary<RailBitmapType, string>
    {
        { RailBitmapType.Center, "Center" },
        { RailBitmapType.BottomToRight, "BottomToRight" },
        { RailBitmapType.RightToEnd, "RightToEnd" },
        { RailBitmapType.BottomToLeft, "BottomToLeft" },
        { RailBitmapType.LeftToEnd, "LeftToEnd" },
        { RailBitmapType.BottomToTop, "BottomToTop" },
        { RailBitmapType.BottomToEnd, "BottomToEnd" },
        { RailBitmapType.TopToEnd, "TopToEnd" },
        { RailBitmapType.TopToRight, "TopToRight" },
        { RailBitmapType.LeftToRight, "LeftToRight" },
        { RailBitmapType.LeftToUp, "LeftToUp" }
    };
    public void SetSprite(RailBitmapType type)
    {
        this.TileRenderer.sprite = RailSprites[(int)type];
        this.currentType = type;
        this.SpriteData = new SpriteData
        {
            name = RailBitmap[type],
            sprite = RailSprites[(int)type],
            type = SpriteType.Rail
        };
    }
}
