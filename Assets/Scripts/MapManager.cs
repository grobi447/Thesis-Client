using System.Collections.Generic;
using System.Diagnostics;

public class MapManager
{
    private static MapManager instance;

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

    public string activeMapId;
    public List<string> installedMaps = new List<string>();

    public void LoadNextMap()
    {
        int activeMapIdIndex = installedMaps.IndexOf(activeMapId);
        activeMapId = activeMapIdIndex + 1 < installedMaps.Count ? installedMaps[activeMapIdIndex + 1] : installedMaps[0];
    }
}
