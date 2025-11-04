using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : Trap
{
    public Dictionary<string, float> settings = new Dictionary<string, float>
    {
        {"crushTime", 0.3f},
        {"upTime", 0.3f},
        {"reload", 1f}
    };

    private Vector3 startPos;
    private Vector3 endPos;
    private bool canTrigger = true;
    private Player player;
    private MapLoader mapLoader;

    private void Awake()
    {
        trapType = TrapType.Blade;
    }

    void Start()
    {
        startPos = transform.position;
        if(inGame) 
        {
            player = FindObjectOfType<Player>();
            mapLoader = FindObjectOfType<MapLoader>();
        }
    }

    void Update()
    {
        bool currentlyBelow;
        if (inGame)
        {
            currentlyBelow = IsPlayerBelow();
        }
        else
        {
            currentlyBelow = IsMouseBelow();
        }
        if (canTrigger && currentlyBelow)
        {
            StartCoroutine(Smash());
        }
    }

    private bool IsMouseBelow()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Bounds bounds = TileRenderer.bounds;
        return mousePos.y < bounds.min.y && mousePos.x >= bounds.min.x && mousePos.x <= bounds.max.x;
    }

    private bool IsPlayerBelow()
    {
        Vector3 playerPos = player.transform.position;
        Bounds bounds = TileRenderer.bounds;
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        
        if (playerCollider != null)
        {
            Bounds playerBounds = playerCollider.bounds;
            bool horizontalOverlap = playerBounds.max.x > bounds.min.x && playerBounds.min.x < bounds.max.x;
            bool isBelow = playerBounds.center.y < bounds.min.y;
            return horizontalOverlap && isBelow;
        }
        
        return playerPos.y < bounds.min.y && playerPos.x >= bounds.min.x && playerPos.x <= bounds.max.x;
    }
    private IEnumerator Smash()
    {
        endPos = GetEndPosition();
        canTrigger = false;
        float elapsed = 0f;
        while (elapsed < settings["crushTime"])
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / settings["crushTime"]);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        elapsed = 0f;
        while (elapsed < settings["upTime"])
        {
            transform.position = Vector3.Lerp(endPos, startPos, elapsed / settings["upTime"]);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;
        yield return new WaitForSeconds(settings["reload"]);
        canTrigger = true;
    }

    private Vector3 GetEndPosition()
    {
        Vector3 currentPos = transform.position;
        for (int y = (int)currentPos.y - 1; y >= 0; y--)
        {
            Vector3 checkPos = new Vector3(currentPos.x, y, currentPos.z);
            
            if (inGame && mapLoader != null)
            {
                if (mapLoader.HasTileAtPosition(checkPos))
                {
                    return new Vector3(currentPos.x, y + 1, currentPos.z);
                }
            }
            else if (!inGame && gridManager != null)
            {
                Tile tile = gridManager.GetTileAtPosition(checkPos);
                if (tile != null && tile.SpriteData != null && tile.SpriteData.type == SpriteType.Block)
                {
                    return new Vector3(currentPos.x, y + 1, currentPos.z);
                }
            }
        }
        return new Vector3(currentPos.x, 0, currentPos.z);
    }
}
