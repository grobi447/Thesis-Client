using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
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

    public List<Vector3> GetConnectedDirections()
    {
        List<Vector3> directions = new List<Vector3>();
        switch (currentType)
        {
            case RailBitmapType.Center:
                break;
            case RailBitmapType.BottomToRight:
                directions.Add(Vector3.down);
                directions.Add(Vector3.right);
                break;
            case RailBitmapType.RightToEnd:
                directions.Add(Vector3.right);
                break;
            case RailBitmapType.BottomToLeft:
                directions.Add(Vector3.down);
                directions.Add(Vector3.left);
                break;
            case RailBitmapType.LeftToEnd:
                directions.Add(Vector3.left);
                break;
            case RailBitmapType.BottomToTop:
                directions.Add(Vector3.down);
                directions.Add(Vector3.up);
                break;
            case RailBitmapType.BottomToEnd:
                directions.Add(Vector3.down);
                break;
            case RailBitmapType.TopToEnd:
                directions.Add(Vector3.up);
                break;
            case RailBitmapType.TopToRight:
                directions.Add(Vector3.up);
                directions.Add(Vector3.right);
                break;
            case RailBitmapType.LeftToRight:
                directions.Add(Vector3.left);
                directions.Add(Vector3.right);
                break;
            case RailBitmapType.LeftToUp:
                directions.Add(Vector3.left);
                directions.Add(Vector3.up);
                break;
        }
        return directions;
    }
}
