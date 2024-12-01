using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Editor : Thing
{
    public static HashSet<string> Categories = new HashSet<string>() { };

    public Thing Target => Game.Root.Dynamic.Children?.FirstOrDefault();
    public Room Room;

    public Dictionary<int, HistoryStep> History = new Dictionary<int, HistoryStep>() { };
    public int HistoryIndex = 0;

    public Sprite Mouse;
    public Thing CameraTarget;
    Sprite gridMouse;
    Sprite previewBackground;
    Sprite preview;
    Text previewText;
    VerticalFlexControl verticalFlex;

    Vector2 cameraVelocity;

    public bool InteractDisabled => Input.Holdup || Game.GetThings<Room>().Where(r => r != Room).Any(r => r.Hovered);

    public Vector2 MousePosition;
    public Vector2 GridPosition;
    public Vector2 LastGridPosition;
    public Vector2? LastGridInteractPosition;
    HashSet<Vector2> recentGridInteractionPositions = new HashSet<Vector2>() { };

    List<Stamp> stamps = new List<Stamp>() { };
    public List<Tool> Tools = new List<Tool>() { };
    Tool tool = null;
    public Stamp Stamp = null;
    bool hoveringControl = false;
    bool focusedControl = false;

    public override void Init()
    {
        base.Init();

        RefreshStamps();
        Tools = new List<Tool>()
        {
            new BrushTool(this),
            new RectangleTool(this),
            new FillTool(this),
            new WandTool(this),
            
            new RoomTool(this),
            new EditTool(this),
            // new PickerTool(this),
            // new HandTool(this)
        };

        tool = new BrushTool(this);

        Raylib.HideCursor();

        DrawMode = DrawMode.Absolute;
        DrawOrder = 99;

        AddChild(new Drawer("Background", drawOrder: -200, action: d =>
        {
            d.DrawSprite(Library.Textures["Pixel"], position: new Vector2(-2), scale: new Vector2(1, 1088 * 4), color: PaletteBasic.Green, origin: new Vector2(0));
            d.DrawSprite(Library.Textures["Pixel"], position: new Vector2(-2), scale: new Vector2(1920 * 4, 1), color: PaletteBasic.Red, origin: new Vector2(0));
            d.DrawSprite(Library.Textures["Pixel"], position: new Vector2(-2), scale: new Vector2(1, 1), color: PaletteBasic.Yellow, origin: new Vector2(0));
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    d.DrawSprite(Library.Textures["EditorBackground"], origin: new Vector2(0), position: new Vector2(x * 1920, y * 1088));
                }
            }
        }));
        AddChild(new Drawer("Tool", action: d =>
        {
            if (tool != null) tool.Draw();
        }));

        if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Grid)
        {
            gridMouse = AddChild(new Sprite("Pixel", drawOrder: 100, scale: new Vector2(CONSTANTS.TILE_SIZE, CONSTANTS.TILE_SIZE))) as Sprite;
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Oblique)
        {
            gridMouse = AddChild(new Sprite($"{CONSTANTS.TILE_SIZE}ObliqueGridCursor", drawOrder: 100)) as Sprite;
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Isometric)
        {
            // TODO
        }
        gridMouse.Color = new Color(0, 255, 0, 50);

        Mouse = AddChild(new Sprite("Mouse", tileSize: 16, tileNumber: 0, drawOrder: 110, drawMode: DrawMode.Absolute)) as Sprite;

        Stamp = stamps.First();
        previewBackground = AddChild(new Sprite("Pixel", drawOrder: 108, color: PaletteBasic.Black, scale: new Vector2(18), drawMode: DrawMode.Absolute)) as Sprite;
        preview = AddChild(new Sprite(Stamp.Name, drawOrder: 109, tileSize: CONSTANTS.TILE_SIZE, color: CONSTANTS.PRIMARY_COLOR, drawMode: DrawMode.Absolute)) as Sprite;
        previewText = AddChild(new Text(Stamp.Name, position: new Vector2(0), Library.Font, color: CONSTANTS.PRIMARY_COLOR, outlineColor: PaletteBasic.Black, order: 111, drawMode: DrawMode.Absolute)) as Text;
        SelectStamp(Stamp);

        CameraTarget = AddChild(new Thing("CameraTarget"));

        Focus();

        AdjusterControl adjuster = new AdjusterControl();
        AddChild(adjuster);
        verticalFlex = new VerticalFlexControl();
        verticalFlex.DrawMode = DrawMode.Absolute;
        verticalFlex.Position = new Vector2(4, 16);
        verticalFlex.Spacing = 1;
        adjuster.AddChild(verticalFlex);
        TilesetUi();
        ToolsUi();
    }

    void TilesetUi()
    {
        int width = 64;

        DropdownControl categoryControl = new DropdownControl("", Categories.ToList());
        categoryControl.DrawMode = DrawMode.Absolute;
        categoryControl.Bounds.X = width;
        categoryControl.Bounds.Y = 16;
        categoryControl.Texture = Library.Textures["BoxInsideSlice"];
        categoryControl.DrawOrder = 20;
        categoryControl.HighlightColor = PaletteBasic.DarkGreen;
        categoryControl.Color = PaletteBasic.White;
        // categoryControl.TextPadding.X = 3;
        categoryControl.Padding = new Vector2(2);
        categoryControl.TextHighlightColor = PaletteBasic.White;
        categoryControl.Text = Categories.FirstOrDefault();
        verticalFlex.AddChild(categoryControl);
 
        ScrollableControl scrollable = new ScrollableControl(new Vector2(width + ScrollableControl.ScrollBarWidth + 1, 80), new Vector2(width + ScrollableControl.ScrollBarWidth + 1, 160));
        scrollable.DrawMode = DrawMode.Absolute;
        scrollable.Texture = Library.Textures["BoxThinSlice"];
        scrollable.TileSize = 5;
        scrollable.Padding = new Vector2(3);
        verticalFlex.AddChild(scrollable);
        
        GridFlexControl flex = new GridFlexControl();
        flex.DrawMode = DrawMode.Texture;
        flex.SubViewport = scrollable;
        flex.Bounds = new Vector2(width, 160);
        flex.Color = new Color(0, 0, 0, 1);
        AddChild(flex);

        string category = categoryControl.Text;

        Action refresh = () =>
        {
            flex.Clear();

            foreach (Stamp stamp in Library.ThingImports.Where(t => t.Value.Category?.ToLower() == category?.ToLower()).Select(t => new Stamp(t.Key)).ToList())
            {
                ButtonControl control = new ButtonControl();
                control.Pressed = () => 
                {
                    SelectStamp(stamp);
                };
                control.DrawOrder = 10;
                control.Texture = stamp.Texture;
                control.Bounds = new Vector2(16);
                control.Name = stamp.Name;
                flex.AddChild(control);
            }

            flex.Refresh();
        };
        refresh();

        categoryControl.LostFocus = () =>
        {
            category = categoryControl.Text;
            refresh();
        };
    }

    void ToolsUi()
    {
        int width = 64;

        GridFlexControl flex = new GridFlexControl();
        flex.DrawMode = DrawMode.Absolute;
        flex.Bounds = new Vector2(width, 32);
        flex.Texture = Library.Textures["BoxThinSlice"];
        flex.TileSize = 5;
        flex.Padding = new Vector2(3);
        verticalFlex.AddChild(flex);

        foreach (Tool tool in Tools)
        {
            ButtonControl control = new ButtonControl();
            control.Pressed = () => 
            {
                SelectTool(tool);
            };
            control.Texture = Library.Textures["Mouse"];
            control.Bounds = new Vector2(16);
            control.DrawOrder = 0;
            control.TileNumber = tool.TileNumber;
            control.TileSize = 16;
            flex.AddChild(control);
        }
    }

    public override void Update()
    {
        base.Update();

        if (Game.Root.DeveloperTools.Cli.Active) return;
        hoveringControl = Game.GetThings<Control>().Any(c => c.IsHovered || c.IsHeld);
        focusedControl = Control.TextInputControl != null;

        // Log.Write(Viewport.RelativeLayer.Camera.Zoom);
        TogglePreview(Input.IsMouseInsideWindow() && Room != null && tool?.ShowPreview == true && !focusedControl);
        UpdatePosition();
        if (focusedControl)
        {
            Mouse.TileNumber = 18;
            return;
        }
        UpdateCamera();
        UpdateShortcuts();
        LastGridPosition = GridPosition;
        if (hoveringControl)
        {
            Mouse.TileNumber = 17;
            return;
        }
        UpdateZoom();
        if (tool != null) tool.Update();
        UpdateCharacter();
    }

    void UpdateShortcuts()
    {
        Tools.ForEach(t =>
        {
            if (Input.IsPressed(t.Shortcut)) SelectTool(t);
        });
        
        if (!Input.ShiftIsHeld && Input.IsRepeating(KeyboardKey.Z))
        {
            Undo();
        }
        if (Input.ShiftIsHeld && Input.IsRepeating(KeyboardKey.Z))
        {
            Redo();
        }
    }

    void UpdatePosition()
    {
        MousePosition = Input.MousePositionRelative();
        Mouse.Position = Input.MousePositionAbsolute() + new Vector2(8);
        Mouse.SetVisible(Input.IsMouseInsideWindow());

        if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Grid)
        {
            GridPosition = (MousePosition - new Vector2(CONSTANTS.TILE_SIZE / 2)).ToNearest(CONSTANTS.TILE_SIZE) 
                + new Vector2(CONSTANTS.TILE_SIZE / 2);
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Oblique)
        {
            GridPosition.Y = (MousePosition.Y + (CONSTANTS.TILE_SIZE_THIRD / 2)).ToNearest(CONSTANTS.TILE_SIZE_THIRD) 
                + CONSTANTS.TILE_SIZE_THIRD / 2;

            if ((GridPosition.Y - (CONSTANTS.TILE_SIZE_THIRD / 2)) % (CONSTANTS.TILE_SIZE - CONSTANTS.TILE_SIZE_THIRD) == 0)
            {
                GridPosition.X = (MousePosition.X).ToNearest(CONSTANTS.TILE_SIZE_OBLIQUE)
                    - CONSTANTS.TILE_SIZE_THIRD / 2;

                GridPosition.X += CONSTANTS.TILE_SIZE_THIRD;
            }
            else
            {
                GridPosition.X = (MousePosition.X + (CONSTANTS.TILE_SIZE_THIRD / 2)).ToNearest(CONSTANTS.TILE_SIZE_OBLIQUE) 
                    - CONSTANTS.TILE_SIZE_THIRD / 2;
            }
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Isometric)
        {
            GridPosition = new Vector2(MousePosition.X.ToNearest(CONSTANTS.TILE_SIZE_HALF), MousePosition.Y.ToNearest(CONSTANTS.TILE_SIZE_QUARTER));
        }
        gridMouse.Position = GridPosition;
        gridMouse.SetVisible(!hoveringControl && Input.IsMouseInsideWindow() && Room != null && tool?.ShowPreview == true);

        preview.Position = preview.Position.MoveOverTime(Mouse.Position + new Vector2((previewBackground.Scale.X / 2f) + 8, -4), 0.00001f);
        previewText.Position = preview.Position + new Vector2(-7, preview.TileSize / 2f);
        previewBackground.Position = preview.Position;
    }

    void UpdateCamera()
    {
        float speed = 1200f / Viewport.RelativeLayer.Camera.Zoom;
        // float speed = 1200f / Viewport.Zoom;
        Vector2 input = Vector2.Zero;
        if (Input.IsHeld(KeyboardKey.W)) input.Y--;
        if (Input.IsHeld(KeyboardKey.S)) input.Y++;
        if (Input.IsHeld(KeyboardKey.A)) input.X--;
        if (Input.IsHeld(KeyboardKey.D)) input.X++;
        cameraVelocity = cameraVelocity.MoveOverTime(input * speed, 0.0001f);
        CameraTarget.Position += cameraVelocity * Time.Delta;
    }

    void UpdateZoom()
    {
        float scroll = Raylib.GetMouseWheelMove();
        if (scroll != 0)
        {
            // Select Stamp
            if (Input.AltIsHeld)
            {
                int index = stamps.IndexOf(Stamp);
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
                Viewport.Zoom = Math.Clamp(Viewport.Zoom * scaleFactor, 0.125f, 64.0f);
            }

        }
    }

    void UpdateCharacter()
    {
        if (Input.ShiftIsHeld)
        {
            if (Game.CharacterType != null)
            {
                Thing player = Game.Things.Find(t => t.TypeName == Game.CharacterType);
                if (player != null) player.Position = MousePosition;
            }
        }
    }

    public void Reset()
    {
        Room = null;
        History.Clear();
        HistoryIndex = 0;
    }

    public void Save(bool changeHistory = true)
    {
        if (Target?.Map != null) Target.Map.Save();
        if (Room?.Map != null) Room.Map.Save();
        if (changeHistory) IncrementHistory();
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

    public void Focus()
    {
        Room = Game.GetThing<Room>();
        if (Room != null)
        {
            CameraTarget.Position = Room.TopLeft;
            Viewport.Zoom = 0.5f;
            Viewport.RelativeLayer.Camera.Offset = Vector2.Zero;
        }
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
            return Chance.Range(0, (Stamp.Texture.Width / CONSTANTS.TILE_SIZE));
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

            MapCell cell = Room.Map.GetCell(c.Position, c.Import.Layer);
            if (cell == null || cell.Name != c.Name)
            {
                AddCell(c.Position + Room.Position, c.Name, c.Import.Layer);
            }
        });
    }

    public void AddCell(MapCell cell, bool changeHistory = true, Room room = null) => AddCell(cell.Position + (room ?? Room).Position, cell.Name, cell.Import.Layer, changeHistory, room);
    public void AddCell(Vector2 position, bool changeHistory = true, Room room = null) => AddCell(position, Stamp.Name, Stamp.Import.Layer, changeHistory, room);
    public void AddCell(Vector2 position, string name, int layer, bool changeHistory = true, Room room = null)
    {
        if (room == null) room = Room;

        if (!room.IsPositionInside(position)) return;

        new ParticleEffect(
            position: position, 
            amount: 2, 
            texture: Library.Textures.Get("5Star"),
            dampening: 0.001f,
            speed: 5,
            time: 0.3f,
            color: PaletteAapSplendor128.NightlyAurora
        );

        RemoveCell(position, layer, changeHistory);
        position -= room.Position;

        MapCell cell = new MapCell(
            name: name,
            position: position,
            tileNumber: 0,
            parent: room.Name
        );
        room.Map.AddCell(cell);
        cell.TileNumber = GetTileNumber(cell);
        cell.Create(room);

        recentGridInteractionPositions.AddRange(PositionWithNeighbors(position));
        // LastGridInteractPosition = position;
        if (changeHistory)
        {
            Vector2 storedRoomPosition = room.Position;
            Room storedRoom = room;
            AddHistoryAction(new HistoryAction(
                "AddCell",
                () =>
                {
                    if (storedRoom.GlobalRemoved) storedRoom = Game.GetThings<Room>().Find(t => t.Name == storedRoom.Name);
                    RemoveCell(position + storedRoomPosition, changeHistory: false, room: storedRoom);
                },
                () =>
                {
                    if (storedRoom.GlobalRemoved) storedRoom = Game.GetThings<Room>().Find(t => t.Name == storedRoom.Name);
                    AddCell(position + storedRoomPosition, name, layer, changeHistory: false, room: storedRoom);
                }
            ));
        }
    }


    public void RemoveCell(MapCell cell, bool changeHistory = true, Room room = null) => RemoveCell(cell.Position + (room ?? Room).Position, cell.Import.Layer, changeHistory, room);
    public void RemoveCell(Vector2 position, int? layer = null, bool changeHistory = true, Room room = null)
    {
        if (room == null) room = Room;

        position -= room.Position;

        MapCell cell = room.Map.GetCell(position, layer);
        if (cell == null) return;

        if (cell.Import.Thing == "BakedTilemap")
        {
            BakedTilemap bakedTilemap = layer != null
                ? room.GetThings<BakedTilemap>().Find(b => b.Layer == layer)
                : room.GetThings<BakedTilemap>().Where(b => b.Tiles.Any(t => t.Position == position)).OrderByDescending(b => b.Layer).FirstOrDefault();

            if (bakedTilemap != null) bakedTilemap.RemoveTile(position);
        }
        else
        {
            room.Children.Where(t => t.Cell?.Name == cell.Name && t.Cell?.Position == cell.Position).ToList().ForEach(t => t.Destroy());
        }
        room.Map.RemoveCell(cell);

        recentGridInteractionPositions.AddRange(PositionWithNeighbors(position));
        // LastGridInteractPosition = position;

        if (changeHistory)
        {
            Vector2 storedRoomPosition = room.Position;
            Room storedRoom = room;
            AddHistoryAction(new HistoryAction(
                "RemoveCell",
                () =>
                {
                    if (storedRoom.GlobalRemoved) storedRoom = Game.GetThings<Room>().Find(t => t.Name == storedRoom.Name);
                    AddCell(cell.Position + storedRoomPosition, cell.Name, cell.Import.Layer, changeHistory: false, room: storedRoom);
                },
                () => 
                {
                    if (storedRoom.GlobalRemoved) storedRoom = Game.GetThings<Room>().Find(t => t.Name == storedRoom.Name);
                    RemoveCell(position + storedRoomPosition, layer, changeHistory: false, room: storedRoom);
                }
            ));
        }
    }

    public void PickCell(Vector2 position)
    {
        position -= Room.Position;

        MapCell cell = Room.Map.GetCell(position);
        if (cell == null) return;

        SelectStamp(stamps.Find(s => s.Name == cell.Name));
    }

    public void SelectTool(Tool tool)
    {
        this.tool.Exit();
        this.tool = tool;
        // Mouse.TileNumber = tool.TileNumber;
    }

    public void SelectStamp(Stamp stamp)
    {
        this.Stamp = stamp;
        preview.Texture = stamp.Texture;
        previewText.Content = stamp.Name;

        preview.TileSize = Library.ThingImports[stamp.Name].TileSize ?? stamp.Texture.Height;
        previewBackground.Scale = new Vector2(preview.TileSize + 2);
    }

    public void AddHistoryAction(HistoryAction historyAction)
    {
        History = History.Where(h => h.Key <= HistoryIndex).ToDictionary(x => x.Key, x => x.Value);

        if (!History.ContainsKey(HistoryIndex)) History[HistoryIndex] = new HistoryStep();

        if (History[HistoryIndex].Committed) History[HistoryIndex].Clear();
        History[HistoryIndex].Actions.Add(historyAction);
    }

    public void IncrementHistory()
    {
        if (History.Get(HistoryIndex) == null || History[HistoryIndex].Actions.Count <= 0)
        {
            return;
        }
        History[HistoryIndex].Committed = true;
        HistoryIndex++;

        int highest = History.Select(h => h.Key).OrderBy(h => h).LastOrDefault();
        int lowest = History.Select(h => h.Key).OrderBy(h => h).FirstOrDefault();
        if (History.Count <= 0) HistoryIndex = 0;
        else if (HistoryIndex > highest + 1) HistoryIndex = highest + 1;
        else if (HistoryIndex < lowest) HistoryIndex = lowest;

        if (History.Count > 0 && History.ContainsKey(HistoryIndex)) History[HistoryIndex].Actions.Clear();
    }

    public void Undo()
    {
        HistoryIndex--;
        if (HistoryIndex < 0)
        {
            HistoryIndex = 0;
            return;
        } 
        if (History.Count <= 0)
        {
            return;
        }

        int highest = History.Select(h => h.Key).OrderBy(h => h).LastOrDefault();
        if (HistoryIndex > highest) HistoryIndex = highest;

        History[HistoryIndex].Actions.Reversed().ToList().ForEach(x => x.Undo());

        Save(false);
    }

    public void Redo()
    {
        int highest = History.Select(h => h.Key).OrderBy(h => h).LastOrDefault();

        if (HistoryIndex > highest || History.Count <= 0)
        {
            // Nothing to redo
            return;
        }

        int lowest = History.Select(h => h.Key).OrderBy(h => h).FirstOrDefault();
        if (HistoryIndex < lowest) HistoryIndex = lowest;

        History[HistoryIndex].Actions.ForEach(x => x.Redo());

        HistoryIndex++;
        if (HistoryIndex > highest + 1) HistoryIndex = highest + 1;

        Save(false);
    }

    public override void Draw()
    {
        base.Draw();

        if (Target?.Map != null)
        {
            DrawText(
                $"{Target?.Map?.Name}{(Room?.Map != null ? $", {Room.Map.Name}" : "")}", 
                Viewport.Adjust(new Vector2(4, 2), AdjustFrom.TopLeft), 
                color: PaletteBasic.White, 
                outlineColor: PaletteBasic.Black, 
                font: Library.FontSmall
            );
        }

        DrawText(
            text: $"{tool?.Name} {GridPosition}", 
            position: Viewport.Adjust(new Vector2(4, CONSTANTS.VIRTUAL_HEIGHT - Library.FontSmall.BaseSize - 4), AdjustFrom.BottomLeft), 
            color: PaletteBasic.White, 
            outlineColor: PaletteBasic.Black, 
            font: Library.FontSmall
        );
        // DrawText($"{HistoryIndex}: <{History.ToList().Select(h => (h.Value.Committed ? "*" : "") + h.Key).Join(", ")}>", new Vector2(4, CONSTANTS.VIRTUAL_HEIGHT - (Library.FontSmall.BaseSize + 4) * 2), color: PaletteBasic.White, outlineColor: PaletteBasic.Black, font: Library.FontSmall);
    }
}
