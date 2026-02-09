# Sweet Pain — 2D Platformer with Map Editor (Unity)

A Windows-only 2D pixel-art platformer featuring a full in-game map editor, online leaderboard, and FastAPI backend integration. Built with Unity 2022.3.63f2 LTS.

## Overview
- Create, save, and share custom maps.
- Play community maps with dynamic traps and pixel-art visuals.
- Track deaths per map via an online leaderboard.
- Client–Server architecture: Unity client, FastAPI backend, PostgreSQL database, NGINX static hosting.

## Features
- Map Editor with layers:
  - Playfield and Background editing.
  - Blocks, backgrounds, and traps.
- Traps
  - Spike (directional, placement rules).
  - Saw (requires rail network; speed configurable).
  - Cannon (directional or targeting player; reload and bullet speed).
  - Axe/Bard (swing or rotate; speed configurable).
  - Guillotine (crush/up/reload timing; auto distance calculation).
- Rail system
  - Auto-typed sprites per neighbors, locking to prevent unintended changes.
- Save/Load
  - Requires exactly one Spawn and one Finish; auto-screenshot generated.
- Leaderboard
  - Stores per-map death counts; updates on completion.

## System Requirements
- OS: Windows 11 (64-bit).
- CPU: 2+ cores, 64-bit.
- RAM: 4 GB.
- GPU: DirectX 12-compatible (WDDM 2.0).
- Storage: ~85 MB.
- Display: 1920×1080, 16:9.
- Network: Required for online features.

## Installation

### Option A: Play the Release
1. Download SweetPain.exe from:
   https://github.com/grobi447/Thesis-Client/releases/
2. On first run, Windows Defender may warn due to missing code signing. Use “More info” → “Run anyway”.

### Option B: Build From Source (Unity)
- Install Unity Hub and Unity 2022.3.63f2 LTS.
- Open the project folder “Thesis-Client” in Unity.
- Scenes:
  - MainMenu (Assets/Scenes/MainMenu.unity)
  - MapEditor (Assets/Scenes/MapEditor.unity)
  - InGame (Assets/Scenes/InGame.unity)
- Build target: Windows 64-bit.

## Controls
- Move: A/D or Left/Right arrows.
- Jump: W or Up arrow (hold for 2-block jump, tap for 1-block jump).
- Crouch: S or Down arrow.
- Pause: ESC.

## Usage

### Map Editor
- Views: Sky, Block, Trap; cycle via arrows in the left panel.
- Layers: Playfield (game layer) and Background (visual layer).
- Tools: Brush, Eraser; Trap Settings tool (Trap view).
- Placement rules:
  - Spawn and Finish must be on top of blocks; only one of each.
  - Spikes require a neighboring block in the correct direction.
  - Saw requires rails; rail painting on empty cells only.
- Save Map:
  - Provide Name.
  - Ensure Spawn + Finish exist.
  - Auto-screenshot is created for the selector.

### Play Maps
- Press PLAY in Main Menu → select a map from the list.
- Leaderboard shows deaths for the selected map.
- Completion fades to loading and proceeds to next map.

## Project Structure (key folders)
- Assets/
  - Scenes/ — MainMenu, MapEditor, InGame.
  - Prefabs/ — Player, traps, tiles, rails, bullets.
  - Scripts/ — UI, MapEditor, InGame, Objects, API, Managers.
  - Sprites/ and Aseprite/ — art assets.
  - Maps/ — local map JSON and screenshots.
- Documentation/
  - html/ and pdf/ — generated Doxygen client docs.

## Backend Integration
- FastAPI endpoints (examples used by the client):
  - GET /download/{map_id} — fetch map preview image.
  - POST /register/ — create user with validation and bcrypt hashing.
  - PATCH /leaderboard/current/{map_id}/{user} — update current deaths.
- NGINX serves static uploads; images named by map_id.
- See Thesis-Backend README for setup.

## Development Notes
- API calls use UnityWebRequest with a certificate handler (self-signed HTTPS).
- Data model: JSON map data, trap settings, and assets per map folder.
- Singletons:
  - UserManager — current user state.
  - MapManager — active map id and installed maps.

## Testing
- Editor tests: Tile behavior, hover logic, trap base class, UI states.
- PlayMode tests: timed trap activation, collider toggling, timers, respawn.

## Documentation
- Full thesis summary and developer docs are in Thesis.pdf (root) and client Doxygen:
  - Thesis-Client/Documentation/html/
  - Thesis-Client/Documentation/pdf/
