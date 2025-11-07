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
            SpriteData selectedTile = new SpriteData
            {
                name = "TestTile",
                sprite = null,
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
            SpriteData selectedTile = new SpriteData
            {
                name = "SpikeTop",
                sprite = null,
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
                sprite = null,
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
            SpriteData selectedTile = new SpriteData
            {
                name = "SpikeTop",
                sprite = null,
                type = SpriteType.Trap,
                trapType = TrapType.Spike
            };

            tile.uiHandler.currentTool = Tool.Brush;
            tile.uiHandler.currentView = View.Traps;
            tile.gridManager.lastSelectedSprite[View.Traps] = selectedTile;


            tile.OnMouseEnter();

            Assert.AreEqual(new Color(1, 0, 0, 0.7f), tile.TileRenderer.color);
        }

        [Test]
        public void ShouldNotSetTileColor_OnEmptyTile_WhenSawSpriteSelected()
        {
            SpriteData selectedTile = new SpriteData
            {
                name = "Saw",
                sprite = null,
                type = SpriteType.Trap,
                trapType = TrapType.Saw
            };

            tile.uiHandler.currentTool = Tool.Brush;
            tile.uiHandler.currentView = View.Traps;
            tile.gridManager.lastSelectedSprite[View.Traps] = selectedTile;

            tile.OnMouseEnter();

            Assert.AreEqual(new Color(1, 0, 0, 0.7f), tile.TileRenderer.color);
        }

        [Test]
        public void ShouldSetTileColor_OnRail_WhenSawSpriteSelected()
        {

            tile.SpriteData = new SpriteData
            {
                name = "Rail",
                sprite = null,
                type = SpriteType.Rail,
                trapType = TrapType.Empty
            };

            tileObj.AddComponent<Rail>();

            Texture2D texture = new Texture2D(16, 16);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), Vector2.zero);

            SpriteData selectedTile = new SpriteData
            {
                name = "Saw",
                sprite = sprite,
                type = SpriteType.Trap,
                trapType = TrapType.Saw
            };

            tile.uiHandler.currentTool = Tool.Brush;
            tile.uiHandler.currentView = View.Traps;
            tile.gridManager.lastSelectedSprite[View.Traps] = selectedTile;

            tile.OnMouseEnter();

            Assert.AreEqual(new Color(1, 0, 0, 0.7f), tile.TileRenderer.color);
        }

        [Test]
        public void ShouldSetTileColor_WhenSpawnIsSelected_NoOtherSpawns()
        {
            SpriteData selectedTile = new SpriteData
            {
                name = "Spawn",
                sprite = null,
                type = SpriteType.Spawn,
                trapType = TrapType.Empty
            };

            tile.uiHandler.currentTool = Tool.Brush;
            tile.uiHandler.currentView = View.Blocks;
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;

            GameObject blockTileObj = new GameObject("BlockTile");
            Tile blockTile = blockTileObj.AddComponent<Tile>();
            blockTile.transform.position = new Vector3(0, 0, 0);
            blockTile.Init(gridManager, blockTile.transform.position);
            blockTile.SpriteData = new SpriteData
            {
                name = "TestBlock",
                sprite = null,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };
            tile.gridManager.tiles.Add(blockTile.transform.position, blockTile);

            tile.OnMouseEnter();

            Assert.AreEqual(new Color(0, 1, 0, 0.7f), tile.TileRenderer.color);
        }

        [Test]
        public void ShouldNotSetTileColor_WhenSpawnIsSelected_OtherSpawnsExist()
        {
            SpriteData selectedTile = new SpriteData
            {
                name = "Spawn",
                sprite = null,
                type = SpriteType.Spawn,
                trapType = TrapType.Empty
            };

            tile.uiHandler.currentTool = Tool.Brush;
            tile.uiHandler.currentView = View.Blocks;
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;

            GameObject blockTileObj = new GameObject("BlockTile");
            Tile blockTile = blockTileObj.AddComponent<Tile>();
            blockTile.transform.position = new Vector3(0, 0, 0);
            blockTile.Init(gridManager, blockTile.transform.position);
            blockTile.SpriteData = new SpriteData
            {
                name = "TestBlock",
                sprite = null,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };
            tile.gridManager.tiles.Add(blockTile.transform.position, blockTile);

            GameObject spawnTileObj = new GameObject("Spawn");
            Tile spawnTile = spawnTileObj.AddComponent<Tile>();
            spawnTile.transform.position = new Vector3(2, 0, 0);
            spawnTile.Init(gridManager, spawnTile.transform.position);
            tile.gridManager.tiles.Add(spawnTile.transform.position, spawnTile);

            tile.OnMouseEnter();

            Assert.AreEqual(new Color(1, 0, 0, 0.7f), tile.TileRenderer.color);
        }

        [Test]
        public void ShouldNotSetTileColor_WhenSpawnIsSelected_NoBlockBelow()
        {
            SpriteData selectedTile = new SpriteData
            {
                name = "Spawn",
                sprite = null,
                type = SpriteType.Spawn,
                trapType = TrapType.Empty
            };

            tile.uiHandler.currentTool = Tool.Brush;
            tile.uiHandler.currentView = View.Blocks;
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;

            tile.OnMouseEnter();

            Assert.AreEqual(new Color(1, 0, 0, 0.7f), tile.TileRenderer.color);
        }

        [Test]
        public void ShouldSetTileColor_WhenFinishIsSelected_NoOtherFinishes()
        {
            SpriteData selectedTile = new SpriteData
            {
                name = "Finish",
                sprite = null,
                type = SpriteType.Finish,
                trapType = TrapType.Empty
            };

            tile.uiHandler.currentTool = Tool.Brush;
            tile.uiHandler.currentView = View.Blocks;
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;

            GameObject blockTileObj = new GameObject("BlockTile");
            Tile blockTile = blockTileObj.AddComponent<Tile>();
            blockTile.transform.position = new Vector3(0, 0, 0);
            blockTile.Init(gridManager, blockTile.transform.position);
            blockTile.SpriteData = new SpriteData
            {
                name = "TestBlock",
                sprite = null,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };
            tile.gridManager.tiles.Add(blockTile.transform.position, blockTile);

            tile.OnMouseEnter();

            Assert.AreEqual(new Color(0, 1, 0, 0.7f), tile.TileRenderer.color);
        }

        [Test]
        public void ShouldNotSetTileColor_WhenFinishIsSelected_OtherFinishesExist()
        {
            SpriteData selectedTile = new SpriteData
            {
                name = "Finish",
                sprite = null,
                type = SpriteType.Finish,
                trapType = TrapType.Empty
            };

            tile.uiHandler.currentTool = Tool.Brush;
            tile.uiHandler.currentView = View.Blocks;
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;

            GameObject blockTileObj = new GameObject("BlockTile");
            Tile blockTile = blockTileObj.AddComponent<Tile>();
            blockTile.transform.position = new Vector3(0, 0, 0);
            blockTile.Init(gridManager, blockTile.transform.position);
            blockTile.SpriteData = new SpriteData
            {
                name = "TestBlock",
                sprite = null,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };
            tile.gridManager.tiles.Add(blockTile.transform.position, blockTile);

            GameObject finishTileObj = new GameObject("Finish");
            Tile finishTile = finishTileObj.AddComponent<Tile>();
            finishTile.transform.position = new Vector3(2, 0, 0);
            finishTile.Init(gridManager, finishTile.transform.position);
            tile.gridManager.tiles.Add(finishTile.transform.position, finishTile);

            tile.OnMouseEnter();

            Assert.AreEqual(new Color(1, 0, 0, 0.7f), tile.TileRenderer.color);
        }

        [Test]
        public void ShouldNotSetTileColor_WhenFinishIsSelected_NoBlockBelow()
        {
            SpriteData selectedTile = new SpriteData
            {
                name = "Finish",
                sprite = null,
                type = SpriteType.Finish,
                trapType = TrapType.Empty
            };

            tile.uiHandler.currentTool = Tool.Brush;
            tile.uiHandler.currentView = View.Blocks;
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;

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

            tile.OnMouseExit();

            Assert.AreEqual(new Color(1, 1, 1, 0), tile.TileRenderer.color);
        }

        [Test]
        public void ShouldResetTileColor_OnNonEmptyTile()
        {
            tile.SpriteData = new SpriteData
            {
                name = "TestTile",
                sprite = null,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };

            tile.OnMouseExit();

            Assert.AreEqual(new Color(1, 1, 1, 1), tile.TileRenderer.color);
        }
    }

    [TestFixture]
    public class OnMouseOver : MapeditorTileTest
    {
        [Test]
        public void ShouldNotSetSpriteData_WhenPaused()
        {
            PauseMenu.GameIsPaused = true;
            tile.canPlace = true;
            GridManager.isMouseDown = true;
            GridManager.hasSelectedSprite = true;

            SpriteData selectedTile = new SpriteData
            {
                name = "TestBlock",
                sprite = null,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };
            tile.uiHandler.currentView = View.Blocks;
            tile.uiHandler.currentTool = Tool.Brush;
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;

            tile.OnMouseOver();

            Assert.IsNull(tile.SpriteData);
        }

        [Test]
        public void ShouldNotSetSpriteData_WhenCannotPlace()
        {
            tile.canPlace = false;
            GridManager.isMouseDown = true;
            GridManager.hasSelectedSprite = true;

            SpriteData selectedTile = new SpriteData
            {
                name = "TestBlock",
                sprite = null,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };
            tile.uiHandler.currentView = View.Blocks;
            tile.uiHandler.currentTool = Tool.Brush;
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;

            tile.OnMouseOver();

            Assert.IsNull(tile.SpriteData);
        }

        [Test]
        public void ShouldNotSetSpriteData_WhenMouseNotDown()
        {
            tile.canPlace = true;
            GridManager.isMouseDown = false;
            GridManager.hasSelectedSprite = true;

            SpriteData selectedTile = new SpriteData
            {
                name = "TestBlock",
                sprite = null,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };
            tile.uiHandler.currentView = View.Blocks;
            tile.uiHandler.currentTool = Tool.Brush;
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;

            tile.OnMouseOver();

            Assert.IsNull(tile.SpriteData);
        }

        [Test]
        public void ShouldSetSpriteData_WhenBlockSpriteSelected()
        {
            tile.canPlace = true;
            GridManager.isMouseDown = true;
            GridManager.hasSelectedSprite = true;

            Texture2D texture = new Texture2D(16, 16);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), Vector2.zero);

            SpriteData selectedTile = new SpriteData
            {
                name = "TestBlock",
                sprite = sprite,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };
            tile.uiHandler.currentView = View.Blocks;
            tile.uiHandler.currentTool = Tool.Brush;
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;

            tile.OnMouseOver();

            Assert.AreEqual(selectedTile, tile.SpriteData);
            Assert.AreEqual(sprite, tile.TileRenderer.sprite);
        }

        [Test]
        public void ShouldNotSetSpriteData_WhenNoSpriteSelected()
        {
            tile.canPlace = true;
            GridManager.isMouseDown = true;
            GridManager.hasSelectedSprite = true;

            tile.uiHandler.currentView = View.Blocks;
            tile.uiHandler.currentTool = Tool.Brush;
            tile.gridManager.lastSelectedSprite[View.Blocks] = null;

            tile.OnMouseOver();

            Assert.IsNull(tile.SpriteData);
        }
    }

    [TestFixture]
    public class UseTool : MapeditorTileTest
    {
        [Test]
        public void ShouldSetAlphaTo05_WhenBrushTool_OnLayer0()
        {
            tile.uiHandler.currentTool = Tool.Brush;
            GridManager.currentLayer = 0;

            tile.UseTool();

            Assert.AreEqual(new Color(1, 1, 1, 0.5f), tile.TileRenderer.color);
        }

        [Test]
        public void ShouldSetAlphaTo1_WhenBrushTool_OnLayer1()
        {
            tile.uiHandler.currentTool = Tool.Brush;
            GridManager.currentLayer = 1;

            tile.UseTool();

            Assert.AreEqual(new Color(1, 1, 1, 1f), tile.TileRenderer.color);
        }

        [Test]
        public void ShouldSetSpriteDataToNull_WhenRubberSelected_OnBlockTile()
        {
            tile.canPlace = true;
            GridManager.isMouseDown = true;

            tile.uiHandler.currentView = View.Blocks;
            tile.uiHandler.currentTool = Tool.Rubber;

            tile.SpriteData = new SpriteData
            {
                name = "TestBlock",
                sprite = null,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };

            tile.UseTool();

            Assert.IsNull(tile.SpriteData);
        }
    }
}

[TestFixture]
public class InGameTileTest
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
        tile.inGame = true;

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

    [TestFixture]
    public class OnMouseEnter : InGameTileTest
    {
        [Test]
        public void ShouldNotHandleHover_WhenInGame()
        {
            SpriteData selectedTile = new SpriteData
            {
                name = "TestBlock",
                sprite = null,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };
            tile.uiHandler.currentView = View.Blocks;
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;
            
            var initialColor = tile.TileRenderer.color;

            tile.OnMouseEnter();

            Assert.AreEqual(initialColor, tile.TileRenderer.color);
        }
    }

    [TestFixture]
    public class OnMouseExit : InGameTileTest
    {
        [Test]
        public void ShouldNotResetColor_WhenInGame()
        {
            tile.TileRenderer.color = new Color(1, 0, 0, 0.5f);
            var initialColor = tile.TileRenderer.color;

            tile.OnMouseExit();

            Assert.AreEqual(initialColor, tile.TileRenderer.color);
        }
    }

    [TestFixture]
    public class OnMouseOver : InGameTileTest
    {
        [Test]
        public void ShouldNotPlaceTile_WhenInGame()
        {
            tile.canPlace = true;
            GridManager.isMouseDown = true;
            GridManager.hasSelectedSprite = true;
            tile.uiHandler.currentView = View.Blocks;
            tile.uiHandler.currentTool = Tool.Brush;

            SpriteData selectedTile = new SpriteData
            {
                name = "TestBlock",
                sprite = null,
                type = SpriteType.Block,
                trapType = TrapType.Empty
            };
            tile.gridManager.lastSelectedSprite[View.Blocks] = selectedTile;

            tile.OnMouseOver();

            Assert.IsNull(tile.SpriteData);
        }

        [Test]
        public void ShouldNotUseTool_WhenInGame()
        {
            tile.canPlace = true;
            GridManager.isMouseDown = true;
            GridManager.hasSelectedSprite = true;
            tile.uiHandler.currentView = View.Blocks;
            tile.uiHandler.currentTool = Tool.Brush;
            GridManager.currentLayer = 0;

            var initialColor = tile.TileRenderer.color;

            tile.OnMouseOver();

            Assert.AreEqual(initialColor, tile.TileRenderer.color);
        }
    }
}