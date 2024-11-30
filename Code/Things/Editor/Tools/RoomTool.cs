using System.Numerics;
using System.Runtime.InteropServices;

namespace Thingus;

public class RoomTool : Tool
{
    public override string Name => "Room";
    public override KeyboardKey Shortcut => KeyboardKey.R;
    public override int TileNumber => 7;

    Vector2? actionMousePositionStart = null;
    Vector2? actionRoomPositionStart = null;
    Vector2? actionRoomBoundsStart = null;
    Dictionary<int, Vector2> actionRoomChildPositions = new Dictionary<int, Vector2>() { };
    Dictionary<int, Vector2> actionRoomCellPositions = new Dictionary<int, Vector2>() { };
    Room actionRoom = null;

    List<(Room Room, Vector2 StartPosition)> moveRooms = new List<(Room Room, Vector2 StartPosition)>() { };

    bool resizeReverse = false;

    Action state = null;

    public RoomTool(Editor editor) : base(editor)
    {
        state = Normal;
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
                        i++;    
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
                        i++;
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
                RemoveRoom(hoveredRoom);
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
                    AddRoom();
                }
            }
            else
            {
                if (Input.LeftMouseButtonIsPressed)
                {
                    state = Drag;
                }
            }
        }
    }

    RectangleSelection selection = null;

    void Drag()
    {
        editor.Mouse.TileNumber = 7;

        if (Input.LeftMouseButtonIsHeld && editor.LastGridInteractPosition != editor.GridPosition && selection == null)
        {
            selection = new RectangleSelection(editor.GridPosition, () => editor.GridPosition, new Color(0, 255, 0, 255));
            editor.Holdup = true;
        }

        if (Input.LeftMouseButtonIsReleased && selection != null)
        {
            List<Room> rooms = Game.GetThings<Room>().Where(r => selection.Positions.Any(p => r.IsPositionInside(p))).ToList();
            rooms.ForEach(r => Log.Write(r.Name));
            selection.Destroy();
            selection = null;
            actionMousePositionStart = editor.GridPosition;
            if (rooms.Count <= 0)
            {
                state = Normal;
            } 
            else
            {
                moveRooms = rooms.Select(r => (r, r.Position)).ToList();
                rooms.ForEach(r => r.GroupMove = true);
                state = MoveRooms;
            }
        }
    }

    Room AddRoom()
    {
        Thing child = editor.Target.Children.OrderByDescending(c => c.Map.Name).FirstOrDefault();
        string index = child == null ? "000" : (child.Map.Name.Split("_").Last().ToInt() + 1).ToString().PadLeft(3, '0');
        string mapName = $"{editor.Target.Map.Name}_{index}";
        int width = CONSTANTS.VIRTUAL_WIDTH;
        int height = CONSTANTS.VIRTUAL_HEIGHT;
        Vector2 position = editor.GridPosition;
        if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Grid)
        {
            while (width % CONSTANTS.TILE_SIZE != 0) width++;
            while (height % CONSTANTS.TILE_SIZE != 0) height++;
            position = editor.GridPosition - new Vector2(CONSTANTS.TILE_SIZE / 2);
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Oblique)
        {
            while (width % CONSTANTS.TILE_SIZE_OBLIQUE != 0) width++;
            while (height % CONSTANTS.TILE_SIZE_OBLIQUE / 2 != 0) height++;
            position = editor.GridPosition - new Vector2(CONSTANTS.TILE_SIZE_THIRD / 2);
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Isometric)
        {
            // TODO
        }

        return AddRoom(mapName, position, width, height);
    }

    Room AddRoom(string mapName, Vector2 position, int width, int height, bool changeHistory = true)
    {
        Room room = new Room(mapName, position, new Vector2(width, height));
        room.Map = new Map(mapName);
        room.Cell = new MapCell(mapName, "Room", Vector2.Zero, bounds: room.Bounds);
        room.Map.AddCell(room.Cell);
        room.Map.Path = editor.Target.Map.Path;
        room.ParentCell = new MapCell($"={mapName}", position: room.Position, parent: editor.Target.Name);
        editor.Target.AddChild(room);
        editor.Target.Map.AddCell(room.ParentCell);
        editor.Room = room;

        if (changeHistory)
        {
            editor.AddHistoryAction(new HistoryAction(
                "NewRoom",
                () =>
                {
                    RemoveRoom(room, false);
                },
                () => 
                {
                    room = AddRoom(mapName, position, width, height, false);
                }
            ));
        }

        return room;
    }

    void RemoveRoom(Room room, bool changeHistory = true)
    {
        editor.Target.Map.RemoveCell(room.ParentCell);
        editor.Room = null;
        room.Children.Where(c => c.Cell != null).ToList().ForEach(c =>
        {
            editor.RemoveCell(c.Cell, room: room, changeHistory: changeHistory);
        });
        room.GetThings<BakedTilemap>().ForEach(b =>
        {
            b.Tiles.ForEach(t =>
            {
                editor.RemoveCell(t.Position + room.Position, b.Layer, room: room, changeHistory: changeHistory);
            });
        });
        room.Map.Delete();
        room.Destroy();

        if (changeHistory)
        {
            editor.AddHistoryAction(new HistoryAction(
                "RemoveRoom",
                () =>
                {
                    room = AddRoom(room.Map.Name, room.Position, (int)room.Bounds.X, (int)room.Bounds.Y, false);
                },
                () => 
                {
                    RemoveRoom(room, false);
                }
            ));
        }
    }

    void MoveRooms()
    {
        moveRooms.ForEach(m =>
        {
            Vector2 difference = editor.GridPosition - actionMousePositionStart.Value;
            m.Room.Position = m.Room.ParentCell.Position = m.StartPosition + difference;
        });

        if (Input.LeftMouseButtonIsPressed)
        {
            List<(Room Room, Vector2 StartPosition, Vector2 Destination)> storedMoveRooms = new List<(Room Room, Vector2 StartPosition, Vector2 Destination)>() { };
            moveRooms.ForEach(m => storedMoveRooms.Add((m.Room, m.StartPosition, m.Room.Position)));

            editor.AddHistoryAction(new HistoryAction(
                "MoveRooms",
                () =>
                {
                    storedMoveRooms.ForEach(s =>
                    {
                        if (s.Room.GlobalRemoved) s.Room = Game.GetThings<Room>().Find(t => t.Name == s.Room.Name);
                        s.Room.Position = s.StartPosition;
                    });
                },
                () =>
                {
                    storedMoveRooms.ForEach(s =>
                    {
                        if (s.Room.GlobalRemoved) s.Room = Game.GetThings<Room>().Find(t => t.Name == s.Room.Name);
                        s.Room.Position = s.Destination;
                    });
                }
            ));
            Reset();
        }
    }

    void MoveRoom()
    {
        Vector2 difference = editor.GridPosition - actionMousePositionStart.Value;
        actionRoom.Position = actionRoom.ParentCell.Position = actionRoomPositionStart.Value + difference;

        if (Input.LeftMouseButtonIsReleased)
        {
            Room storedRoom = actionRoom;
            Vector2 storedDestination = actionRoom.Position;
            Vector2 storedStart = actionRoomPositionStart.Value;

            editor.AddHistoryAction(new HistoryAction(
                "MoveRoom",
                () =>
                {
                    if (storedRoom.GlobalRemoved) storedRoom = Game.GetThings<Room>().Find(t => t.Name == storedRoom.Name);
                    storedRoom.Position = storedStart;
                },
                () =>
                {
                    if (storedRoom.GlobalRemoved) storedRoom = Game.GetThings<Room>().Find(t => t.Name == storedRoom.Name);
                    storedRoom.Position = storedDestination;
                }
            ));
            Reset();
        }
    }

    void ResizeRoomHorizontal()
    {
        if (editor.GridPosition == editor.LastGridInteractPosition && Input.LeftMouseButtonIsHeld) return;

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
                i++;
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

            actionRoom.Map.Cells.ToList().ForEach(c =>
            {
                if (c.Position.X < 0)
                {
                    editor.RemoveCell(c);
                }

                if (c.Position.X > actionRoom.Bounds.X + (CONSTANTS.TILE_SIZE / 2))
                {
                    editor.RemoveCell(c);
                }
            });

            Room storedRoom = actionRoom;
            Vector2 storedDestination = actionRoom.Position;
            Vector2 storedStart = actionRoomPositionStart.Value;
            Vector2 storedBoundsStart = actionRoomBoundsStart.Value;
            Vector2 storedBoundsDestination = actionRoom.Bounds;
            Vector2 storedDifference = difference;
            bool storedResizeReverse = resizeReverse;

            editor.AddHistoryAction(new HistoryAction(
                "ResizeRoomHorizontal",
                () =>
                {
                    if (storedRoom.GlobalRemoved) storedRoom = Game.GetThings<Room>().Find(t => t.Name == storedRoom.Name);
                    if (storedResizeReverse)
                    {
                        storedRoom.Children.ForEach(c =>
                        {
                            if (c.TypeName == "BakedTilemap") (c as BakedTilemap).Tiles.ForEach(t => t.Position.X += storedDifference.X);
                            else c.Position.X += storedDifference.X;
                        });

                        storedRoom.Map.Cells.Where(c => c != storedRoom.Cell).ToList().ForEach(c =>
                        {
                            c.Position.X += storedDifference.X;
                        });
                    }
                    storedRoom.Position = storedRoom.ParentCell.Position = storedStart;
                    storedRoom.Bounds = storedRoom.Cell.Bounds = storedBoundsStart;
                },
                () =>
                {
                    if (storedRoom.GlobalRemoved) storedRoom = Game.GetThings<Room>().Find(t => t.Name == storedRoom.Name);
                    if (storedResizeReverse)
                    {
                        storedRoom.Children.ForEach(c =>
                        {
                            if (c.TypeName == "BakedTilemap") (c as BakedTilemap).Tiles.ForEach(t => t.Position.X -= storedDifference.X);
                            else c.Position.X -= storedDifference.X;
                        });

                        storedRoom.Map.Cells.Where(c => c != storedRoom.Cell).ToList().ForEach(c =>
                        {
                            c.Position.X -= storedDifference.X;
                        });
                    }
                    storedRoom.Position = storedRoom.ParentCell.Position = storedDestination;
                    storedRoom.Bounds = storedRoom.Cell.Bounds = storedBoundsDestination;
                }
            ));

            Reset();
        }
    }

    void ResizeRoomVertical()
    {
        Vector2 difference = editor.GridPosition - actionMousePositionStart.Value;
        if (resizeReverse)
        {
            actionRoom.Position.Y = actionRoom.ParentCell.Position.Y = actionRoomPositionStart.Value.Y + difference.Y;
            actionRoom.Bounds.Y = actionRoom.Cell.Bounds.Y = actionRoomBoundsStart.Value.Y - difference.Y;
            actionRoom.Children.ForEach(c =>
            {
                c.Position.Y = actionRoomChildPositions[c.Id].Y - difference.Y;
            });
            int i = 0;
            actionRoom.Map.Cells.Where(c => c != actionRoom.Cell).ToList().ForEach(c =>
            {
                c.Position.Y = actionRoomCellPositions[i].Y - difference.Y;
                i++;
            }); 
        }
        else
        {
            actionRoom.Bounds.Y = actionRoom.Cell.Bounds.Y = actionRoomBoundsStart.Value.Y + difference.Y;
        }

        if (Input.LeftMouseButtonIsReleased)
        {
            if (resizeReverse)
            {
                actionRoom.GetThings<BakedTilemap>().ForEach(b =>
                {
                    b.Position.Y += difference.Y;
                    b.Tiles.ForEach(t => t.Position.Y -= difference.Y);
                });
            }

            actionRoom.Map.Cells.ToList().ForEach(c =>
            {
                if (c.Position.Y < 0)
                {
                    editor.RemoveCell(c);
                }

                if (c.Position.Y > actionRoom.Bounds.Y + (CONSTANTS.TILE_SIZE / 2))
                {
                    editor.RemoveCell(c);
                }
            });

            Room storedRoom = actionRoom;
            Vector2 storedDestination = actionRoom.Position;
            Vector2 storedStart = actionRoomPositionStart.Value;
            Vector2 storedBoundsStart = actionRoomBoundsStart.Value;
            Vector2 storedBoundsDestination = actionRoom.Bounds;
            Vector2 storedDifference = difference;
            bool storedResizeReverse = resizeReverse;

            editor.AddHistoryAction(new HistoryAction(
                "ResizeRoomVertical",
                () =>
                {
                    if (storedRoom.GlobalRemoved) storedRoom = Game.GetThings<Room>().Find(t => t.Name == storedRoom.Name);
                    if (storedResizeReverse)
                    {
                        storedRoom.Children.ForEach(c =>
                        {
                            if (c.TypeName == "BakedTilemap") (c as BakedTilemap).Tiles.ForEach(t => t.Position.Y += storedDifference.Y);
                            else c.Position.Y += storedDifference.Y;
                        });

                        storedRoom.Map.Cells.Where(c => c != storedRoom.Cell).ToList().ForEach(c =>
                        {
                            c.Position.Y += storedDifference.Y;
                        });
                    }
                    storedRoom.Position = storedRoom.ParentCell.Position = storedStart;
                    storedRoom.Bounds = storedRoom.Cell.Bounds = storedBoundsStart;
                },
                () =>
                {
                    if (storedRoom.GlobalRemoved) storedRoom = Game.GetThings<Room>().Find(t => t.Name == storedRoom.Name);
                    if (storedResizeReverse)
                    {
                        storedRoom.Children.ForEach(c =>
                        {
                            if (c.TypeName == "BakedTilemap") (c as BakedTilemap).Tiles.ForEach(t => t.Position.Y -= storedDifference.Y);
                            else c.Position.Y -= storedDifference.Y;
                        });

                        storedRoom.Map.Cells.Where(c => c != storedRoom.Cell).ToList().ForEach(c =>
                        {
                            c.Position.Y -= storedDifference.Y;
                        });
                    }
                    storedRoom.Position = storedRoom.ParentCell.Position = storedDestination;
                    storedRoom.Bounds = storedRoom.Cell.Bounds = storedBoundsDestination;
                }
            ));

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
        moveRooms.ForEach(m => m.Room.GroupMove = false);
        moveRooms.Clear();
    }

    public override void Draw()
    {
        base.Draw();
    }
}