using System.Collections.Generic;
using System.Diagnostics;

#nullable enable

public class MapManager
{
    private static MapManager? instance;

    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MapManager();
            }
            return instance;
        }
    }

    public string? activeMapId;
    public string? nextMapId;

    public void SetMapHandler(string activeMapId, string? nextMapId)
    {
        this.activeMapId = activeMapId;
        this.nextMapId = nextMapId;
    }
}
