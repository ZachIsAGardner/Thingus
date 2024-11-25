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

    public static Room Create(Thing root, ThingModel model) => new Room(model.Name, model.Position, model.Bounds);
    public Room() { }
    public Room(string name, Vector2 position, Vector2 bounds) : base(name, position)
    {
        Bounds = bounds;
    }

    public override void Init()
    {
        base.Init();
        DrawOrder = 100;
    }

    public override void Start()
    {
        base.Start();

        AddChild(new Drawer("RoomBackground", drawOrder: -110, action: d =>
        {
            if (Game.Mode == GameMode.Play) return;
            if (Focused)
            {
                d.DrawOrder = -100;
            }
            else
            {
                d.DrawOrder = -110;
            }
            Vector2 bounds = Bounds;
            bounds += (CONSTANTS.PROJECTION_TYPE == ProjectionType.Grid
                ? new Vector2(CONSTANTS.TILE_SIZE)
                : CONSTANTS.PROJECTION_TYPE == ProjectionType.Oblique
                    ? new Vector2(0)
                    : new Vector2(CONSTANTS.TILE_SIZE_HALF, CONSTANTS.TILE_SIZE_QUARTER)
            );
            // Shapes.DrawSprite(
            //     texture: Library.Textures["Pixel"],
            //     position: TopLeft - new Vector2(4),
            //     origin: new Vector2(0),
            //     scale: bounds + new Vector2(8),
            //     color: Focused 
            //         ? PaletteBasic.White 
            //         : Hovered 
            //             ? PaletteBasic.White 
            //             : PaletteBasic.DarkGray 
            // );
            // Drop Shadow
            if (Focused)
            {
                Shapes.DrawSprite(
                    texture: Library.Textures["Pixel"],
                    position: TopLeft + new Vector2(4),
                    origin: new Vector2(0),
                    scale: bounds + new Vector2(4),
                    color: new Color(0, 0, 0, 32)
                );
            }
            // Outline
            Shapes.DrawSprite(
                texture: Library.Textures["Pixel"],
                position: TopLeft - new Vector2(1),
                origin: new Vector2(0),
                scale: bounds + new Vector2(2),
                color: PaletteBasic.Black
            );
            // Background
            Shapes.DrawSprite(
                texture: Library.Textures["Pixel"],
                position: TopLeft,
                origin: new Vector2(0),
                scale: bounds,
                color: Focused 
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

        if (Game.Root.DeveloperTools.Cli.Active) return;

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
                }
            }
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
        Editor editor = Game.GetThing<Editor>();
        if (editor != null) editor.Holdup = true;


        if (Game.Root.DeveloperTools.Editor != null) Game.Root.DeveloperTools.Editor.Room = this;
    }

    void DrawGrid()
    {
        if (CONSTANTS.PROJECTION_TYPE == ProjectionType.Grid)
        {
            for (int c = 0; c <= (Bounds.Y / CONSTANTS.TILE_SIZE); c++)
            {
                for (int r = 0; r <= (Bounds.X / CONSTANTS.TILE_SIZE); r++)
                {
                    Shapes.DrawSprite(
                        texture: Library.Textures[$"{CONSTANTS.TILE_SIZE}Grid"],
                        origin: Vector2.Zero,
                        color: new Color(255, 255, 255, Focused ? 128 : 16),
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
                    Shapes.DrawSprite(
                        texture: Library.Textures[$"{(CONSTANTS.TILE_SIZE)}ObliqueGrid"],
                        origin: Vector2.Zero,
                        color: new Color(255, 255, 255, Focused ? 128 : 16),
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
                    Shapes.DrawSprite(
                        texture: Library.Textures[$"{CONSTANTS.TILE_SIZE_HALF}x{CONSTANTS.TILE_SIZE_QUARTER}Grid"],
                        origin: Vector2.Zero,
                        color: new Color(255, 255, 255, Focused ? 128 : 16),
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
        Shapes.DrawText(
            text: message,
            position: new Vector2(Center.X, TopLeft.Y) - new Vector2(Raylib.MeasureText(message, Library.Font.BaseSize) / 2f, Library.Font.BaseSize * 2.25f),
            color: PaletteBasic.White,
            outlineColor: PaletteBasic.Black
        );

        message = $"{Position.X}x,{Position.Y}y {Bounds.X}w,{Bounds.Y}h";
        Shapes.DrawText(
            text: message,
            position: new Vector2(Center.X, TopLeft.Y) - new Vector2(Raylib.MeasureText(message, Library.Font.BaseSize) / 2f, Library.Font.BaseSize * 1.25f),
            color: PaletteBasic.LightGray,
            outlineColor: PaletteBasic.Black
        );
    }
}
