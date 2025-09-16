using UnityEngine;

public enum TrapType
{
    Empty,
    Spike,
    Saw
}

public class Trap : Tile
{
    public TrapType trapType;
    public bool isActive = false;

    public GameObject border;

    public void SetBorder()
    {
        if (isActive) border.SetActive(true);
        else border.SetActive(false);
    }
}