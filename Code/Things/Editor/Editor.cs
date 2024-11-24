using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Editor : Thing
{
    public Thing Target => Game.Root.Dynamic.Children?.FirstOrDefault();
    public Room Room;

    public Sprite Mouse;
    public Thing CameraTarget;
    Sprite gridMouse;
    Sprite previewBackground;
    Sprite preview;
    Text previewText;

    Vector2 cameraVelocity;

    public bool Holdup = false;
    public bool InteractDisabled => Holdup || Game.GetThings<Room>().Where(r => r != Room).Any(r => r.Hovered);

    public Vector2 MousePosition;
    public Vector2 GridPosition;
    Vector2 lastGridPosition;
    public Vector2? LastGridInteractPosition;
    HashSet<Vector2> recentGridInteractionPositions = new HashSet<Vector2>() { };

    List<Stamp> stamps = new List<Stamp>() { };

    Tool tool = null;
    Stamp stamp = null;

    public override void Init()
    {
        base.Init();

        RefreshStamps();

        tool = new BrushTool(this);

        Raylib.HideCursor();

        DrawMode = DrawMode.Absolute;
        DrawOrder = 99;

        AddChild(new Drawer("Background", drawOrder: -200, action: d =>
        {
            d.DrawSprite(Library.Textures["Pixel"], position: new Vector2(-2), scale: new Vector2(1, 1080 * 4), color: PaletteBasic.Green, origin: new Vector2(0));
            d.DrawSprite(Library.Textures["Pixel"], position: new Vector2(-2), scale: new Vector2(1920 * 4, 1), color: PaletteBasic.Red, origin: new Vector2(0));
            d.DrawSprite(Library.Textures["Pixel"], position: new Vector2(-2), scale: new Vector2(1, 1), color: PaletteBasic.Yellow, origin: new Vector2(0));
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    d.DrawSprite(Library.Textures["EditorBackground"], origin: new Vector2(0), position: new Vector2(x * 1920, y * 1080));
                }
            }
        }));
        AddChild(new Drawer("Tool", action: d =>
        {
            if (tool != null) tool.Draw();
        }));

        if (Game.ProjectionType == ProjectionType.Grid)
        {
            gridMouse = AddChild(new Sprite("Pixel", drawOrder: 100, scale: new Vector2(CONSTANTS.TILE_SIZE, CONSTANTS.TILE_SIZE))) as Sprite;
        }
        else if (Game.ProjectionType == ProjectionType.Oblique)
        {
            gridMouse = AddChild(new Sprite($"{CONSTANTS.TILE_SIZE}ObliqueGridCursor", drawOrder: 100)) as Sprite;
        }
        else if (Game.ProjectionType == ProjectionType.Isometric)
        {
            // TODO
        }
        gridMouse.Color = new Color(0, 255, 0, 50);

        Mouse = AddChild(new Sprite("Mouse", tileSize: 16, tileNumber: 0, drawOrder: 110, drawMode: DrawMode.Absolute, adjustFrom: AdjustFrom.None)) as Sprite;

        stamp = stamps.First();
        previewBackground = AddChild(new Sprite("Pixel", drawOrder: 108, color: PaletteBasic.Black, scale: new Vector2(18), drawMode: DrawMode.Absolute, adjustFrom: AdjustFrom.None)) as Sprite;
        preview = AddChild(new Sprite(stamp.Name, drawOrder: 109, tileSize: CONSTANTS.TILE_SIZE, color: CONSTANTS.PRIMARY_COLOR, drawMode: DrawMode.Absolute, adjustFrom: AdjustFrom.None)) as Sprite;
        previewText = AddChild(new Text(stamp.Name, position: new Vector2(0), Library.Font, color: CONSTANTS.PRIMARY_COLOR, outlineColor: PaletteBasic.Black, order: 111, drawMode: DrawMode.Absolute, adjustFrom: AdjustFrom.None)) as Text;
        SelectStamp(stamp);

        CameraTarget = AddChild(new Thing("CameraTarget"));
    }

    public override void Update()
    {
        base.Update();

        if (Game.Root.DeveloperTools.Cli.Active) return;

        // Log.Write(Viewport.RelativeLayer.Camera.Zoom);
        TogglePreview(Input.IsMouseInsideWindow() && Room != null);

        if (Input.IsPressed(KeyboardKey.B))
        {
            tool = new BrushTool(this);
        }
        else if (Input.IsPressed(KeyboardKey.U))
        {
            tool = new RectangleTool(this);
        }
        else if (Input.IsPressed(KeyboardKey.I))
        {
            tool = new PickerTool(this);
        }
        else if (Input.IsPressed(KeyboardKey.R))
        {
            tool = new RoomTool(this);
        }

        if (tool != null) tool.Update();

        if (!Input.LeftMouseButtonIsHeld)
        {
            Holdup = false;
        }

        MousePosition = Input.MousePositionRelative();
        Mouse.Position = Input.MousePositionAbsolute() + new Vector2(8);
        Mouse.SetVisible(Input.IsMouseInsideWindow());

        if (Game.ProjectionType == ProjectionType.Isometric)
        {
            GridPosition = new Vector2(MousePosition.X.ToNearest(CONSTANTS.TILE_SIZE_HALF), MousePosition.Y.ToNearest(CONSTANTS.TILE_SIZE_QUARTER));
        }
        else if (Game.ProjectionType == ProjectionType.Oblique)
        {
            // 48
            // GridPosition.X = MousePosition.X.ToNearest(CONSTANTS.TILE_SIZE - CONSTANTS.TILE_SIZE_THIRD) - (CONSTANTS.TILE_SIZE_THIRD / 2);
            // GridPosition.Y = MousePosition.Y.ToNearest(CONSTANTS.TILE_SIZE_THIRD) + (CONSTANTS.TILE_SIZE_THIRD / 2);
            // if ((GridPosition.Y - (CONSTANTS.TILE_SIZE_THIRD / 2)) % (CONSTANTS.TILE_SIZE - CONSTANTS.TILE_SIZE_THIRD) == 0)
            // {
            //     GridPosition.X += CONSTANTS.TILE_SIZE_THIRD;
            // }
            GridPosition.X = MousePosition.X.ToNearest(CONSTANTS.TILE_SIZE_OBLIQUE)
                - CONSTANTS.TILE_SIZE_THIRD / 2;
            GridPosition.Y = MousePosition.Y.ToNearest(CONSTANTS.TILE_SIZE_THIRD) 
                + CONSTANTS.TILE_SIZE_THIRD / 2;
            if ((GridPosition.Y - (CONSTANTS.TILE_SIZE_THIRD / 2)) % (CONSTANTS.TILE_SIZE - CONSTANTS.TILE_SIZE_THIRD) == 0)
            {
                GridPosition.X += CONSTANTS.TILE_SIZE_THIRD;
            }
        }
        else
        {
            GridPosition = MousePosition.ToNearest(CONSTANTS.TILE_SIZE);
        }
        gridMouse.Position = GridPosition;
        gridMouse.SetVisible(Input.IsMouseInsideWindow() && Room != null);

        preview.Position = preview.Position.MoveOverTime(Mouse.Position + new Vector2((previewBackground.Scale.X / 2f) + 8, -4), 0.00001f);
        previewText.Position = preview.Position + new Vector2(-7, preview.TileSize / 2f);
        previewBackground.Position = preview.Position;

        float speed = 1200f / Viewport.RelativeLayer.Camera.Zoom;
        // float speed = 1200f / Viewport.Zoom;
        Vector2 input = Vector2.Zero;
        if (Input.IsHeld(KeyboardKey.W)) input.Y--;
        if (Input.IsHeld(KeyboardKey.S)) input.Y++;
        if (Input.IsHeld(KeyboardKey.A)) input.X--;
        if (Input.IsHeld(KeyboardKey.D)) input.X++;
        cameraVelocity = cameraVelocity.MoveOverTime(input * speed, 0.0001f);
        CameraTarget.Position += cameraVelocity * Time.Delta;

        float scroll = Raylib.GetMouseWheelMove();
        if (scroll != 0)
        {
            // Select Stamp
            if (Input.AltIsHeld)
            {
                int index = stamps.IndexOf(stamp);
                index += (int)scroll.Sign();
                if (index < 0) index = stamps.Count - 1;
                if (index > stamps.Count - 1) index = 0;
                SelectStamp(stamps[index]);
            }
            // Zoom in/out
            else
            {
                Viewport.RelativeLayer.Camera.Offset = Raylib.GetMousePosition() - (Viewport.Margin * Viewport.VirtualRatio.Value);
                CameraTarget.Position = MousePosition + new Vector2(
                    CONSTANTS.VIRTUAL_WIDTH / 2f,
                    CONSTANTS.VIRTUAL_HEIGHT / 2f
                );

                float scaleFactor = 1.0f + (0.25f * scroll.Abs());
                if (scroll < 0) scaleFactor = 1.0f / scaleFactor;
                Viewport.Zoom = Math.Clamp(Viewport.Zoom * scaleFactor, 0.25f, 64.0f);
            }

        }
        lastGridPosition = GridPosition;

        if (Input.ShiftIsHeld)
        {
            if (Game.CharacterType != null)
            {
                Thing player = Game.Things.Find(t => t.TypeName == Game.CharacterType);
                if (player != null) player.Position = MousePosition;
            }
        }
    }

    public void Save()
    {
        if (Target?.Map != null) Target.Map.Save();
        if (Room?.Map != null) Room.Map.Save();
    }

    public void TogglePreview(bool show)
    {
        preview.SetVisible(show);
        previewBackground.SetVisible(show);
        previewText.SetVisible(show);
    }

    public void RefreshStamps()
    {
        stamps = Library.ThingImports.Select(t => new Stamp(t.Key)).ToList();
    }

    public int GetTileNumber(MapCell cell)
    {
        if (cell.Import.StampType.Contains("Auto16"))
        {
            return AutoTilemap.AutoTilemap16.GetTileNumber(cell) ?? 0;
        }
        else if (cell.Import.StampType.Contains("AutoWall"))
        {
            return AutoTilemap.AutoTilemapWall.GetTileNumber(cell) ?? 0;
        }
        else
        {
            return Chance.Range(0, (stamp.Texture.Width / CONSTANTS.TILE_SIZE));
        }
    }

    public List<Vector2> PositionWithNeighbors(Vector2 position) => new List<Vector2>() { position }.Concat(Utility.Angles.Select(a => position + (a * CONSTANTS.TILE_SIZE))).ToList();

    public void RefreshAutoTilemaps(Vector2 position)
        => RefreshAutoTilemaps(new List<Vector2>() { position }.Concat(Utility.Angles.Select(a => position + (a * CONSTANTS.TILE_SIZE))).ToList());
    public void RefreshAutoTilemaps()
    {
        RefreshAutoTilemaps(recentGridInteractionPositions.ToList());
        recentGridInteractionPositions.Clear();
    }
    public void RefreshAutoTilemaps(List<Vector2> positions)
    {
        if (Room == null) return;
        
        Room.Map.Cells.ToList().ForEach(c =>
        {
            if (positions != null && !positions.Contains(c.Position)) return;

            AddCell(c);
        });
    }

    public void AddCell(MapCell cell) => AddCell(cell.Position + Room.Position, cell.Name, cell.Import.Layer);

    public void AddCell(Vector2 position) => AddCell(position, stamp.Name, stamp.Import.Layer);

    public void AddCell(Vector2 position, string name, int layer)
    {
        RemoveCell(position, layer);
        position -= Room.Position;

        MapCell cell = new MapCell(
            name: name,
            position: position,
            tileNumber: 0,
            parent: Room.Name
        );
        Room.Map.AddCell(cell);
        cell.TileNumber = GetTileNumber(cell);
        cell.Create(Room);

        recentGridInteractionPositions.AddRange(PositionWithNeighbors(position));
        // LastGridInteractPosition = position;
    }

    public void RemoveCell(Vector2 position, int? layer = null)
    {
        position -= Room.Position;

        MapCell cell = Room.Map.GetCell(position, layer);
        if (cell == null) return;

        if (cell.Import.Thing == "BakedTilemap")
        {
            BakedTilemap bakedTilemap = layer != null
                ? Room.GetThings<BakedTilemap>().Find(b => b.Layer == layer)
                : Room.GetThings<BakedTilemap>().Where(b => b.Tiles.Any(t => t.Position == position)).OrderByDescending(b => b.Layer).FirstOrDefault();

            if (bakedTilemap != null) bakedTilemap.RemoveTile(position);
        }
        else
        {
            Room.Children.Where(t => t.Cell?.Name == cell.Name && t.Cell?.Position == cell.Position).ToList().ForEach(t => t.Destroy());
        }
        Room.Map.RemoveCell(cell);

        recentGridInteractionPositions.AddRange(PositionWithNeighbors(position));
        // LastGridInteractPosition = position;
    }

    public void PickCell(Vector2 position)
    {
        position -= Room.Position;

        MapCell cell = Room.Map.GetCell(position);
        if (cell == null) return;

        SelectStamp(stamps.Find(s => s.Name == cell.Name));
    }

    public void SelectStamp(Stamp stamp)
    {
        this.stamp = stamp;
        preview.Texture = stamp.Texture;
        previewText.Content = stamp.Name;

        preview.TileSize = Library.ThingImports[stamp.Name].TileSize ?? stamp.Texture.Height;
        previewBackground.Scale = new Vector2(preview.TileSize + 2);
    }

    public override void Draw()
    {
        base.Draw();

        if (Target?.Map != null) DrawText($"Map: {Target?.Map?.Name}.map", new Vector2(4, 2), color: PaletteBasic.White, outlineColor: PaletteBasic.Black, font: Library.FontSmall);
        if (Room?.Map != null) DrawText($"Room: {Room?.Map?.Name}.map", new Vector2(4, 2 + Library.FontSmall.BaseSize * 2), color: PaletteBasic.White, outlineColor: PaletteBasic.Black, font: Library.FontSmall);
        DrawText($"{GridPosition}", new Vector2(4, CONSTANTS.VIRTUAL_HEIGHT - Library.FontSmall.BaseSize - 4), color: PaletteBasic.White, outlineColor: PaletteBasic.Black, font: Library.FontSmall);
    }
}
