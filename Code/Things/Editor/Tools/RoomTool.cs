using System.Numerics;

namespace Thingus;

public class RoomTool : Tool
{
    Vector2? actionMousePositionStart = null;
    Vector2? actionRoomPositionStart = null;
    Vector2? actionRoomBoundsStart = null;
    Dictionary<int, Vector2> actionRoomChildPositions = new Dictionary<int, Vector2>() { };
    Dictionary<int, Vector2> actionRoomCellPositions = new Dictionary<int, Vector2>() { };
    Room actionRoom = null;
    bool resizeReverse = false;

    Action state = null;

    public RoomTool(Editor editor) : base(editor)
    {
        state = Normal;
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

        if (state != null) state();

        base.Update();
    }

    void Normal()
    {
        Room hoveredRoom = Game.GetThings<Room>().Find(r => r.Hovered);

        if (hoveredRoom != null)
        {
            // Left
            if (editor.MousePosition.X <= hoveredRoom.Left.X + 32)
            {
                editor.Mouse.TileNumber = 10;

                if (Input.LeftMouseButtonIsPressed)
                {
                    actionRoomPositionStart = hoveredRoom.Position;
                    actionRoomBoundsStart = hoveredRoom.Bounds;
                    actionMousePositionStart = editor.GridPosition;
                    actionRoom = hoveredRoom;
                    resizeReverse = true;
                    actionRoom.Children.ForEach(c =>
                    {
                        actionRoomChildPositions[c.Id] = c.Position;
                    });
                    int i = 0;
                    actionRoom.Map.Cells.Where(c => c != actionRoom.Cell).ToList().ForEach(c =>
                    {
                        actionRoomCellPositions[i] = c.Position;
                    });

                    state = ResizeRoomHorizontal;
                }
            }
            // Right
            else if (editor.MousePosition.X >= hoveredRoom.Right.X - 32)
            {
                editor.Mouse.TileNumber = 10;

                if (Input.LeftMouseButtonIsPressed)
                {
                    actionRoomPositionStart = hoveredRoom.Position;
                    actionRoomBoundsStart = hoveredRoom.Bounds;
                    actionMousePositionStart = editor.GridPosition;
                    actionRoom = hoveredRoom;

                    state = ResizeRoomHorizontal;
                }
            }
            // Top
            else if (editor.MousePosition.Y <= hoveredRoom.Top.Y + 32)
            {
                editor.Mouse.TileNumber = 9;

                if (Input.LeftMouseButtonIsPressed)
                {
                    actionRoomPositionStart = hoveredRoom.Position;
                    actionRoomBoundsStart = hoveredRoom.Bounds;
                    actionMousePositionStart = editor.GridPosition;
                    actionRoom = hoveredRoom;
                    resizeReverse = true;
                    actionRoom.Children.ForEach(c =>
                    {
                        actionRoomChildPositions[c.Id] = c.Position;
                    });
                    int i = 0;
                    actionRoom.Map.Cells.Where(c => c != actionRoom.Cell).ToList().ForEach(c =>
                    {
                        actionRoomCellPositions[i] = c.Position;
                    });

                    state = ResizeRoomVertical;
                }
            }
            // Bottom
            else if (editor.MousePosition.Y >= hoveredRoom.Bottom.Y - 32)
            {
                editor.Mouse.TileNumber = 9;

                if (Input.LeftMouseButtonIsPressed)
                {
                    actionRoomPositionStart = hoveredRoom.Position;
                    actionRoomBoundsStart = hoveredRoom.Bounds;
                    actionMousePositionStart = editor.GridPosition;
                    actionRoom = hoveredRoom;

                    state = ResizeRoomVertical;
                }
            }
            // Move
            else
            {
                editor.Mouse.TileNumber = 11;

                if (Input.LeftMouseButtonIsPressed)
                {
                    actionRoomPositionStart = hoveredRoom.Position;
                    actionMousePositionStart = editor.GridPosition;
                    actionRoom = hoveredRoom;

                    state = MoveRoom;
                }
            }

            // Delete
            if (Input.RightMouseButtonIsPressed)
            {
                editor.Target.Map.RemoveCell(hoveredRoom.ParentCell);
                editor.Room = null;
                hoveredRoom.Map.Delete();
                hoveredRoom.Destroy();
                hoveredRoom = null;
            }
        }
        else
        {
            editor.Mouse.TileNumber = 7;

            // Make new Room
            if (Input.CtrlIsHeld)
            {
                editor.Mouse.TileNumber = 8;

                actionRoomPositionStart = null;
                actionMousePositionStart = null;

                if (Input.LeftMouseButtonIsPressed)
                {
                    Thing child = editor.Target.Children.OrderByDescending(c => c.Name).FirstOrDefault();
                    string index = child == null ? "000" : (child.Map.Name.Split("_").Last().ToInt() + 1).ToString().PadLeft(3, '0');
                    string mapName = $"{editor.Target.Map.Name}_{index}";
                    string roomName = "Room";
                    int width = CONSTANTS.VIRTUAL_WIDTH;
                    int height = CONSTANTS.VIRTUAL_HEIGHT;
                    Vector2 position = editor.GridPosition;
                    if (Game.ProjectionType == ProjectionType.Grid)
                    {
                        while (width % CONSTANTS.TILE_SIZE != 0) width++;
                        while (height % CONSTANTS.TILE_SIZE != 0) height++;
                        position = editor.GridPosition;
                    }
                    else if (Game.ProjectionType == ProjectionType.Oblique)
                    {
                        while (width % CONSTANTS.TILE_SIZE_OBLIQUE != 0) width++;
                        while (height % CONSTANTS.TILE_SIZE_OBLIQUE / 2 != 0) height++;
                        position = editor.GridPosition - new Vector2(CONSTANTS.TILE_SIZE_THIRD / 2);
                    }
                    else if (Game.ProjectionType == ProjectionType.Isometric)
                    {
                        // TODO
                    }
        
                    Room room = new Room(roomName, position, new Vector2(width, height));
                    room.Map = new Map(mapName);
                    room.Cell = new MapCell(roomName, room.Position, bounds: room.Bounds);
                    room.Map.AddCell(room.Cell);
                    room.Map.Path = editor.Target.Map.Path;
                    room.ParentCell = new MapCell($"={mapName}", room.Position, parent: editor.Target.Name);
                    editor.Target.AddChild(room);
                    editor.Target.Map.AddCell(room.ParentCell);
                    editor.Room = room;
                    editor.Save();
                }
            }
        }
    }

    void MoveRoom()
    {
        Vector2 difference = editor.GridPosition - actionMousePositionStart.Value;
        actionRoom.Position = actionRoom.ParentCell.Position = actionRoomPositionStart.Value + difference;

        if (Input.LeftMouseButtonIsReleased)
        {
            Reset();
        }
    }

    void ResizeRoomHorizontal()
    {
        Vector2 difference = editor.GridPosition - actionMousePositionStart.Value;
        if (resizeReverse)
        {
            actionRoom.Position.X = actionRoom.ParentCell.Position.X = actionRoomPositionStart.Value.X + difference.X;
            actionRoom.Bounds.X = actionRoom.Cell.Bounds.X = actionRoomBoundsStart.Value.X - difference.X;
            actionRoom.Children.ForEach(c =>
            {
                c.Position.X = actionRoomChildPositions[c.Id].X - difference.X;
            });
            int i = 0;
            actionRoom.Map.Cells.Where(c => c != actionRoom.Cell).ToList().ForEach(c =>
            {
                c.Position.X = actionRoomCellPositions[i].X - difference.X;
            }); 
        }
        else
        {
            actionRoom.Bounds.X = actionRoom.Cell.Bounds.X = actionRoomBoundsStart.Value.X + difference.X;
        }

        if (Input.LeftMouseButtonIsReleased)
        {
            if (resizeReverse)
            {
                actionRoom.GetThings<BakedTilemap>().ForEach(b =>
                {
                    b.Position.X += difference.X;
                    b.Tiles.ForEach(t => t.Position.X -= difference.X);
                });
            }
            Reset();
        }
    }

    void ResizeRoomVertical()
    {
        Vector2 difference = editor.GridPosition - actionMousePositionStart.Value;
        actionRoom.Bounds.Y = actionRoomBoundsStart.Value.Y + difference.Y;

        if (Input.LeftMouseButtonIsReleased)
        {
            Reset();
        }
    }

    void Reset()
    {
        actionRoomPositionStart = null;
        actionRoomBoundsStart = null;
        actionRoom = null;
        actionMousePositionStart = null;
        state = Normal;
        resizeReverse = false;
        actionRoomChildPositions.Clear();
        actionRoomCellPositions.Clear();
    }

    public override void Draw()
    {
        base.Draw();
    }
}