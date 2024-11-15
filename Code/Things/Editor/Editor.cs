using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Stamp
{
    public string Name;
    public Texture2D Texture;
    public ThingImport Import;

    public Stamp() { }
    public Stamp(string name)
    {
        Name = name;
        Texture = Library.Textures[name];
        Import = Library.ThingImports[name];
    }
}

public class Tool
{
    protected Editor editor;
    protected Vector2? dragCameraPosition = null;
    protected Vector2? dragMousePosition = null;

    public Tool(Editor editor)
    {
        this.editor = editor;
    }

    public virtual void Update()
    {
        if (Input.LeftMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition)
        {
            editor.Mouse.ShakeY();
            editor.LastGridInteractPosition = editor.GridPosition;
        }

        if (Input.RightMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition)
        {
            editor.Mouse.ShakeX();
            editor.LastGridInteractPosition = editor.GridPosition;
        }

        if (Input.LeftMouseButtonIsReleased || Input.RightMouseButtonIsReleased)
        {
            editor.LastGridInteractPosition = null;
            editor.RefreshAutoTilemaps();
            Game.Root.Zone?.Map?.Save();
            Game.Root.Room?.Map?.Save();
        }
    }

    public void Picker()
    {
        editor.Mouse.TileNumber = 2;

        if (Input.LeftMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition)
        {
            editor.PickCell(editor.GridPosition);
        }
    }

    public void Move()
    {
        editor.Mouse.TileNumber = 5;

        if (dragMousePosition == null)
        {
            dragMousePosition = (Raylib.GetMousePosition() / Viewport.RelativeLayer.Camera.Zoom);
            dragCameraPosition = editor.CameraTarget.Position;
        }
        else
        {
            Vector2 difference = dragMousePosition.Value - (Raylib.GetMousePosition() / Viewport.RelativeLayer.Camera.Zoom);
            editor.CameraTarget.Position = dragCameraPosition.Value + difference;
        }
    }

    public virtual void Draw()
    {

    }
}

public class BrushTool : Tool
{
    public BrushTool(Editor editor) : base(editor)
    {

    }

    public override void Update()
    {
        if (Input.AltIsHeld)
        {
            Picker();
            return;
        }

        if (Input.MiddleMouseButtonIsHeld)
        {
            Move();
            return;
        }

        editor.Mouse.TileNumber = 3;

        dragMousePosition = null;
        dragCameraPosition = null;

        if (Game.Root.Room == null)
        {
            editor.TogglePreview(false);
            return;
        }

        editor.TogglePreview(true);

        if (editor.InteractDisabled) return;

        if (Input.LeftMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition)
        {
            editor.AddCell(editor.GridPosition);
        }

        if (Input.RightMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition)
        {
            editor.RemoveCell(editor.GridPosition);
        }

        base.Update();
    }

    public override void Draw()
    {
        base.Draw();
    }
}

public class RectangleTool : Tool
{
    Vector2? start = null;
    bool erase = false;
    List<Vector2> positions = new List<Vector2>() { };
    Texture2D square = Library.Textures[$"{CONSTANTS.TILE_SIZE}White"];

    public RectangleTool(Editor editor) : base(editor)
    {

    }

    public override void Update()
    {
        if (Input.AltIsHeld)
        {
            Picker();
            return;
        }

        if (Input.MiddleMouseButtonIsHeld)
        {
            Move();
            return;
        }

        dragMousePosition = null;
        dragCameraPosition = null;

        editor.Mouse.TileNumber = 12;

        if (Game.Root.Room == null)
        {
            editor.TogglePreview(false);
            return;
        }

        editor.TogglePreview(true);

        if (editor.InteractDisabled) return;

        if (Input.LeftMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition && start == null)
        {
            start = editor.GridPosition;
            erase = false;
        }

        if (Input.RightMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition && start == null)
        {
            start = editor.GridPosition;
            erase = true;
        }

        if (start != null)
        {
            positions.Clear();
            Vector2 distance = editor.GridPosition - start.Value;
            int row = (int)(distance.X / CONSTANTS.TILE_SIZE).Abs();
            int column = (int)(distance.Y / CONSTANTS.TILE_SIZE).Abs();
            for (int r = 0; r <= row; r++)
            {
                for (int c = 0; c <= column; c++)
                {
                    positions.Add(start.Value + new Vector2(r * CONSTANTS.TILE_SIZE * distance.X.Sign(), c * CONSTANTS.TILE_SIZE * distance.Y.Sign()));
                }
            }
        }

        if (Input.LeftMouseButtonIsReleased && start != null && !erase)
        {
            positions.ForEach(p =>
            {
                editor.AddCell(p);
            });
        }

        if (Input.RightMouseButtonIsReleased && start != null && erase)
        {
            positions.ForEach(p =>
            {
                editor.RemoveCell(p);
            });
        }

        if (Input.LeftMouseButtonIsReleased || Input.RightMouseButtonIsReleased)
        {
            editor.LastGridInteractPosition = null;
            start = null;
            erase = false;
            positions.Clear();
        }

        base.Update();
    }

    public override void Draw()
    {
        base.Draw();

        if (start != null)
        {
            positions.ForEach(p =>
            {
                Shapes.DrawSprite(texture: square, position: p, color: erase ? new Color(0, 0, 0, 150) : CONSTANTS.PRIMARY_COLOR);
            });
        }
    }
}

public class PickerTool : Tool
{
    public PickerTool(Editor editor) : base(editor)
    {

    }

    public override void Update()
    {
        if (Input.MiddleMouseButtonIsHeld)
        {
            Move();
            return;
        }

        dragMousePosition = null;
        dragCameraPosition = null;

        editor.Mouse.TileNumber = 2;

        if (Game.Root.Room == null)
        {
            editor.TogglePreview(false);
            return;
        }

        Picker();

        editor.TogglePreview(true);

        base.Update();
    }

    public override void Draw()
    {
        base.Draw();
    }
}

public class RoomTool : Tool
{
    Vector2? moveMouseStart = null;
    Vector2? moveRoomStart = null;

    public RoomTool(Editor editor) : base(editor)
    {

    }

    public override void Update()
    {
        editor.TogglePreview(false);

        if (Input.AltIsHeld)
        {
            Picker();
            return;
        }

        if (Input.MiddleMouseButtonIsHeld)
        {
            Move();
            return;
        }

        dragMousePosition = null;
        dragCameraPosition = null;

        editor.Mouse.TileNumber = 7;

        if (editor.InteractDisabled) return;

        if (Input.RightMouseButtonIsPressed)
        {
            if (Game.Root.Room == null) return;
            Game.Root.Zone.Map.RemoveCell(Game.Root.Room.Cell);
            Game.Root.Room.Destroy();
            Game.Root.Room = null;
        }

        if (Input.CtrlIsHeld)
        {
            editor.Mouse.TileNumber = 8;

            moveRoomStart = null;
            moveMouseStart = null;

            if (Input.LeftMouseButtonIsPressed)
            {
                Thing child = Game.Root.Zone.Children.OrderByDescending(c => c.Name).FirstOrDefault();
                string index = child == null ? "000" : (child.Name.Split("_").Last().ToInt() + 1).ToString().PadLeft(3, '0');
                string name = $"{Game.Root.Zone.Name}_{index}";
                Room room = new Room(name, editor.GridPosition);
                room.Map = new Map(name);
                room.Map.Path = Game.Root.Zone.Map.Path;
                Game.Root.Zone.AddChild(room);
                Game.Root.Zone.Map.AddCell(new MapCell($"={room.Name}", room.Position));
                Game.Root.Zone.Map.Save();
                Game.Root.Room = room;
                room.Map.Save();
            }
        }
        else
        {
            if (Game.Root.Room == null) return;
            
            if (Input.LeftMouseButtonIsPressed)
            {
                moveRoomStart = Game.Root.Room.Position;
                moveMouseStart = editor.GridPosition;
            }

            if (moveRoomStart != null)
            {
                Vector2 difference = editor.GridPosition - moveMouseStart.Value;
                Game.Root.Room.Position = Game.Root.Room.Cell.Position = moveRoomStart.Value + difference;
            }

            if (Input.LeftMouseButtonIsReleased)
            {
                moveRoomStart = null;
                moveMouseStart = null;
            }
        }


        base.Update();
    }

    public override void Draw()
    {
        base.Draw();
    }
}

public class Editor : Thing
{
    public Sprite Mouse;
    public Thing CameraTarget;
    Sprite gridMouse;
    Sprite previewBackground;
    Sprite preview;
    Text previewText;

    Vector2 cameraVelocity;

    public bool Holdup = false;
    public bool InteractDisabled => Holdup || Game.GetThings<Room>().Where(r => r != Game.Root.Room).Any(r => r.Hovered);

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

        stamps = Library.ThingImports.Select(t => new Stamp(t.Key)).ToList();

        stamp = stamps.First();
        tool = new BrushTool(this);

        Raylib.HideCursor();

        DrawMode = DrawMode.Relative;
        DrawOrder = 99;

        gridMouse = AddChild(new Sprite("16White", drawOrder: 100)) as Sprite;
        gridMouse.Color = new Color(255, 0, 0, 50);

        Mouse = AddChild(new Sprite("Mouse", tileSize: 16, tileNumber: 0, drawOrder: 110, drawMode: DrawMode.Absolute, adjustWithMargin: false)) as Sprite;

        previewBackground = AddChild(new Sprite("Pixel", drawOrder: 108, color: Colors.Black, scale: new Vector2(18), drawMode: DrawMode.Absolute, adjustWithMargin: false)) as Sprite;
        preview = AddChild(new Sprite(stamp.Name, drawOrder: 109, tileSize: 16, color: CONSTANTS.PRIMARY_COLOR, drawMode: DrawMode.Absolute, adjustWithMargin: false)) as Sprite;
        previewText = AddChild(new Text(stamp.Name, position: new Vector2(0), Library.Font, color: CONSTANTS.PRIMARY_COLOR, outlineColor: Colors.Black, order: 111, drawMode: DrawMode.Absolute, adjustWithMargin: false)) as Text;

        CameraTarget = AddChild(new Thing("CameraTarget"));
    }

    public override void Update()
    {
        base.Update();

        // Log.Write(Viewport.RelativeLayer.Camera.Zoom);

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

        MousePosition = Input.MousePosition();
        Mouse.Position = Input.MousePositionAbsolute() + new Vector2(8);
        Mouse.SetVisible(Input.IsMouseInsideWindow());

        GridPosition = MousePosition.ToNearest(16);
        gridMouse.Position = GridPosition;
        gridMouse.SetVisible(Input.IsMouseInsideWindow());

        preview.Position = preview.Position.MoveOverTime(Mouse.Position + new Vector2((previewBackground.Scale.X / 2f) + 8, -4), 0.00001f);
        previewText.Position = preview.Position + new Vector2(-7, preview.TileSize / 2f);
        previewBackground.Position = preview.Position;
        TogglePreview(Input.IsMouseInsideWindow());

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

    public void TogglePreview(bool show)
    {
        preview.SetVisible(show);
        previewBackground.SetVisible(show);
        previewText.SetVisible(show);
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
            return Chance.Range(0, (stamp.Texture.Width / 16));
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
        if (Game.Root.Room == null) return;
        
        Game.Root.Room.Map.Cells.ToList().ForEach(c =>
        {
            if (positions != null && !positions.Contains(c.Position)) return;

            AddCell(c);
        });
    }

    public void AddCell(MapCell cell) => AddCell(cell.Position + Game.Root.Room.Position, cell.Name, cell.Import.Layer);

    public void AddCell(Vector2 position) => AddCell(position, stamp.Name, stamp.Import.Layer);

    public void AddCell(Vector2 position, string name, int layer)
    {
        RemoveCell(position, layer);
        position -= Game.Root.Room.Position;

        MapCell cell = new MapCell(
            name: name,
            position: position,
            tileNumber: 0
        );
        Game.Root.Room.Map.AddCell(cell);
        cell.TileNumber = GetTileNumber(cell);
        cell.Create(Game.Root.Room);

        recentGridInteractionPositions.AddRange(PositionWithNeighbors(position));
        LastGridInteractPosition = position;
    }

    public void RemoveCell(Vector2 position, int? layer = null)
    {
        position -= Game.Root.Room.Position;

        MapCell cell = Game.Root.Room.Map.GetCell(position, layer);
        if (cell == null) return;

        if (cell.Import.Thing == "BakedTilemap")
        {
            BakedTilemap bakedTilemap = layer != null
                ? Game.Root.Room.GetThings<BakedTilemap>().Find(b => b.Layer == layer)
                : Game.Root.Room.GetThings<BakedTilemap>().Where(b => b.Tiles.Any(t => t.Position == position)).OrderByDescending(b => b.Layer).FirstOrDefault();

            if (bakedTilemap != null) bakedTilemap.RemoveTile(position);
        }
        else
        {
            Game.Root.Room.Children.Where(t => t.Cell?.Name == cell.Name && t.Cell?.Position == cell.Position).ToList().ForEach(t => t.Destroy());
        }
        Game.Root.Room.Map.RemoveCell(cell);

        recentGridInteractionPositions.AddRange(PositionWithNeighbors(position));
        LastGridInteractPosition = position;
    }

    public void PickCell(Vector2 position)
    {
        position -= Game.Root.Room.Position;

        MapCell cell = Game.Root.Room.Map.GetCell(position);
        if (cell == null) return;

        SelectStamp(stamps.Find(s => s.Name == cell.Name));
    }

    public void SelectStamp(Stamp stamp)
    {
        this.stamp = stamp;
        preview.Texture = stamp.Texture;
        previewText.Content = stamp.Name;

        preview.TileSize = Library.ThingImports[stamp.Name].TileSize ?? 16;
        previewBackground.Scale = new Vector2(preview.TileSize + 2);
    }

    public override void Draw()
    {
        base.Draw();

        if (tool != null) tool.Draw();
    }
}
