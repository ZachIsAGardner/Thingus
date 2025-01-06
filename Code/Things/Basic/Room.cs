using System.Numerics;
using Raylib_cs;

namespace Thingus;

public class Room : Thing
{
    public Vector2 Bounds;
    public Vector2 TopLeft => Position;
    public Vector2 BottomRight => Position + Bounds;
    public Vector2 Center => Position + (Bounds / 2);
    public Vector2 Left => new Vector2(TopLeft.X, Center.Y);
    public Vector2 Right => new Vector2(BottomRight.X, Center.Y);
    public Vector2 Top => new Vector2(Center.X, TopLeft.Y);
    public Vector2 Bottom => new Vector2(Center.X, BottomRight.Y);

    public bool Hovered = false;
    public bool Focused => Game.Root.DeveloperTools.Editor.Room == this;
    public bool GroupMove = false;

    public static Room Create(ThingModel model) => new Room(model.Name, model.Position, model.Bounds);
    public Room() { }
    public Room(string name, Vector2 position, Vector2 bounds) : base(name, position)
    {
        Bounds = bounds;
    }

    public override void Start()
    {
        base.Start();

        AddChild(new Drawer("RoomBackground", drawOrder: -190, action: d =>
        {
            if (Game.Mode == GameMode.Play) return;
            if (Focused)
            {
                d.DrawOrder = -190;
            }
            else
            {
                d.DrawOrder = -200;
            }
            Vector2 bounds = Bounds;
            bounds += (CONSTANTS.PROJECTION_TYPE == ProjectionType.Grid
                ? new Vector2(CONSTANTS.TILE_SIZE)
                : CONSTANTS.PROJECTION_TYPE == ProjectionType.Oblique
                    ? new Vector2(0)
                    : new Vector2(CONSTANTS.TILE_SIZE_HALF, CONSTANTS.TILE_SIZE_QUARTER)
            );
        
            // Drop Shadow
            if (!GroupMove && Focused)
            {
                DrawSprite(
                    texture: Library.Textures["Pixel"],
                    position: TopLeft + new Vector2(4),
                    origin: new Vector2(0),
                    scale: bounds + new Vector2(4),
                    color: new Color(0, 0, 0, 32)
                );
            }
            // Outline
            DrawSprite(
                texture: Library.Textures["Pixel"],
                position: TopLeft - new Vector2(1),
                origin: new Vector2(0),
                scale: bounds + new Vector2(2),
                color: PaletteBasic.Black
            );
            // Background
            DrawSprite(
                texture: Library.Textures["Pixel"],
                position: TopLeft,
                origin: new Vector2(0),
                scale: bounds,
                color: GroupMove
                    ? Theme.Primary
                    : Focused 
                        ? PaletteBasic.Gray 
                        : Hovered 
                            ? PaletteBasic.LightGray 
                            : PaletteBasic.DarkGray 
            );
        }));
    }

    public override void Update()
    {
        base.Update();

        if (Game.Root.DeveloperTools == null || Game.Root.DeveloperTools?.Cli?.Active == true || Game.Mode != GameMode.Edit) return;

        if (Input.Holdup || Game.GetThings<Control>().Any(c => c.IsHovered))
        {
            Hovered = false;
            return;
        }

        Hovered = Utility.CheckRectangleOverlap(
            Game.Root.DeveloperTools.Editor.GridPosition - new Vector2(16),
            Game.Root.DeveloperTools.Editor.GridPosition,
            TopLeft,
            BottomRight
        );

        if (Game.Root.DeveloperTools.Editor?.Room != this)
        {
            if (Hovered)
            {
                if (Input.LeftMouseButtonIsPressed)
                {
                    Click();
                    DeveloperStorage.Room = Map?.Name;
                    DeveloperStorage.Save();
                }
            }
        }
    }

    public override void PersistentUpdate()
    {
        base.PersistentUpdate();

        if (Game.Mode == GameMode.Play)
        {
            // DrawOrder = 0;
        }
        else if (Game.Mode == GameMode.Edit)
        {
            DrawOrder = 100;
        }
    }

    public bool CheckOverlap(Collider other)
    {
        return Utility.CheckRectangleOverlap(TopLeft, BottomRight, other.GlobalTopLeft, other.GlobalBottomRight);
    }

    public bool IsPositionInside(Vector2 other)
    {
        return Utility.CheckRectangleOverlap(TopLeft, BottomRight + new Vector2(CONSTANTS.TILE_SIZE), other, other);
    }

    public void Click()
    {
        Input.Holdup = true;

        if (Game.Root.DeveloperTools?.Editor != null) Game.Root.DeveloperTools.Editor.Room = this;
    }

    void DrawGrid()
    {
        if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Grid)
        {
            for (int c = 0; c <= (Bounds.Y / CONSTANTS.TILE_SIZE); c++)
            {
                for (int r = 0; r <= (Bounds.X / CONSTANTS.TILE_SIZE); r++)
                {
                    DrawSprite(
                        texture: Library.Textures[$"{CONSTANTS.TILE_SIZE}Grid"],
                        origin: Vector2.Zero,
                        color: new Color(255, 255, 255, Focused ? 100 : 2),
                        position: TopLeft + new Vector2(r * CONSTANTS.TILE_SIZE, c * CONSTANTS.TILE_SIZE)
                    );
                }
            }
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Oblique)
        {
            for (int c = 0; c < (Bounds.Y / (CONSTANTS.TILE_SIZE_OBLIQUE)); c++)
            {
                for (int r = 0; r < (Bounds.X / (CONSTANTS.TILE_SIZE_OBLIQUE)); r++)
                {
                    DrawSprite(
                        texture: Library.Textures[$"{(CONSTANTS.TILE_SIZE)}ObliqueGrid"],
                        origin: Vector2.Zero,
                        color: new Color(255, 255, 255, Focused ? 100 : 2),
                        position: TopLeft + new Vector2(
                            r * (CONSTANTS.TILE_SIZE_OBLIQUE), 
                            c * (CONSTANTS.TILE_SIZE_OBLIQUE)
                        )
                    );
                }
            }
        }
        else if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Isometric)
        {
            for (int c = 0; c <= (Bounds.Y / CONSTANTS.TILE_SIZE_QUARTER); c++)
            {
                for (int r = 0; r <= (Bounds.X / CONSTANTS.TILE_SIZE_HALF); r++)
                {
                    DrawSprite(
                        texture: Library.Textures[$"{CONSTANTS.TILE_SIZE_HALF}x{CONSTANTS.TILE_SIZE_QUARTER}Grid"],
                        origin: Vector2.Zero,
                        color: new Color(255, 255, 255, Focused ? 100 : 2),
                        position: TopLeft + new Vector2(r * CONSTANTS.TILE_SIZE_HALF, c * CONSTANTS.TILE_SIZE_QUARTER)
                    );
                }
            }
        }
    }

    public override void Draw()
    {
        base.Draw();

        if (Game.Mode == GameMode.Play) return;

        DrawGrid();

        string message = $"{Name}";
        DrawText(
            text: message,
            position: new Vector2(Center.X, TopLeft.Y) - new Vector2(Raylib.MeasureText(message, Library.Font.BaseSize) / 2f, Library.Font.BaseSize * 2.25f),
            color: PaletteBasic.White,
            outlineColor: PaletteBasic.Black
        );

        message = $"{Position.X}x,{Position.Y}y {Bounds.X}w,{Bounds.Y}h";
        DrawText(
            text: message,
            position: new Vector2(Center.X, TopLeft.Y) - new Vector2(Raylib.MeasureText(message, Library.Font.BaseSize) / 2f, Library.Font.BaseSize * 1.25f),
            color: PaletteBasic.LightGray,
            outlineColor: PaletteBasic.Black
        );
    }
}
