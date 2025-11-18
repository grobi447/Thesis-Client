using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

namespace Objects
{
    public class MapItem : MonoBehaviour
    {
        public string mapId;
        public string mapName;
        public string createdBy;
        public MapSelector mapSelector;
        public void Initialize(string mapId, string mapName, string createdBy)
        {
            this.mapId = mapId;
            this.mapName = mapName;
            this.createdBy = createdBy;
        }

        public void Awake()
        {
            mapSelector = FindObjectOfType<MapSelector>();
        }

        public void OnSelectButtonPressed()
        {
            mapSelector.UpdateActiveMap(mapId);
        }
    }
}