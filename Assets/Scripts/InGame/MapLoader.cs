using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public API api;
    public API.MapData map;

    void Awake()
    {
        LoadJSON();
    }

    private void LoadJSON() {
        string activeMapId = MapManager.Instance.activeMapId;
        string mapsPath = Application.dataPath + $"/Maps/{activeMapId}/{activeMapId}.json";
        string jsonContent = File.ReadAllText(mapsPath);

        var jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        map = JsonConvert.DeserializeObject<API.MapData>(jsonContent, jsonSettings);

        foreach (var tileWrapper in map.data.tiles)
        {
            if (tileWrapper == null || tileWrapper.tile == null) continue;
            if (tileWrapper.trapSettings is JObject jObj)
            {
                switch (tileWrapper.tile.trapType)
                {
                    case TrapType.Spike:
                        tileWrapper.trapSettings = jObj.ToObject<SpikeSettings>();
                        break;
                    case TrapType.Saw:
                        tileWrapper.trapSettings = jObj.ToObject<SawSettings>();
                        break;
                    case TrapType.Canon:
                        tileWrapper.trapSettings = jObj.ToObject<CanonSettings>();
                        break;
                    case TrapType.Axe:
                        tileWrapper.trapSettings = jObj.ToObject<AxeSettings>();
                        break;
                    case TrapType.Blade:
                        tileWrapper.trapSettings = jObj.ToObject<BladeSettings>();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}