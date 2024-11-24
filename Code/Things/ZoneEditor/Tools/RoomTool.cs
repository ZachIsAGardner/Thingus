using System.Numerics;

namespace Thingus;

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

        Room hoveredRoom = Game.GetThings<Room>().Find(r => r.Hovered);

        if (hoveredRoom != null)
        {
            // Left
            if (editor.MousePosition.X <= hoveredRoom.Left.X + 32)
            {
                editor.Mouse.TileNumber = 10;
            }
            // Right
            else if (editor.MousePosition.X >= hoveredRoom.Right.X - 32)
            {
                editor.Mouse.TileNumber = 10;
            }
            // Top
            else if (editor.MousePosition.Y <= hoveredRoom.Top.Y + 32)
            {
                editor.Mouse.TileNumber = 9;
            }
            // Bottom
            else if (editor.MousePosition.Y >= hoveredRoom.Bottom.Y - 32)
            {
                editor.Mouse.TileNumber = 9;
            }
            // Move
            else
            {
                editor.Mouse.TileNumber = 11;

                if (Input.LeftMouseButtonIsPressed)
                {
                    moveRoomStart = hoveredRoom.Position;
                    moveMouseStart = editor.GridPosition;
                }

                if (moveRoomStart != null)
                {
                    Vector2 difference = editor.GridPosition - moveMouseStart.Value;
                    hoveredRoom.Position = hoveredRoom.Cell.Position = moveRoomStart.Value + difference;
                }

                if (Input.LeftMouseButtonIsReleased)
                {
                    moveRoomStart = null;
                    moveMouseStart = null;
                }
            }

            // Delete
            if (Input.RightMouseButtonIsPressed)
            {
                editor.Target.Map.RemoveCell(hoveredRoom.Cell);
                editor.Room = null;
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

                moveRoomStart = null;
                moveMouseStart = null;

                if (Input.LeftMouseButtonIsPressed)
                {
                    Thing child = editor.Target.Children.OrderByDescending(c => c.Name).FirstOrDefault();
                    string index = child == null ? "000" : (child.Name.Split("_").Last().ToInt() + 1).ToString().PadLeft(3, '0');
                    string mapName = $"{editor.Target.Name}_{index}";
                    string roomName = "Room";
                    int width = CONSTANTS.VIRTUAL_WIDTH;
                    int height = CONSTANTS.VIRTUAL_HEIGHT;
                    if (Game.ProjectionType == ProjectionType.Grid)
                    {
                        while (width % CONSTANTS.TILE_SIZE != 0) width++;
                        while (height % CONSTANTS.TILE_SIZE != 0) height++;
                    }
                    else if (Game.ProjectionType == ProjectionType.Oblique)
                    {
                        while (width % CONSTANTS.TILE_SIZE_OBLIQUE != 0) width++;
                        while (height % CONSTANTS.TILE_SIZE_OBLIQUE / 2 != 0) height++;
                    }
                    else if (Game.ProjectionType == ProjectionType.Isometric)
                    {
                        // ...
                    }
                    Room room = new Room(roomName, Vector2.Zero, new Vector2(width, height));
                    room.Map = new Map(mapName);
                    room.Map.AddCell(new MapCell(roomName, room.Position, bounds: room.Bounds));
                    room.Map.Path = editor.Target.Map.Path;
                    room.Cell = new MapCell($"={mapName}", room.Position, parent: editor.Target.Name);
                    editor.Target.AddChild(room);
                    editor.Target.Map.AddCell(room.Cell);
                    editor.Room = room;
                    editor.Save();
                }
            }
        }

        base.Update();
    }

    public override void Draw()
    {
        base.Draw();
    }
}