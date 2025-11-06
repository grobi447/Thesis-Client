using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;


[TestFixture]
public class MapeditorTileTest
{
    private GameObject uiHandlerObj;
    private GameObject tileObj;
    private GameObject gridManagerObj;
    private Tile tile;
    private UiHandler uiHandler;
    private GridManager gridManager;

    [SetUp]
    public void Setup()
    {
        uiHandlerObj = new GameObject("UiHandler");
        uiHandler = uiHandlerObj.AddComponent<UiHandler>();

        tileObj = new GameObject("Tile");
        tile = tileObj.AddComponent<Tile>();
        tile.TileRenderer = tileObj.AddComponent<SpriteRenderer>();
        tileObj.AddComponent<BoxCollider2D>();
        tile.inGame = false;

        gridManagerObj = new GameObject("GridManager");
        gridManager = gridManagerObj.AddComponent<GridManager>();
        gridManager.tiles = new Dictionary<Vector3, Tile>();
        tileObj.transform.position = new Vector3(0, 1, 0);
        tile.Init(gridManager, tileObj.transform.position);
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(uiHandlerObj);
        GameObject.DestroyImmediate(tileObj);
        GameObject.DestroyImmediate(gridManagerObj);
        PauseMenu.GameIsPaused = false;
    }

    [Test]
    public void ShouldInitializeTileWithCorrectPosition()
    {
        Assert.AreEqual(new Vector3(0, 1, 0), tile.position);
    }

    [TestFixture]
    public class OnMouseEnter : MapeditorTileTest
    {
        [Test]
        public void ShouldNotSetTileColor_WhenPaused()
        {
            PauseMenu.GameIsPaused = true;
            var initialColor = tile.TileRenderer.color;

            tile.OnMouseEnter();

            var afterColor = tile.TileRenderer.color;
            Assert.AreEqual(initialColor, afterColor);
        }

        [Test]
        public void ShouldNotSetTileColor_WhenNoSpriteIsSelected()
        {
            var initialColor = tile.TileRenderer.color;

            tile.OnMouseEnter();

            var afterColor = tile.TileRenderer.color;
            Assert.AreEqual(initialColor, afterColor);
        }

        [Test]
        public void ShouldSetTileColor_OnEmptyTile_WhenBlockSpriteSelected()
        {
            Texture2D texture = new Texture2D(16, 16);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), Vector2.zero);

            SpriteData selectedTile = new SpriteData
            {
                name = "TestTile",
                sprite = sprite,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };
            tile.uiHandler.currentView = View.Blocks;
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;

            tile.OnMouseEnter();

            Assert.AreEqual(new Color(1, 1, 1, 0.7f), tile.TileRenderer.color);
        }

        [Test]
        public void ShouldSetTileColor_OnEmptyTile_WhenSpikeSpriteSelected()
        {
            Texture2D texture = new Texture2D(16, 16);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), Vector2.zero);

            SpriteData selectedTile = new SpriteData
            {
                name = "SpikeTop",
                sprite = sprite,
                type = SpriteType.Trap,
                trapType = TrapType.Spike
            };

            tile.uiHandler.currentTool = Tool.Brush;
            tile.uiHandler.currentView = View.Traps;
            tile.gridManager.lastSelectedSprite[View.Traps] = selectedTile;

            Tile blockTile = new GameObject("BlockTile").AddComponent<Tile>();
            blockTile.transform.position = new Vector3(0, 0, 0);
            blockTile.Init(gridManager, blockTile.transform.position);
            blockTile.SpriteData = new SpriteData
            {
                name = "TestBlock",
                sprite = sprite,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };

            tile.gridManager.tiles.Add(blockTile.transform.position, blockTile);

            tile.OnMouseEnter();

            Assert.AreEqual(new Color(0, 1, 0, 0.7f), tile.TileRenderer.color);
        }
        
        [Test]
        public void ShouldNotSetTileColor_OnEmptyTile_WhenSpikeSpriteSelected()
        {
            Texture2D texture = new Texture2D(16, 16);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), Vector2.zero);

            SpriteData selectedTile = new SpriteData
            {
                name = "SpikeTop",
                sprite = sprite,
                type = SpriteType.Trap,
                trapType = TrapType.Spike
            };

            tile.uiHandler.currentTool = Tool.Brush;
            tile.uiHandler.currentView = View.Traps;
            tile.gridManager.lastSelectedSprite[View.Traps] = selectedTile;


            tile.OnMouseEnter();

            Assert.AreEqual(new Color(1, 0, 0, 0.7f), tile.TileRenderer.color);
        }

    }

    [TestFixture]
    public class OnMouseExit : MapeditorTileTest
    {
        [Test]
        public void ShouldResetTileColor_OnEmptyTile()
        {
            // Empty tile (spriteData == null) should become invisible on mouse exit
            tile.TileRenderer.color = new Color(1, 1, 1, 0.7f);

            tile.OnMouseExit();

            Assert.AreEqual(new Color(1, 1, 1, 0), tile.TileRenderer.color);
        }
    }
}